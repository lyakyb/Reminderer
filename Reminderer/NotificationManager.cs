using Reminderer.Framework;
using Reminderer.Models;
using Reminderer.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Data;

namespace Reminderer
{
    public class NotificationManager
    {
        private static object _lock = new object();

        private IReminderRepository _reminderRepository;
        private IScheduleRepository _scheduleRepository;

        public NotificationManager(IReminderRepository reminderRepository, IScheduleRepository scheduleRepository)
        {
            _remindersNotificationList = new Dictionary<int, Timer>();
            _schedulesNotificationList = new Dictionary<int, Timer>();
            _scheduleRepository = scheduleRepository;
            _reminderRepository = reminderRepository;
            LoadSchedulesAndReminders();
            BindingOperations.EnableCollectionSynchronization(Schedules, _lock);
            BindingOperations.EnableCollectionSynchronization(Reminders, _lock);
            BindingOperations.EnableCollectionSynchronization(_remindersNotificationList, _lock);
            BindingOperations.EnableCollectionSynchronization(_schedulesNotificationList, _lock);


            updateTasksToNotifyList();
        }

        private void LoadSchedulesAndReminders()
        {
            Schedules = new ObservableCollection<Schedule>(_scheduleRepository.ReadAll().Where(x => x.DesiredDateTime > DateTime.Now).ToList());
            Reminders = new ObservableCollection<Reminder>(_reminderRepository.ReadAll().Where(x => x.DesiredDateTime > DateTime.Now || x.ShouldRepeat || x.Type == Reminder.ReminderType.IsAtSetInterval).ToList());
        }

        #region Properties
        private ObservableCollection<Schedule> _schedules;
        public ObservableCollection<Schedule> Schedules
        {
            get { return _schedules; }
            set { _schedules = value; }
        }
        private ObservableCollection<Reminder> _reminders;
        public ObservableCollection<Reminder> Reminders
        {
            get { return _reminders; }
            set { _reminders = value; }
        }
        private Dictionary<int, Timer> _remindersNotificationList;
        private Dictionary<int, Timer> _schedulesNotificationList;


        #endregion

        #region Alarm Related Methods
        private void updateTasksToNotifyList()
        {
            foreach(Schedule s in Schedules)
            {
                // check and see if notification prior to due date is set.
                if (s.ShouldRemind && s.ShouldNotifyToday())
                {
                    NotifyForSchedule(s);
                }
            }
            foreach(Reminder r in Reminders)
            {
                if (r.Type == Reminder.ReminderType.IsFromSavedTime && !TimeAheadNow(r.DesiredDateTime))
                {
                    NotifyFromNow(r);
                }
                else if (r.Type == Reminder.ReminderType.IsAtSpecificTime && r.ShouldNotifyToday() && !TimeAheadNow(r.DesiredDateTime))
                {
                    NotifyAtThisTime(r);
                }
            }
        }

        private void RemoveFromNotifyListIfNeeded(Task task)
        {
            if (task.GetType() == typeof(Reminder))
            {
                if(_remindersNotificationList.ContainsKey(task.Id))
                {
                    if (((Reminder)task).Type == Reminder.ReminderType.IsAtSetInterval ||
                        (((Reminder)task).Type == Reminder.ReminderType.IsAtSpecificTime && ((Reminder)task).RepeatingDays.Count > 0))
                    {
                        return;
                    }
                    var r = Reminders.Where(x => x.Id == task.Id).ToList().FirstOrDefault();
                    Reminders.Remove(r);
                    DeleteReminder((Reminder)task);
                    _remindersNotificationList.Remove(task.Id);
                }

            } else if (task.GetType() == typeof(Schedule))
            {
                if (_schedulesNotificationList.ContainsKey(task.Id))
                {
                    if (task.DesiredDateTime > DateTime.Now) return;
                    Schedules.Where(x => x.Id == task.Id).ToList().All(x => Schedules.Remove(x));
                    DeleteSchedule((Schedule)task);
                    _schedulesNotificationList.Remove(task.Id);
                }
            }
            task.NotificationOn = false;
            
        }

        private void AddToNotifyIfNeeded(Task task)
        {
            if (task.GetType() == typeof(Reminder))
            {
                var type = ((Reminder)task).Type;
                if (type == Reminder.ReminderType.IsFromSavedTime)
                {
                    NotifyFromNow(task);
                } else if (type == Reminder.ReminderType.IsAtSetInterval)
                {
                    NotifyEveryInterval(task);
                } else if (type == Reminder.ReminderType.IsAtSpecificTime)
                {
                    NotifyAtThisTime(task);
                }

            } else if (task.GetType() == typeof(Schedule) && task.ShouldRemind)
            {
                NotifyForSchedule(task);
            }
        }

        private bool TimeAheadNow(DateTime dateTime)
        {
            if (dateTime.Hour > DateTime.Now.Hour)
            {
                return false;
            }
            else if (dateTime.Hour == DateTime.Now.Hour && dateTime.Minute > DateTime.Now.Minute)
            {
                return false;
            }

            return true;
        }

        private void NotifyForSchedule(Task task)
        {
            var timer = TimerService.instance.ScheduleTaskFromNow(0, 1, () =>
            {
                Mediator.Broadcast(Constants.FireNotification, task);
                RemoveFromNotifyListIfNeeded(task);
            });
            _schedulesNotificationList[task.Id] = timer;
            task.NotificationOn = true;
        }
        private void NotifyFromNow(Task task)
        {
            NotificationForDelayAndInterval(0, task);
        }
        private void NotifyEveryInterval(Task task)
        {
            NotificationForDelayAndInterval(task.DesiredDateTime.Hour * 60 + task.DesiredDateTime.Minute, task);
        }
        private void NotifyAtThisTime(Task task)
        {
            NotificationForDelayAndInterval(60 * 24, task);
        }
        private void NotificationForDelayAndInterval(double interval, Task task)
        {
            var timer = TimerService.instance.ScheduleTaskForInterval(task.DesiredDateTime, interval, () =>
            {
                Mediator.Broadcast(Constants.FireNotification, task);
                RemoveFromNotifyListIfNeeded(task);
            });
            _remindersNotificationList[task.Id] = timer;
            task.NotificationOn = true;
        }

        public bool NotificationExistsForReminder(Reminder r)
        {
            return _remindersNotificationList.ContainsKey(r.Id);
        }

        public bool NotificationExistsForSchedule(Schedule s)
        {
            return _schedulesNotificationList.ContainsKey(s.Id);
        }

        #endregion

        #region Database Related Methods
        public void LoadReminders()
        {
            Reminders = new ObservableCollection<Reminder>(_reminderRepository.ReadAll());
        }
        public void LoadSchedules()
        {
            Schedules = new ObservableCollection<Schedule>(_scheduleRepository.ReadAll());
        }

        public void DeleteReminder(Reminder reminder)
        {
            Reminders.Remove(reminder);
            DeleteReminderWithId(reminder.Id.ToString());
        }
        public void DeleteSchedule(Schedule schedule)
        {
            Schedules.Remove(schedule);
            DeleteScheduleWithId(schedule.Id.ToString());
        }
        private void DeleteReminderWithId(string id)
        {
            Reminder r = new Reminder
            {
                Id = int.Parse(id)
            };
            _reminderRepository.Delete(r);
        }
        private void DeleteScheduleWithId(string id)
        {
            Schedule s = new Schedule
            {
                Id = int.Parse(id)
            };
            _scheduleRepository.Delete(s);
        }

        public void EditReminder(Reminder reminder)
        {
            Reminder prevReminder = Reminders.Where(r => r.Id == reminder.Id).FirstOrDefault();
            _reminderRepository.Update(reminder);
            RemoveFromNotifyListIfNeeded(reminder);
            AddToNotifyIfNeeded(reminder);
        }
        public void EditSchedule(Schedule schedule)
        {
            Schedule prevSchedule = Schedules.Where(s => s.Id == schedule.Id).FirstOrDefault();
            _scheduleRepository.Update(schedule);
            RemoveFromNotifyListIfNeeded(schedule);
            AddToNotifyIfNeeded(schedule);
        }

        public void CreateReminder(Reminder reminder)
        {
            Reminders.Add(reminder);
            _reminderRepository.Add(reminder);
            AddToNotifyIfNeeded(reminder);
        }
        public void CreateSchedule(Schedule schedule)
        {
            Schedules.Add(schedule);
            _scheduleRepository.Add(schedule);
            AddToNotifyIfNeeded(schedule);
        }
        #endregion

    }
}

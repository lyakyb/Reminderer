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
            _scheduleRepository = scheduleRepository;
            _reminderRepository = reminderRepository;
            LoadSchedulesAndReminders();
            _taskNotificationDict = new Dictionary<int, Timer>();
            BindingOperations.EnableCollectionSynchronization(Schedules, _lock);
            BindingOperations.EnableCollectionSynchronization(Reminders, _lock);
            BindingOperations.EnableCollectionSynchronization(_taskNotificationDict, _lock);


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
        private Dictionary<int, Timer> _taskNotificationDict;


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

        private void RemoveFromNotifyList(int taskId)
        {
            if (_taskNotificationDict.ContainsKey(taskId))
            {
                _taskNotificationDict[taskId].Dispose();
                _taskNotificationDict.Remove(taskId);
            }
        }

        private void RemoveFromNotifyListIfNeeded(Task task)
        {
            if (_taskNotificationDict.ContainsKey(task.Id))
            {
                if (task.GetType() == typeof(Reminder))
                {
                    if (((Reminder)task).Type == Reminder.ReminderType.IsAtSetInterval ||
                        (((Reminder)task).Type == Reminder.ReminderType.IsAtSpecificTime && ((Reminder)task).RepeatingDays.Count > 0))
                    {
                        return;
                    }
                    var r = Reminders.Where(x => x.Id == task.Id).ToList().FirstOrDefault();
                    Reminders.Remove(r);
                    DeleteReminder((Reminder)task);

                } else if (task.GetType() == typeof(Schedule))
                {
                    if (task.DesiredDateTime > DateTime.Now) return;
                    Schedules.Where(x => x.Id == task.Id).ToList().All(x => Schedules.Remove(x));
                    DeleteSchedule((Schedule)task);
                }
                RemoveFromNotifyList(task.Id);
                task.NotificationOn = false;
            }
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
            _taskNotificationDict[task.Id] = timer;
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
            _taskNotificationDict[task.Id] = timer;
            task.NotificationOn = true;
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
            RemoveFromNotifyList(reminder.Id);
        }
        public void DeleteSchedule(Schedule schedule)
        {
            Schedules.Remove(schedule);
            DeleteScheduleWithId(schedule.Id.ToString());
            RemoveFromNotifyList(schedule.Id);
        }
        private void DeleteReminderWithId(string id)
        {
            Reminder r = new Reminder
            {
                Id = int.Parse(id)
            };
            _reminderRepository.Delete(r);
            RemoveFromNotifyList(int.Parse(id));
        }
        private void DeleteScheduleWithId(string id)
        {
            Schedule s = new Schedule
            {
                Id = int.Parse(id)
            };
            _scheduleRepository.Delete(s);
            RemoveFromNotifyList(int.Parse(id));
        }

        public void EditReminder(Reminder reminder)
        {
            Reminder prevReminder = Reminders.Where(r => r.Id == reminder.Id).FirstOrDefault();
            _reminderRepository.Update(reminder);
            RemoveFromNotifyList(reminder.Id);
            AddToNotifyIfNeeded(reminder);
        }
        public void EditSchedule(Schedule schedule)
        {
            Schedule prevSchedule = Schedules.Where(s => s.Id == schedule.Id).FirstOrDefault();
            _scheduleRepository.Update(schedule);
            RemoveFromNotifyList(schedule.Id);
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

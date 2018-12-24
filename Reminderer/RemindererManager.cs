using Reminderer.Framework;
using Reminderer.Models;
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
    public class RemindererManager
    {
        private static object _lock = new object();
        //TODO: would it be better to store everything locally and only update to DB on app close/open?  
        private static readonly RemindererManager _instance = new RemindererManager();
        private DatabaseManager databaseManager;
        static RemindererManager()
        {
        }
        private RemindererManager()
        {
            databaseManager = new DatabaseManager("Test1");
            Schedules = new ObservableCollection<Schedule>();
            Reminders = new ObservableCollection<Reminder>();
            _taskNotificationDict = new Dictionary<int, Timer>();
            BindingOperations.EnableCollectionSynchronization(Schedules, _lock);
            BindingOperations.EnableCollectionSynchronization(Reminders, _lock);
            BindingOperations.EnableCollectionSynchronization(_taskNotificationDict, _lock);
            CreateRemindersTable();
            CreateSchedulesTable();
        }
        public static RemindererManager Instance
        {
            get { return _instance; }
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
            string s = $"SELECT * FROM {Constants.RemindersTable}";
            var ds = databaseManager.ReadData(s);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Reminder reminder = ReminderFromDataRow(dr);
                Reminders.Add(reminder);
            }
            updateTasksToNotifyList();
        }
        public void LoadSchedules()
        {
            string s = $"SELECT * FROM {Constants.SchedulesTable}";

            var ds = databaseManager.ReadData(s);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                Schedule schedule = ScheduleFromDataRow(dr);

                if (schedule.DesiredDateTime > DateTime.Now)
                {
                    Schedules.Add(schedule);
                } else
                {
                    DeleteSchedule(schedule);
                }
            }
            updateTasksToNotifyList();
        }

        private void CreateRemindersTable()
        {
            string s = $"SELECT name FROM sqlite_master WHERE name='{Constants.RemindersTable}'";

            var result = databaseManager.ExecuteScalarCommand(s);

            if (result != null && string.Equals(result.ToString(), Constants.RemindersTable))
            {
                Console.WriteLine("Table exists already");
                return;
            }

            s = $"CREATE TABLE {Constants.RemindersTable} (id INTEGER PRIMARY KEY AUTOINCREMENT, Description Text, ExtraDetail text, DesiredDateTime text, ShouldRemind integer, ShouldRepeat integer, RepeatingDays text, Type int)";

            databaseManager.ExecuteNonQueryCommand(s);
        }
        private void CreateSchedulesTable()
        {
            string s = $"SELECT name FROM sqlite_master WHERE name='{Constants.SchedulesTable}'";

            var result = databaseManager.ExecuteScalarCommand(s);

            if (result != null && string.Equals(result.ToString(), Constants.SchedulesTable))
            {
                Console.WriteLine("Table exists already");
                return;
            }

            s = $"CREATE TABLE {Constants.SchedulesTable} (id INTEGER PRIMARY KEY AUTOINCREMENT, Description text, ExtraDetail text, DesiredDateTime text, ShouldRemind integer, NumDaysBeforeNotify int)";

            databaseManager.ExecuteNonQueryCommand(s);
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
            DeleteTaskWithId(id, Constants.RemindersTable);
        }
        private void DeleteScheduleWithId(string id)
        {
            DeleteTaskWithId(id, Constants.SchedulesTable);
        }
        private void DeleteTaskWithId(string id, string table)
        {
            string s = $"DELETE FROM {table} WHERE Id=@idParam";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@idParam", id);
            var result = databaseManager.InsertUpdateDeleteWithParams(s, dict);
            if (result != 1)
            {
                //exception handling
            }
            RemoveFromNotifyList(int.Parse(id));
        }

        public void EditReminder(Reminder reminder)
        {
            Reminder prevReminder = Reminders.Where(r => r.Id == reminder.Id).FirstOrDefault();
            RemoveFromNotifyList(reminder.Id);
            string query = $"UPDATE {Constants.RemindersTable} SET Description=@descParam, ExtraDetail=@extParam, DesiredDateTime=@ddtParam, ShouldRemind=@remindParam, ShouldRepeat=@repeatParam, RepeatingDays=@daysParam, Type=@typeParam WHERE id=@idParam";
            var dict = DictionaryRepresentationForReminder(reminder);

            dict.Add("@idParam", reminder.Id);
            var result = databaseManager.InsertUpdateDeleteWithParams(query, dict);
            if (result != 1)
            {
                //exception handling
            }
            AddToNotifyIfNeeded(reminder);
        }
        public void EditSchedule(Schedule schedule)
        {
            Schedule prevSchedule = Schedules.Where(s => s.Id == schedule.Id).FirstOrDefault();
            RemoveFromNotifyList(schedule.Id);
            string query = $"UPDATE {Constants.SchedulesTable} SET Description=@descParam, ExtraDetail=@extParam, DesiredDateTime=@ddtParam, ShouldRemind=@remindParam, NumDaysBeforeNotify=@numDaysParam WHERE id=@idParam";
            var dict = DictionaryRepresentationForSchedule(schedule);
            dict.Add("@idParam", schedule.Id);
            var result = databaseManager.InsertUpdateDeleteWithParams(query, dict);
            if (result != 1)
            {
            }
            AddToNotifyIfNeeded(schedule);
        }

        public void CreateReminder(Reminder reminder)
        {
            Reminders.Add(reminder);
            string s = $"INSERT INTO {Constants.RemindersTable} (Description, ExtraDetail, DesiredDateTime, ShouldRemind, ShouldRepeat, RepeatingDays, Type) VALUES (@descParam,@extParam,@ddtParam,@remindParam,@repeatParam,@daysParam,@typeParam)";
            var id = databaseManager.InsertUpdateDeleteWithParamsGetLastInsertId(s, DictionaryRepresentationForReminder(reminder));

            reminder.Id = id;

            AddToNotifyIfNeeded(reminder);
        }
        public void CreateSchedule(Schedule schedule)
        {
            Schedules.Add(schedule);
            string s = $"INSERT INTO {Constants.SchedulesTable} (Description, ExtraDetail, DesiredDateTime, ShouldRemind, NumDaysBeforeNotify) VALUES (@descParam,@extParam,@ddtParam,@remindParam,@numDaysParam)";
            var id = databaseManager.InsertUpdateDeleteWithParamsGetLastInsertId(s, DictionaryRepresentationForSchedule(schedule));

            schedule.Id = id;

            AddToNotifyIfNeeded(schedule);
        }


        private Dictionary<string, object> DictionaryRepresentationForReminder(Reminder reminder)
        {
            var dict = DictionaryRepresentationForTask(reminder);
            var repeatingDays = reminder.RepeatingDays != null ? string.Join(",", reminder.RepeatingDays) : "";
            dict.Add("@daysParam", repeatingDays);
            dict.Add("@typeParam", reminder.Type);
            dict.Add("@repeatParam", reminder.ShouldRepeat);

            return dict;
        }
        private Dictionary<string, object> DictionaryRepresentationForSchedule(Schedule schedule)
        {
            var dict = DictionaryRepresentationForTask(schedule);
            dict.Add("@numDaysParam", schedule.NumDaysBeforeNotify);

            return dict;
        }
        private Dictionary<string, object> DictionaryRepresentationForTask(Task task)
        {
            task.ExtraDetail = task.ExtraDetail == null ? "-" : task.ExtraDetail;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@descParam", task.Description);
            dict.Add("@extParam", task.ExtraDetail);
            dict.Add("@ddtParam", task.DesiredDateTime.ToBinary());
            dict.Add("@remindParam", task.ShouldRemind);

            return dict;
        }

        private Reminder ReminderFromDataRow(DataRow dr)
        {
            Reminder r = new Reminder();
            r.Description = dr["Description"].ToString();
            r.ExtraDetail = dr["ExtraDetail"].ToString();
            r.DesiredDateTime = DateTime.FromBinary(long.Parse(dr["DesiredDateTime"].ToString()));
            r.ShouldRemind = int.Parse(dr["ShouldRemind"].ToString()) != 0;
            r.Id = int.Parse(dr["id"].ToString());
            r.ShouldRepeat = int.Parse(dr["ShouldRepeat"].ToString()) != 0;
            r.Type = (Reminder.ReminderType)int.Parse(dr["Type"].ToString());
            var days = dr["RepeatingDays"].ToString().Split(',');
            if (days != null && days.Count() > 0)
            {
                foreach (var day in days)
                {
                    if (string.IsNullOrWhiteSpace(day)) continue;

                    r.RepeatingDays.Add(Reminder.StringToDaysConverter(day));
                }
            }
            else
            {
                r.RepeatingDays = null;
            }
            return r;
        }
        private Schedule ScheduleFromDataRow(DataRow dr)
        {
            Schedule s = new Schedule();
            s.Description = dr["Description"].ToString();
            s.ExtraDetail = dr["ExtraDetail"].ToString();
            s.DesiredDateTime = DateTime.FromBinary(long.Parse(dr["DesiredDateTime"].ToString()));
            s.ShouldRemind = int.Parse(dr["ShouldRemind"].ToString()) != 0;
            s.Id = int.Parse(dr["id"].ToString());

            s.NumDaysBeforeNotify = int.Parse(dr["NumDaysBeforeNotify"].ToString());

            return s;
        }
        #endregion

    }
}

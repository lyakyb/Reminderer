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

namespace Reminderer
{
    public class RemindererManager
    {
        //TODO: would it be better to store everything locally and only update to DB on app close/open?  
        private static readonly RemindererManager _instance = new RemindererManager();
        private DatabaseManager databaseManager;
        static RemindererManager()
        {
        }
        private RemindererManager()
        {
            databaseManager = new DatabaseManager("Test1");
            Schedules = new ObservableCollection<Task>();
            Reminders = new ObservableCollection<Task>();
            _taskNotificationDict = new Dictionary<int, Timer>();
            CreateTasksTable();
        }
        public static RemindererManager Instance
        {
            get { return _instance; }
        }

        #region Properties
        private ObservableCollection<Task> _schedules;
        public ObservableCollection<Task> Schedules
        {
            get { return _schedules; }
            set { _schedules = value; }
        }
        private ObservableCollection<Task> _reminders;
        public ObservableCollection<Task> Reminders
        {
            get { return _reminders; }
            set { _reminders = value; }
        }
        private Dictionary<int, Timer> _taskNotificationDict;

        #endregion

        #region Alarm Related Methods
        private void updateTasksToNotifyList()
        {
            foreach(Task t in Schedules)
            {
                // check and see if notification prior to due date is set.
            }
            foreach(Task t in Reminders)
            {
                if (t.IsFromSavedTime)
                {

                }else if (t.IsAtSetInterval)
                {
                    
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
            if (task.ReminderSetting != 2 && _taskNotificationDict.ContainsKey(task.TaskId))
            {
                _taskNotificationDict[task.TaskId].Dispose();
                _taskNotificationDict.Remove(task.TaskId);
            }
        }

        private void AddToNotifyIfNeeded(Task task)
        {
            //Check if reminder or schedule
            //Only requires heavy checking on reminder 
            if (task.Type == Task.TaskType.Reminder)
            {
                if(task.ReminderSetting == 1)
                {
                    NotifyFromNow(task);
                }else if(task.ReminderSetting == 2)
                {
                    NotifyEveryInterval(task);
                }else if(task.ReminderSetting == 3)
                {
                    NotifyAtThisTime(task);
                }
            }
            else
            {
                //Only add notifications for when the user wants a notification for it.
            }
            
        }
        //Timer parameters, (callback,null,timeUntilCallBack,RepeatInterval)

        private void notifyForTask(object state)
        {
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
            _taskNotificationDict[task.TaskId] = timer;
        }
        #endregion

        #region Database Related Methods
        public void LoadTasks()
        {
            string s = $"SELECT * FROM {Constants.TasksTable}";

            var ds = databaseManager.ReadData(s);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                Task t = taskFromDataRow(dr);
                if (t.Type == Task.TaskType.Reminder)
                {
                    Reminders.Add(t);
                }
                else
                {
                    Schedules.Add(t);
                }
            }
            updateTasksToNotifyList();
        }
        private void CreateTasksTable()
        {
            string s  = $"SELECT name FROM sqlite_master WHERE name='{Constants.TasksTable}'";

            var result = databaseManager.ExecuteScalarCommand(s);

            if (result != null && string.Equals(result.ToString(), Constants.TasksTable))
            {
                Console.WriteLine("Table exists already");
                return;
            }

            s = $"CREATE TABLE {Constants.TasksTable} (taskId INTEGER PRIMARY KEY AUTOINCREMENT, Description text, ExtraDetail text, DesiredDateTime text, ShouldRemind integer, ShouldRepeat integer, RepeatingDays text, Type int, ReminderSetting int, NumDaysBeforeNotify int)";

            databaseManager.ExecuteNonQueryCommand(s);
        }
        public void DeleteTask(Task task)
        {
            if (Reminders.Contains(task))
            {
                Reminders.Remove(task);
            }
            else if (Schedules.Contains(task))
            {
                Schedules.Remove(task);
            }
            deleteTaskWithId(task.TaskId.ToString());
            RemoveFromNotifyList(task.TaskId);
        }

        private void deleteTaskWithId(string id)
        {
            string s = $"DELETE FROM {Constants.TasksTable} WHERE taskId=@taskIdParam";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@taskIdParam", id);
            var result = databaseManager.InsertUpdateDeleteWithParams(s, dict);
            if (result != 1)
            {
                //exception handling
            }
            RemoveFromNotifyList(int.Parse(id));
        }
        public void EditTask(Task task)
        {
            Task prevTask;
            RemoveFromNotifyList(task.TaskId);
            if (task.Type == Task.TaskType.Reminder)
            {
                prevTask = Reminders.Where(t => t.TaskId == task.TaskId).FirstOrDefault();
            }
            else
            {
                prevTask = Schedules.Where(t => t.TaskId == task.TaskId).FirstOrDefault();
            }
            prevTask = task;

            string s = $"UPDATE {Constants.TasksTable} SET Description=@descParam, ExtraDetail=@extParam, DesiredDateTime=@ddtParam, ShouldRemind=@remindParam, ShouldRepeat=@repeatParam, RepeatingDays=@daysParam, Type=@typeParam, ReminderSetting=@settingParam, NumDaysBeforeNotify=@numDaysParam WHERE taskId=@taskIdParam";
            var dict = dictionaryRepresentationForTask(task);
            dict.Add("@taskIdParam", task.TaskId);
            var result = databaseManager.InsertUpdateDeleteWithParams(s, dict);

            if (result != 1)
            {
                //exception handling
            }

            AddToNotifyIfNeeded(task);
        }
        public void CreateTask(Task task)
        {
            if (task.Type == Task.TaskType.Reminder)
            {
                Reminders.Add(task);
            }
            else if (task.Type == 0)
            {
                Schedules.Add(task);
            }
            string s = $"INSERT INTO {Constants.TasksTable} (Description, ExtraDetail, DesiredDateTime, ShouldRemind, ShouldRepeat, RepeatingDays, Type, ReminderSetting, NumDaysBeforeNotify) VALUES (@descParam,@extParam,@ddtParam,@remindParam,@repeatParam,@daysParam,@typeParam,@settingParam,@numDaysParam)";
                        
            var result = databaseManager.InsertUpdateDeleteWithParams(s, dictionaryRepresentationForTask(task));
            if (result != 1)
            {
                //exception handling
            }

            AddToNotifyIfNeeded(task);
        }
        private Dictionary<string, object> dictionaryRepresentationForTask(Task task)
        {
            task.ExtraDetail = task.ExtraDetail == null ? "-" : task.ExtraDetail;
            var repeatingDays = task.RepeatingDays != null ? string.Join(",", task.RepeatingDays) : "";

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@descParam", task.Description);
            dict.Add("@extParam", task.ExtraDetail);
            dict.Add("@ddtParam", task.DesiredDateTime.ToBinary());
            dict.Add("@remindParam", task.ShouldRemind);
            dict.Add("@repeatParam", task.ShouldRepeat);
            dict.Add("@daysParam", repeatingDays);
            dict.Add("@typeParam", task.Type);
            dict.Add("@settingParam", task.ReminderSetting);
            dict.Add("@numDaysParam", task.NumDaysBeforeNotify);

            return dict;
        }
        private Task taskFromDataRow(DataRow dr)
        {
            Task t = new Task();
            t.Description = dr["Description"].ToString();
            t.ExtraDetail = dr["ExtraDetail"].ToString();
            t.DesiredDateTime = DateTime.FromBinary(long.Parse(dr["DesiredDateTime"].ToString()));
            t.ShouldRemind = int.Parse(dr["ShouldRemind"].ToString()) != 0;
            t.ShouldRepeat = int.Parse(dr["ShouldRepeat"].ToString()) != 0;
            t.Type = int.Parse(dr["Type"].ToString()) == 1 ? Task.TaskType.Reminder : Task.TaskType.Schedule;
            t.TaskId = int.Parse(dr["TaskId"].ToString());
            t.NumDaysBeforeNotify = int.Parse(dr["NumDaysBeforeNotify"].ToString());

            var setting = int.Parse(dr["ReminderSetting"].ToString());
            switch (setting)
            {
                case 1:
                    t.IsFromSavedTime = true;
                    t.IsAtSetInterval = false;
                    t.IsAtSpecificTime = false;
                    break;
                case 2:
                    t.IsFromSavedTime = false;
                    t.IsAtSetInterval = true;
                    t.IsAtSpecificTime = false;
                    break;
                case 3:
                    t.IsFromSavedTime = false;
                    t.IsAtSetInterval = false;
                    t.IsAtSpecificTime = true;
                    break;
                default:
                    t.IsFromSavedTime = false;
                    t.IsAtSetInterval = false;
                    t.IsAtSpecificTime = false;
                    break;
            }

            var days = dr["RepeatingDays"].ToString().Split(',');
            if (days != null && days.Count() > 0)
            {
                foreach (var day in days)
                {
                    if (string.IsNullOrWhiteSpace(day)) continue;
                    t.RepeatingDays.Add(Convert.ToInt32(day));
                }
            }
            else
            {
                t.RepeatingDays = null;
            }

            return t;
        }
        #endregion

    }
}

using Reminderer.Framework;
using Reminderer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;

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

            createTasksTable();
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
        private object timer;
        private ObservableCollection<object> _notifications;
        private int _tickRate;

        #endregion

        #region Methods
        private void updateNotifications()
        {

        }

        private void calculateAppropriateTickRate()
        {

        }

        private void updateTickRate()
        {

        }
        
        public void LoadTasks()
        {
            string s = $"SELECT * FROM {Constants.TasksTable}";

            var ds = databaseManager.ReadData(s);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                Task t = taskFromDataRow(dr);
                if (t.Type == 1)
                {
                    Reminders.Add(t);
                }
                else
                {
                    Schedules.Add(t);
                }
            }

        }

        private void createTasksTable()
        {
            string s  = $"SELECT name FROM sqlite_master WHERE name='{Constants.TasksTable}'";

            var result = databaseManager.ExecuteScalarCommand(s);

            if (result != null && string.Equals(result.ToString(), Constants.TasksTable))
            {
                Console.WriteLine("Table exists already");
                return;
            }

            s = $"CREATE TABLE {Constants.TasksTable} (taskId INTEGER PRIMARY KEY AUTOINCREMENT, Description text, ExtraDetail text, DesiredDateTime text, ShouldRemind integer, ShouldRepeat integer, RepeatingDays text, Type int)";

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
        }

        public void EditTask(Task task)
        {
            Task prevTask;
            if (task.Type == 1)
            {
                prevTask = Reminders.Where(t => t.TaskId == task.TaskId).FirstOrDefault();
            }
            else
            {
                prevTask = Schedules.Where(t => t.TaskId == task.TaskId).FirstOrDefault();
            }
            prevTask = task;

            string s = $"UPDATE {Constants.TasksTable} SET Description=@descParam, ExtraDetail=@extParam, DesiredDateTime=@ddtParam, ShouldRemind=@remindParam, ShouldRepeat=@repeatParam, RepeatingDays=@daysParam, Type=@typeParam WHERE taskId=@taskIdParam";
            var dict = dictionaryRepresentationForTask(task);
            dict.Add("@taskIdParam", task.TaskId);
            var result = databaseManager.InsertUpdateDeleteWithParams(s, dict);

            if (result != 1)
            {
                //exception handling
            }
        }

        public void CreateTask(Task task)
        {
            if (task.Type == 1)
            {
                Reminders.Add(task);
            }
            else if (task.Type == 0)
            {
                Schedules.Add(task);
            }
            string s = $"INSERT INTO {Constants.TasksTable} (Description, ExtraDetail, DesiredDateTime, ShouldRemind, ShouldRepeat, RepeatingDays, Type) VALUES (@descParam,@extParam,@ddtParam,@remindParam,@repeatParam,@daysParam,@typeParam)";
                        
            var result = databaseManager.InsertUpdateDeleteWithParams(s, dictionaryRepresentationForTask(task));
            if (result != 1)
            {
                //exception handling
            }
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
            t.Type = int.Parse(dr["Type"].ToString());
            t.TaskId = int.Parse(dr["TaskId"].ToString());
            
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

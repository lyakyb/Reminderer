using Reminderer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Reminderer
{
    public class RemindererManager
    {
        //TODO: would it be better to store everything locally and only update to DB on app close/open?  
        private static readonly RemindererManager _instance = new RemindererManager();
        static RemindererManager()
        {
        }
        private RemindererManager()
        {
            _taskDatabaseManager = new TaskDatabaseManager();
            Schedules = new ObservableCollection<Task>();
            Reminders = new ObservableCollection<Task>();
            _taskDatabaseManager.CreateNewDatabase("test1");
            _taskDatabaseManager.ConnectToDatabase("test1");
        }

        public static RemindererManager Instance
        {
            get { return _instance; }
        }

        #region Properties
        private TaskDatabaseManager _taskDatabaseManager;
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
        
        public void UpdateTasks()
        {
            var tasks = _taskDatabaseManager.LoadSavedTasks();
            Reminders.Clear();
            Schedules.Clear();
            foreach (Task t in tasks)
            {
                if (t.Type == 0 && !Schedules.Contains(t))
                {
                    Schedules.Add(t);
                }
                else if (!Reminders.Contains(t))
                {
                    Reminders.Add(t);
                }
            }
        }

        public void DeleteTask(Task task)
        {
            _taskDatabaseManager.DeleteTask(task);
            if (Reminders.Contains(task))
            {
                Reminders.Remove(task);
            }
            else if (Schedules.Contains(task))
            {
                Schedules.Remove(task);
            }
        }

        public void DeleteTaskWithId(string id)
        {
            _taskDatabaseManager.DeleteTask(new Task() { TaskId= int.Parse(id)});
        }

        public void EditTask(Task task)
        {
            Task prevTask;
            _taskDatabaseManager.UpdateTask(task);
            if (task.Type == 1)
            {
                prevTask = Reminders.Where(t => t.TaskId == task.TaskId).FirstOrDefault();
            }
            else
            {
                prevTask = Schedules.Where(t => t.TaskId == task.TaskId).FirstOrDefault();
            }
            prevTask = task;
        }

        public void CreateTask(Task task)
        {
            _taskDatabaseManager.InsertTask(task);
            if (task.Type == 1)
            {
                Reminders.Add(task);
            } else if (task.Type == 0)
            {
                Schedules.Add(task);
            }
        }

               
        #endregion

    }
}

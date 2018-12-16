using Reminderer.Framework;
using Reminderer.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Reminderer
{
    class TaskDatabaseManager : DatabaseManager
    {

        public const string TASK_DB_TABLE_NAME = "Tasks";

        public void InsertTasks(IList<Task> tasks)
        {
            foreach (Task task in tasks)
            {
                InsertTask(task);
            }
        }

        public void InsertTask(Task task)
        {
            string sqlCommand = $"insert into {TASK_DB_TABLE_NAME} (Description, ExtraDetail, DesiredDateTime, ShouldRemind, ShouldRepeat, RepeatingDays, Type) VALUES (@descParam,@extParam,@ddtParam,@remindParam,@repeatParam,@daysParam,@typeParam)";
            SQLiteCommand command = new SQLiteCommand(sqlCommand, this.DbConnection);
            command.Parameters.AddWithValue("@descParam", task.Description);
            command.Parameters.AddWithValue("@extParam", task.ExtraDetail);
            command.Parameters.AddWithValue("@ddtParam", task.DesiredDateTime.ToBinary());
            command.Parameters.AddWithValue("@remindParam", task.ShouldRemind);
            command.Parameters.AddWithValue("@repeatParam", task.ShouldRepeat);
            if (task.RepeatingDays != null)
            {
                command.Parameters.AddWithValue("@daysParam", string.Join(",", task.RepeatingDays));
            }
            else
            {
                command.Parameters.AddWithValue("@daysParam", "");
            }
            command.Parameters.AddWithValue("@typeParam", task.Type);

            command.ExecuteNonQuery();
        }

        public void UpdateTask(Task task)
        {
            string sqlCommand = $"UPDATE {TASK_DB_TABLE_NAME} SET Description=@descParam, ExtraDetail=@extParam, DesiredDateTime=@ddtParam, ShouldRemind=@remindParam, ShouldRepeat=@repeatParam, RepeatingDays=@daysParam, Type=@typeParam WHERE taskId=@taskIdParam"; 
               
            SQLiteCommand command = new SQLiteCommand(sqlCommand, this.DbConnection);
            command.Parameters.AddWithValue("@descParam", task.Description);
            command.Parameters.AddWithValue("@extParam", task.ExtraDetail);
            command.Parameters.AddWithValue("@ddtParam", task.DesiredDateTime.ToBinary());
            command.Parameters.AddWithValue("@remindParam", task.ShouldRemind);
            command.Parameters.AddWithValue("@repeatParam", task.ShouldRepeat);
            command.Parameters.AddWithValue("@taskIdParam", task.TaskId);
            if (task.RepeatingDays != null)
            {
                command.Parameters.AddWithValue("@daysParam", string.Join(",", task.RepeatingDays));
            }
            else
            {
                command.Parameters.AddWithValue("@daysParam", "");
            }
            command.Parameters.AddWithValue("@typeParam", task.Type);

            command.ExecuteNonQuery();
        }

        public void DeleteTask(Task task)
        {
            string sqlCommand = $"DELETE FROM {TASK_DB_TABLE_NAME} WHERE taskId=@taskIdParam";
            SQLiteCommand command = new SQLiteCommand(sqlCommand, this.DbConnection);
            command.Parameters.AddWithValue("@taskIdParam", task.TaskId);

            command.ExecuteNonQuery();
        }

        public void CreateTasksTable()
        {
            string sqlCommand2 = $"SELECT name FROM sqlite_master WHERE name='{TASK_DB_TABLE_NAME}'";

            SQLiteCommand command2 = new SQLiteCommand(sqlCommand2, this.DbConnection); 
            var result = command2.ExecuteScalar();

            if (result != null && string.Equals(result.ToString(), TASK_DB_TABLE_NAME))
            {
                Console.WriteLine("Table exists already");
                return;
            }

            string sqlCommand = $"CREATE TABLE {TASK_DB_TABLE_NAME} (taskId INTEGER PRIMARY KEY AUTOINCREMENT, Description text, ExtraDetail text, DesiredDateTime text, ShouldRemind integer, ShouldRepeat integer, RepeatingDays text, Type int)";
            SQLiteCommand command = new SQLiteCommand(sqlCommand, this.DbConnection);
            command.ExecuteNonQuery();
        }

        public IList<Task> LoadSavedTasks()
        {
            string sql = $"select * from {TASK_DB_TABLE_NAME}";
            SQLiteCommand command = new SQLiteCommand(sql, this.DbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<Task> tasks = new List<Task>();

            while (reader.Read())
            {
                Task t = new Task();
                t.TaskId = int.Parse(reader["taskId"].ToString());
                t.Description = reader["Description"].ToString();
                t.ExtraDetail = reader["ExtraDetail"].ToString();
                t.DesiredDateTime = DateTime.FromBinary(long.Parse(reader["DesiredDateTime"].ToString()));
                t.ShouldRemind = int.Parse(reader["ShouldRemind"].ToString()) != 0;
                t.ShouldRepeat = int.Parse(reader["ShouldRepeat"].ToString()) != 0;
                t.Type = int.Parse(reader["Type"].ToString());
                var days = reader["RepeatingDays"].ToString().Split(',');
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

                tasks.Add(t);
            }

            return tasks;
        }

    }
}

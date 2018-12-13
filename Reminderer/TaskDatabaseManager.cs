﻿using Reminderer.Framework;
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
            string sqlCommand = $"insert into {TASK_DB_TABLE_NAME} (Description, Importance, DesiredDateTime, ShouldRemind, ShouldRepeat, RepeatingDays) VALUES (@descParam,@impParam,@ddtParam,@remindParam,@repeatParam,@daysParam)";
            SQLiteCommand command = new SQLiteCommand(sqlCommand, this.DbConnection);
            command.Parameters.AddWithValue("@descParam", task.Description);
            command.Parameters.AddWithValue("@impParam", task.Importance);
            command.Parameters.AddWithValue("@ddtParam", task.DesiredDateTime.ToLongTimeString());
            command.Parameters.AddWithValue("@remindParam", task.ShouldRemind);
            command.Parameters.AddWithValue("@repeatParam", task.ShouldRepeat);
            command.Parameters.AddWithValue("@daysParam", string.Join(",", task.RepeatingDays));

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

            string sqlCommand = $"CREATE TABLE {TASK_DB_TABLE_NAME} (Description text, Importance integer, DesiredDateTime text, ShouldRemind integer, ShouldRepeat integer, RepeatingDays text)";
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
                t.Description = reader["Description"].ToString();
                t.Importance = int.Parse(reader["Importance"].ToString());
                t.DesiredDateTime = DateTime.Parse(reader["DesiredDateTime"].ToString());
                t.ShouldRemind = int.Parse(reader["ShouldRemind"].ToString()) != 0;
                t.ShouldRepeat = int.Parse(reader["ShouldRepeat"].ToString()) != 0;
                foreach (var day in reader["RepeatingDays"].ToString().Split(','))
                {
                    t.RepeatingDays.Add(day.ToCharArray().First());
                }

                tasks.Add(t);
            }

            return tasks;
        }

    }
}
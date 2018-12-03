using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Reminderer.Models;

namespace Reminderer.Framework
{
    class DatabaseManager
    {
        SQLiteConnection m_dbConnection;

        public DatabaseManager()
        {

        }

        public void CreateNewDatabase(string database)
        {
            SQLiteConnection.CreateFile($"{database}.sqlite");
        }

        public void ConnectToDatabase(string databBase)
        {
            m_dbConnection = new SQLiteConnection($"Data Source={databBase}.sqlite;Version=3;");
            m_dbConnection.Open();
        }

        public void CreateTable(string tableName, Dictionary<string , string> tableHeaders)
        {
            string commandString = $"CREATE TABLE {tableName} (";
            int iterator = 1;


            foreach(string key in tableHeaders.Keys)
            {
                commandString = $"{commandString}{key} {tableHeaders[key]}";

                if (tableHeaders.Count > iterator)
                {
                    iterator++;
                    commandString = commandString + ", ";
                }
            }
           
            SQLiteCommand command = new SQLiteCommand($"{commandString})", m_dbConnection);
        }

        public void ExecuteCommand(string commandString)
        {
            SQLiteCommand command = new SQLiteCommand(commandString, m_dbConnection);
            command.ExecuteNonQuery();
        }


        #region Task Specific Members

        public void Insert(string table, IList<Task> tasks)
        {
            foreach(Task task in tasks)
            {
                string sqlCommand = $"insert into {table} (Description, Importance, DesiredDateTime, ShouldRemind, ShouldRepeat, RepeatingDays) VALUES (@descParam,@impParam,@ddtParam,@remindParam,@repeatParam,@daysParam)";
                SQLiteCommand command = new SQLiteCommand(sqlCommand, m_dbConnection);
                command.Parameters.AddWithValue("@descParam", task.Description);
                command.Parameters.AddWithValue("@impParam", task.Importance);
                command.Parameters.AddWithValue("@ddtParam", task.DesiredDateTime.ToLongTimeString());
                command.Parameters.AddWithValue("@remindParam", task.ShouldRemind);
                command.Parameters.AddWithValue("@repeatParam", task.ShouldRepeat);
                command.Parameters.AddWithValue("@daysParam", string.Join(",",task.RepeatingDays));

                command.ExecuteNonQuery();
            }
        }

        public void CreateTasksTable(string table)
        {
            string sqlCommand = $"CREATE TABLE {table} (Description text, Importance integer, DesiredDateTime text, ShouldRemind integer, ShouldRepeat integer, RepeatingDays text)";
            SQLiteCommand command = new SQLiteCommand(sqlCommand, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public IList<Task> LoadSavedTasks(string table)
        {
            string sql = $"select * from {table}";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<Task> tasks = new List<Task>();

            while(reader.Read())
            {
                Task t = new Task();
                t.Description = reader["Description"].ToString();
                t.Importance = int.Parse(reader["Importance"].ToString());
                t.DesiredDateTime = DateTime.Parse(reader["DesiredDateTime"].ToString());
                Console.WriteLine($"should remind: {reader["ShouldRemind"].ToString()}");
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
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Reminderer.Framework
{
    class DatabaseManager
    {
        SQLiteConnection m_dbConnection;
        public SQLiteConnection DbConnection { get { return m_dbConnection; } }

        public DatabaseManager()
        {

        }

        public void CreateNewDatabase(string database)
        {
            if (File.Exists($"{database}.sqlite"))
            {
                Console.WriteLine($"database with name {database} already exists");
                return;
            }


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

        public void DisconnectFromDatabase()
        {
            m_dbConnection.Close();
        }


    }
}

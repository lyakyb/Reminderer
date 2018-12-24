using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Reminderer.Framework
{
    public class DatabaseManager : IDatabaseManager
    {
        private SQLiteConnection _dbCon;
        private SQLiteCommand _dbCmd;
        private SQLiteDataReader _dbReader;
        private SQLiteDataAdapter _dbAdapter;
        private SQLiteConnectionStringBuilder _dbConStringBuilder;


        public SQLiteConnection DbConnection { get { return _dbCon; } }

        public DatabaseManager()
        {

        }

        public DatabaseManager(string dbName)
        {
            _dbConStringBuilder = new SQLiteConnectionStringBuilder();
            _dbConStringBuilder.DataSource = dbName;
            _dbConStringBuilder.FailIfMissing = false;
            _dbConStringBuilder.Version = 3;

            _dbCon = new SQLiteConnection();
            _dbCon.ConnectionString = _dbConStringBuilder.ConnectionString;
        }

        public int InsertUpdateDelete(string sqlCommandString)
        {
            try
            {
                EnsureConnectionOpen();
                _dbCmd = new SQLiteCommand
                {
                    Connection = _dbCon,
                    CommandText = sqlCommandString
                };

                return _dbCmd.ExecuteNonQuery();

            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                CloseConnection();
            }
        }

        public int InsertUpdateDeleteWithParams(string sqlStringCommand, Dictionary<string, object> parameters)
        {
            try
            {
                EnsureConnectionOpen();

                _dbCmd = new SQLiteCommand
                {
                    Connection = _dbCon,
                    CommandText = sqlStringCommand
                };

                foreach(KeyValuePair<string,object> pair in parameters)
                {
                    _dbCmd.Parameters.AddWithValue(pair.Key.ToString(), pair.Value);
                }               

                return _dbCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseConnection();
            }
        }

        public int InsertUpdateDeleteWithParamsGetLastInsertId(string sqlStringCommand, Dictionary<string, object> parameters)
        {
            try
            {
                EnsureConnectionOpen();

                _dbCmd = new SQLiteCommand
                {
                    Connection = _dbCon,
                    CommandText = sqlStringCommand
                };

                foreach (KeyValuePair<string, object> pair in parameters)
                {
                    _dbCmd.Parameters.AddWithValue(pair.Key.ToString(), pair.Value);
                }

                _dbCmd.ExecuteNonQuery();
                return Convert.ToInt32(_dbCon.LastInsertRowId);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataSet ReadData(string sqlCommandString)
        {
            try
            {
                EnsureConnectionOpen();
                DataSet ds = new DataSet();
                _dbCmd = new SQLiteCommand
                {
                    Connection = _dbCon,
                    CommandText = sqlCommandString
                };
                
                using(_dbAdapter = new SQLiteDataAdapter())
                {
                    _dbAdapter.SelectCommand = _dbCmd;
                    _dbAdapter.Fill(ds);

                    return ds;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseConnection();
            }
        } 

        public void ExecuteNonQueryCommand(string sqlCommandString)
        {
            try
            {
                EnsureConnectionOpen();
                _dbCmd = new SQLiteCommand
                {
                    Connection = _dbCon,
                    CommandText = sqlCommandString
                };
                _dbCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseConnection();
            }
        }

        public object ExecuteScalarCommand(string sqlCommandString)
        {
            try
            {
                EnsureConnectionOpen();
                _dbCmd = new SQLiteCommand
                {
                    Connection = _dbCon,
                    CommandText = sqlCommandString
                };
                return _dbCmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseConnection();
            }

        }

        private void EnsureConnectionOpen()
        {
            if (_dbCon.State != System.Data.ConnectionState.Open)
            {
                _dbCon.Open();
            }
        }

        private void CloseConnection()
        {
            if (_dbCon.State == System.Data.ConnectionState.Open)
            {
                _dbCon.Close();
            }
        }



    }
}

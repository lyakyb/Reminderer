using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reminderer.Framework;
using Reminderer.Models;

namespace Reminderer.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly IDatabaseManager _databaseManager;
        private int _lastInsertedId;

        public ReminderRepository(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
            CreateRemindersTable();
        }

        public void Add(Reminder reminder)
        {
            string s = $"INSERT INTO {Constants.RemindersTable} (Description, ExtraDetail, DesiredDateTime, ShouldRemind, ShouldRepeat, RepeatingDays, Type) VALUES (@descParam,@extParam,@ddtParam,@remindParam,@repeatParam,@daysParam,@typeParam)";
            var id = _databaseManager.InsertUpdateDeleteWithParamsGetLastInsertId(s, DictionaryRepresentationForReminder(reminder));

            reminder.Id = id;
            _lastInsertedId = id;
        }

        public void Delete(Reminder reminder)
        {
            string s = $"DELETE FROM {Constants.RemindersTable} WHERE Id=@idParam";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@idParam", reminder.Id);
            var result = _databaseManager.InsertUpdateDeleteWithParams(s, dict);
            if (result != 1)
            {
                //exception handling
            }
        }

        public int LastInsertedId()
        {
            return _lastInsertedId;
        }

        public List<Reminder> ReadAll()
        {
            List<Reminder> reminders = new List<Reminder>();

            string s = $"SELECT * FROM {Constants.RemindersTable}";
            var ds = _databaseManager.ReadData(s);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Reminder reminder = ReminderFromDataRow(dr);
                reminders.Add(reminder);
            }
            return reminders;
        }

        public void Update(Reminder reminder)
        {
            string query = $"UPDATE {Constants.RemindersTable} SET Description=@descParam, ExtraDetail=@extParam, DesiredDateTime=@ddtParam, ShouldRemind=@remindParam, ShouldRepeat=@repeatParam, RepeatingDays=@daysParam, Type=@typeParam WHERE id=@idParam";
            var dict = DictionaryRepresentationForReminder(reminder);

            dict.Add("@idParam", reminder.Id);
            var result = _databaseManager.InsertUpdateDeleteWithParams(query, dict);
            if (result != 1)
            {
                //exception handling
            }
        }


        private void CreateRemindersTable()
        {
            string s = $"SELECT name FROM sqlite_master WHERE name='{Constants.RemindersTable}'";

            var result = _databaseManager.ExecuteScalarCommand(s);

            if (result != null && string.Equals(result.ToString(), Constants.RemindersTable))
            {
                Console.WriteLine("Table exists already");
                return;
            }

            s = $"CREATE TABLE {Constants.RemindersTable} (id INTEGER PRIMARY KEY AUTOINCREMENT, Description Text, ExtraDetail text, DesiredDateTime text, ShouldRemind integer, ShouldRepeat integer, RepeatingDays text, Type int)";

            _databaseManager.ExecuteNonQueryCommand(s);
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
        private Dictionary<string, object> DictionaryRepresentationForReminder(Reminder reminder)
        {
            reminder.ExtraDetail = reminder.ExtraDetail == null ? "-" : reminder.ExtraDetail;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            var repeatingDays = reminder.RepeatingDays != null ? string.Join(",", reminder.RepeatingDays) : "";
            dict.Add("@descParam", reminder.Description);
            dict.Add("@extParam", reminder.ExtraDetail);
            dict.Add("@ddtParam", reminder.DesiredDateTime.ToBinary());
            dict.Add("@remindParam", reminder.ShouldRemind);
            dict.Add("@daysParam", repeatingDays);
            dict.Add("@typeParam", reminder.Type);
            dict.Add("@repeatParam", reminder.ShouldRepeat);

            return dict;
        }
    }
}

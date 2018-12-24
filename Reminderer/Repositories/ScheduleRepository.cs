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
    class ScheduleRepository : IScheduleRepository
    {
        private readonly IDatabaseManager _databaseManager;

        public ScheduleRepository(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
            CreateSchedulesTable();
        }

        public void Add(Schedule schedule)
        {
            string s = $"INSERT INTO {Constants.SchedulesTable} (Description, ExtraDetail, DesiredDateTime, ShouldRemind, NumDaysBeforeNotify) VALUES (@descParam,@extParam,@ddtParam,@remindParam,@numDaysParam)";
            var id = _databaseManager.InsertUpdateDeleteWithParamsGetLastInsertId(s, DictionaryRepresentationForSchedule(schedule));
            schedule.Id = id;
        }

        public void Delete(Schedule schedule)
        {
            string s = $"DELETE FROM {Constants.SchedulesTable} WHERE Id=@idParam";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@idParam", schedule.Id);
            var result = _databaseManager.InsertUpdateDeleteWithParams(s, dict);
            if (result != 1)
            {
                //exception handling
            }
        }

        public List<Schedule> ReadAll()
        {
            string s = $"SELECT * FROM {Constants.SchedulesTable}";
            List<Schedule> schedules = new List<Schedule>();
            var ds = _databaseManager.ReadData(s);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Schedule schedule = ScheduleFromDataRow(dr);
                schedules.Add(schedule);
            }
            return schedules;
        }

        public void Update(Schedule schedule)
        {
            string query = $"UPDATE {Constants.SchedulesTable} SET Description=@descParam, ExtraDetail=@extParam, DesiredDateTime=@ddtParam, ShouldRemind=@remindParam, NumDaysBeforeNotify=@numDaysParam WHERE id=@idParam";
            var dict = DictionaryRepresentationForSchedule(schedule);
            dict.Add("@idParam", schedule.Id);
            var result = _databaseManager.InsertUpdateDeleteWithParams(query, dict);
            if (result != 1)
            {
            }
        }


        private void CreateSchedulesTable()
        {
            string s = $"SELECT name FROM sqlite_master WHERE name='{Constants.SchedulesTable}'";

            var result = _databaseManager.ExecuteScalarCommand(s);

            if (result != null && string.Equals(result.ToString(), Constants.SchedulesTable))
            {
                Console.WriteLine("Table exists already");
                return;
            }

            s = $"CREATE TABLE {Constants.SchedulesTable} (id INTEGER PRIMARY KEY AUTOINCREMENT, Description text, ExtraDetail text, DesiredDateTime text, ShouldRemind integer, NumDaysBeforeNotify int)";

            _databaseManager.ExecuteNonQueryCommand(s);
        }
        private Dictionary<string, object> DictionaryRepresentationForSchedule(Schedule schedule)
        {
            schedule.ExtraDetail = schedule.ExtraDetail == null ? "-" : schedule.ExtraDetail;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@numDaysParam", schedule.NumDaysBeforeNotify);
            dict.Add("@descParam", schedule.Description);
            dict.Add("@extParam", schedule.ExtraDetail);
            dict.Add("@ddtParam", schedule.DesiredDateTime.ToBinary());
            dict.Add("@remindParam", schedule.ShouldRemind);

            return dict;
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
    }
}

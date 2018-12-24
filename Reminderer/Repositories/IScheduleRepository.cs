using Reminderer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer.Repositories
{
    public interface IScheduleRepository
    {
        void Add(Schedule schedule);
        void Delete(Schedule schedule);
        void Update(Schedule schedule);
        List<Schedule> ReadAll();
    }
}

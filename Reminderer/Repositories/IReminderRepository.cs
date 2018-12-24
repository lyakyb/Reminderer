using Reminderer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer.Repositories
{
    public interface IReminderRepository
    {
        void Add(Reminder reminder);
        void Delete(Reminder reminder);
        void Update(Reminder reminder);
        List<Reminder> ReadAll();
        int LastInsertedId();
    }
}

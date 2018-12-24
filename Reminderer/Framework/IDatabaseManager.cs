using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer.Framework
{
    public interface IDatabaseManager
    {
        int InsertUpdateDelete(string sqlCommandString);
        int InsertUpdateDeleteWithParams(string sqlCommandString, Dictionary<string, object> parameters);
        int InsertUpdateDeleteWithParamsGetLastInsertId(string sqlCommandString, Dictionary<string, object> parameters);
        DataSet ReadData(string sqlCommandString);
        void ExecuteNonQueryCommand(string sqlCommandString);
        object ExecuteScalarCommand(string sqlCommandString);
        
    }
}

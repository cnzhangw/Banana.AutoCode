using Banana.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.AutoCode
{
    public class DBService : BaseService<object>
    {
        public static DBService Instance { get; private set; }

        static DBService()
        {
            Instance = new DBService();
        }

        public DataTable GetDataTable(Sql sql)
        {
            return this.GetDataTable(sql, string.Empty);
        }

        public DataTable GetDataTable(Sql sql, string tableName)
        {
            var table = Service.GetDataTable(sql);
            if (tableName.HasValue())
                table.TableName = tableName;
            return table;
        }

    }
}

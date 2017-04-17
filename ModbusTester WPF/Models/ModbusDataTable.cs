using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTester_WPF.Models
{
    public class ModbusDataTable
    {
        public static DataTable CreateMbTable(string name)
        {
            var table = new DataTable(name);
            DataColumn column;
            DataRow row;

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "id";
            column.AutoIncrement = true;
            column.ReadOnly = true;
            column.Unique = true;
            table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "Запрос";
            column.AutoIncrement = false;
            column.Caption = "ParentItem";
            column.ReadOnly = false;
            column.Unique = false;
            table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "Ответ";
            column.AutoIncrement = false;
            column.Caption = "ParentItem";
            column.ReadOnly = false;
            column.Unique = false;
            table.Columns.Add(column);

            return table;
        }
    }
}

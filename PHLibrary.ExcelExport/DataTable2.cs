using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.ExcelExport
{
    public class DataTable2
    {
        public void AddColumn(DataColumn column) { 
            Columns.Add(column);
            }
        public IList<DataColumn> Columns { get;}
        
        public class DataColumn { 
            public string Name { get;set;}
            public string Format { get;set;}
            public Type DataType { get;set;}
            }
        public class DataRow { 
            
            }
    }
}

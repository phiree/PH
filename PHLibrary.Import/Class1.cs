using System;
using System.IO;
using FluentExcel;
using System.Data;
using FastMember;

namespace PHLibrary.Import
{
    public interface IImportor<T>
        where T:class
    {
        
        DataTable Import(Stream fileStream, FluentConfiguration<T> configuration);
    }
    public class ExcelImportor<T>:IImportor<T>
        where T:class,new()
    {
        
         
        public DataTable Import(Stream fileStream,FluentConfiguration<T> configuration )
        {
           
            var listT = Excel.Load<T>(fileStream);
            DataTable table = new DataTable();
            using (var reader = ObjectReader.Create(listT))
            {
                table.Load(reader);
            }
            return table;
            
            //convert t to datatable
        }
    }
    public interface IImportRepository
    {
        void Insert();
        void Update();
        void Delete();
        void Select();

    }
    public class ImportConfig
    { 
        public string ConnectionStringName { get; set; }
        public string TableName { get; set; }
    }
    public class ImportResult
    { 
        
    }
}

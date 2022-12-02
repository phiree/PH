using System;
using System.Collections.Generic;
using System.Data;

namespace PHLibrary.ExcelExportExcelCreator
{
    /// <summary>
    /// 二维列的定义
    /// </summary>
    public class TwoDimensionalX : IEquatable<TwoDimensionalX>
    {
        public string Name { get; set; }
        public string Guid { get; set; }

        public TwoDimensionalX(string name, string guid)
        {
            Name = name;
            Guid = guid;
        }
        public DataColumn BuildColumn()
        {
            DataColumn column = new DataColumn();
            column.Caption = Name;
            column.ColumnName = String.IsNullOrEmpty(Guid) ? Name : Guid;
            return column;
        }
        public string GetColumnName()
        {
          return string.IsNullOrEmpty(Guid)?Name:Guid;
        }
        public bool Equals(TwoDimensionalX x, TwoDimensionalX y)
        {
            return x.Name == y.Name && x.Guid == y.Guid;
        }

        public bool Equals(TwoDimensionalX other)
        {
            return Name == other.Name && Guid == other.Guid;
        }

        public override int GetHashCode()
        {
            return (Name + Guid).GetHashCode();
        }
    }

    /// <summary>
    /// 二维列的值
    /// 
    /// </summary>
    public class TwoDimensionalValue : IEquatable<TwoDimensionalValue>
    {
        public TwoDimensionalValue(string name, Guid? id)
        {
            Name = name;
            Id = id;
        }

        public TwoDimensionalValue(TwoDimensionalX twoDimensionalX, DataRow row)
        {
            this.Name = row[twoDimensionalX.Name].ToString();
            if (twoDimensionalX.Guid == null)
            {
                this.Id = null;
            }
            else
            {
                this.Id = new Guid(row[twoDimensionalX.Guid].ToString());
            }

        }
        public void SetFlattenRow(TwoDimensionalX twoDimensionalX, DataRow originalRow, DataRow flattenRow)
        {
            flattenRow[Name] = originalRow[twoDimensionalX.Name];
            if (Id != null)
            {
                flattenRow[Id.ToString()] = originalRow[twoDimensionalX.Guid];
            }
        }
        /// <summary>
        /// 值 如 M码
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 值的Id，如 guid 类型
        /// </summary>
        public Guid? Id { get; set; }

        public bool Equals(TwoDimensionalValue other)
        {
            return other.Name == Name && other.Id == Id;
        }
        public override int GetHashCode()
        {
            return (Name + Id).GetHashCode();
        }
    }
}

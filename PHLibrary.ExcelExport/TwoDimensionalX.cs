using System;
using System.Collections.Generic;

namespace PHLibrary.ExcelExportExcelCreator
{
    public class TwoDimensionalX: IEquatable<TwoDimensionalX>
    {
        public string Name { get;set;}
        public string Guid { get;set;}

        public TwoDimensionalX(string name, string guid)
        {
            Name = name;
            Guid = guid;
        }
          public bool Equals(TwoDimensionalX x, TwoDimensionalX y)
        {
            return x.Name==y.Name&&x.Guid==y.Guid;
        }

        public bool Equals(TwoDimensionalX other)
        {
           return Name==other.Name&&Guid==other.Guid;
        }

        public override int GetHashCode()
        {
            return (Name+Guid).GetHashCode();
        }
    }
}

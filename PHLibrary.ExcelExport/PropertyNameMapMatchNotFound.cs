using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.ExcelExportExcelCreator
{
    /// <summary>
    /// 类属性名映射表中没有知道
    /// </summary>
   public class PropertyMapMatchNotFound:Exception
    {
        string propertyName;
public PropertyMapMatchNotFound(string propertyName)
        {
            this.propertyName = propertyName;
        }
        public override string Message => "没有找到属性对应的映射名称";
    }
}

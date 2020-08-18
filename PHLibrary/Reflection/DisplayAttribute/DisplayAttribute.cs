using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace PHLibrary.Reflection.DisplayAttribute
{
    public static class ReflectHelper
    {
        /// <summary>
        /// 获取属性的displayname属性
        /// </summary>
        /// <param name="t"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this MemberInfo member, bool isRequired)
     where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0} attribute must be defined on member {1}",
                        typeof(T).Name,
                        member.Name));
            }

            return (T)attribute;
        }

        public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            var attr = memberInfo.GetAttribute<DisplayNameAttribute>(false);
            if (attr == null)
            {
                return memberInfo.Name;
            }

            return attr.DisplayName;
        }

        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            Debug.Assert(propertyExpression != null, "propertyExpression != null");
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }
           
            if (memberExpr != null && memberExpr.Member. MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }

        public static IDictionary<string, string> GetPropertyMaps<T>() //where T : class
        {
            var propertyInfos = typeof(T).GetProperties();
            var map = new Dictionary<string, string>();
            foreach (var p in propertyInfos)
            {
                var attr = p.GetAttribute<DisplayNameAttribute>(false);
                string display = attr == null ? p.Name : attr.DisplayName;
                map.Add(p.Name, display);
            }
            return map;

        }
    }
}

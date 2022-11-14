using System;
using System.Collections.Generic;
using System.Dynamic;

namespace PHLibrary.ExcelExportExcelCreator
{
    public static class DictToExpando
    {
        /// <summary>
        /// Extension method that turns a dictionary to an ExpandoObject, recursively (sub-dictionaries and sub-collections are also turned into ExpandoObjects).
        /// If TKey is not the String type and no keyTransformer has been provided, an InvalidCastException will be thrown (unless the dictionary is empty). For
        /// sub-dictionaries, if an Exception occurs during the transformation of dictionary to ExpandoObject, the sub-dictionary is left as is and not transformed.
        /// </summary>
        /// <param name="dictionary">The dictionary to transform into an ExpandoObject</param>
        /// <param name="keyTransformer">An optional delegate function who will be passed each dictionary key and must return the corresponding string key</param>
        /// <returns>The ExpandoObject</returns>
        /// <throws>InvalidCastException when a non-string key has been found by the default keyTransformer</throws>
        public static ExpandoObject ToExpando<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<object, string> keyTransformer = null)
        {
            if (keyTransformer == null)
            {
                // When no keyTransformer has been provided, simply assume every key is a string (will throw InvalidCastException when not the case)
                keyTransformer = delegate (object key) { return (string)key; };
            }
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                if (kvp.Value is System.Collections.IDictionary || (kvp.Value != null && kvp.Value.GetType().IsGenericType && kvp.Value.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                {
                    // if value is a dictionary (generic or non-generic), convert it to ExpandoObject
                    expandoDic.Add(keyTransformer(kvp.Key), TryConvertToExpandoIfDictionary(kvp.Value));
                }
                else if (kvp.Value != null && null != kvp.Value.GetType().GetInterface("System.Collections.ICollection"))
                {
                    // if value is a collection, convert it to ExpandoObject
                    expandoDic.Add(keyTransformer(kvp.Key), TryConvertToExpandoIfCollection(kvp.Value));
                }
                else
                {
                    expandoDic.Add(keyTransformer(kvp.Key), kvp.Value);
                }
            }
            return expando;
        }

        /// <summary>
        /// Extension method that turns a dictionary to an ExpandoObject, recursively (sub-dictionaries and sub-collections are also turned into ExpandoObjects).
        /// When a non-string key is found and no keyTransformer has been provided, an InvalidCastException will be thrown. For sub-dictionaries, if an Exception
        /// occurs during the transformation of dictionary to ExpandoObject, the sub-dictionary is left as is and not transformed.
        /// </summary>
        /// <param name="dictionary">The dictionary to transform into an ExpandoObject</param>
        /// <param name="keyTransformer">An optional delegate function who will be passed each dictionary key and must return the corresponding string key</param>
        /// <returns>The ExpandoObject</returns>
        /// <throws>InvalidCastException when a non-string key has been found by the default keyTransformer</throws>

        private static object TryConvertToExpandoIfDictionary(dynamic dictionary, Func<object, string> keyTransformer = null)
        {
            if (keyTransformer == null)
            {
                keyTransformer = delegate (object key) { return (string)key; };
            }
            try
            {
                if (dictionary is System.Collections.IDictionary || (dictionary.GetType().IsGenericType && dictionary.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                {
                    return dictionary.ToExpando(keyTransformer);
                }
            }
            catch (Exception) { }
            return dictionary;
        }

        private static object TryConvertToExpandoIfCollection(object collection, Func<object, string> keyTransformer = null)
        {
            try
            {
                if (null != collection.GetType().GetInterface("System.Collections.ICollection"))
                {
                    var newList = new List<object>();
                    foreach (var item in (System.Collections.ICollection)collection)
                    {
                        if (item is System.Collections.IDictionary || (item != null && item.GetType().IsGenericType && item.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                        {
                            newList.Add(TryConvertToExpandoIfDictionary(item, keyTransformer));
                        }
                        else if (item != null && null != item.GetType().GetInterface("System.Collections.ICollection"))
                        {
                            newList.Add(TryConvertToExpandoIfCollection(item, keyTransformer));
                        }
                        else
                        {
                            newList.Add(item);
                        }
                    }
                    return newList;
                }
            }
            catch (Exception) { }
            return collection;
        }
    }
}

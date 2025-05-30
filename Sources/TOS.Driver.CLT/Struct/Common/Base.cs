using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Apache.NMS.Util;

namespace TOS.Driver.CLT.Struct.Common
{

    /// <summary>
    /// CLT IoT Platform Common Base 
    /// </summary>
    public class Base
    {
        public string CallMethod = string.Empty;
        public string Name = string.Empty;

        public string getSendData()
        {
            var dic = new Dictionary<string, string>();

            var data = string.Empty;

            try
            {
                dic = getPropertyList(this);
               
                data = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception e)
            {
                return string.Empty;
            }

            return data;
        }

        public Dictionary<string, string> getPropertyList(object? obj, string groupName = "")
        {
            var dic = new Dictionary<string, string>();
            var fields = obj.GetType().GetProperties().ToList();

            fields.ForEach(field =>
            {
                var value = field.GetValue(obj);

                if (field.GetSetMethod() != null)
                {
                    var dicName = string.Empty;

                    if (string.IsNullOrEmpty(groupName))
                        dicName = field.Name;
                    else
                        dicName = groupName + "." + field.Name;

                    if (field.PropertyType == typeof(string) || field.PropertyType == typeof(int) ||
                        field.PropertyType == typeof(long) || field.PropertyType == typeof(double) ||
                        field.PropertyType == typeof(decimal) || field.PropertyType == typeof(DateTime))
                    {

                        if (value != null)
                            dic.Add(dicName, value.ToString());
                        else
                            dic.Add(dicName, string.Empty);
                    }
                    else
                    {
                        //var subObj = Activator.CreateInstance(field.PropertyType);
                        var subDic = getPropertyList(value, dicName);

                        if (subDic.Count > 0)
                        {
                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                            list.Add(dic);
                            list.Add(subDic);


                            dic = MergeDictionary(list);
                        }
                    }
                }
            });

            return dic;
        }

        public Dictionary<K, V> MergeDictionary<K, V>(IEnumerable<Dictionary<K, V>> dictionaries)
        {
            return dictionaries.SelectMany(x => x)
                .GroupBy(d => d.Key)
                .ToDictionary(x => x.Key, y => y.First().Value);
        }
    }

}

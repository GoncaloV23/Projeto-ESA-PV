using EasyFitHub.Models.Statistics;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace EasyFitHub.Utils
{
    /// <summary>
    /// AUTHOR: André P.
    /// Auxiliar ao Modelo StatisticEntity
    /// </summary>
    public class StatisticsUtils<T,E>
    {
       public static IList<StatisticEntity?> GetDictionaryToList(Dictionary<T,E> dic) {
            var list = new List<StatisticEntity?>();
            foreach (var k in dic.Keys)
                list.Add(new StatisticEntity { StringValue = $"{k}, {dic[k]}" });
            return list;
        }

        public static Dictionary<T, E> GetListToDictionary(IList<StatisticEntity> list)
        {
            var dic = new Dictionary<T, E>();
            foreach (var entity in list)
            {
                T key = GetT<T>(entity.EntityKey);
                E value = GetE<E>(entity.EntityValue);
                if(!dic.ContainsKey(key)) 
                    dic.Add(key, value);
            }
            return dic;
        }

        public static E GetE<E>(string value)
        {
            if (typeof(E) == typeof(int))
            {
                int intValue;
                if (int.TryParse(value, out intValue))
                    return (E)(object)intValue;
            }
            else if (typeof(E) == typeof(double))
            {
                double doubleValue;
                if (double.TryParse(value, out doubleValue))
                    return (E)(object)doubleValue;
            }
            else if (typeof(E) == typeof(string))
                return (E)(object)value;
            

            return default(E);
        }
        public static T GetT<T>(string value)
        {
            if (typeof(T) == typeof(int))
            {
                int intValue;
                if (int.TryParse(value, out intValue))
                    return (T)(object)intValue;
            }
            else if (typeof(T) == typeof(double))
            {
                double doubleValue;
                if (double.TryParse(value, out doubleValue))
                    return (T)(object)doubleValue;
            }
            else if (typeof(T) == typeof(string))
                return (T)(object)value;
            return default(T);
        }



    }
}

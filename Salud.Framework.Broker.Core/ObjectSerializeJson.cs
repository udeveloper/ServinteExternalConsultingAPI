using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salud.Framework.Broker.Core
{
    public static class ObjectSerializeJson
    {
        public static byte[] Serialize(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var json = JsonConvert.SerializeObject(obj);
            return Encoding.ASCII.GetBytes(json);
        }

        public static object DeSerialize(this byte[] arrBytes, Type type)
        {
            var json = Encoding.Default.GetString(arrBytes);
            return JsonConvert.DeserializeObject(json, type);
        }

        public static string DeSerializeText(this byte[] arrBytes)
        {
            return Encoding.Default.GetString(arrBytes);
        }

        public static byte[] SerializeText(this string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }
    }
}

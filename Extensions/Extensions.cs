using System.IO;
using Newtonsoft.Json;

namespace FreedomFridayServerless.Extensions
{
    public static class Ext
    {
        public static T Deserialize<T>(this Stream s)
        {
            using (var reader = new StreamReader(s))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var ser = new JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            }
        }  
    }
}
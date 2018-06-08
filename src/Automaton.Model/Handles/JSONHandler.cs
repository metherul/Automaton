using Newtonsoft.Json;
using System;

namespace Automaton.Model.Handles
{
    public class JSONHandler
    {
        /// <summary>
        /// Deserialize json data into object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        public static T DeserializeJson<T>(string jsonContent)
        {
            T deserializedJson = JsonConvert.DeserializeObject<T>(jsonContent);

            return deserializedJson;
        }

        public static T TryDeserializeJson<T>(string jsonContent, out string parseError)
            where T : new()
        {
            parseError = string.Empty;

            try
            {
                return DeserializeJson<T>(jsonContent);
            }
            catch (Exception e)
            {
                parseError = e.Message;

                return new T();
            }
        }

        /// <summary>
        /// Serialize object of type T into string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static string SerializeJson<T>(T jsonObject)
        {
            return JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
        }
    }
}
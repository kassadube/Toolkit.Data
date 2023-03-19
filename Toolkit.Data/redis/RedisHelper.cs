
using ProtoBuf;
using StackExchange.Redis;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace Toolkit.Data
{
    public static class RedisHelper
    {
        private const string _lastSpExecDateTimeKey = "LastSpExecDateTime";
        private const string _lastDataUpdateDateTimeKey = "LastDataUpdateDateTime";

        public static object SetLastSpExecDateTime(IConnectionMultiplexer connectionMultiplexer, bool sendPubEvent)
        {
            var db = connectionMultiplexer.GetDatabase();
            //db.StringSet(_lastSpExecDateTimeKey, DalMngr.LastExecDttm.ToString("O"));
            //if (sendPubEvent)
            //{
            //    var sub = connectionMultiplexer.GetSubscriber();
            //    sub.PublishAsync($"db{db.Database}_{_lastSpExecDateTimeKey}", DalMngr.LastExecDttm.ToString("O"));
            //}
            //Log.Information(new LogEvent(Program._name, $"RedisHelper.SetLastDateTimeToRedis (Updated)").ToString());
            return null;
        }

        public static object SetLastDataUpdateDateTime(IConnectionMultiplexer connectionMultiplexer, bool sendPubEvent)
        {
            var db = connectionMultiplexer.GetDatabase();
            //db.StringSet(_lastDataUpdateDateTimeKey, DalMngr.LastExecDttm.ToString("O"));
            //if (sendPubEvent)
            //{
            //    var sub = connectionMultiplexer.GetSubscriber();
            //    sub.PublishAsync($"db{db.Database}_{_lastDataUpdateDateTimeKey}", DalMngr.LastExecDttm.ToString("O"));
            //}
            //Log.Information(new LogEvent(Program._name, $"RedisHelper.SetLastDateTimeToRedis (Updated)").ToString());
            return null;
        }

        public static object GetLastDateTimeFromRedis(IConnectionMultiplexer connectionMultiplexer, bool sendPubEvent = false)
        {
            var db = connectionMultiplexer.GetDatabase();
            var dttmString = db.StringGet(_lastSpExecDateTimeKey);
            if (dttmString.IsNullOrEmpty)
                return DateTime.MinValue;
            return DateTime.ParseExact(dttmString, "O", CultureInfo.InvariantCulture);
        }

        public static byte[] SerializeProtobuf(object o)
        {
            if (o == null)
            {
                return null;
            }
            
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }


        public static T DeserializeProtobuf<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                T result = Serializer.Deserialize<T>(memoryStream);
                return result;
            }
        }
        public static string SerializeJson(object o)
        {
            if (o == null)
            {
                return null;
            }
            string jsonString = JsonSerializer.Serialize(o);
            return jsonString;                
        }

        public static T DeserializeJson<T>(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return default(T);
            }

            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            
                T result = JsonSerializer.Deserialize<T>(val);
                return result;
           
        }

    }
}
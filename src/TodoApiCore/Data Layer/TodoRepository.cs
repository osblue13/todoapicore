using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;

namespace TodoApiCore.Models
{
    public class TodoRepository : ITodoRepository
    {
        private static ConcurrentDictionary<string, TodoItem> _todos = new ConcurrentDictionary<string, TodoItem>();
        private static ConnectionMultiplexer _redis;
        private static IDatabase _db;
        private static string _redisHashKey = "todoitems";

        public TodoRepository()
        {
            SetupRedisConnection();

            Add(new TodoItem { Name = "Add Redis Support" });
            Add(new TodoItem { Name = "Connect to a different container" });
        }

        public IEnumerable<TodoItem> GetAll()
        {
            Dictionary<string, TodoItem> todos = new Dictionary<string, TodoItem>();
            var result = _db.HashGetAll(_redisHashKey);

            foreach (var item in result)
            {
                todos.Add(item.Name, JsonConvert.DeserializeObject<TodoItem>(item.Value));
            }

            return todos.Values;

            //return _todos.Values;
        }

        public void Add(TodoItem item)
        {
            item.Key = Guid.NewGuid().ToString();
            _db.HashSet(_redisHashKey, item.Key, JsonConvert.SerializeObject(item));

            //_todos[item.Key] = item;
        }

        public TodoItem Find(string key)
        {
            return JsonConvert.DeserializeObject<TodoItem>(_db.StringGet(key));

            //_todos.TryGetValue(key, out item);
            //return item;
        }

        public void Remove(string key)
        {
            //TodoItem item;
            //_todos.TryRemove(key, out item);
            //return item;

            _db.HashDelete(_redisHashKey, key);
        }

        public void Update(TodoItem item)
        {
            _db.HashSet(_redisHashKey, item.Key, JsonConvert.SerializeObject(item));
        }

        public void LogToRedis(string value)
        {
            TodoItem item = new TodoItem
            {
                Key = Guid.NewGuid().ToString(),
                Name = value.ToString(),
                IsComplete = false

            };

            _db.HashSet(_redisHashKey, item.Key, JsonConvert.SerializeObject(item));
        }

        private bool IsIpAddress(string host)
        {
            string ipPattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
            return Regex.IsMatch(host, ipPattern);
        }

        private void SetupRedisConnection()
        {
            // connection by name currently does not work with redis and .net core therefore we have to explictly set the ip 
            // see https://github.com/StackExchange/StackExchange.Redis/issues/410 for issue and workaround

            ConfigurationOptions config = ConfigurationOptions.Parse("redis");

            try
            {
                DnsEndPoint addressEndpoint = config.EndPoints.First() as DnsEndPoint;
                int port = addressEndpoint.Port;

                bool isIp = IsIpAddress(addressEndpoint.Host);
                if (!isIp)
                {
                    //Please Don't use this line in blocking context. Please remove ".Result"
                    //Just for test purposes
                    IPHostEntry ip = Dns.GetHostEntryAsync(addressEndpoint.Host).Result;
                    config.EndPoints.Remove(addressEndpoint);
                    config.EndPoints.Add(ip.AddressList.First(), port);
                }
            }
            catch (Exception ex)
            {

            }

            _redis = ConnectionMultiplexer.Connect(config);
            _db = _redis.GetDatabase();
        }
    }
}

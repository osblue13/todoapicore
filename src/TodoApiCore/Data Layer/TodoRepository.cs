using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApiCore.Models
{
    public class TodoRepository : ITodoRepository
    {
        private static ConcurrentDictionary<string, TodoItem> _todos = new ConcurrentDictionary<string, TodoItem>();
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        private static IDatabase db = redis.GetDatabase();
        private static string redisHashKey = "todoitems";

        public TodoRepository()
        {
            Add(new TodoItem { Name = "Add Redis Support" });
            Add(new TodoItem { Name = "Connect to a different container" });            
        }

        public IEnumerable<TodoItem> GetAll()
        {
            Dictionary<string, TodoItem> todos = new Dictionary<string, TodoItem>();            
            var result = db.HashGetAll(redisHashKey);

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
            db.HashSet(redisHashKey, item.Key, JsonConvert.SerializeObject(item));

            //_todos[item.Key] = item;
        }

        public TodoItem Find(string key)
        {
            return JsonConvert.DeserializeObject<TodoItem>(db.StringGet(key));
                        
            //_todos.TryGetValue(key, out item);
            //return item;
        }

        public void Remove(string key)
        {
            //TodoItem item;
            //_todos.TryRemove(key, out item);
            //return item;

            db.HashDelete(redisHashKey, key);
        }

        public void Update(TodoItem item)
        {
            db.HashSet(redisHashKey, item.Key, JsonConvert.SerializeObject(item));
        }

    }
    
}

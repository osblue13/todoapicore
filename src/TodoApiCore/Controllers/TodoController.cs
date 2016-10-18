using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApiCore.Models;
using Nest;

//http://192.168.99.100:9200/"

namespace TodoApiCore.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        public ITodoRepository TodoItems { get; set; }
        public Uri _node;
        public ConnectionSettings _settings;
        public ElasticClient _client;

        public TodoController(ITodoRepository todoItems)
        {
            TodoItems = todoItems;   
        }

        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            SetupElkConnection();
            Log("GET ALL");

            return TodoItems.GetAll();
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(string id)
        {
            SetupElkConnection();
            Log("GET");

            var item = TodoItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {
            SetupElkConnection();
            Log("POST");

            if (item == null)
            {
                return BadRequest();
            }

            TodoItems.Add(item);
            return CreatedAtRoute("GetTodo", new { id = item.Key }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] TodoItem item)
        {
            SetupElkConnection();
            Log("PUT");

            if (item == null || item.Key != id)
            {
                return BadRequest();
            }

            var todo = TodoItems.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            TodoItems.Update(item);
            return new NoContentResult();
        }

        [HttpPatch("{id}")]
        public IActionResult Update([FromBody] TodoItem item, string id)
        {
            SetupElkConnection();
            Log("PATCH");

            if (item == null)
            {
                return BadRequest();
            }

            var todo = TodoItems.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            item.Key = todo.Key;

            TodoItems.Update(item);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            SetupElkConnection();
            Log("DELETE");

            var todo = TodoItems.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            TodoItems.Remove(id);
            return new NoContentResult();
        }

        public void SetupElkConnection()
        {
            try
            {
                
                var host = $"http://{Request.Host.Host}:9200";

                _node = new Uri(host);
                _settings = new ConnectionSettings(_node);
                _client = new ElasticClient(_settings);
            }
            catch (Exception ex)
            {
                TodoItems.LogToRedis(ex.ToString());
            }
        }

        public void Log(string action)
        {
            try
            {
                var log = new Log
                {
                    Key = Guid.NewGuid().ToString(),
                    TimeStamp = DateTime.Now.ToString(),
                    ControllerAction = $"{action} /api/todo"
                };

                var response = _client.Index(log, idx => idx.Index("apilogger"));
            }
            catch (Exception ex)
            {
                TodoItems.LogToRedis(ex.ToString());
            }            
        }
    }
}

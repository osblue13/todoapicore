using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApiCore.Models;
using Nest;
using Microsoft.AspNetCore.Authorization;

//http://192.168.99.100:9200/"

namespace TodoApiCore.Controllers
{
    [Route("api/[controller]")]
    public class LogController : Controller {
        public ITodoRepository TodoItems { get; set; }

        public LogController(ITodoRepository todoItems)
        {
            TodoItems = todoItems;
        }

        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            TodoItems.Add(item);
            return CreatedAtRoute("GetTodo", new { id = item.Key }, item);
        }
    }
}
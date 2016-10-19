using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TodoApiCore.Controllers
{
    [Route("claims")]
    public class ClaimsController : Controller
    {
        [Authorize]
        [HttpGet("claims")]
        public object Claims()
        {
            return User.Claims.Select(c => 
            new 
            {
                Type = c.Type,
                Value = c.Value
            });
        }

    }
}
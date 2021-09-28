using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("Identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        public IActionResult Get()
        {
            return new JsonResult(User.Claims.Select(x => new {x.Type, x.Value}));
        }
    }
}
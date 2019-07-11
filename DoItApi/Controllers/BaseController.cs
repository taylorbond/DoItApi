using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace DoItApi.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        public List<Claim> Claims => User.Claims.ToList();

        public string UserId
        {
            get
            {
                if (User == null) return null;
                var claims = User.Claims;
                var userId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return userId?.Value;
            }
        }
    }
}
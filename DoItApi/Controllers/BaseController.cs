using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace DoItApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        public List<Claim> Claims => User.Claims.ToList();
        public string UserId => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}
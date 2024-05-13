using Tel.Api.Filters;
using TelServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tel.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(CustomExceptionFilterAttribute))]
    public class BaseController : ControllerBase
    {
        protected ApiResponse ApiResponse = new ApiResponse();
    }
}

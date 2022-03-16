using Microsoft.AspNetCore.Mvc;

namespace First_Project.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class Testing : ControllerBase
    {
        [HttpGet]
        public String Test() {
            return "Test";
        }
    }
} 
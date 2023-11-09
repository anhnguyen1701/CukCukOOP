using CukCukOOP.Model;
using Microsoft.AspNetCore.Mvc;

namespace CukCukOOP.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeeController : BaseController<Employee>
    {
    }
}

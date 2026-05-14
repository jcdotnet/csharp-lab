using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.WebAPI.Controllers;

/// <summary>
/// 
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class DemoController : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetActionMethod()
    {
        return "Hello, World!";
    }
}

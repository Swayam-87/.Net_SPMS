using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    [HttpGet("summary")]
    public IActionResult GetSummary([FromQuery] string role, [FromQuery] int userId)
    {
        if (role == "Admin")
        {
        }
        else if (role == "Faculty")
        {
        }
        else
        {
        }
        return Ok();
    }
}

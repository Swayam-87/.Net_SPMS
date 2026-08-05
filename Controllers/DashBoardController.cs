using Microsoft.AspNetCore.Mvc;
using SPM.Data;

[ApiController]
[Route("api/[controller]")]
public class DashBoardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashBoardController(AppDbContext context)
    {
        _context = context;
    }
}

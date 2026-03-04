using CodePodium.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodePodium.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContestsController(ContestService contestService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? platform,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var (items, total) = await contestService.GetPagedContestsAsync(platform, page, pageSize);
        return Ok(new { items, total, page, pageSize });
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming() =>
        Ok(await contestService.GetUpcomingContestsAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var contest = await contestService.GetContestByIdAsync(id);
        return contest is null ? NotFound() : Ok(contest);
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        await contestService.SyncContestsAsync();
        return Ok(new { message = "Sync completed." });
    }
}

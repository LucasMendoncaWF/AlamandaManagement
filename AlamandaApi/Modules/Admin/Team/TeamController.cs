using AlamandaApi.Services.CRUD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Services.Team {
  [ApiController]
  [Route("[controller]")]
  public class TeamController : ControllerBase {
    private readonly TeamService _teamService;

    public TeamController(TeamService teamService) {
      _teamService = teamService;
    }

    [HttpPost("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] TeamMemberCreationModel member) {
      try {
        var response = await _teamService.Create(member);
        return Ok(new { response });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpPut("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromBody] TeamMemberEditModel member) {
      try {
        var result = await _teamService.Update(member);
        return Ok(new { result });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }


    [HttpGet("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll([FromQuery] ListQueryParams query) {
      try {
        var result = await _teamService.GetAll(query);
        return Ok(result);
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }  

    [HttpDelete("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete([FromQuery] int id) {
      try {
        await _teamService.Delete(id);
        return Ok(new { success = true });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    } 
  }
}

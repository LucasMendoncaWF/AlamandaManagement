using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Services.Team
{
  [ApiController]
  [Route("[controller]")]
  public class TeamController : ControllerBase
  {
    private readonly TeamService _teamService;

    public TeamController(TeamService teamService)
    {
      _teamService = teamService;
    }

    [HttpPost("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] TeamMemberCreationModel member)
    {
      await _teamService.Create(member);
      var newMember = await _teamService.GetBySocial(member.Social);
      return Ok(new { newMember });
    }

    [HttpPut("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromBody] TeamMemberModel member)
    {
      var existingUser = await _teamService.GetById(member.Id);

      if (existingUser == null)
      {
        return BadRequest("Usuário não existe!");
      }

      await _teamService.Update(member);
      return Ok(new { member });
    }


    [HttpGet("")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
      var members = await _teamService.GetAll();
      return Ok(new { members });
    }  
  }
}

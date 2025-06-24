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
    public async Task<IActionResult> Create([FromBody] TeamMemberCreationModel member)
    {
      await _teamService.Create(member);
      var newMember = await _teamService.GetBySocial(member.Social);
      return Ok(new { newMember });
    }

    [HttpPut("")]
    public async Task<IActionResult> Update([FromBody] TeamMemberModel member)
    {
      if (member.Id == null)
      {
        return BadRequest("Id inválido!");
      }

      var existingUser = await _teamService.GetById(member.Id);

      if (existingUser == null)
      {
        return BadRequest("Usuário não existe!");
      }

      await _teamService.Update(member);
      return Ok(new { member });
    }
        
        
    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromBody] TeamMemberModel member)
    {
      var members = await _teamService.GetAll();
      return Ok(new {members});
    }  
  }
}

using AlamandaApi.Services.FieldsSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Services.Team {
  [ApiController]
  [Route("[controller]")]
  public class TeamController : ControllerBase {
    private readonly TeamService _teamService;
    private readonly FieldsSchemaService _fieldSchemaService;

    public TeamController(TeamService teamService, FieldsSchemaService fieldSchemaService) {
      _teamService = teamService;
      _fieldSchemaService = fieldSchemaService;
    }

    [HttpPost("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] TeamMemberCreationModel member) {
      try {
        await _teamService.Create(member);
        var newMember = await _teamService.GetBySocial(member.Social);
        return Ok(new { newMember });
      }
      catch (Exception ex) {
        return BadRequest(new { ex.Message });
      }
    }

    [HttpPut("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromBody] TeamMemberModel member) {
      try {
        var existingUser = await _teamService.GetById(member.Id);
        await _teamService.Update(member);
        return Ok(new { member });
      }
      catch (Exception ex) {
        return BadRequest(new { ex.Message });
      }
    }


    [HttpGet("")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, string queryString = "") {
      try {
        var result = await _teamService.GetAll(page, pageSize, queryString);
        return Ok(result);
      }
      catch (Exception ex) {
        return BadRequest(new { ex.Message });
      }
    }  
    
    [HttpGet("fields")]
    [AllowAnonymous]
    public async Task<List<FieldInfo>> getFields() {
      var result = await _fieldSchemaService.GetFieldTypes("teammembers");
      return result;
    }  
  }
}

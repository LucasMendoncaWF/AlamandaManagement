using AlamandaApi.Services.CRUD;
using AlamandaApi.Services.FieldsSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Services.Role {
  [ApiController]
  [Route("[controller]")]
  public class RoleController : ControllerBase {
    private readonly RoleService _roleService;
    private readonly FieldsSchemaService _fieldSchemaService;

    public RoleController(RoleService roleService, FieldsSchemaService fieldSchemaService) {
      _roleService = roleService;
      _fieldSchemaService = fieldSchemaService;
    }

    [HttpPost("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] RoleCreationModel role) {
      try {
        var result = await _roleService.Create(role);
        return Ok(new { result });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpPut("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromBody] RoleModel role) {
      try {
        var result = await _roleService.Update(role);
        return Ok(new { result });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }


    [HttpGet("")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] ListQueryParams query) {
      try {
        var result = await _roleService.GetAll(query);
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
        await _roleService.Delete(id);
        return Ok(new { success = true });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }  
    
    [HttpGet("fields")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> getFields() {
      try {
        var result = await _fieldSchemaService.GetFieldTypes("Roles");
        result.RemoveAll(item => item.FieldName == "teammembers");
        return Ok(result);
      }
      catch {
        return BadRequest("An error happened when trying to fetch fields");
      }
    }  
  }
}

using AlamandaApi.Services.CRUD;
using AlamandaApi.Services.FieldsSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Services.User {
  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase {
    private readonly UserService _userService;
    private readonly FieldsSchemaService _fieldSchemaService;

    public UserController(UserService userService, FieldsSchemaService fieldSchemaService) {
      _userService = userService;
      _fieldSchemaService = fieldSchemaService;
    }

    [HttpPut("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AdminUpdateUser([FromBody] UserEdit user) {
      try {
        var result = await _userService.AdminUpdateUser(user);
        return Ok(new { result });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpPost("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AdminCreateUser([FromBody] UserCreate user) {
      try {
        var result = await _userService.AdminCreateUser(user);
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
        var result = await _userService.GetAll(query);
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
        await _userService.Delete(id);
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
        var result = await _fieldSchemaService.GetFieldTypes("Users", new List<string> { "Password", "RefreshToken" });
        result.RemoveAll(item => item.FieldName.ToLower() == "comics");
        return Ok(result);
      }
      catch {
        return BadRequest("An error happened when trying to fetch fields");
      }
    }  
  }
}

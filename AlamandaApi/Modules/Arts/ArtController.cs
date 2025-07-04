using AlamandaApi.Services.CRUD;
using AlamandaApi.Services.FieldsSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Services.Art {
  [ApiController]
  [Route("[controller]")]
  public class ArtController : ControllerBase {
    private readonly ArtService _artService;
    private readonly FieldsSchemaService _fieldSchemaService;

    public ArtController(ArtService artService, FieldsSchemaService fieldSchemaService) {
      _artService = artService;
      _fieldSchemaService = fieldSchemaService;
    }

    [HttpPost("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] ArtCreationModel art) {
      try {
        var result = await _artService.Create(art);
        return Ok(new { result });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpPut("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromBody] ArtModel art) {
      try {
        var result = await _artService.Update(art);
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
        var result = await _artService.GetAll(query);
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
        await _artService.Delete(id);
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
        var result = await _fieldSchemaService.GetFieldTypes("FanArts");
        return Ok(result);
      }
      catch {
        return BadRequest("An error happened when trying to fetch fields");
      }
    }  
  }
}

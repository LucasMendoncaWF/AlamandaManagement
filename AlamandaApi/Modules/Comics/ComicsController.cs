using AlamandaApi.Services.CRUD;
using AlamandaApi.Services.FieldsSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Services.Comics {
  [ApiController]
  [Route("[controller]")]
  public class ComicController : ControllerBase {
    private readonly ComicsService _comicsService;
    private readonly FieldsSchemaService _fieldSchemaService;

    public ComicController(ComicsService comicsService, FieldsSchemaService fieldSchemaService) {
      _comicsService = comicsService;
      _fieldSchemaService = fieldSchemaService;
    }

    [HttpPost("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] ComicCreationModel comic) {
      try {
        var response = await _comicsService.Create(comic);
        return Ok(new { response });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpPut("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromBody] ComicModel comic) {
      try {
        var result = await _comicsService.Update(comic);
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
        var result = await _comicsService.GetAll(query);
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
        await _comicsService.Delete(id);
        return Ok(new { success = true });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }  
    
    [HttpGet("fields")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> getFields() {
      var ignoreFields = new[] { "teammembers", "cart" };
      try {
        var result = await _fieldSchemaService.GetFieldTypes("Comics");
        result.RemoveAll(item => ignoreFields.Contains(item.FieldName?.ToLower()));
        return Ok(result);
      }
      catch {
        return BadRequest("An error happened when trying to fetch fields");
      }
    }  
  }
}

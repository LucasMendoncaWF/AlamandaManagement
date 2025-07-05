using AlamandaApi.Services.CRUD;
using AlamandaApi.Services.FieldsSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Services.Category {
  [ApiController]
  [Route("[controller]")]
  public class CategoryController : ControllerBase {
    private readonly CategoryService _categoryService;
    private readonly FieldsSchemaService _fieldSchemaService;

    public CategoryController(CategoryService categoryService, FieldsSchemaService fieldSchemaService) {
      _categoryService = categoryService;
      _fieldSchemaService = fieldSchemaService;
    }

    [HttpPost("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CategoryCreationModel category) {
      try {
        var result = await _categoryService.Create(category);
        return Ok(new { result });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpPut("")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromBody] CategoryModel category) {
      try {
        var result = await _categoryService.Update(category);
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
        var result = await _categoryService.GetAll(query);
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
        await _categoryService.Delete(id);
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
        var result = await _fieldSchemaService.GetFieldTypes("Categories");
        return Ok(result);
      }
      catch {
        return BadRequest("An error happened when trying to fetch fields");
      }
    }  
  }
}

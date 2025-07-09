using AlamandaApi.Services.CRUD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Services.Comics {
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class ComicController : ControllerBase {
    private readonly ComicsService _comicsService;

    public ComicController(ComicsService comicsService) {
      _comicsService = comicsService;
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] ComicModel comic) {
      try {
        var response = await _comicsService.Create(comic, true);
        return Ok(new { response });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpPut]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromBody] ComicModel comic) {
      try {
        var result = await _comicsService.Update(comic, true);
        return Ok(new { result });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }


    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll([FromQuery] ListQueryParams query) {
      try {
        var result = await _comicsService.GetAll(query);
        return Ok(result);
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpDelete]
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

    //_________________________ OPEN URLS __________________________________________
    [HttpPost("add")]
    public async Task<IActionResult> CreateByUser([FromBody] ComicModel comic) {
      try {
        var response = await _comicsService.Create(comic);
        return Ok(new { response });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateByUser([FromBody] ComicModel comic) {
      try {
        var response = await _comicsService.Create(comic);
        return Ok(new { response });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }
  }
}

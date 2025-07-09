using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Services.Chapters {
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class ChaptersController : ControllerBase {
    private readonly ChaptersService _chaptersService;

    public ChaptersController(ChaptersService chaptersService) {
      _chaptersService = chaptersService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ChapterFormModel chapter) {
      try {
        var response = await _chaptersService.Create(chapter);
        return Ok(new { response });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ChapterFormModel chapter) {
      try {
        var result = await _chaptersService.Update(chapter);
        return Ok(new { result });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpPut("move")]
    public async Task<IActionResult> Move([FromBody] List<ChapterMoveModel> chapter) {
      try {
        var result = await _chaptersService.Move(chapter);
        return Ok(new { result });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }


    [HttpGet]
    public async Task<IActionResult> GetAllFromComic([FromQuery] ChaptersListQueryParams query) {
      try {
        ChaptersPagedResult? result = await _chaptersService.GetAllFromComic(query);
        if (result == null) {
          return NotFound();
        }
        return Ok(result);
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete([FromQuery] int id) {
      try {
        await _chaptersService.Delete(id);
        return Ok(new { success = true });
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }

    [HttpGet("fields")]
    public async Task<IActionResult> getFields() {
      try {
        var result = await _chaptersService.GetFields();
        return Ok(result);
      }
      catch (Exception ex) {
        return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
      }
    }
  }
}

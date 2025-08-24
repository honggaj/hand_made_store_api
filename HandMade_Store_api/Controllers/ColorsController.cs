using HandMade_Store_api.DTOs.Color;
using HandMade_Store_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandMade_Store_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly IColorService _service;

        public ColorsController(IColorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var colors = await _service.GetAllAsync();
            return Ok(colors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var color = await _service.GetByIdAsync(id);
            if (color == null) return NotFound();
            return Ok(color);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ColorRequest request)
        {
            var color = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = color.ColorId }, color);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ColorUpdateRequest request)
        {
            var color = await _service.UpdateAsync(id, request);
            if (color == null) return NotFound();
            return Ok(color);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}

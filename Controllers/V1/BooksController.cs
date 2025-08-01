using BookstoreApi.Application.Services;
using BookstoreApi.Domain.Entities;
using BookstoreApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
        _bookService = bookService;
    }

    [Authorize(Policy = "RequireMaintainer")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var user = UserHttpContextHelper.GetCurrentUser(HttpContext);
        if (user is null)
            return Unauthorized();

        var books = await _bookService.GetAllAsync(user.Id, user.Role);
        return Ok(books);
    }

    [Authorize(Policy = "RequireMaintainer")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = UserHttpContextHelper.GetCurrentUser(HttpContext);
        if (user is null)
            return Unauthorized();

        var book = await _bookService.GetByIdAsync(id, user.Id, user.Role);
        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [Authorize(Policy = "RequireMaintainer")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Book book)
    {
        var user = UserHttpContextHelper.GetCurrentUser(HttpContext);
        if (user is null)
            return Unauthorized();

        book.CreatedBy = user.Id;
        var created = await _bookService.CreateAsync(book);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Policy = "RequireMaintainer")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Book book)
    {
        var user = UserHttpContextHelper.GetCurrentUser(HttpContext);
        if (user is null)
            return Unauthorized();

        var existing = await _bookService.GetByIdAsync(id, user.Id, user.Role);
        if (existing == null)
            return NotFound();

        existing.Title = book.Title;
        existing.Genre = book.Genre;
        await _bookService.UpdateAsync(existing);

        return Ok(existing);
    }

    [Authorize(Policy = "RequireGrocery")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = UserHttpContextHelper.GetCurrentUser(HttpContext);
        if (user is null)
            return Unauthorized();

        var existing = await _bookService.GetByIdAsync(id, user.Id, user.Role);
        if (existing == null)
            return NotFound();

        await _bookService.DeleteAsync(existing);
        return NoContent();
    }
}

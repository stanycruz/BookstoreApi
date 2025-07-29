using BookstoreApi.Domain.Entities;
using BookstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApi.Application.Services;

public class BookService
{
    private readonly ApplicationDbContext _context;

    public BookService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAllAsync(Guid userId, string role)
    {
        if (role == "rookie")
            return await _context.Books.Where(b => b.CreatedBy == userId).ToListAsync();

        return await _context.Books.ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(Guid id, Guid userId, string role)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
            return null;

        if (role == "rookie" && book.CreatedBy != userId)
            return null;

        return book;
    }

    public async Task<Book> CreateAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
}

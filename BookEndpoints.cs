using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using PublisherData;
using PublisherDomain;
namespace PubAPI;

public static class BookEndpoints
{
    public static void MapBookEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Book").WithTags(nameof(Book));

        group.MapGet("/", async (PubContext db) =>
        {
            return await db.Books.ToListAsync();
        })
        .WithName("GetAllBooks")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Book>, NotFound>> (int id, PubContext db) =>
        {
            return await db.Books.AsNoTracking()
                .FirstOrDefaultAsync(model => model.BookId == id)
                is Book model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetBookById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Book book, PubContext db) =>
        {
            var affected = await db.Books
                .Where(model => model.BookId == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.BookId, book.BookId)
                    .SetProperty(m => m.Title, book.Title)
                    .SetProperty(m => m.PublishDate, book.PublishDate)
                    .SetProperty(m => m.BasePrice, book.BasePrice)
                    .SetProperty(m => m.AuthorId, book.AuthorId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateBook")
        .WithOpenApi();

        group.MapPost("/", async (Book book, PubContext db) =>
        {
            db.Books.Add(book);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Book/{book.BookId}",book);
        })
        .WithName("CreateBook")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, PubContext db) =>
        {
            var affected = await db.Books
                .Where(model => model.BookId == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteBook")
        .WithOpenApi();
    }
}

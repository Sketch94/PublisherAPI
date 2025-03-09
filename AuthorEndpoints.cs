using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PublisherData;
using PublisherDomain;
namespace PubAPI;

public static class AuthorEndpoints
{
    public static void MapAuthorEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Author").WithTags(nameof(Author));

        // Hämta alla författare
        group.MapGet("/", async (PubContext db) =>
        {
            return await db.Authors.Include(a => a.Books).AsNoTracking().ToListAsync();
        })
        .WithName("GetAllAuthors")
        .WithOpenApi();

        // Hämta idt på en författare
        group.MapGet("/{id}", async Task<Results<Ok<Author>, NotFound>> (int id, PubContext db) =>
        {
            return await db.Authors.Include(a => a.Books).AsNoTracking()
                .FirstOrDefaultAsync(model => model.AuthorId == id)
                is Author model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetAuthorById")
        .WithOpenApi();

        // POST fungerar inte trots flera försök via Swagger att lägga Author till i databas

        //group.MapPost("/", async (Author author, PubContext db) =>
        //{
        //    db.Authors.Add(author);
        //    await db.SaveChangesAsync();
        //    return TypedResults.Created($"/api/Author/{author.AuthorId}", author);
        //});



        // Uppdatera en författare
        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Author author, PubContext db) =>
        {
            var existingAuthor = await db.Authors.FindAsync(id);
            if (existingAuthor == null)
            {
                return TypedResults.NotFound();
            }

            existingAuthor.FirstName = author.FirstName;
            existingAuthor.LastName = author.LastName;

            await db.SaveChangesAsync();
            return TypedResults.Ok();

        })
            .WithName("UpdateAuthor")
            .WithOpenApi();

        // Radera en författare
        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, PubContext db) =>
        {
            var affected = await db.Authors
                .Where(model => model.AuthorId == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteAuthor")
        .WithOpenApi();
    }
}

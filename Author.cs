using System.ComponentModel.DataAnnotations;

namespace PublisherDomain;

public class Author
{
    public int AuthorId { get; set; }
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }
    public List<Book> Books { get; set; } = [];
}

using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Коментар обов'язковий")]
        [StringLength(500)]
        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Зв'язок з фільмом
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;
    }
}

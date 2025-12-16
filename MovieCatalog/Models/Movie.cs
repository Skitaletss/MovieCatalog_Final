using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва фільму обов'язкова")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Режисер обов'язковий")]
        [StringLength(100)]
        public string Director { get; set; } = string.Empty;

        [Required(ErrorMessage = "Жанр обов'язковий")]
        [StringLength(50)]
        public string Genre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Рік випуску обов'язковий")]
        [Range(1888, 2025, ErrorMessage = "Рік має бути від 1888 до 2025")]
        public int Year { get; set; }

        [StringLength(500)]
        public string PosterUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Опис обов'язковий")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        // Рейтинг фільму (від 0 до 10)
        [Range(0, 10, ErrorMessage = "Рейтинг має бути від 0 до 10")]
        public double Rating { get; set; } = 0;

        // Кількість голосів
        public int RatingCount { get; set; } = 0;

        // Навігаційна властивість для коментарів
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}

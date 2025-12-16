using Microsoft.EntityFrameworkCore;
using MovieCatalog.Models;

namespace MovieCatalog.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфігурація зв'язку Movie-Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Movie)
                .WithMany(m => m.Comments)
                .HasForeignKey(c => c.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data для фільмів з рейтингами
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    Id = 1,
                    Title = "Інтерстеллар",
                    Director = "Крістофер Нолан",
                    Genre = "Наукова фантастика",
                    Year = 2014,
                    PosterUrl = "/images/movies/1.jpg",
                    Description = "Група дослідників використовує щойно відкриту червоточину, щоб подолати обмеження космічних подорожей і здійснити міжзоряну подорож.",
                    Rating = 8.6,
                    RatingCount = 1520
                },
                new Movie
                {
                    Id = 2,
                    Title = "Хрещений батько",
                    Director = "Френсіс Форд Коппола",
                    Genre = "Драма, Кримінал",
                    Year = 1972,
                    PosterUrl = "/images/movies/2.jpg",
                    Description = "Історія могутньої італійсько-американської мафіозної родини Корлеоне та перехід влади від батька до сина.",
                    Rating = 9.2,
                    RatingCount = 2100
                },
                new Movie
                {
                    Id = 3,
                    Title = "Темний лицар",
                    Director = "Крістофер Нолан",
                    Genre = "Бойовик, Кримінал",
                    Year = 2008,
                    PosterUrl = "/images/movies/3.jpg",
                    Description = "Бетмен протистоїть новій загрозі у вигляді Джокера, який створює хаос у Готем-Сіті.",
                    Rating = 9.0,
                    RatingCount = 1850
                },
                new Movie
                {
                    Id = 4,
                    Title = "Форрест Гамп",
                    Director = "Роберт Земекіс",
                    Genre = "Драма, Мелодрама",
                    Year = 1994,
                    PosterUrl = "/images/movies/4.jpg",
                    Description = "Історія життя простодушного чоловіка, який став свідком найважливіших подій в американській історії.",
                    Rating = 8.8,
                    RatingCount = 1620
                },
                new Movie
                {
                    Id = 5,
                    Title = "Початок",
                    Director = "Крістофер Нолан",
                    Genre = "Наукова фантастика, Трилер",
                    Year = 2010,
                    PosterUrl = "/images/movies/5.jpg",
                    Description = "Злодій, який краде корпоративні секрети через технологію спільного сновидіння, отримує завдання впровадити ідею в свідомість людини.",
                    Rating = 8.7,
                    RatingCount = 1700
                },
                new Movie
                {
                    Id = 6,
                    Title = "Матриця",
                    Director = "Вачовські",
                    Genre = "Наукова фантастика, Бойовик",
                    Year = 1999,
                    PosterUrl = "/images/movies/6.jpg",
                    Description = "Комп'ютерний хакер дізнається про справжню природу своєї реальності та свою роль у війні проти її контролерів.",
                    Rating = 8.7,
                    RatingCount = 1680
                },
                new Movie
                {
                    Id = 7,
                    Title = "Зелена миля",
                    Director = "Френк Дарабонт",
                    Genre = "Драма, Детектив",
                    Year = 1999,
                    PosterUrl = "/images/movies/7.jpg",
                    Description = "Начальник охорони камери смертників зустрічає засудженого з надприродними здібностями.",
                    Rating = 8.5,
                    RatingCount = 1420
                },
                new Movie
                {
                    Id = 8,
                    Title = "Втеча з Шоушенка",
                    Director = "Френк Дарабонт",
                    Genre = "Драма",
                    Year = 1994,
                    PosterUrl = "/images/movies/8.jpg",
                    Description = "Два ув'язнені зав'язують дружбу протягом років, знаходячи втіху та можливе відновлення через акти звичайної порядності.",
                    Rating = 9.3,
                    RatingCount = 2300
                },
                new Movie
                {
                    Id = 9,
                    Title = "Список Шиндлера",
                    Director = "Стівен Спілберг",
                    Genre = "Драма, Історичний",
                    Year = 1993,
                    PosterUrl = "/images/movies/9.jpg",
                    Description = "Німецький бізнесмен рятує життя понад тисячі євреїв під час Голокосту.",
                    Rating = 8.9,
                    RatingCount = 1560
                },
                new Movie
                {
                    Id = 10,
                    Title = "Бійцівський клуб",
                    Director = "Девід Фінчер",
                    Genre = "Драма, Трилер",
                    Year = 1999,
                    PosterUrl = "/images/movies/10.jpg",
                    Description = "Службовець, який страждає від безсоння, зустрічає харизматичного виробника мила і вони разом створюють підпільний бійцівський клуб.",
                    Rating = 8.8,
                    RatingCount = 1750
                }
            );
        }
    }
}

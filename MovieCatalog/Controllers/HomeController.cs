using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieCatalog.Data;
using MovieCatalog.Models;

namespace MovieCatalog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Список всіх фільмів з пошуком, фільтрацією, сортуванням та пагінацією
        public async Task<IActionResult> Index(string searchString, string genre, int? year, string sortOrder, int page = 1)
        {
            const int pageSize = 6; // Кількість фільмів на сторінці

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentGenre"] = genre;
            ViewData["CurrentYear"] = year;
            ViewData["CurrentSort"] = sortOrder;

            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["YearSortParm"] = sortOrder == "year" ? "year_desc" : "year";
            ViewData["RatingSortParm"] = sortOrder == "rating" ? "rating_desc" : "rating";

            var movies = from m in _context.Movies.Include(m => m.Comments)
                         select m;

            // Пошук за назвою або режисером
            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Title.Contains(searchString) || m.Director.Contains(searchString));
            }

            // Фільтрація за жанром
            if (!String.IsNullOrEmpty(genre))
            {
                movies = movies.Where(m => m.Genre.Contains(genre));
            }

            // Фільтрація за роком
            if (year.HasValue)
            {
                movies = movies.Where(m => m.Year == year);
            }

            // Сортування
            switch (sortOrder)
            {
                case "title_desc":
                    movies = movies.OrderByDescending(m => m.Title);
                    break;
                case "year":
                    movies = movies.OrderBy(m => m.Year);
                    break;
                case "year_desc":
                    movies = movies.OrderByDescending(m => m.Year);
                    break;
                case "rating":
                    movies = movies.OrderBy(m => m.Rating);
                    break;
                case "rating_desc":
                    movies = movies.OrderByDescending(m => m.Rating);
                    break;
                default:
                    movies = movies.OrderBy(m => m.Title);
                    break;
            }

            // Загальна кількість фільмів після фільтрації
            int totalMovies = await movies.CountAsync();
            int totalPages = (int)Math.Ceiling(totalMovies / (double)pageSize);

            // Пагінація
            var moviesList = await movies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Передаємо дані для пагінації
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            // Отримуємо унікальні жанри та роки для фільтрів
            ViewData["Genres"] = await _context.Movies
                .Select(m => m.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            ViewData["Years"] = await _context.Movies
                .Select(m => m.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            return View(moviesList);
        }

        // GET: Детальна інформація про фільм
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Форма для створення нового фільму
        public IActionResult Create()
        {
            return View();
        }

        // POST: Створення нового фільму
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Director,Genre,Year,Description,Rating")] Movie movie, IFormFile? posterFile)
        {
            if (ModelState.IsValid)
            {
                // Обробка завантаження зображення
                if (posterFile != null && posterFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/movies");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(posterFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await posterFile.CopyToAsync(fileStream);
                    }

                    movie.PosterUrl = "/images/movies/" + uniqueFileName;
                }
                else
                {
                    movie.PosterUrl = "/images/movies/default.jpg";
                }

                movie.RatingCount = 0; // Початкова кількість голосів

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Форма для редагування фільму
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Редагування фільму
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Director,Genre,Year,Description,PosterUrl,Rating,RatingCount")] Movie movie, IFormFile? posterFile)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Обробка нового зображення
                    if (posterFile != null && posterFile.Length > 0)
                    {
                        // Видаляємо старе зображення, якщо воно не default
                        if (!string.IsNullOrEmpty(movie.PosterUrl) && movie.PosterUrl != "/images/movies/default.jpg")
                        {
                            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, movie.PosterUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/movies");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(posterFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await posterFile.CopyToAsync(fileStream);
                        }

                        movie.PosterUrl = "/images/movies/" + uniqueFileName;
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Форма для видалення фільму
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Видалення фільму
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie != null)
            {
                // Видаляємо файл зображення
                if (!string.IsNullOrEmpty(movie.PosterUrl) && movie.PosterUrl != "/images/movies/default.jpg")
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, movie.PosterUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Додавання коментаря до фільму
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int movieId, string authorName, string text)
        {
            if (string.IsNullOrWhiteSpace(authorName) || string.IsNullOrWhiteSpace(text))
            {
                TempData["Error"] = "Заповніть всі поля коментаря!";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }

            var comment = new Comment
            {
                MovieId = movieId,
                AuthorName = authorName.Trim(),
                Text = text.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Коментар успішно додано!";
            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        // POST: Видалення коментаря
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id, int movieId)
        {
            var comment = await _context.Comments.FindAsync(id);
            
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Коментар видалено!";
            }

            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        // POST: Оцінювання фільму
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateMovie(int movieId, int rating)
        {
            if (rating < 1 || rating > 10)
            {
                TempData["Error"] = "Оцінка має бути від 1 до 10!";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }

            var movie = await _context.Movies.FindAsync(movieId);

            if (movie == null)
            {
                return NotFound();
            }

            // Перераховуємо середній рейтинг
            double totalRating = movie.Rating * movie.RatingCount;
            movie.RatingCount++;
            movie.Rating = Math.Round((totalRating + rating) / movie.RatingCount, 1);

            _context.Update(movie);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Дякуємо за вашу оцінку!";
            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}

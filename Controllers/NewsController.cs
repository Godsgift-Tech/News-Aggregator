using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using News_Aggregator.Data;
using News_Aggregator.DTO;
using News_Aggregator.EntityModel;
using News_Aggregator.Models.DomainModel;
using News_Aggregator.Models.ViewModel;
using News_Aggregator.Service;
using X.PagedList.Extensions;

namespace News_Aggregator.Controllers
{
    public class NewsController : Controller
    {
        private readonly NewsService _newsService;
        private readonly EmailService _emailService;
        private readonly NewsContext _context;
        private readonly IArticleService _articleService;

        public NewsController(NewsContext context, IArticleService articleService, NewsService newsService, EmailService emailService)
        {
            _context = context;
            _newsService = newsService;
            _emailService = emailService;
            _articleService = articleService;
        }
        //Get News from the Api and implement pagination
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm, string category, int page = 1)
        {
            var newsResponse = await _newsService.GetTopHeadlinesAsync(category, searchTerm);
            var newsArticles = newsResponse?.articles ?? new List<Article>();

            // Create the PagedArticlesViewModel
            var pagedArticles = newsArticles.ToPagedList(page, 10); // Ensure you have a proper pagination logic

            var pagedArticlesViewModel = new PagedArticlesViewModel
            {
                Articles = pagedArticles.ToList(),
                CurrentPage = page,
                TotalPages = pagedArticles.PageCount
            };

            // Pass the correct model to the view
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Category = category;
            return View(pagedArticlesViewModel); // Return PagedArticlesViewModel
        }


        // GET: News/ReadArticle
        public async Task<IActionResult> ReadArticle(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return RedirectToAction(nameof(Index)); // Redirect to index if no URL provided
            }

            // Fetch article details from the API using the URL
            var articleDto = await _articleService.GetArticleDetailsByUrlAsync(url);

            if (articleDto == null)
            {
                return NotFound(); // Show 404 page if the article is not found
            }

            // Map DTO to ViewModel (if you have a DetailedArticleViewModel)
            var detailedArticleViewModel = new DetailedArticleViewModel
            {
                Title = articleDto.title,
                Description = articleDto.description,
                Content = articleDto.content,
                Source = articleDto.Source.name,
                Url = articleDto.url,
                PublishedAt = articleDto.publishedAt
            };

            return View(detailedArticleViewModel); // Pass the ViewModel to the view
        }

        // GET: News/Search
        public async Task<IActionResult> Search(string query, int? page)
        {
            if (string.IsNullOrEmpty(query)) return RedirectToAction("Index");

            var newsResponse = await _newsService.GetTopHeadlinesAsync("", query);
            var pageNumber = page ?? 1;
            var pageSize = 10;

            if (newsResponse != null)
            {
                // Save search query to search history
                var searchHistory = new SavedHistory
                {
                    SearchQuery = query,
                    SearchDate = DateTime.Now
                };

                _context.SearchHistories.Add(searchHistory);
                await _context.SaveChangesAsync();

                var searchPaged = newsResponse.articles.ToPagedList(pageNumber, pageSize);

                // Return a view model
                var model = new PagedArticlesViewModel
                {
                    Articles = searchPaged.ToList(),
                    CurrentPage = pageNumber,
                    TotalPages = searchPaged.PageCount,
                    SearchTerm = query,
                    Category = "" // No category filtering in this action
                };

                return View(model);
            }

            return View("Error");
        }

        // GET: News/Create
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article)
        {
            if (ModelState.IsValid)
            {
                // Add the article to the database
                _context.Add(article);
                await _context.SaveChangesAsync();

                // Send an email notification after saving the article
                var emailDto = new EmailDto
                {
                    From = "your-email@example.com",
                    To = "recipient-email@example.com",
                    Body = $"New article created: {article.title}"
                };

                _emailService.SendEmail(emailDto);

                // Redirect to the Saved Articles page
                return RedirectToAction(nameof(SavedArticles));
            }
            return View(article);
        }

        // GET: News/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var article = await _context.SavedArticles.FindAsync(id);
            if (article == null) return NotFound();

            return View(article);
        }

        // POST: News/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SavedArticleId,Title,Description,Source,Url")] Article article)
        {
            // if (id != article.SavedArticleId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();

                    // Send email notification on editing an article
                    var emailDto = new EmailDto
                    {
                        From = "your-email@example.com",
                        To = "recipient-email@example.com",
                        Body = $"Article updated: {article.title}"
                    };
                    _emailService.SendEmail(emailDto);

                    return RedirectToAction(nameof(SavedArticles));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the article.");
                    // Consider logging the error (ex) here
                }
            }
            return View(article);
        }//

        private bool ArticleExists(Guid id)
        {
            return _context.SavedArticles.Any(e => e.SavedArticleId == id);
        }

        //Saved articles
        [HttpGet]
        public async Task<IActionResult> SavedArticles(int? page)
        {
            // Set pagination parameters
            const int pageSize = 10; // Items per page
            var pageNumber = page ?? 1; // Default to page 1 if no page number is provided

            // Fetch the total count of saved articles
            var totalArticles = await _context.SavedArticles.CountAsync();

            // Fetch the saved articles with pagination
            var savedArticles = await _context.SavedArticles
                .OrderByDescending(a => a.SavedArticleId) // Sort by ID in descending order
                .Skip((pageNumber - 1) * pageSize) // Skip the previous pages' articles
                .Take(pageSize) // Take only the articles for the current page
                .ToListAsync(); // Execute the query

            // Create a pagination model
            var paginationModel = new PaginationModel<Article>
            {
                Items = savedArticles,
                TotalCount = totalArticles,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            // Return the view with the pagination model
            return View(paginationModel);
        }

        // GET: News/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var article = await _context.SavedArticles.FirstOrDefaultAsync(m => m.SavedArticleId == id);
            if (article == null) return NotFound();

            return View(article);
        }

        // POST: News/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid? id)
        {
            var article = await _context.SavedArticles.FindAsync(id);
            _context.SavedArticles.Remove(article);
            await _context.SaveChangesAsync();

            // Send email notification on deleting an article
            var emailDto = new EmailDto
            {
                From = "your-email@example.com",
                To = "recipient-email@example.com",
                Body = $"Article deleted: {article.Title}"
            };
            _emailService.SendEmail(emailDto);

            return RedirectToAction(nameof(SavedArticles));
        }


    }
}



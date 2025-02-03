using Blog_Website.Data;
using Blog_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Blog_Website.Controllers
{
    [AuthorizeUser]
    public class DashBoardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashBoardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userName = HttpContext.Session.GetString("UserName");

            if (userId == null || userEmail == null)
            {
                return RedirectToAction("Login", "UserManage");
            }

            var model = new Blogg_Model
            {
                UserId = userId.Value
            };

            ViewBag.UserId = userId;
            ViewBag.UserEmail = userEmail;
            ViewBag.UserName = userName;

            // Fetch statistics asynchronously
            ViewBag.PostCount = await _context.Bloggers
                .Where(post=>!post.Is_Deleted)
                .CountAsync();
            ViewBag.CategoryCount = await _context.Categories.Select(b => b.Name).Distinct().CountAsync();
            ViewBag.CommentCount = await _context.Comments
                .Where(C => !C.Is_Deleted)
                .CountAsync();
            ViewBag.UserCount = await _context.Users.CountAsync();

            // Fetch recent posts asynchronously
            var recentPosts = await _context.Bloggers
                .Where(post => !post.Is_Deleted)
                .OrderByDescending(post => post.CreatedDate)
                .Take(5)
                .ToListAsync();

            return View("Dashboard", recentPosts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(Blogg_Model model, IFormFile Image)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (ModelState.IsValid)
            {
                if (Image != null && Image.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Image.FileName}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    model.ImagePath = $"/images/{fileName}";
                }

                model.UserId = (int)userId;
                model.CreatedDate = DateTime.Now;

                _context.Bloggers.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("PostDetail", new { id = model.Id });
            }

            TempData["ErrorMessage"] = "There were some errors with your submission. Please correct them and try again.";
            TempData["InvalidModel"] = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            return RedirectToAction("CreatePost");
        }

        [HttpGet]
        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _context.Bloggers.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(Blogg_Model model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingPost = await _context.Bloggers.FirstOrDefaultAsync(p => p.Id == model.Id);
            if (existingPost == null)
            {
                return NotFound();
            }

            existingPost.BlogTitle = model.BlogTitle;
            existingPost.Content = model.Content;
            existingPost.Category = model.Category;

            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/images", file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    existingPost.ImagePath = "/images/" + file.FileName;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("PostDetail", new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Bloggers.FirstOrDefaultAsync(b => b.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePostConfirmed(int id)
        {
            var post = await _context.Bloggers.FirstOrDefaultAsync(p => p.Id == id);
            if (post != null)
            {
                post.Is_Deleted = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PostDetail(int id)
        {
            var blogPost = await _context.Bloggers.FirstOrDefaultAsync(b => b.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Where(c => c.PostId == id)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            ViewBag.Comments = comments;
            return View(blogPost);
        }

        public async Task<IActionResult> AllPosts()
        {
            var blogPosts = await _context.Bloggers
                .Where(p => !p.Is_Deleted)
                .ToListAsync();

            return View(blogPosts);
        }

        //Categories
        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.Where(c => !c.Is_Deleted).ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedDate = DateTime.Now;
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Categories");
            }

            return View(category);
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Category category)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);
            if (existingCategory == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                await _context.SaveChangesAsync();
                return RedirectToAction("Categories");
            }

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category != null)
            {
                category.Is_Deleted=true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Categories");
        }

        //Comment

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int PostId, int UserId, string CommenterName, string Text)
        {
            if (string.IsNullOrWhiteSpace(CommenterName) || string.IsNullOrWhiteSpace(Text))
            {
                TempData["ErrorMessage"] = "Name and Comment text are required.";
                return RedirectToAction("PostDetail", new { id = PostId });
            }

            var comment = new Comment
            {
                PostId = PostId,
                UserId = UserId,
                CommenterName = CommenterName,
                Text = Text,
                CreatedDate = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("PostDetail", new { id = PostId });
        }
        //Comment Management

        public async Task<IActionResult> CommentList()
        {
            var comments = await (from c in _context.Comments
                                  join b in _context.Bloggers on c.PostId equals b.Id
                                  where !c.Is_Deleted
                                  select new
                                  {
                                      c.CommentId,
                                      c.CommenterName,
                                      c.CreatedDate,
                                      c.Text,
                                      PostTitle = b.BlogTitle, // Selecting the content from Blog
                                      AuthorName = b.Name,
                                      BlogTitle = b.BlogTitle
                                  }).ToListAsync();

            return View(comments);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            // Soft delete approach
            comment.Is_Deleted = true;
            _context.Comments.Update(comment);

            // Hard delete
            // _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();

            return RedirectToAction("CommentList");
        }


        //

        // User Dashboard View
        public async Task<IActionResult> User_Dashboard()
        {
            var categories = await _context.Bloggers
                .Where(b => !b.Is_Deleted)
                .Select(b => b.Category)
                .Distinct()
                .ToListAsync();

            var posts = await _context.Comments
                .Where(b => !b.Is_Deleted)
                .Include(b => b.Text)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();

            ViewBag.Categories = categories;
            return View(posts);
        }

        // Fetch posts by category
        public async Task<IActionResult> GetPostsByCategory(string category)
        {
            var posts = await (from blog in _context.Bloggers
                               join comment in _context.Comments
                               on blog.Id equals comment.PostId into blogComments
                               where blog.Category == category && !blog.Is_Deleted
                               orderby blog.CreatedDate descending
                               select new
                               {
                                   blog.Id,
                                   blog.Name,
                                   blog.BlogTitle,
                                   blog.Content,
                                   blog.ImagePath,
                                   blog.CreatedDate,
                                   blog.Category,
                                   blog.Is_Deleted,
                                   Comments = blogComments.Where(c => !c.Is_Deleted) // Filtering non-deleted comments
                               }).ToListAsync();

            // Return the posts with their associated comments
            // We need to map this anonymous object to a proper model if you want to use it in the view
            return PartialView("_PostList", posts);
        }







    }
}

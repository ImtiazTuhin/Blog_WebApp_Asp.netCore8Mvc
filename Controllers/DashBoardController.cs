using Blog_Website.Data;
using Blog_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Linq;

namespace Blog_Website.Controllers
{
    [AuthorizeUser] // Ensures only logged-in users can access this controller
    public class DashBoardController : Controller
    {

        private readonly ApplicationDbContext _context;
        //For Category Manage
        private static List<Category> _categories = new List<Category>();
        public DashBoardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userName = HttpContext.Session.GetString("UserName");

            if (userId == null || userEmail == null)
            {
                return RedirectToAction("Login", "UserManage");
            }

            // Prepare the model
            var model = new Blogg_Model
            {
                UserId = userId.Value, // Assuming UserId is nullable
                                       // Set other properties as needed, like default values for Dashboard
            };

            ViewBag.UserId = userId;
            ViewBag.UserEmail = userEmail;
            ViewBag.UserName = userName;

            // Fetch statistics for the dashboard
            ViewBag.PostCount = _context.Bloggers.Count(); // Count of posts
            ViewBag.CategoryCount = _context.Categories.Select(b => b.Name).Distinct().Count(); // Count of distinct categories
            ViewBag.CommentCount = _context.Comments.Count(); // Count of comments
            ViewBag.UserCount = _context.Users.Count(); // Count of users

            // Fetch recent posts (e.g., last 5 posts)
            var recentPosts = _context.Bloggers
                  .Where(post => !post.Is_Deleted)
                .OrderByDescending(post => post.CreatedDate)
                
                .Take(5)
                .ToList();

            return View("Dashboard", recentPosts); // Pass the recentPosts to the view
        }

        // GET: Render the form
        public IActionResult CreatePost()
        {
            return View();
        }

        // POST: Save the blog post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost(Blogg_Model model, IFormFile Image)
         {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (ModelState.IsValid)
            {

                if (Image != null && Image.Length > 0)
                {
                    // Generate a unique file name and save the image
                    var fileName = $"{Guid.NewGuid()}_{Image.FileName}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }

                    // Save the image path to the model
                    model.ImagePath = $"/images/{fileName}";
                   
                }
                model.UserId = (int)userId;
                // Set default values for CreatedDate
                model.CreatedDate = DateTime.Now;
               

                // Add the model to the database
                _context.Bloggers.Add(model);
                _context.SaveChanges();

                // Redirect to the PostDetail view with the new blog post ID
                return RedirectToAction("PostDetail", new { id = model.Id });
            }

            //Error check of Model

            // Log errors when ModelState is invalid
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
            }

            //Model Error Dbugg close

            // If the model is invalid, pass it to the Index view
            TempData["ErrorMessage"] = "There were some errors with your submission. Please correct them and try again.";
            TempData["InvalidModel"] = Newtonsoft.Json.JsonConvert.SerializeObject(model); // Serialize model if needed

            return RedirectToAction("CreatePost");
        }

        //Edit 

        [HttpGet]
        public IActionResult EditPost(int id)
        {
            // Fetch the blog post by ID
            var post = _context.Bloggers.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        public IActionResult EditPost(Blogg_Model model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Fetch the existing post
            var existingPost = _context.Bloggers.FirstOrDefault(p => p.Id == model.Id);

            if (existingPost == null)
            {
                return NotFound();
            }

            // Update the fields
            existingPost.BlogTitle = model.BlogTitle;
            existingPost.Content = model.Content;
            existingPost.Category = model.Category;

            // Optional: Handle image upload
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/images", file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    existingPost.ImagePath = "/images/" + file.FileName;
                }
            }

            _context.SaveChanges();

            return RedirectToAction("PostDetail", new { id = model.Id });
        }


        //Delete

        [HttpGet]
        public IActionResult DeletePost(int id)
        {
            // Fetch the post from the database using the ID (replace with your actual DB logic)
            var post = _context.Bloggers.FirstOrDefault(b => b.Id == id);

            if (post == null)
            {
                return NotFound(); // Return a 404 if the post doesn't exist
            }

            return View(post); // Pass the post model to the view
        }

        [HttpPost]

        public IActionResult DeletePostConfirmed(int id)
        {
            var post= _context.Bloggers.FirstOrDefault(p=>p.Id == id);
            if (post != null)
            {
                post.Is_Deleted = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //public IActionResult PostDetail(int id)
        //{
        //    // Fetch the blog post by ID
        //    var blogPost = _context.Bloggers.FirstOrDefault(b => b.Id == id);

        //    // If the blog post is not found, return a "Not Found" page or redirect
        //    if (blogPost == null)
        //    {
        //        return NotFound();
        //    }

        //    // Pass the blog post to the view
        //    return View(blogPost);
        //}

        public IActionResult PostDetail(int id)
        {
            // Fetch the blog post by ID
            var blogPost = _context.Bloggers.FirstOrDefault(b => b.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            // Fetch comments for the post
            var comments = _context.Comments
                .Where(c => c.PostId == id)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            // Pass the blog post and comments to the view
            ViewBag.Comments = comments;

            return View(blogPost);
        }


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
                UserId= UserId,
                CommenterName = CommenterName,
                Text = Text,
                CreatedDate = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("PostDetail", new { id = PostId });
        }


        // All posts
        public IActionResult AllPosts()
        {
            // Fetch all blog posts from the database
            var blogPosts = _context.Bloggers
                .Where(p=>!p.Is_Deleted)
                .ToList();

            // Pass the list of blog posts to the view
            return View(blogPosts);
        }

        //Category Mangement

        

        // Display Categories
        public IActionResult Categories()
        {
            var categories = _context.Categories.ToList(); // Fetch categories from the database
            return View(categories); // Pass the list to the view
        }

        // Create Category (GET)
        public IActionResult CreateCategory()
        {
            return View();
        }

        // Create Category (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                
                category.CreatedDate = DateTime.Now;
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Categories");
            }
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {

                    Console.WriteLine(error.ErrorMessage); // Log the error messages
                }
            }
            return View(category);
        }

        // Edit Category (GET)
        public IActionResult EditCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Edit Category (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(Category category)
        {
            var existingCategory = _context.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
            if (existingCategory == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                _context.SaveChanges();
                return RedirectToAction("Categories");
            }
            return View(category);
        }

        // Delete Category (GET)
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Delete Category (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategoryConfirmed(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction("Categories");
        }
    }
}

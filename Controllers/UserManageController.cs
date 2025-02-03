using Blog_Website.Data;
using Blog_Website.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Blog_Website.Controllers
{
    public class UserManageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserManageController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                // Hash the password
                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, user.Password);

                // Add the user to the database
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage); // Log the error messages
                }
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email); // Retrieve user by email
            if (user != null)
            {
                // Verify the hashed password
                
                var passwordHasher = new PasswordHasher<User>();
                //var pwd = PasswordHasher.HashPassword(password);
                var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, password);

                if (verificationResult == PasswordVerificationResult.Success)
                {
                    // Store user info in session
                    HttpContext.Session.SetInt32("UserId", user.Id); // Store user ID
                    HttpContext.Session.SetString("UserEmail", user.Email); // Store user email
                    HttpContext.Session.SetString("UserName", user.FirstName + " " + user.LastName); // Store user name

                    // Redirect to Dashboard
                    return RedirectToAction("Index", "DashBoard");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session data
            return RedirectToAction("Login", "UserManage");
        }

        // Forgot Password
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.example.com") // Replace with your SMTP details
            {
                Port = 587,
                Credentials = new NetworkCredential("your-email@example.com", "your-email-password"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("your-email@example.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            // Get the logged-in user
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Email == model.Email);

            if (user == null)
            {
                return RedirectToAction("Login"); // Redirect if user is not found
            }

            // Verify the current password
            var passwordHasher = new PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, model.CurrentPassword);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View(model);
            }

            // Hash the new password before saving
            user.Password = passwordHasher.HashPassword(user, model.NewPassword);
            _context.Users.Update(user); 
            await _context.SaveChangesAsync();

            TempData["Message"] = "Password changed successfully!";
            return RedirectToAction("Login", "UserManage"); // Redirect after password change
        }



        public async Task<IActionResult> SignInWithFacebook(string accessToken)
        {
            // Mock logic for Facebook sign-in (replace with actual API call)
            var mockEmail = "fbuser@example.com";
            var mockFirstName = "Facebook";
            var mockLastName = "User";

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == mockEmail);

            if (user == null)
            {
                user = new User
                {
                    Email = mockEmail,
                    FirstName = mockFirstName,
                    LastName = mockLastName,
                    Password = "" // Password not needed for Facebook sign-in
                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard");
        }

        //Adding User

        public async Task<IActionResult> AllUsers()
        {
            var users=await _context.Users.Where(u=>!u.Is_Deleted).ToListAsync();
            return View(users);
        }
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Users/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Email,Password,FirstName,LastName,UserType")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        user.CreatedDate = DateTime.UtcNow;
        //        _context.Add(user);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(user);
        //}

        //// GET: Users/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(user);
        //}

        //// POST: Users/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password,FirstName,LastName,UserType,CreatedDate")] User user)
        //{
        //    if (id != user.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(user);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!_context.Users.Any(e => e.Id == user.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(user);
        //}

        //// GET: Users/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _context.Users
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}

        //Block Users
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Is_Deleted = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AllUsers");
        }

        //Unblock Users
        [HttpGet]
        public async Task<IActionResult> UnblockUser()
        {
            var users = await _context.Users.Where(u => u.Is_Deleted).ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Is_Deleted = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AllUsers");
        }


    }
}

using Blog_Website.Data;
using Blog_Website.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace Blog_Website.Controllers
{
    public class UserManageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public object GoogleJsonWebSignature { get; private set; }

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
        public IActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                // Hash the password
                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, user.Password);

                // Add the user to the database
                _context.Users.Add(user);
                _context.SaveChanges();

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
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email); // Retrieve user by email
            if (user != null)
            {
                // Verify the hashed password
                var passwordHasher = new PasswordHasher<User>();
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
        private void SendEmail(string toEmail, string subject, string body)
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

            smtpClient.Send(mailMessage);
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult ForgotPassword(string email)
        //{
        //    // Find the user by email
        //    var user = _context.Users.FirstOrDefault(u => u.Email == email);
        //    if (user == null)
        //    {
        //        // Email not found
        //        TempData["Error"] = "Email not registered.";
        //        return View();
        //    }

        //    // Generate a unique reset token
        //    var token = Guid.NewGuid().ToString(); // You can use other methods like JWT or ASP.NET Identity token generation

        //    // Save the token to the database (or a secure store)
        //    user.ResetToken = token;
        //    user.ResetTokenExpiry = DateTime.Now.AddHours(1); // Token expires in 1 hour
        //    _context.SaveChanges();

        //    // Create reset link
        //    var resetLink = Url.Action("ResetPassword", "UserManage", new { token = token }, Request.Scheme);

        //    // Send email
        //    SendEmail(user.Email, "Password Reset Request",
        //        $"Click the link to reset your password: <a href='{resetLink}'>Reset Password</a>");

        //    TempData["Success"] = "Password reset link has been sent to your email.";
        //    return RedirectToAction("ForgotPassword");
        //}

        // Password Change
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the logged-in user
                var userId = HttpContext.Session.GetInt32("UserId");
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);

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
                _context.SaveChanges();

                TempData["Message"] = "Password changed successfully!";
                return RedirectToAction("Login", "UserManage"); // Redirect to the dashboard after changing password
            }

            return View(model);
        }


        //public async Task<IActionResult> SignInWithGoogle(string idToken)
        //{
        //    try
        //    {
        //        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
        //        {
        //            Audience = new[] { _configuration["Google:ClientId"] }
        //        });

        //        var user = _context.Users.FirstOrDefault(u => u.Email == payload.Email);

        //        if (user == null)
        //        {
        //            user = new User
        //            {
        //                Email = payload.Email,
        //                FirstName = payload.GivenName,
        //                LastName = payload.FamilyName,
        //                Password = "" // Password not needed for Google sign-in
        //            };
        //            _context.Users.Add(user);
        //            _context.SaveChanges();
        //        }

        //        return RedirectToAction("Dashboard");
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Login");
        //    }
        //}
        public IActionResult SignInWithFacebook(string accessToken)
        {
            // Mock logic for Facebook sign-in (replace with actual API call)
            var mockEmail = "fbuser@example.com";
            var mockFirstName = "Facebook";
            var mockLastName = "User";

            var user = _context.Users.FirstOrDefault(u => u.Email == mockEmail);

            if (user == null)
            {
                user = new User
                {
                    Email = mockEmail,
                    FirstName = mockFirstName,
                    LastName = mockLastName,
                    Password = "" // Password not needed for Facebook sign-in
                };
                _context.Users.Add(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Dashboard");
        }
    }
}

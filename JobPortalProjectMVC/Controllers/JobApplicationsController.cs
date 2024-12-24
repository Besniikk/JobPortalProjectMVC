using JobPortalProjectMVC.Data;
using JobPortalProjectMVC.Models;
using JobPortalProjectMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web.Http.ModelBinding;
using Microsoft.AspNetCore.Identity;
using JobPortalProjectMVC.Areas.Identity.Pages;

namespace JobPortalProjectMVC.Controllers
{
    
    public class JobApplicationsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public JobApplicationsController(AppDbContext context, IWebHostEnvironment hostEnvironment, UserManager<Users> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }
        [Authorize(Roles = "JobSeeker")]
        public async Task<IActionResult> Apply(int jobPostId)
        {
            // Check if jobPostId is valid
            if (jobPostId <= 0)
            {
                TempData["ErrorMessage"] = "The job post you're trying to apply to does not exist.";
                return RedirectToAction("Index", "JobPosts");
            }

            var jobPost = await _context.JobPosts.FirstOrDefaultAsync(j => j.Id == jobPostId);

            // Check if jobPost exists
            if (jobPost == null)
            {
                TempData["ErrorMessage"] = "The job post you're trying to apply to does not exist.";
                return RedirectToAction("Index", "JobPosts");
            }

            var model = new JobApplicationViewModel
            {
                JobPostId = jobPostId
            };

            return View(model);
        }
        [Authorize(Roles = "JobSeeker")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(JobApplicationViewModel model)
        {
            // Log the JobPostId to ensure it's being passed correctly
            Console.WriteLine($"JobPostId: {model.JobPostId}");

 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from claims

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "You must be logged in to apply.";
                return RedirectToAction("Login", "Account"); // Redirect to login if not logged in
            }

            // Ensure CoverLetter is not empty
            if (string.IsNullOrEmpty(model.CoverLetter))
            {
                TempData["ErrorMessage"] = "Cover letter URL is required.";
                return View(model); // Return with an error message if no cover letter URL is provided
            }

            // Verify that the JobPostId exists in the JobPosts table
            var jobPost = await _context.JobPosts
                .FirstOrDefaultAsync(j => j.Id == model.JobPostId);

            if (jobPost == null)
            {
                TempData["ErrorMessage"] = "The job post you're trying to apply to does not exist.";
                return View(model); // Return with an error message if JobPostId is invalid
            }

            var existingApplication = await _context.JobApplications
                .FirstOrDefaultAsync(app => app.UserId == userId && app.JobPostId == model.JobPostId);

            if (existingApplication != null)
            {
                TempData["ErrorMessage"] = "You have already applied for this job.";
                return View(model); // Stay on the same page and show the error
            }
            // Assign UserId from the logged-in user
            model.UserId = userId;

            // Set the AppliedDate to current date
            model.AppliedDate = DateTime.Now;

            try
            {
                // Create a new JobApplication entity from the view model
                var application = new JobApplication
                {
                    JobPostId = model.JobPostId,
                    CoverLetter = model.CoverLetter, // Save the URL of the cover letter
                    UserId = model.UserId,
                    AppliedDate = model.AppliedDate
                };

                // Add the application to the context and save changes
                _context.JobApplications.Add(application);
                await _context.SaveChangesAsync();

                /*TempData["SuccessMessage"] = "Your application has been submitted successfully!";
                return RedirectToAction("Details", "Index", new { id = model.JobPostId });*/
                TempData["SuccessMessage"] = "Your application has been submitted successfully!";
                return View(model);
            }
            catch (Exception ex)
            {
                // Log any errors during the save operation
                Console.WriteLine($"Error submitting application: {ex.Message}");
                TempData["ErrorMessage"] = "There was an error submitting your application. Please try again.";
            }

            return View(model); // Return view with an error message if exception occurs
        }

        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> RemoveApplication(int id)
        {
            var jobApplication = await _context.JobApplications
                .Include(ja => ja.User)
                .FirstOrDefaultAsync(ja => ja.ApplicationId == id);

            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        [HttpPost, ActionName("RemoveApplication")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var jobApplication = await _context.JobApplications
                .FirstOrDefaultAsync(ja => ja.ApplicationId == id);

            if (jobApplication == null)
            {
                return NotFound();
            }

            _context.JobApplications.Remove(jobApplication);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { jobPostId = jobApplication.JobPostId });
        }

    }
}

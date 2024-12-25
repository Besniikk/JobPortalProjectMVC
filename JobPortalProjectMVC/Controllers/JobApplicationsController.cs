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
            if (jobPostId <= 0)
            {
                TempData["ErrorMessage"] = "The job post you're trying to apply to does not exist.";
                return RedirectToAction("Index", "JobPosts");
            }

            var jobPost = await _context.JobPosts.FirstOrDefaultAsync(j => j.Id == jobPostId);

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
            Console.WriteLine($"JobPostId: {model.JobPostId}");

 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "You must be logged in to apply.";
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(model.CoverLetter))
            {
                TempData["ErrorMessage"] = "Cover letter URL is required.";
                return View(model);
            }

            var jobPost = await _context.JobPosts
                .FirstOrDefaultAsync(j => j.Id == model.JobPostId);

            if (jobPost == null)
            {
                TempData["ErrorMessage"] = "The job post you're trying to apply to does not exist.";
                return View(model);
            }

            var existingApplication = await _context.JobApplications
                .FirstOrDefaultAsync(app => app.UserId == userId && app.JobPostId == model.JobPostId);

            if (existingApplication != null)
            {
                TempData["ErrorMessage"] = "You have already applied for this job.";
                return View(model);
            }
            model.UserId = userId;

            model.AppliedDate = DateTime.Now;

            try
            {
                var application = new JobApplication
                {
                    JobPostId = model.JobPostId,
                    CoverLetter = model.CoverLetter,
                    UserId = model.UserId,
                    AppliedDate = model.AppliedDate
                };

                _context.JobApplications.Add(application);
                await _context.SaveChangesAsync();

             
                TempData["SuccessMessage"] = "Your application has been submitted successfully!";
                return View(model);
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error submitting application: {ex.Message}");
                TempData["ErrorMessage"] = "There was an error submitting your application. Please try again.";
            }

            return View(model);
        }

    }
}

﻿using JobPortalProjectMVC.Data;
using JobPortalProjectMVC.Interfaces;
using JobPortalProjectMVC.Models;
using JobPortalProjectMVC.Services;
using JobPortalProjectMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortalProjectMVC.Controllers
{
    
    [Authorize(Roles = "Admin,Employer,JobSeeker")]
    public class JobPostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPhotoService _photoService;


        public JobPostController(AppDbContext context, IPhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }

        [Authorize(Roles = "Admin,Employer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Create([Bind("JobTitle,Description,Requirements,Salary,Location,Category,Image")] JobPostViewModel jobPost)//mos duhet mja shtu userid mas category
        {

            if (ModelState.IsValid)
            {
                 var result = await _photoService.AddPhotoAsync(jobPost.Image); //to add a image or photoupload
                if (result == null || string.IsNullOrEmpty(result.Url.ToString()))
                {
                    ModelState.AddModelError("Image", "Image upload failed. Please try again.");
                    return View(jobPost);
                }

                var JobModel = new JobPost();
                    JobModel.Category = jobPost.Category;
                    JobModel.JobTitle = jobPost.JobTitle;
                    JobModel.Description = jobPost.Description;
                    JobModel.Requirements = jobPost.Requirements;
                    JobModel.Salary = jobPost.Salary;
                    JobModel.Location = jobPost.Location;
                    JobModel.Image = result.Url.ToString();
                    JobModel.PostedDate = DateTime.Now;

                    JobModel.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                    _context.Add(JobModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                
            }
                return View(jobPost);
           
        }

        [Authorize(Roles ="Admin,Employer,JobSeeker")]
        public IActionResult Index()
        {
            
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            bool isAdminOrJobSeeker = User.IsInRole("Admin") || User.IsInRole("JobSeeker");

            var jobPosts = isAdminOrJobSeeker
                ? _context.JobPosts.ToList() 
                : _context.JobPosts.Where(i => i.UserId == userId).ToList();

            var jobModel = jobPosts.Select(item => new JobPostViewModel
            {
                Id = item.Id,
                Category = item.Category,
                JobTitle = item.JobTitle,
                Description = item.Description,
                Requirements = item.Requirements,
                Salary = item.Salary,
                Location = item.Location,
                PostedDate = item.PostedDate
            }).ToList();

            return View(jobModel);
        }

        // GET: JobPost/Edit/5
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPost = await _context.JobPosts.FindAsync(id);

            if (jobPost == null)
                return NotFound();

            var _obj = new JobPostViewModel();

            _obj.Id = jobPost.Id;
            _obj.Category = jobPost.Category;
            _obj.JobTitle = jobPost.JobTitle;
            _obj.Description = jobPost.Description;
            _obj.Requirements = jobPost.Requirements;
            _obj.Salary = jobPost.Salary;
            _obj.Location = jobPost.Location;
            
            return View(_obj);
        }

        // POST: JobPost/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JobTitle,Description,Requirements,Salary,Location,Category,PostedDate")] JobPostViewModel jobPost)
        {
            if (id != jobPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var JobModel = new JobPost();

                    if (jobPost.Id == null)
                        return NotFound();

                    JobModel.Id = jobPost.Id ?? 0;
                    JobModel.Category = jobPost.Category;
                    JobModel.JobTitle = jobPost.JobTitle;
                    JobModel.Description = jobPost.Description;
                    JobModel.Requirements = jobPost.Requirements;
                    JobModel.Salary = jobPost.Salary;
                    JobModel.Location = jobPost.Location;
                    JobModel.PostedDate = DateTime.Now;
                    JobModel.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                    _context.Update(JobModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobPostExists(jobPost.Id ?? 0))
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
            return View(jobPost);
        }

        // GET: JobPost/Delete/5
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPost = await _context.JobPosts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPost == null)
            {
                return NotFound();
            }

            var _obj = new JobPostViewModel();

            _obj.Id = jobPost.Id;
            _obj.Category = jobPost.Category;
            _obj.JobTitle = jobPost.JobTitle;
            _obj.Description = jobPost.Description;
            _obj.Requirements = jobPost.Requirements;
            _obj.Salary = jobPost.Salary;
            _obj.Location = jobPost.Location;
            return View(_obj);
        }

        // POST: JobPost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobPost = await _context.JobPosts.FindAsync(id);
            _context.JobPosts.Remove(jobPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobPostExists(int id)
        {
            return _context.JobPosts.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var jobPost = await _context.JobPosts
                .Where(j => j.Id == id)
                .Select(j => new DetailsViewModel
                {
                    JobTitle = j.JobTitle,
                    Description = j.Description,
                    Salary = j.Salary,
                    Requirements = j.Requirements,
                    Location = j.Location,
                    Category = j.Category,
                    PostedDate = j.PostedDate,
                    ImagePath = j.Image // Adjust field names as needed
                })
                .FirstOrDefaultAsync();

            if (jobPost == null)
            {
                return NotFound();
            }

            return View(jobPost);
        }
    }

}
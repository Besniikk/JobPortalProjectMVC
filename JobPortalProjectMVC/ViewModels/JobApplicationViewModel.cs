using JobPortalProjectMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace JobPortalProjectMVC.ViewModels
{
    public class JobApplicationViewModel
    {
        public int Id { get; set; }
        [Required]
        public int JobPostId { get; set; }

        [Display(Name = "Cover Letter")] 
        public string CoverLetter { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime AppliedDate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace JobPortalProjectMVC.ViewModels
{
    public class CreateJobViewModelcs
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100)]
        public string JobTitle { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Requirements { get; set; }

        public decimal Salary { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Category { get; set; }

        public IFormFile Image { get; set; }

        // Add the UserId to associate the job with the logged-in employer
        public string? UserId { get; set; } = null;
        // To associate the job with the logged-in user (Employer)
    }
}

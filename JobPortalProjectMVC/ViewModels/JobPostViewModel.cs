using JobPortalProjectMVC.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace JobPortalProjectMVC.ViewModels
{
    public class JobPostViewModel
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

        public DateTime PostedDate { get; set; }

        public IFormFile? Image { get; set; }
        public string ImagePath { get; set; }

        public string? UserId { get; set; } = null;
        //public string EmployerName { get; set; } // New field for employer name or email

    }
}
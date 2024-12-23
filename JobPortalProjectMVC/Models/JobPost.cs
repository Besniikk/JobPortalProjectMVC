using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace JobPortalProjectMVC.Models
{
    public class JobPost
    {
        [Key]
        public int Id { get; set; }
        
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

        public string Image { get; set; }

        [ForeignKey("User")] 
        //public string? UserId { get; set; }
       
        public string UserId { get; set; }

        [IgnoreDataMember]
        public virtual Users User { get; set; }
        public virtual ICollection<JobComment> JobComments { get; set; }
        public virtual ICollection<JobApplication> JobApplications { get; set; }


    }
}

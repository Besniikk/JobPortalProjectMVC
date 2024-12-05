using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace JobPortalProjectMVC.Models
{
    public class JobApplication
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required]
        public int JobPostId { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime AppliedDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(500)]
        public string CoverLetter { get; set; }

        [ForeignKey("JobPostId")]
        public virtual JobPost JobPost { get; set; }

        [ForeignKey("UserId")]
        public virtual Users User { get; set; }
    }
}

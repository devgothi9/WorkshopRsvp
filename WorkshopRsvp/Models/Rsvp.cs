using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopRsvp.Models
{
    /// <summary>
    /// Represents one person's RSVP for a workshop.
    /// </summary>
    public class Rsvp
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(80, ErrorMessage = "Name can be at most 80 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "I need an accommodation")]
        public bool NeedsAccommodation { get; set; }

        [Required(ErrorMessage = "Please choose a workshop.")]
        [Display(Name = "Workshop")]
        public string WorkshopTitle { get; set; } = string.Empty;

        [Display(Name = "Registered On")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // Preserve the Lab 3 form and confirmation without adding a sixth DB column.
        [NotMapped]
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}

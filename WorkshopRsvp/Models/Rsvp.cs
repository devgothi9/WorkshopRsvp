using System.ComponentModel.DataAnnotations;

namespace WorkshopRsvp.Models
{
    /// <summary>
    /// Represents one person's RSVP for a workshop.
    /// </summary>
    public class Rsvp
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(80, ErrorMessage = "Name can be at most 80 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please choose a workshop.")]
        [Display(Name = "Workshop")]
        public string WorkshopTitle { get; set; } = string.Empty;

        [Display(Name = "I need an accommodation")]
        public bool NeedsAccommodation { get; set; }

        [Display(Name = "Registered On")]
        public DateTime RegisteredOn { get; set; } = DateTime.Now;
    }
}

using System.ComponentModel.DataAnnotations;

namespace Inlamning_Webbapp.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "First name")]
        public string? FirstName { get; set; }
        [Required]
        [Display(Name = "Last name")]
        public string? LastName { get; set; }
        [Required]
        public int Age { get; set; }
    }
}

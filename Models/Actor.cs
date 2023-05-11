using System.ComponentModel.DataAnnotations;

namespace Inlamning_Webbapp.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Display(Name = "First name")]
        public string? FirstName { get; set; }
        [Display(Name = "Last name")]
        public string? LastName { get; set; }

        public int Age { get; set; }
    }
}

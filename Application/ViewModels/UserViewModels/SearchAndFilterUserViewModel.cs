using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.UserViewModels
{
    public class SearchAndFilterUserViewModel
    {
        public Guid? Id { get; set; }
        public string? Fullname { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string Level { get; set; }
        public string Type { get; set; }
    }
}

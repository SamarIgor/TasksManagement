using Microsoft.AspNetCore.Identity;

namespace task.Models{
    public class AccountUser : IdentityUser{
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? City { get; set; }
    }
}
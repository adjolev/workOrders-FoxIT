using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WorkOrders.Models
{
    public class Login
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
    public class Register
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Role { get; set; }
        [Required]
        public string? Password { get; set; }
        public List<IdentityRole>? Roles { get; set; }
    }
    public class ForgotPassword
    {
        [Required]
        public string? Username { get; set; }
        public string? Token { get; set; }
    }

    public class ResetPassword
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

        public string Token { get; set; }


    }
}

using NexDevs.Models;
using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class UserImage
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Bio { get; set; }

        public IFormFile ProfilePictureUrl { get; set; }

        public string ImageUrl{get; set;}

        public char ProfileType { get; set; }

        public string Salt { get; set; }
    }
}
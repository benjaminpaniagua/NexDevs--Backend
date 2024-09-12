﻿using NexDevs.Models;
using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Bio { get; set; }
        public string ProfilePictureUrl { get; set; }
        public char ProfileType { get; set; }
    }
}
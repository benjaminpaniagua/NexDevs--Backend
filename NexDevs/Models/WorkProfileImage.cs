﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexDevs.Models
{
    public class WorkProfileImage
    {
        [Key]
        public int WorkId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Number { get; set; }

        public string Password { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string WorkDescription { get; set; }

        public IFormFile ProfilePictureUrl { get; set; }

        public string ImageUrl { get; set; }

        public char ProfileType { get; set; }
        
        public string Salt { get; set; }
    }
}

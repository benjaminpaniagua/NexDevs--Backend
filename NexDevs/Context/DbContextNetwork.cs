﻿using Microsoft.EntityFrameworkCore;
using NexDevs.Models;

namespace NexDevs.Context
{
    public class DbContextNetwork: DbContext
    {
        public DbContextNetwork(DbContextOptions<DbContextNetwork> options) : base(options) {}

        public DbSet<Collection> Collections { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkCategorie> WorkCategories { get; set; }
        public DbSet<WorkProfile> WorkProfiles { get; set; }
        public DbSet<WorkSkill> WorkSkills { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<StudentCategoryModel> StudentCategories { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<PermissionModel> Permissions { get; set; }
        public DbSet<StudentModel> Students { get; set; }

        // Other DbSet properties for additional entities

        // Your DbContext configuration goes here

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entity relationships and constraints here
            modelBuilder.Entity<StudentCategoryModel>().ToTable("tb_student_category");
            modelBuilder.Entity<RoleModel>().ToTable("tb_role");
            modelBuilder.Entity<PermissionModel>().ToTable("tb_permission");
            modelBuilder.Entity<StudentModel>().ToTable("tb_student");
        }
    }
}

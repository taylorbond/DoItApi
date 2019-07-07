using System;
using DoItApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DoItApi.Data
{
    public class DoItDbContext : DbContext
    {
        public DoItDbContext(DbContextOptions<DoItDbContext> options) : base(options)
        {
        }

        public DbSet<Task> Tasks { get; set; }
    }
}

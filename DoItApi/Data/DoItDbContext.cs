using DIA.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DoItApi.Data
{
    public class DoItDbContext : DbContext
    {
        public DoItDbContext(DbContextOptions<DoItDbContext> options) : base(options)
        {
        }

        public DbSet<DiaTask> Tasks { get; set; }
    }
}

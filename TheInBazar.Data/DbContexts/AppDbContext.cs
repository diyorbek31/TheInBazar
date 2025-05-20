using Microsoft.EntityFrameworkCore;
using TheInBazar.Domain.Entities;

namespace TheInBazar.Data.DbContexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    public DbSet<User> Users { get; set; }
}

using DevopsBasic.Models;
using Microsoft.EntityFrameworkCore;
using DevopsBasic.Models;

namespace DevopsBasic.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Student> Students => Set<Student>();
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Repository.Context;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        this.Database.SetCommandTimeout(60);
    }

    public DbSet<Ad> Ads { get; set; }
}


using Microsoft.EntityFrameworkCore;

namespace ISRORBilling.Database;

public class NemoAccountContext : DbContext
{
    public DbSet<NemoUser> Users { get; set; }

    public NemoAccountContext(DbContextOptions<NemoAccountContext> options) : base(options)
    {
    }
}
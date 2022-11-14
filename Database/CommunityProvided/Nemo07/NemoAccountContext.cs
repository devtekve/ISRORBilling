using Microsoft.EntityFrameworkCore;

namespace ISRORBilling.Database.CommunityProvided.Nemo07;

public class NemoAccountContext : DbContext
{
    public DbSet<NemoUser> Users { get; set; }

    public NemoAccountContext(DbContextOptions<NemoAccountContext> options) : base(options)
    {
    }
}
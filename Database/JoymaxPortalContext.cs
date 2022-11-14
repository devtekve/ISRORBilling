using ISRORBilling.Models.Authentication;
using Microsoft.EntityFrameworkCore;

namespace ISRORBilling.Database;

public class JoymaxPortalContext : DbContext
{
    public virtual DbSet<AUserLoginResponse> AUserLoginResponses { get; set; }

    public JoymaxPortalContext(DbContextOptions<JoymaxPortalContext> options) : base(options)
    {
        
    }
}
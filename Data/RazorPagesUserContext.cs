using Microsoft.EntityFrameworkCore;

using alma.Models;

namespace alma.Data {
    public class RazorPagesUserContext : DbContext {
        public RazorPagesUserContext(DbContextOptions<RazorPagesUserContext> options)
            : base(options) {
        }

        public DbSet<User> User { get; set; } = default!;
    }
}

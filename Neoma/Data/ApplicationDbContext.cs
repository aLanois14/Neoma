using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Neoma.Models;

namespace Neoma.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Specialite> Specialite { get; set; }
        public DbSet<Organisme> Organisme { get; set; }
        public DbSet<SpecialiteUtilisateur> SpecialiteUtilisateur { get; set; }
        public DbSet<Projet> Projet { get; set; }
        public DbSet<Candidature> Candidature { get; set; }
        public DbSet<Membre> Membre { get; set; }
        public DbSet<MembreSpecialite> MembreSpecialite { get; set; }
        public DbSet<TypeProjet> TypeProjet { get; set; }
        public DbSet<Besoins> Besoins { get; set; }
        public DbSet<BesoinsSpecialite> BesoinsSpecialite { get; set; }
        public DbSet<RoleUtilisateur> RoleUtilisateur { get; set; }
        public DbSet<Selection> Selection { get; set; }
        public DbSet<Proposition> Proposition { get; set; }
        public DbSet<Conversation> Conversation { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<ConversationUtilisateur> ConversationUtilisateur { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

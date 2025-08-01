using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SipkaTemplate.Core.Models; 
using SipkaTemplate.Repository.Configurations;
namespace SipkaTemplate.Repository
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        #region Base 
        public DbSet<User> Users { get; set; }
        public DbSet<OtpResetToken> OtpResetTokens { get; set; }
       

        #endregion



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Metabol.DbModels.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 
        /// </summary>
        public string Affiliation { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }



        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<AnalysisModels> Analyses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="authenticationType"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MetabolApiDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AnalysisModels> Analyses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MetabolApiDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //this.Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer<MetabolApiDbContext>(null);
        }

        public static MetabolApiDbContext Create()
        {
            return new MetabolApiDbContext();
        }
    }
}
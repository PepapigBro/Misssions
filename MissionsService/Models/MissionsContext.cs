using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MissionsService.Models
{
    public class MissionsContext : DbContext
    {
        public MissionsContext()
            : base("MissionsContext")
    {
        }
        public DbSet<Mission> Missions { get; set; }
    }


    class MissionsContextInitializer : DropCreateDatabaseIfModelChanges<MissionsContext>
    {
        protected override void Seed(MissionsContext db)
        {
           
        }
    }
}



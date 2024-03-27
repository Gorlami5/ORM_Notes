using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker
{
    public class ConnectionDb:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            //Yukarıda default olarak ayarlanmıştır.İstenilirse NoTracking veya NoTrackingWithIdentityResoluation olarak da ayarlanabilir.
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var datas = ChangeTracker.Entries();
            foreach (var data in datas)
            {
                if(data.State == EntityState.Added)
                {
                    //data.State = EntityState.Modified; bu yapı gibi manevralar yapmamızı sağlayacaktır.
                    //Genelde kullanılma sebebi auto generate edilmeyen ıd kolonunu ekleme durumunda manuel olarak atanmasıdır.
                }
            }
            return base.SaveChangesAsync(cancellationToken);    
        }
        public DbSet<Personal> Personals { get; set; }
        public class Personal
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Surname { get; set; }
        }
    }
}

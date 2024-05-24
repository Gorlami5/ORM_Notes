using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Metadata;

#region OnModelCreating
//Genel anlamda veritabanı konfigürasyonları dışında entityler için yapılan konfigürasyonların yapılığı fonksiyondur.
#endregion
#region IEntityTypeConfiguration
//IEntityTypeConfiguration yapısı bizlere her entity için bir harici dosya verir bunlar içerisinde tablolar konfigüre edilir.
//Bunun amacı Context içerisinde OnModelCreating metodu tüm entityler için her konfigürasyonu barındırıp çok şişmesin ve sistematik bir yapıda olalım.

#endregion
#region ApplyConfigurationFromAssembly
//Her entity için configure sınıfı oluşturduktan sonra bunları tek tek OnModelCreating içine eklemektense bu metot yardımıyla tek seferde ekleyebiliriz.
#endregion
#region Seed Data
//EF Core ile inşa edilen veri tabanında default olarak eklenen veya eklenmek istenen verilere seed data denir.Her migrate işleminde de seed dataları veri tablosunda otomatik olarak ekleyebiliriz.
//Seed datalar genellikle yazılım tarafında tutularak her migrate işleminde gerçekleştirilebilir.
//Test için geçici verilere veya temel konfigürasyonlar için seed datalar kullanılabilir.
#endregion
class Post
{
    public int Id { get; set; }
    public string MyProperty { get; set; }
    public int MyProperty1 { get; set; }
    public Blog Blog { get; set; }
}
class Blog
{
    public int Id { get; set; }
    public string MyProperty { get; set; }
    public List<Post> Posts { get; set; }
}
class Order
{
    public int OrderId { get; set; }
    public string Description { get; set; }
    public int OrderDate { get; set; }
}
class OrderConfiguration : IEntityTypeConfiguration<Order> //Bu class adı üzerinde konfigürasyon işlemlerinin yapıldığı class olacaktır.Bütün işlemler bu class içerisinde Order'a özel olacak ve arandığında kolayca bulunacak.
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.OrderId);
    }
}
class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }

    public DbSet<Order> Orders { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       /*modelBuilder.ApplyConfiguration(new OrderConfiguration());*/ //Yapının modelcreating içerisinde tanımlaması burda bulunuyor.
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyBuilder.GetExecutingAssembly()); // artık execute olmuş assembly'ler arasından arayıp buraya ekleyecektir.
        #region Seed Data Ekleme
        modelBuilder.Entity<Blog>().HasData(
              new Blog()
              {
                  Id = 1,
                  MyProperty = "1",
              },
               new Blog()
               {
                   Id = 1,
                   MyProperty = "1",
               }
               );

        //Seed data eklerken Id kolonunu manuel olarak bildirmemiz lazım aksi takdirde hata alırız.Auto Increment özelliği olsa dahi bildirmemiz gerekiyor.
        //İlişkisel verilerde seed data eklerken dependent entity içerisinde olan FK değerini yine manuel olarak bildirmeye ihtiyacımız var.Burda bildirilen data
        //principal entity içerisinde yoksa kendi otomatik olarka bir seed data oluşturup veritabanına basacaktır.Ama en kolay yolu dependent entity' de bir seed data eklemek olacaktır.

        //Seed dataların Id'leri değiştirilirse Cascade işlemi gibi davranıp eskiyi silip yeniyi ekleyecektir.Bağlantılı olduğu veriyi de silecektir.Migrate ayrıntılarında da bunu görebiliriz.
        //Her seed data bir defa eklenir.HHer migrate edilme sürecinde tekrardan eklenme gibi bir durum yaşamayacağız.Yani dependent entity içinde artık olmayan FK değerli bir nesne 
        //Varsa bile dahha önceden eklendiği için herhangi bir hata döndermeyecektir.
        #endregion
    }
}

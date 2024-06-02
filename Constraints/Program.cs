// See https://aka.ms/new-console-template for more information
#region Primary Key
//Bir veri tablosunda benzersiz kayıtları tanımlamak için kullanılan sütün veya sütün kombinasyonlarıdır.
//Bir tabloda PK tanımlamak için Default Convention dışına çıkıyorsak eğer Fluent API(HasKey) veya DataAttributes([Key]) kullanırız.
#region Alternate Keys
//Bir entityde PK dışında benzersiz verileri tutacak olan propery özelliğine alternate keys denir.Composite yapıda da olabilir.Fluent API ile kullanılır.
#endregion
#endregion
#region Foreign Key
//Bir entity'de başka bir entity'nin PK değerine işaret eden kolondur.Bu iki kolon arasında bir ilişki fiziksel olarak olmaz zorunda değildir.
//DEfault convention durumunda entity içerisinde tanımlamasak da olur  çünkü default olarak shadow porperty olarak tanımlayacaktır.Eğer tanımlamak istersek Entity_adı+Id olarak tanımlarsak yine ef core anlayacaktır.
//FluentAPI ile veya Data Attributes ile de FK tanımlamaları yapabiliriz.
#endregion
#region Unique Constraint
//Bir sütünda verilerin benzersiz olmalarını sağlayan constrainttir.Daha önce örneği olan Alternate Key'de buna bir örnektir.
//Data Attribute ve Fluent API ile konfigürasyonları yapılır.
#endregion
#region CheckConstraint
//Check constraint belirli koşullar sağlayarak verilerin ona göre tabloya eklenmesine şartlar koşan yapıdır.
#endregion
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

[Index(nameof(Blog.Url),IsUnique = true)] //Index attribute sadece class seviyesinde kullanılır.Bu yapı Url prop'unu artık unique constraint gibi davranmasına yarar.
class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public int A { get; set; }
    public int B { get; set; }

    public List<Post> Posts { get; set; }
}
class Post
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Defination { get; set; }
    //public DateTime CreatedDate { get; set; } Shadow ile fiziksel tanımmlamaya ihtiyacımız kalmayan property
    public Blog Blog { get; set; }
}
class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }

    public DbSet<Blog> Orders { get; set; }
    public DbSet<Post> Posts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Alternate key kullanımına örnek bir durum.Name artık kendi içerisinde benzersiz değerler almalıdır.
        modelBuilder.Entity<Blog>().HasAlternateKey(b => b.Name);
        modelBuilder.Entity<Blog>().Property<int>("FK_Blog"); //Eğer entity içersinde property tanımlamadan sadece veritabanına yansıtmak istiyorsak bu şekilde kolon ekleme işlemi yaparız.
        modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(b => b.Blog).HasForeignKey("FK_Blog"); //İki tablo arasındaki ilişkiyi tanımladık ve FK değerini burda bildirdik.
        //Eğer FLuentApı ile bunu yapmıyor olsaydık ve DefaultConvention ile bunu yapıyor olsaydık ve FK'yı entity içerisinde tanımlamasaydık aynı işlemi EF Core kendi yapacaktır.
        modelBuilder.Entity<Blog>().HasIndex(b => b.Url).IsUnique(); // FLuent API ile de bunu yapabiliyoruz.
        modelBuilder.Entity<Blog>().HasCheckConstraint("c_constraint", "[A] < [B]"); //Bu constraint sayesinde veri girişi kontrol edilir ve şartları sağlayan veriler kabul edilir.
      
    }
}

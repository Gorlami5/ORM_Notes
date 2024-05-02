// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

#region Özelleştirici Entity Özellikleri
//Entityleri tanımlarken birçok özelliği default olarak entityframework Core tanımlar.Fakat biz bu default davranışlardan çıkmak ve kendi ayarlarımızı yapmak istersek bu özellikleri kullanırız.
//Bu özellikleri genellikle Data Annotations Attributeları ile yada Fluent Api ile OnModelCreating fonksiyonu içerisinde belirtiriz.
#endregion
#region Table - ToTable
//EFCore oluşturulan entity ile veritabanında tablo oluştururken default olarak Set edildiği yerdeki ismi alacaktır.DEğiştirmek istenirse Table attribute ile veya Fluent api ile yapılabilir.[Table("Kişiler")]{1}
#endregion
#region HasColumn&HasColumnType
//EfCore da entity tanımlarken entity propertyleri veritabanındaki kolonlara karşılık gelir ve bu kolonlar isimlerini entity proplarından alır.Eğer bunu değiştirmek istersek data annotations veya fluent api kullanılabilir.{2}
#endregion
#region ForeingKey&HasForeignKey
//EfCore ilişkili entityler için kendi otomatik olarak FK tanımlar.Eğer bu yapıyı özelleştirmek istersek Data annotations kullanılabilir.Yani Fk kolonunu farklı bir isimler farklı bir propa atayabiliriz.{3}
#endregion
#region NotMapped&Ignore
//Tanımlanman entityler veritabnanına yansıtıldığında entity içinde proplar kolonlara eş olur.Fakat bazen bazı propları veritabanı kolonu olarak kullanmak istemeyebilriiz.Bu durumda NotMapped ve Ignore kullanılmalı.{4}
#endregion
#region Key-HasKey
//EfCore DEfault convention olarak Id Veya entityId şeklinde proplara primary key constraint olarak işaretler ve uygular.Key ve HasKey ile bunu istediğimiz propa verebilir veya Id kolonunun ismini değiştirebiliriz.{5}
#endregion
#region Required&IsRequired
//Entity içerisindeki proplara karşılık gelen tablo kolonlarının null olup olamayacağını belirler.Default olarak notnull'dır.Fakat burda nullable olarak da işaretleme yapabiliriz.Nullable ? ile yapılacaktır.{6}
#endregion
#region MaxLength & StringLength
//Entity içerisinde propertrylerin karakter sayılarını kontrol etmemize yarayan bir özelliktir.{7}
#endregion
#region Precision & HasPrecision
//Sayılarda kaç haneli olup olmadığının kontrolünü yapar.Virgülden sonrası için de kontrol yapar.{8}
#endregion
#region Unicode&IsUniCode
//İçeriğin unicode içerip içermediğinin kontrolünü yapar.
#endregion
#region Comment
//Veri eklerken o veriye etki etmeyen fakat yorum satırı olarak gözükmesine yarayan özelliktir.{9}
#endregion
#region InverseProperty
//Bire çok ilişkili entitylerde bazı durumlarda 2 defa ilişki kurma gereğinde olabiliriz.Yani iki tane navigation proplarımız olabilir.Bu durumda migrate ettiğimizde hata alırız.Çözmek için 
//Data Annotations attributları ile InversProperty ile işaretlememiz gerekecektir.Bu durumda sorunsuz olarak çalışmaya devam edecektir.
#endregion
Console.WriteLine("Hello, World!");
class DbConnection:DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region GetEntityTypes
        //Tabımlanmış entitlerin tür tiplerini bize dönderecektir.
        //var entites = modelBuilder.Model.GetEntityTypes();
        #endregion
        #region HasColumn&HasColumnType
        //modelBuilder.Entity<Personal>().Property(p => p.Name).HasColumnName("Ad").HasColumnType("string"); fluent api ile birlikte column ayarları
        #endregion
        #region ForeignKey&HasForeignKey
        //modelBuilder.Entity<Personal>().HasOne(p => p.Departman).WithMany(p => p.Personals).HasForeignKey(p => p.DId);
        #endregion
        #region NotMapped&Ignore
        /* modelBuilder.Entity<Personal>().Ignore(p => p.SodexoId);*/ //Fluent Api ile Ignore yapma
        #endregion
        #region Key&HasKey
        //modelBuilder.Entity<Personal>().HasKey(P => P.PersonalIdKey);
        #endregion
        #region Composite Key
        //Composite Key bir tabloda birden fazla PK bulunmasıdır.İkisi de birbiriyle beraber toplu olarak çalışır.
        //modelBuilder.Entity<Personal>().HasKey("PersonalIdKey", "Dıd");
        #endregion
        #region HasDefaultSchema
        //Ef Core ile oluşturlan tablolar default olarak bir schema ismine sahiptir.Bunları HasDefaultSchema ile değiştirebiliriz.
        //modelBuilder.HasDefaultSchema("ExampleSchema");
        #endregion
        #region HasDefaultValue
        //Tabloya eklenecek yeni bir veride bir kolonun default değeri bulunabilir.Yani null olarak gidecekken bir değer alarak bu değeri de otomatik olarak alacak şekilde ayarlanır.
        //modelBuilder.Entity<Personal>().Property(p=> p.Salary).HasDefaultValue(100);
        //Yeni bir Personal ekleneceği zaman eğer değiştirlmezse Salary değeri 100 olarak girilecektir.
        #endregion
        #region HasComputedColumnSql
        //Bir tabloda compute yapabilen bir kolon bulunabilir.Bunu otomatik olarak ayarlayıp başka bir config gerektirmeden veritabanında yaptırabiliriz.Örneğin X ve Y in türünde iki kolonu otomatik olarak toplayan bir örnek.
        //Bunu da tabloda tutan bir örnekle pekiştirebiliriz.
        //modelBuilder.Entity<Example>().Property(p=> p.Computed).HasComputedColumnSql("[X] + [Y]");
        #endregion
        #region HasConstraintName
        //Constraintlerin default adlarını değiştirmemize ve yönetmemize yarayacaktır.Bunu farklı yollarla DataAnnotationslarla da yapabiliyorduk.
        #endregion
        #region HasData
        //Veritabanına inşa ederken yazılım üzerinden mock dataları eklemek isteyebiliriz.Bunu Seed Data ile daha da pekiştireceğiz.Fluent API ile hazır data eklemek istersek bunu HasData ile yaparız.
        #endregion
        #region HasDiscriminator
        //En basit tanımı birbirinden kalıtım almış tabloların veritabanına yansıtılımasında bazı ayarlara ihtiyaç duyarız.Bunu HasDiscriminator ile gerçekleştiririz.Daha detaylı incelenecek.
        #endregion
        #region HasNoKey
        //Normal şartlarda her entity'de bir PK olmak zorundadır.Eğer ki PK olamayacaksa bunu bildirme zorunluluğumuz vardır.
        //modelBuilder.Entity<Example>().HasNoKey();
        #endregion
        #region HasQueryFilter
        //Bazı entityler üzerinde yapacağımız sorgulara default sorgular ekleyebiliriz.Yani gelecek veriler her zaman bu filtreye maruz kalarak gelecektir.Herhangi bir sorgu yaparken ek olarak bu da varmış gibi düşünülebilir.
        //modelBuilder.Entity<Example>().HasQueryFilter(a=> a.X == 13);
        //Example üzerinden yapılacak her sorgu artık X kolon değeri 13 olanlardır.Global filtreler incelenirken daha detaylı incelenebilir.
        #endregion
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<Personal> Personals { get; set; }
    public DbSet<Departman> Departments { get; set; }
    public DbSet<Example> Examples { get; set; }
}

//[Table("Kişiler")] Data Annotations ile tablo ismi değiştirme {1}
class Personal
{
    [Key] //Artık PersonalIdKey kolonu Primary Key constraint ile işlenecektir.{5}
    public int PersonalIdKey { get; set; }
    [ForeignKey(nameof(Departman))] // Burda Efcore default convention ile fk'yı yakalayamaz ve shadow olarak kendi Fk oluşturur.Fakat biz bu attribute ile DId kolonunu FK olarak generate edebileceğiz.{3}
    public int DId { get; set; }

    [Column("Ad",TypeName ="string")] //DataAnnotations ile Kolon ismi değiştirme {2}
    public string? Name { get; set; }
    [Required] // Notnull olarak işaretler. ? işareti de nullable olarak işaretler.Örnek olması için ikiside aynı anda bulunuyor.{6}
    public string? Surname { get; set; }
    [NotMapped] //Attribue bu şekilde veritabanına map etmez.{4}
    public string SodexoId { get; set; }
    [MaxLength(100)] // {7}
    //[StringLength(100)]
    public string City { get; set; }
    [Precision(2,2)] // 2 haneden fazla sayı olamaz ve virgülden sonrasını sadece 2 hane olarak gösterir.{8}
    public int Salary { get; set; }

    public Departman Departman { get; set; }
}
class Departman
{
    public int Id { get; set; }
    public string? Name { get; set; }
    [Comment("Address is address")] //Artık bu kolonun bir açıklaması olacaktır.{9}
    public string Address { get; set; }
    public List<Personal> Personals { get; set; }

}
class Example
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Computed { get; set; }
}
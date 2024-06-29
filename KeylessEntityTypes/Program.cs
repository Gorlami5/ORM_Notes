// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
ConnectionDb cbd = new ConnectionDb();

#region Keyless Entity Types 
//Keyless entity types normal entity'ler dışında primary key içermeyen ve özel sorgular sonucu query'leri karşılamaya yarayan tiplerdir.
//Genellikle aggregate fonksiyonları tarafından oluşturulan query'ler bir primary key ihtiyacı duymayan sonuclar dönderir.GroupBy örnekleri de bunlara dahildir.Bu queryler için keyless entity typeslar kullanılabilir.
#region Keyless Entity Type Tanımlama
//Keyless Entity Type tanımlarken bunun bir Entity gibi davranan class'ı,Bir DbSet Propertysi ve belirli konfigürasyonları olmalıdır.Bu konfigürasyonlar HasNoKey gibi bazı fonksiyonlardır.
//İlk olarak class oluşturulur ve bu bir Entitymiş gibi davranılarak DbSet property'si verilir.
//Fakat bu keyless entity type kullanılması için bunu view,storedprocedure veya inline fuction gibi yapılara atayarak sorgular yaparız.
//Migration ile oluşturulan bir view ile sorgunun kendisi yazılır ve bu migrationdan dönen tabloyu bu keyless yapıda tutarız.
//View ile ilişkilendirmeyi OnModelCreating metodunda yaparız.Aşağıda bu konfigürasyonlar bulunabilir.
var query = cbd.BlogPostCount.ToListAsync(); // oluşturulan keyless entity ile view direkt bu şekilde sorgulanabilir.
//ek olarak storedprocudre ile keyless entity type kullanmak istersek bazı durumlarda [NotMapped] attribute'u kullanılabilir.Fakat toView kullnımında Ef Core buna dikkat edip veritabanında mapleme yapmaya çalışmıyor.
#endregion
#endregion
class BlogPostCount()
{
    public string Name { get; set; }
    public int Count { get; set; }
}
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
class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }

    public DbSet<Blog> Orders { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<BlogPostCount> BlogPostCount { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlogPostCount>().HasNoKey().ToView("vw_ViewBlogPostCount");
    }
}
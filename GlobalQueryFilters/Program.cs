
using Microsoft.EntityFrameworkCore;
#region GlobalQueryFilters
//Bir entity üzerinden yaptığımız sorgularda belirli kolonlara dair genel geçer sorgu parametrelerini her seferinde kullanmak yerine global bir filtre ekleyerek sürekli sorguların bu şekilde davranmasını sağlayabiliriz.
//İlişkisel tablolar da bu işlemi yaparken ilişki üzerinden basit bir şekilde global query filter ekleyebiliriz.Ama ilişki olmadığı durumlarda bu filter geçerli olmyacaktır.
#endregion
#region Global Query Filter nasıl ignore edilir
//Bazı sorgularda ihtiyaç dahilinde global query'ler ignore edilmek istenebilir.Burda da yapılması gereken sorguya IgnoreQueryFilter eklenmesi yeterli olacaktır.
#endregion
class Person
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public List<Order> Orders { get; set; }
}
class Order
{
    public int Id { get; set; }
    public string OrderName { get; set; }
    public Person Person { get; set; }
}
class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }
    public DbSet<Person> Employees { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().HasQueryFilter(p => p.IsActive); //Artık sadece IsActive true olan person'lar geriye dönecektir.
        modelBuilder.Entity<Person>().HasQueryFilter(o => o.Orders.Count > 0); //
    }
}
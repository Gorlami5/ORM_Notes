// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
#region Owned Entity Types Nedir
//EF Core entity sınıflarını parçalayarak,kümesel olarak ayırıp başka sınıflarda barındırmamıza,ve tüm bu sınıfları asıl entity içerisinde toplayıp kullanabilmemize yarayan duruma owned entity types denir.
//Bir entity birden fazla owned entity birleşimi ile ortaya çıkabilmektedir.
#endregion
#region Owned Entity Types Nasıl Uygulanır
//Owned entityler eğer bir konfigürasyon yapılmaz ise EF Core tarafından iki entity arasında bir ilişki varmıl gibi algılar ve migration sürecinde hata verir.Bunun için EmployeeName ve EmployeeAddress sınıflarını Fluent API ile tanımlarız.
//Bütün bunlara ek olarak da IEntityTypeConfiguration ile de ownedentitytypes tanımlanabilir.
#endregion
#region OwnsMany Metodu
//OwnsMany metodu Own entity sahibi asıl entity içerisinde ICollection tipinde own entity barındırmasını ayarlamak için kullanılan metottur.Bu yapı oneTomany ilişkisine benzer fakat ilişkisel tabloda her iki tablonun da
//birer ID'si ve birbirlerine bağlayan navigation property'leri olur.Ek olarak owned entity bir DbSet gerektirmez fakat ilişkisel tablolarda bu zorunludur.Bu yapıda ICollection tipinde olan owned entity bir tablo olarak olsa da
//tek başına bir şey ifade etmeyecektir.
//Oluşturulacak bir Employee sorgusunda göreceğiz ki Employee'ler Orders içerecekler ve herhangi bir ilşşkisel tablo konfigürasyonu yapmadan.Bu durumda Employee aslında bir Owned Entity sahibi olduğunu anlarız.
//Veritabanı haliyle bu ilişkiyi fluent Apı içerisinde tanımladığımı FK ile kuruyor olacak.
#endregion
class Employee
{
    public int Id { get; set; }
    //public string Surname { get; set; }
    //public string Name { get; set; }
    //public string SecondName { get; set; }
    //public string Address { get; set; }
    //public string Location { get; set; }
    public bool IsActive { get; set; }
    public EmployeeName EmployeeName { get; set; }
    public EmployeeAddress EmployeeAddress { get; set; }
    public List<Order> Orders { get; set; }

}
class EmployeeName
{
    public string Surname { get; set; }
    public string Name { get; set; }
    public string SecondName { get; set; }

}
//[Owned] attribute'u ile de işaretleme yapabiliriz.
class EmployeeAddress
{
    public string Address { get; set; }
    public string Location { get; set; }
}
class Order
{
    public string OrderName { get; set; }
    public DateTime OrderDate { get; set; }
}
class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().OwnsOne(e => e.EmployeeName);
        modelBuilder.Entity<Employee>().OwnsOne(e => e.EmployeeAddress); // Burda Owned Entity olduğunu tanımlarız.builder parametresi ile de veritabanında oluşacak kolon ismi ayarlanabilir.
        modelBuilder.Entity<Employee>().OwnsMany(e => e.Orders,builder =>
        {
            builder.WithOwner().HasForeignKey("OwnedEmployeeId"); //Yani burda DbSet olmadığı halde oluşacak bağımlı tablonun bilgilerini veriyoruz.Ef Core buna göre bir Order tablosu oluşacak.
            builder.Property<int>("Id"); //Fakat oluşacak tabloya DbSet'i olmadığından direkt olarak erişme imkanımız olmayacak.Bu tablo bağımlı bir tablo olarak kalacak.
            builder.HasKey("Id");
        });

    }
}
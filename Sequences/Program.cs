#region Sequence Nedir
//Veritabanında benzersiz ve ardışık sayılar üreten bir veritabanı nesnesidir.
//Tablo özelliği değildir ayrı bir şekilde yapılandırılır.Ve birçok tabloda kullanılabilir.
//Sequence tanımlarken Fluent API kullanırız ve burdan sequence tanımlarız ardından sequencleri istediğimiz tablolarla ilişkilendiririz.
//Sequence yapısını aldığımız yerde sql cümleciği sql server türüne göre değişebilir.MSSQL ' de farklı iken Oracle'da farklı olabilir.
//Sequence yapısının en önemli özelliği kullanılan tabloları bir bütün olarak görür ve ayrı ayrı veri girişleri bile olsa bu tablolara artmaya devam edecektir.Yani Employees tablosuna bir veri eklendikten sonra
//Customer tablosuna bir veri ekleniyorsa İki tablodan birine eklenecek herhangi bir verinin ID değeri 3 olacaktır.
#endregion
#region Sequence ile identity farkı nelerdir?
//Sequence yapısı bir veritabanı nesnesidir ve herhangi bir tabloya bağımlı değildir fakat Id bir tablo nesnesidir ve bağımlıdır.
//Sequence yapısı bir sonraki değeri ramden alırken Id değeri diskten alır.Bu duurmdan kaynaklı olarak da sequence yapısı daha performanslıdır diyebiliriz.
#endregion
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SurName { get; set; }
    public string? Department { get; set; }
    public int Salary { get; set; }
}
class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SurName { get; set; }
    public string? Department { get; set; }
    public int Salary { get; set; }
}
class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence("EC_Sequence").StartsAt(1).IncrementsBy(1); //Sequence tanımladık
        modelBuilder.Entity<Employee>().Property(p => p.Id).HasDefaultValueSql("NEXT VALUE FOR EC_Sequence");
        modelBuilder.Entity<Customer>().Property(p => p.Id).HasDefaultValueSql("NEXT VALUE FOR EC_Sequence");// tablolarla ilişkilendirdik
    }
}

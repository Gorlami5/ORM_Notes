// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Reflection.Emit;
ConnectionDb cbd = new ConnectionDb();

#region BeginTransaction
//EfCore transaction yöntemini otomatik olarak kendi gerçekleştirir.Ama manuel olarak ele almak istersek aşağıdaki gibi yönetebiliriz.
IDbContextTransaction transaction = cbd.Database.BeginTransaction();
#endregion
#region CommitTransaction
//Yapılan değişiklikler başarıyla tamamlandı ve transaction  işlemini başarıyla bitirdiysek commit işlemi uygulanır.Veritabanına yansıtılır.
cbd.Database.CommitTransaction();
#endregion
#region RollbackTransaction
//Yapılan değişiklikler'de bir hata veya sorunla karşılaşırsak yapılan tüm değişikler iptal edilir ve hiçbiri veritabanına yansıtılmaz.Bu ileme Rollback işlemi denir.
cbd.Database.RollbackTransaction();
#endregion
#region CanConnect
//Veritabanı bağlantımız context sınıfında tasarlarız ve sql server'a yansıtırız fakat fiziksel olarak doğru bir şekilde bağlandığımızı öğrenmek istersek CanConnect metodunu kullanabiliriz.
var result = cbd.Database.CanConnect();
//bool bir değer dönecektir.
#endregion
#region EnsureCreated
//Veritabanına tasarlayıp bunu sql Server'a yansıtmak istersek migration yaparız.Eğer migration kullanmaz ve runTime'da kod üzerinden veritabanına sql server'a yansıtmak isersek bunu EnsureCreated ile yaparız.
var result2 = cbd.Database.EnsureCreated();
#endregion
#region EnsureDeleted
//Mevcut bağlantı halidne olduğumuz veritabanını silmek istersek EnsureDeleted fonksiyonunu kullanırız.
var result3 = cbd.Database.EnsureDeleted();
#endregion
#region GenerateCreateScript
//EfCore üzerinden CodeFirst yaklaşımı ile tasarladığımız veritabnanını tüm ayrıntılarıyla birlikte script halinde almak istersek bu fonksiyon kullanılır.Bu da bizi Ef Core bağımlılığından kurtarır.
cbd.Database.GenerateCreateScript();
#endregion

#region ExecuteSql
//Insert,Delete,Update işlemlerini ham sql sorglarıyla execute etmek istersek bu fonksiyonu kullanırız.Parametre olarak Formattablestring tipinde bir parametre alır.
cbd.Database.ExecuteSql($"Sql query");
//Eğer ExecuteSqlRaw kullanmak istersek bu yapı bize sql injection saldırılarına karşı korumaz ve sorumluluk bizdedir fakat ExecuteSql bunu otomatik olarak korumalı hale getirir.
#endregion
#region SqlQuery
//Ham sql sorgularıyla query yapmak istersek SqlQuery fonksiyonunu kullanırız.Fakat yeni sürümlerde desteklenmediğinde artık FromSql kullanılmaktadır.

#endregion
#region GetMigrations
//Şimdiye kadar oluşturduğumuz bütün migrationları bir koleksiyon olarak geri döndermek istersek GetMigrations metodunu kullanırız.
var query = cbd.Database.GetMigrations();
#endregion
#region GetAppliedMigrations
//Veritabanına yansıtılmış son migraton hangisi ise onu geri dönderen fonksiyondur.
var query2 = cbd.Database.GetAppliedMigrations();
#endregion
#region GetPendingMigrations
//Uygulanmayan migrationları geri dönderen fonksiyondur.
var query3 = cbd.Database.GetPendingMigrations();
#endregion
#region Migrate
//Migrationları programatik olarak migrate eden fonksiyondur.Yani migration oluşturmaz oluşan migrationlarla birlikte veritabanına yansıtır(Update Database).ClI veya packeg manager ihtiyacı olmadan bunları yapar.
cbd.Database.Migrate();
//EnsureCreated fonksiyonu migrationları kapsamaz ve içindekileri yansıtmaz.Fakat migrate fonksiyonu bütün migrationları işler ve sql server'a gönderir.
//Stored Procedure veya view gibi yapılar sadece migration üzerinden yapılandığından bunu EnsureCreated ile yansıtamayız ve birer migrationa ihtiyacımız olur.Bundan kaynaklı migrate fonksiyonu da kullanılabilir.
//Migrationları 
#endregion
#region OpenConnection
//Veritabanına bağlantıyı açma fonksiyonudur.
cbd.Database.OpenConnection();
#endregion
#region CloseConnection
//Veritabanı bağlantısı kapatan fonksiyonudur.
cbd.Database.CloseConnection();
#endregion
#region GetConnectionString
//Connection stringi elde etmeye yarayan fonksiyondur.
cbd.Database.GetConnectionString();
#endregion

class Employee
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasIndex(h => h.Name); // Fluent API ile indexleme.W
        //modelBuilder.Entity<Employee>().HasIndex(h => h.Name).IsUnique(); uniqueness örneği with Fluent API
        modelBuilder.Entity<Employee>().HasIndex(h => h.Name).IsDescending(/*false*/); // false olursa ascending davranışı sergiler.
        modelBuilder.Entity<Employee>().HasIndex(h => h.Name).HasFilter("[NAME] IS NOT NULL"); //filtreleme örneği
        //modelBuilder.Entity<Employee>().HasIndex(h => h.Name).IncludeProperties(k => k.SurName);
    }
}
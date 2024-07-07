// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Reflection.Emit;


#region ConnectionResilency Nedir
//Ef Core üzerinde yaptığımız çalışmalarda bazı durumlarda veritabanı bağlantısı kopmaları yaşanabilir ve bu da bize birçok sorun yaşatabilir.
//Connection Resilency ile bu kopmalar yaşandığında tekrardan bağlantı talepleri yollayabilirz veya exeution managment ile sorguları tekrarlı şekilde çalıştırabiliriz.
#endregion
#region EnableRetryOnFailure
// Bu yapı bizlere veritabanı kopmalarında tekrardan bağlantı kurulması için talep gönderme mekanizmasını aktif etmemizi sağlar.
//Yapıyı connection içerisinde yaparız ve orda değerleri verilir.MaxRetryCount ve MaxRetryDelay parametreleri bu yapının kaç defa tekrarlanacağı ve periyodu ne şekilde olacağını sağlar.
#endregion
#region ExecutionStrategy
//RetryOnFailure ile yaptığımız konfigürasyonlar Ef Core için default değerleri olacaktır.Eğer daha da ayrıntılı bir şekilde özelleştirmek istersek ExecutaionStrategy classından inherit olan bir class yazılmalı
#endregion
//Bazı durumlarda sadece veritabanına tekrardan bağlanmak yeterli olmayabilir ve commit edilmemiş transaction yapısını tekrardan çalıpştırmamız gerekebilir.Bu durumda context nesnesi üzerinden bir metot ile çalışmamız gerekecek.
ConnectionDb connectionDb = new ConnectionDb();
var strategy = connectionDb.Database.CreateExecutionStrategy();
strategy.ExecuteAsync(async () =>
{
    //transaction işlemi ile birlikte veritabanında yapacağımız sorgu buraya gelecektir.
});

class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
}
class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;",builder => builder.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(15), errorCodesToAdd: new List<string> {"4060"})).LogTo(filter:(eventId,level) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,logger:eventData => { Console.WriteLine("Bağlantı kurulamadı"); });
        //Yukarıda EnableRetryOnFailure ile bir bağlantı kurulup gerekeli parametreleri veriliyor.Ek olarak bir log yapısında tutuluyor.

        #region CustomExecutionStrategy connection
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;", builder => builder.ExecutionStrategy(dependencies => new Ex(dependencies, 3, TimeSpan.FromSeconds(2))));
        #endregion
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
     

    }
}
class Ex : ExecutionStrategy
{
    public Ex(DbContext context, int maxRetryCount, TimeSpan maxRetryDelay) : base(context, maxRetryCount, maxRetryDelay)
    {
    }

    public Ex(ExecutionStrategyDependencies dependencies, int maxRetryCount, TimeSpan maxRetryDelay) : base(dependencies, maxRetryCount, maxRetryDelay)
    {
    }

    protected override bool ShouldRetryOn(Exception exception)
    {
        throw new NotImplementedException();
        //Bağlantıyı yeniden kurmak istenildiğinde yapılan işlemler için kullanılan override edilmiş fonksiyon.
    }
}
// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.InteropServices.Marshalling;


#region Neden Loglama yapılır
//çalışan bir sistemde RunTime sürecinde yapılan işlemlerin nasıl bir tavır sergilediklerini kayıt altında tutmak için loglama yapılır.
//Loglama işlemi farklı teknolojilerle veya kütüphanelerle yapılır fakat burda Ef Core dahilinde gelen loglama işlemini inceleyeceğiz.
//Ef Core içerisinde gelen dahili loglama genelde sorguların nasıl davrandığını ve hassas verilerin takibi için kullanılır.
ConnectionDb2 context = new ConnectionDb2();
#endregion
#region Minimum şekilde loglama örneği
//Konfigüre işlemleri OnConfiguring içerisinde yapılır.Aşağıda örneği bulunabilir.
var query = context.Orders.ToList();
#endregion
#region Dosya ile loglama
//Dosya ile loglama durumunda ef core logları uygulama dışına çıkararak bir dosyaya yazabilir.Fakat biz burda bu dosyayı ConnectionDb içerisinde yapacağımızdan debug dosyası içinde oluşacak.
//Konfigürasyon ile ilgili ayarlamalar aşağıda bulunabilir.
//EFCore logları uygulama dışına çıkarmayı kolayca izin vermediğinden kaynaklı olarak istediğimiz veri loglarını görebilmek için EnabledSensetiveLogging konfigürasyonunu yapmamızı istiyor.Yani verideğerlerini debug
//için kullanırken bu değerleri de loglara yazılmasını istersek aktif ederiz.
//Eğer hataları daha detaylı olarka incelemek istersek EnabledDetailedErrorsları kullanabiliriz.
//Son olarak loglama işlemi gelişmiş sistemlerde her servis için her bilgi tutmanın çok zor olduğunu düşünürsek bunun önüne geçebilmek için LogLevellar vardır.Bunları konfigürasyon dosyasından ayaralarız.
//
#endregion
#region QueryLog
//Lınq sorgularını teknik anlamda incelemek amacıyla kullanılan loglardır.Querylerdeki logları rahatlıkla hata ayıklama yapmamıza da yarar.
//Query log için Microsoft.Extension.Logging.Console paketini projemize eklememiz gerekyor.Ardından OnConfiguring içerisinde ayarlamaları yaparız.
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
class ConnectionDb2 : DbContext
{
    StreamWriter _log = new("logs.txt"); // dosya oluşturma
    readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
        /*optionsBuilder.LogTo(Console.WriteLine); */// Konsol örneği.istersek debug penceresine de yazdırabiliriz.
        /* optionsBuilder.LogTo(message => _log.WriteLine(message),Microsoft.Extensions.Logging.LogLevel.Warning).EnableSensitiveDataLogging().EnableDetailedErrors();*/ // Bu dosya içerisinde tutma
                                                                                                                                                                         //LogLeveli warning setleyerek artık logları sadece hata durumunda kayıt edilmesini sağlarız.
        optionsBuilder.UseLoggerFactory(_loggerFactory); // Bu ayarlamadan sonra artık her query sonrası log yapacaktır.
    }
    
   
    public DbSet<Blog> Orders { get; set; }
    public DbSet<Post> Posts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
    public override void Dispose()
    {
        base.Dispose();
        _log.Dispose();
    }

}
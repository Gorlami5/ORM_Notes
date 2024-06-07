using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
ConnectionDb db = new ConnectionDb();
#region Eager Loading
//Veritabanından elde edilen sorgunun ilişkili olduğu tablolardan gelen verileri parça parça ekleme yaklaşımıdır.
#region Include
//Eager loading yaklaşımını yapabilmemiz için Include fonksiyonunu kullanırız.
//Üretilen sorguya ilişkisel olan diğer tabloları dahil etme sürecini Include fonksiyonuyal yürütürüz ve bu yaklaşıma eager loading adı veririz.
var query = await db.Employees.Include(e=> e.Region).Include(e=> e.Orders).ToListAsync();
//Include işlemi sql düzeyinde aslında bir joinleme işlemi yapaacaktır.Bu sebeple ToList ile execute etmeden önce kullanılması gereklidir.
//Where koşulu Include'lar arasına girebilir fakat bu koşullar joinlenmiş tablo sonunu gelen verilere bir şart koşacaktır.
#endregion
#region ThenInclude
//İlişkili tablolarda çoklu include durumları olabilir ve bu durumları belirli bağlantılarla include edebiliriz.Fakat eklenecek diğer tablolalar bir koleksiyonel yapıda olduklarında bağlantıyı kuramayız ve include işlemi başarısız olur.
//Bu durumlarda ThenInclude kullanarak include işlemini başarabiliriz.
//Örneğin Orderlardan başlayıp Regionlara gidebileceğimiz bir yapı inşa edebiliriz.Örneği aşağıda olacaktır.
var query1 = db.Orders.Include(e => e.Employee).Include(e => e.Employee.Region); // İlk joinden sonra gelen employee'ler Regionlar ile birlikte geleceğinden ve her employee tek regiona sahip olduğundan kaynaklı problem yaşamayız.
//Fakat tam tersi bir durumda Regionlardan başlayıp Orderlara gitmek istediğimiz durumda bir Employee'nin birden çok Order'ı olmasından kaynaklı include etme işlemi hatalı olacaktır.Çünkü koleksiyonel bir yapıdıdar.
//Çözüm olarak da ThenInclude işlemiyle bunu çözebiliriz.var query2 = db.Regions.Include(e => e.Employees) bu yapı üzerinden orderlara erişemeyiz çünkü employees içinde artık koleksiyonel bir order olacaktır.
//ThenInclude ile bu sorunu kolaylıkla çözebiliriz.
var query2 = db.Regions.Include(e => e.Employees).ThenInclude(e => e.Orders).ToListAsync();
#endregion
#region FilteredInclude
//Include etmek istediğimiz tabloyu belirli noktalarda filtreleyerek include edebiliyoruz.Bu yaklaşım sorgu sonucu dönecek yapının tamamını değil de sadece include edilecek verilere uygulanan bir filtredir.
var query3 = await db.Regions.Include(r => r.Employees.Where(e => e.Name == "Ahmet")).ToListAsync();
//Bu sorgu sonucu dönen veriler Change Tracker mekanizması yüzünden istenilen filtrelemeye uygun olmayabilir.Bu sorgudan önce başka sorgularla belleğe yüklenmiş verilerden kaynaklı bu sorun yaşanabilir.
//Çözümü ise FilteredInclude yapılanmlarında Tracking mekanizmasının kapatılmasıdır.
#endregion
#region Eager Loading CT mekanizma ilişkisi
//Eager Loading yapılanmasında daha önceden belleğe alınmış ve CT ile takip edilen veriler sonradan yapılacak sorgularda da kullanılır.
var query4 = await db.Orders.ToListAsync();
var query5 = await db.Employees.ToListAsync();
//query4 ile gelen Orderlar belleğe alınacaktır ve query5 ile gelecek Employee'lerin Orderları da query5 ile bize döner.Ef Core bu konuda kolaylık sağlayacaktır.Bu durumlarda tekradan include işlemine ihtiyaç duymayız.
#endregion
#region AutoInclude && IgnoreAutoIncludes
//Yapılacak sorgularda kesinlikle bir include işlemi varsa bunu her seferinde tek tek yazmadan Fluent API ile konfigüre edebiliriz.
//Örneğin Bir employee sorgusunda hepsinde Region bilgisini kesinlikle getirmek istiyorsak bunu konfigüre edebiliriz.
//Eğer bu autoinclude sorgusunu belirli noktalarda ihtiyaç duymadığımızda IgnoreAutoIncludes() ile bundan yararlanmayabiliriz.
#endregion
#region Kalıtımsal Entity'ler arasında include
var query6 = db.Persons.Include(p => ((Employee)p).Orders);
//Employee'de bir Person olacağından(inheritance) bu şekilde bir include yapısı yapılabilir.
#endregion
#endregion
#region Explicit Loading
//Sorgu sonucu dönen veriye eklenecek olan tabloların belirli bir şarta veya koşula göre gelmesi durumuna explicit loading adı verilecektir.
#region Reference
//Sorgulama sürecinde eklenmek istenen ilişkisel tablonun navigation propertysi tekil ise reference fonksiyonu ile bunu ekleyebiliriz.
var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == 2); //bu sorguyu yaptığımızda Employee içinde gelecek region null olacaktır ve eğer biz eager loading yapsak gereksiz bir join yapmış olacaktık.Yani tüm
//employee'ler include edilecek ardından Id=2 olan gelecekti.Explicit loading Reference fonksiyonu ile sadece şarta bağlı ekleme yapılır

await db.Entry(employee).Reference(e => e.Region).LoadAsync(); // Bu satır çalıştıktan sonra artık employee artık bir region'a sahip olacaktır.
#endregion
#region Collection
//Sorgulama sürecinde eklenmek istenen ilişkisel tablonun navigation propertysi çoğul ise reference fonksiyonu ile bunu ekleyebiliriz.
var employee2 = await db.Employees.FirstOrDefaultAsync(e => e.Id == 2);


await db.Entry(employee).Collection(e => e.Orders).LoadAsync();
//Explicit loading ile eklenen verileri IQueryable tipine dönüştürmek istersek .Query() diyerek istediğimiz Aggreagte fonksiyonlarını kullanabiliriz.
//Yine aynı şekilde filtreleme yapmak istersek Query ile filtreleme de yapabiliriz.Execute etmeyi unutmamaız lazım.
#endregion
#endregion


class Person
{
    public int Id { get; set; }
}

class Employee:Person
{
    
    public int RegionId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public Region Region { get; set; }
    public List<Order> Orders { get; set; }
}
class Region
{
    public int Id { get; set; }
    public string RegionName { get; set; }
    public List<Employee> Employees { get; set; }
}
class Order
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public DateTime OrderDate { get; set; }
}
class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().Navigation(e => e.Region).AutoInclude(); // konfigüre örneği
    }
}
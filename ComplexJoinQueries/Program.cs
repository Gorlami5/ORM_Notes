// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");
#region InnerJoin
ConnectionDb connectionDb = new ConnectionDb();
var query = from p in connectionDb.Persons
            join o in connectionDb.Orders on p.Id equals o.Id
            select new
            {
                p.Id,
                o.Name
            };
//İki tabloyu belirli kolonlar üzerinden birleştirme işlemi için join ifadesini kullanırız.Eğer iki farklı kolon üzerinde joinlemek istersek on new{//ortak kolonlar} şeklinde yapabiliriz.
#endregion
#region Multiple Column Join
var query2 = from p in connectionDb.Persons
             join ph in connectionDb.Photos on p.Id equals ph.Id
             join o in connectionDb.Orders on p.Id equals o.PersonId
             select new
             {

             };
//İlk olarak Person tablosuya Photo tablosunu join ederiz adından ikinci join yaparız fakat burda ikinci tabloyu joinlerken neden tablo üzerinden yapmıyoruz.Aslında ikinci p.Id önceki tabloyu belirtiyor.
#endregion
#region Group Join
var query3 = from p in connectionDb.Persons
             join o in connectionDb.Orders on p.Id equals o.PersonId into op
             select new
             {
              p.Name,
              Count = op.Count()
             };
var result = await query3.ToListAsync();
//Joinlerde gruplama işlemi seçtiğimiz bir tablo üzerinde gerçekleşir.Yukarıdaki örnekte Person'lara karşılık gelen orderlar gruplanır.Gruplama işlemi her Persons'ın sahibi olduğu orderlara göre yapılır.
//Yani yukarıdaki sorguda p.Name ile gelen personun order.Countunu bize dönderecektir.
//Eğer tekrardan order kolonunun propertylerine ulaşmak istersek from or in op diyerek tekrardan ulaşabiliriz.Aksi takdirde sadece gruplanmış verilere ulaşabiliriz.

#endregion
#region Left Join
//Soldaki tablo ile sağdaki tablo birleştirilir fakat soldaki tabloya karşılık eşleşmeyen kayıt varsa bu kayıtlar da getirilir ve null değer atanır.
var query4 = from p in connectionDb.Persons
             join o in connectionDb.Orders on p.Id equals o.PersonId into op
             from order in op.DefaultIfEmpty()
             select new
             {
                 p.Id,
                 order.Name
             };
var result2 = query4.ToListAsync();
//DefaultIfEmpty ile eşleşmeyen kayıtlara default değer yani null değer atacanaktır.
#endregion
#region Righ Join
//Sağdaki tablo ile soldaki tablo birleştirilir fakat sağdaki tabloya karşılık gelmeyen kayıt varsa bu kayıtlar da getirilir ve null değer atanır.
var query5 = from p in connectionDb.Orders
             join o in  connectionDb.Persons on p.Id equals o.Id into op
             from person in op.DefaultIfEmpty()
             select new
             {
                 p.Id,
                 person.Name
             };
var result3 = query4.ToListAsync();
#endregion
#region Full Join
//Sağ ve sol join yaptıktan sonra bu iki join işlemini unoin komutu ile birleştirilmesine Full join işlemi adı verilir.Tüm kayıtların birleştirilmesidir.
//Sağdaki tabloya karşılık gelen ve soldaki tabloya karşılık gelen kayıtlar boş olsa dahi getiriliyorsa full join denilebilir.
//Union komutu iki tablonun select edilen yerlerini birleştirir.Tekrar eden kayıtlar otomatik olarak çıkarılır.
#endregion
#region Cross Join
//İki tablonun tüm kayıtları kartezyan çarpım olarak geri getirilir bu işlemde
var query6 = from p in connectionDb.Persons
             from o in connectionDb.Orders//.Where(o=> o.PersonId == person.Id)
             select new
             {
                 o,
                 p
             };
//Bu işlem ef core'da cross join olarak algılanacaktır.
//Eğer Cross join sırasında collection selector'da Where kullanır ve doğru parametreleri girersek bu yapı Inner join olarak algılanacaktır.
//Yukarıda yorum satırları içerisinde bir örneği görülebilir.
#endregion


public class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Order> Orders { get; set; }

}
public class Photo
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string Description { get; set; }
    public Person Person { get; set; }

}
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public Photo Photo { get; set; }
    public List<Order> Orders  { get; set; }
}
public class Order
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Person Person { get; set; }
}

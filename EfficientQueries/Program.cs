
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

#region IQueryable ile IEnumerable farkı
//IQueryable ile yapılan çalışmalarda hedef sorgu hedef verileri elde edilecek şekilde veritabanına yansıtır.IEnumerable ise sql sorgusuyla birlikte veritabanındaki getirebileceği en geniş şekilde getirir ve in-memory'de ayıklar.
//Yani yapılan sorguda IQueryable sorgu içidneki şartlar ve duurmları sorguya ekleyerek veritabanında generate edilen sorguya ekleyecektir.Fakat IEnumerable ise tam tersi olarak en bütünsek halini getirip in-memory'de bu koşulları uygular.
//İki durumda da yapılan sorgu execute edilmeden çalışmayacaktır.Ondan kaynaklıo ikisini de execute etmek zorundayız.Execute edilip farklı bir değişkenle in-memory'de bunu tuttuğumuz durumda bu artık IEnum olarak değişken tipi olacaktır.
ConnectionDb db = new ConnectionDb();
await db.Persons.Where(p => p.Name.Contains("a") && p.Name.EndsWith("d")).Take(3).ToListAsync();// yapı buraya kadar IQueryable şeklinde çalışır yani bu contains sorgusu like sorgusu olarka veri tabanına yansır.Eğer AsEnumerable() ile aksini belirtmezsek IQueryable olarak kalır.
db.Persons.Where(p => p.Name.StartsWith("a")).AsEnumerable().Where(p => p.Name == "a").ToList();
//Yukarıdaki örnekte veritabanına sadece StartsWith yapısı eklenecektir.Çünkü bu sorgu AsEnumerable()'a kadar IQueryable davranışı gösterir.Diğer Where ise veritabanından gelen tüm verilerin memory'e taşınmasının ardından
//Bu verileri memory'de sorgular.Bu durumda bize performans olarak pek doğru bir sonuç vermeyecektir.IEnumerable yapısında olsa dahi execute etmek zorundayız.
#endregion
#region Yalnızca ihtiyaç olan kolonları listeleme
//Bir sorguda tüm tabloyu almak yerine tablo içindeki ihtiyacımız olan kolonları almamız performans açısından her zaman daha iyi olacaktır.Select() fonksiyonu ile bunu gerçekleştirebiliriz.
var persons = await db.Persons.Select(p => p.Name).ToListAsync(); // burda sadece tek bir kolon alma örneği var.
var persons2 = await db.Persons.Select(p => new //Burda ise tipi olmayan bir nesne tanımlayıp istediğimiz kolonları çektik
{
    p.Name,
    p.Id
}).ToListAsync(); //İstersek bir dto ile yada farklı c# tipleriyle(tuple vs) geri dönüş tipi belirleyebiliriz.
#endregion
#region Result limitleme
//Verileri kümülatif bir şekilde çekmek yerine baştan belirli sayıdaki verileri Take() fonksiyonu ile almalıyız.Gerektiği durumlarda bu yola başvurmayı unutmayınız.
await db.Persons.ToListAsync(); // bu şekilde yapmak yerine
await db.Persons.Take(50).ToListAsync(); // bu şekilde kullanmak en doğrusu olacaktır.
#endregion
#region Join Sorgularında Eager Loading Sürecinde Verileri Filtreleyin
//Eager Loading kullanırken yapılan join(include) işlemlerinde join edilecek tabloyu tamamen almak yerine filtreleme yapmak da performans açısından oldukça yararlı olacaktır.
//Ek olarak bu Dependent Entity'e yapılacak olan sorguyu in memory'de deil direkt olarak queryable haldeyken yani sorgudayken yapılması her zaman daha performanslı olacaktır.
var persons3 = await db.Persons.Include(p => p.Orders.Where(o => o.Name.Contains("a"))).ToListAsync(); //bu yapıyla birlikte artık belirli şartlara tabi olan veriler gelecektir. 

#endregion
#region Şartlara bağlı join yapılacaksa explicit loading kullanımının önemi
//Eğer yapılacak join işlemi şart gerçekleştiğinde gerekliyse sadece o veri için join yapılmalıdır.Buy join işlemi içinde kullanılan join davranışı Explicit Jindir.(ComplexJoinQueries projesi içerisinde örneği var.)
var person45 = await db.Persons.Include(p => p.Orders).FirstOrDefaultAsync(p => p.Id == 1);
var person46 = await db.Persons.FirstOrDefaultAsync(p => p.Id == 1);

//Bu şart durumunda sadece ismi mertay olanların order'larına ihtiyacımız varken daha önceden join yapıldığından tamamı gelecektir.Bunun için izlememiz gereken yol aşağıdadır.
if(person46.Name == "Mertay")
{
    //Order'ları getir.
    await db.Entry(person46).Collection(p=> p.Orders).LoadAsync(); // Explicit loading ile gerekli orderları bu şekilde load edebiliyoruz ve performans olarka başarılı oluruz.
}
#endregion
#region Lazy Loading kullanırken dikkat edilmesi gerekenler
//Lazy loading kullanımında iç içe for'larda veya tekrar eden n'2 işlemlerde her dependent entity için tekrardan veritabanına sorgu gönderilme durumu yaşanabilir ve oldukça büyük performans problemleri yaratabilir.
//Bunun önüne geçebilmek için lazy loading öncesi gerekli olan kolonların select edilmesi diyebiliriz.
#endregion
#region İhtiyaç Noktalarında ham sql kullanımı
//Ef Core tarafında bazı sorguları veya farklı durumları ifade edemediğimizde ham sql kullanmak gerekebilir.Ek olarak Sql ile gelen artılar olan store_procedure veya view'ları kullanmak istediğimizde de kullanmaktan kaçmaya gerek yoktur.
#endregion
#region Asenkron fonksiyonların tercihi
//Asenkron fonksiyonlar üzerinden çalışmak kaynak tarafında performansı artıracak ve ölçeklendirecektir.
#endregion

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Order> Orders { get; set; }
}
public class Order
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string Name { get; set; }
    public Person Person { get; set; }
}
public class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");

    }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Order> Orders { get; set; }

}
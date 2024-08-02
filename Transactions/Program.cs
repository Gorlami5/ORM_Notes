// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


#region Transaction Nedir
//Transaction çalışan bir yapının atomik olarak yani kümülatif şekilde çalışmasını amaçlayan yapıdır.Eğer başarılı olduysa bir bütün halidne başarılı eğer ki bir hata alındıysa bir bütün halinde başarısız olmasını sağlar.
//Başarılı olduysa bu bütünü veritabanına yansıtmaya commit eğer başarısız olup tüm yapılanları geri alan yapıya rollback denir.
#endregion
#region Default Transaction Davranışı
// Ef Core kendi başına default olarak bir transaction yapısına sahiptir.Bu transaction yapısı SaveChanges() metodu ile commit edilir.Yani SaveChanges() metodu bir transaction yapısı içerir.
//Fakat bu yapıdan çıkıp daha gelişmiş şekilde kullanmak istersek orda manuel olarak yönetmemiz gerekecektir.Aşağıdaki başlıkta manuel yapı incelenecek.
#endregion
#region Manuel Transaction Kontrolü
//Ef Core içerisindeki default transactiondan çıkarak daha gelişmiş bir transaction ile çalışmak istersek manuel olarak tanımlamamız gerekir.
ConnectionDb db = new ConnectionDb();
IDbContextTransaction transaction = db.Database.BeginTransaction(); //Burda manuel olarak transaction yapısını başlatmış oluruz.
var person = await db.Persons.FirstOrDefaultAsync(w => w.Id == 1);
new Person()
{
    Name = "a"
};
var a = db.Add(person);
a.State = EntityState.Modified;
await db.SaveChangesAsync(); // Eğer SAveChanges manuel olarak bir transaction başlatılmadıysa default olarak görevini yerine getirecektir fakat manuel bir transaction başlatıldıysa artık manuel transaction geçerlidir.
transaction.Commit(); // commit edildiği takdirde artık bütün değişiklikler veritabanına yanısıtılacaktır.
//transaction.RollBack() bu şekilde de rollback edilebilir fakat bir hata durumunda çalışmasını sağlamak her zaman daha doğru olacaktır.
#endregion
#region SavePoint & RollBackToSavePoint
//SavePoint RollBack durumlarında işlemi en baştan almak yerine belirleyeceğimiz bir noktaya dönmesi için oluşturulur.Bu noktaları göstererek RollBackleri bazı sernaryolarda daha farklı şekilde kullanabiliriz.
//Bu SavePoint Noktalarına ise RollBackToSavePoint fonksiyonunu gerekli parametreleri verip tetikleyerek kullanabiliriz.
IDbContextTransaction transaction2 = db.Database.BeginTransaction();
var person1 = await db.Persons.FindAsync(1);
var person2 = await db.Persons.FindAsync(2);
db.RemoveRange(person1, person2);
await db.SaveChangesAsync();
transaction2.CreateSavepoint("t1");
var person3 = await db.Persons.FindAsync(3);
db.Remove(person3);
await db.SaveChangesAsync();
transaction2.RollbackToSavepoint("t1");
transaction2.Commit(); //Burdaki yapıda ilk iki person'u silerken 3. personu silse bile ilk oluşturulan Save noktasına rollback atacaktır.RollBack atınca Commit yapmayacak gibi bir durum düşünülmemeli Commit sırası geldiğinde execute olacaktır.
//Transaction ile ilgili düşülmemesi gereken en büyük hata eğer bir savePoint'e dönüyorsa onu run time'da döndüğünü düşünmektir.Run time'da çalışır fakat arka planda rollback edilecek yerleri veritabanına yansıtmaz.
//Yani transaction2.RollbackToSavepoint("t1"); 'a gelindiğinde tekrardan CreateSavepoint adımına gelmeyecektir.Sadece o noktaya kadar yapılmış işlemleri veritabanına yansıtılmayacak şekilde yok sayacaktır.
#endregion
#region TransactionScope çalışmaları
//Transaction yapısı disposable pattern ile de kullanabiliriz.Bu yapıyı kullanma sebibimiz oluşturulacak yapının disposable pattern ile daha tutarlı çalışması ve belleği daha optimize kullanmasını sağlamak.(Kaynak sızıntılarını da önler)

using (IDbContextTransaction transaction3 = db.Database.BeginTransaction())
{
    //...
    //...
    try
    {
        //En Son Commit
    }
    catch (Exception ex)
    {
        await transaction3.RollbackAsync();
        //Hata durumunda rollback.İstersek Save noktasına da döndürebiliriz
    }
}
#endregion

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
}
public class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }
    public DbSet<Person> Persons { get; set; }

}

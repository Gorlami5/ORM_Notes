// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using SimpleCRUD_Query;


#region Veri Ekleme
ConnectionDb connectionDb = new ConnectionDb();
Personal personal = new Personal();

await connectionDb.AddAsync(personal); //Entity instance ile object tipinde ekleme yapılabilir.Bu tip korumalı değildir.
await connectionDb.Personals.AddAsync(personal); //DbSet ile set ettiğimiz tablonun hangisi olduğunu belirttiğimiz durumdur.Haliyle tipini belirterek göndermesinde sakınca yoktur.
connectionDb.SaveChanges();
#endregion
#region Çoklu Veri ekleme
Personal personal1 = new Personal()
{
    Name = "Foo1",
};
Personal personal2 = new Personal()
{
    Name = "Foo2",
};
Personal personal3 = new Personal()
{
    Name = "Foo3",
};
await connectionDb.AddAsync(personal1);
connectionDb.Add(personal2);
connectionDb.Add(personal3);
connectionDb.SaveChanges();
//Çoklu eklemede dikkat edilecek nokta tek savechanges kullanımı olacaktır.Çünkü her savechanges bir transaction yapısı kurar ve bunlar fazla maliyet doğurur.
await connectionDb.AddRangeAsync(personal1, personal2, personal3);

#endregion
#region SaveChanges
//EntityFrameWork ile oluşturlan sorgunun veri tabanına yanısmasını sağlar.Bir transaction yapısıyla birlikte veritabanına gider ve sorguyu veritabanında execute eder.
//Transaction mantığında olduğu gibi herhangi bir hata aldığı durumda tüm işlemleri rollback edecektir.
#endregion
#region Entry State
Console.WriteLine(connectionDb.Entry(personal).State);
await connectionDb.AddAsync(personal);
Console.WriteLine(connectionDb.Entry(personal).State);
await connectionDb.SaveChangesAsync();
Console.WriteLine(connectionDb.Entry(personal).State);
//Entry State bize modelden oluşturduğumuz instanceın durumunu verir.Db ile herhangi bir etkileşime geçdiğinde detached,herhangi bir işleme tabi tutulduysa ona bağlı değişim(burda Added olacaktır),
//SaveChangestan sonra Unchanged halini alacaktır.Unchanged olması değiştirilmeden transaction işlemim başarılı olduğunu bize gösterir.
#endregion
#region Veri Güncelleme
var personalId3 = await connectionDb.Personals.FirstOrDefaultAsync(p=> p.Id == 3);
personalId3.Name = "Name.1";
personalId3.Surname = "Surname.1";
Console.WriteLine(connectionDb.Entry(personalId3).State); // State unchanged iken modified olarak değişecektir.Eğer bir veri context db üzerinden değişmeden geliyorsa unchanged olur.instance db ile alakalı değilse detached.
await connectionDb.SaveChangesAsync();
//Basit bir güncelleme işlemi.Fakat burda kritik nokta db'den aldığımız verinin ChangeTracker ile izleniyor oluşunu bilmemiz gerekli.ChangeTracker ile izlendiği için veri memoryde kaybolmuyor ve güncelleme işlemi yapacağınbı biliyor.
//Veritabanından gelen veriler changetracker mekanizması ile izlenir.Ekleme işleminde gördüğümüz EntryState durumları da aslında changeTracker mekanizmasından gelen bilgilerle yönetilir.
Personal per1 = new Personal()
{
    Id = 1,
    Name = "nm",
    Surname = "sn"
};
connectionDb.Update(per1);
await connectionDb.SaveChangesAsync();
//Yukarıda changeTracker ile izlenmeyen bir verinin nasıl güncelleneciğinin bir örneği bulunmakta.Update fonksiyonu mantık olarak ilk olarak verilen id ile bir sorgu yapar.Yani Update fonksiyonunu kullanabilmek için
//göndereceğimiz instance bir sorgu parametresi içermelidir.Ardından bulduğu veriyi günceller.
#endregion
#region Basit ChangeTracker
//Veritabanı üzerinden gelen verilerin takip edilmesine yarayan mekanizmadır.Bu izlemelere dayanarak işlemin kimliği anlaşılır ve ona göre uygulanır.
#endregion
#region Veri Silme
var personalDelete = await connectionDb.Personals.FirstOrDefaultAsync(p => p.Id == 1);
connectionDb.Personals.Remove(personalDelete);
await connectionDb.SaveChangesAsync();
//Silme işleminde context üzerinden gelen veri ile çalışılır.Bu durumdan kaynaklı changeTracker mekanizması kullanılır.ChangeTracker kullanmadan silme yapmak istersek verinin contexten gelmeyen halini kullanabiliriz.
Personal personal4 = new Personal()
{
    Id = 2,
};
connectionDb.Remove(personal4);
//Yukarıda olduğu gibi contexte olmayan bir durumla çalışıyorsak kesin olarak onun primary olan kimlik bilgisine ihtiyacımız olacaktır.

connectionDb.Entry(personal4).State = EntityState.Deleted;
await connectionDb.SaveChangesAsync();
//Entry State ile silme işlemi de yukarıdaki şekilde yapılabilir.Update işleminde de Update fonksiyonu yerine entry state değiştirilerek başarıyla güncellenebilir.
//RemoveRange ile çoklu silme de yapabiliriz.
#endregion
#region Basit Sorgulamalar
var personals = connectionDb.Personals.ToList(); //Bu sorgu metot türünde yazılmıştır ve bir IEnumerable tipindedir.Çünkü ToList ile execute edilmiş bir sorgu içerir.In memoryde sorgunun dönderdikleri bulunur.
var personalsq = (from p in personals // Aynı şekilde yine IEnumerable tipindedir fakat query şeklinde yazılmış bir sorgudur.
                 select p).ToList();
var personalQueryable = from p in personals
                         select p; //Bu ifade henüz execute edilmediği için IQueryable tipindedir.Yani sorgu oluşturulmuştur fakat in memorye alınmamıştır.
foreach (var item in personalQueryable)
{
    Console.WriteLine(item.Name);
}
//IQueryable tipinde bir sorgu foreach ile execute edilebilir.Yani yukarıda sorgu foreach adımında execute edilmiştir.
int personId = 6;
var personalQueryable2 =from p in personals
                        where p.Id > personId
                        select p;
personId = 7;
foreach (var item in personalQueryable2)
{
    Console.WriteLine(item.Name);
}
//Yukarıda sorgu oluşturulup execute edilmedi.Ardından sorguyu etkilyecek bir değişken değeri değiştiriliyor.Bu durumda değiştirilen değeri kullanarak sorgu çalışır.Buna Deffered Execution denir.
//Yani oluşturulan sorgu execute edilene kadar tam olarak çalıştırılmaz.Execute edildiğinde son değeri neyse ona göre çalışır ve in memorye alınır.
#endregion
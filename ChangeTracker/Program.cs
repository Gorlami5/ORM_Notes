#region ChangeTracker Nedir
//Context üzerinde gelen verileri izleyen mekanizmaya change tracker adı verilir.Gelen bütün veriler izleniyor diye bir kaide yoktur.
//Change tracker ile izlenen veriler üzerinde yapılan değişiklikleri veri tabanına yansıtmaya ise change tracking adı verilir.
//Change Tracker aslında DbContext classında erişebildiğimiz bir member'dır
using ChangeTracker;
using Microsoft.EntityFrameworkCore;
using static ChangeTracker.ConnectionDb;

ConnectionDb connectionDb = new ConnectionDb();
var personals = connectionDb.Personals.ToList(); // gelen tüm veriler burda change tracker ile izleniyor.
var datas = connectionDb.ChangeTracker.Entries();
//Yukarıda tüm veriler change tracker ile izleniyor demiştik.Burda bu verilerin entity durumlarının listesi elimize ulaşacaktır.İzlenen veriler üzerinde bir değişiklik yapmadığmız içib hepsi unchanged olarak geri dönecektir.
personals[6].Name = "Name";
var datas2 = connectionDb.ChangeTracker.Entries(); // burda personal listesinin 7. elemanı için gelen entity state modified olarak döner.Eğer SaveChanges metodu çağrılmadıysa change tracker bu verileri izlemeye devam eder.
#endregion
#region DetectChanges
//ChangeTracker ile izlenen veriler üzerinde yapılan son değişikliklerin hala track ediliyor mu kontrolünü yapar.Bu işlem SaveChanges içerisinde default olarak yapılsa da bazı durumlarda kendi irademizle tetiklemek isteyebiliriz.
connectionDb.ChangeTracker.DetectChanges(); // Son kontrol durumu gibi düşünebiliriz.(Savechanges içerisinde sadece tetiklendiğini unutmayalım.)
//Peki SaveChanges içerisindeki detectChanges metodunu nasıl disabled hhala getirebiliriz.Belirli durumlarda ihtiyacımız olmadığından performans odaklı iyileştirmeler yaparken bunu etkisiz hale getirmek isteyebiliriz.
connectionDb.ChangeTracker.AutoDetectChangesEnabled = false; // bu şekilde artık burdan sonra çalıştıracağımız SaveChanges içerisinde DetectChanges çalışmayacaktır.
															 //Tüm context için bunu yapmak pek doğru olmayacağı için genelde belirli şart koşullarıyla birlikte kullanmak daha doğru olacaktır.Çünkü change tracker her context için tek bir konfigürasyon alır ve tüm her yerde uygular.

#endregion
#region AcceptAllChanges ve HasNoChanges metodu
//SaveChanges metodu ve parametre olarak true almış overload SaveChanges metodu takip ettiği bütün verilerle ilişkisini kesecektir
connectionDb.SaveChanges(true); //Başarıyla çalıştığı durumları ele alıyoruz aksi takdirde transaction yapısı ile tüm değişiklikler yok sayılacaktır.
connectionDb.SaveChanges(false); // Parametre olarak false bir değer gönderirsek değişiklikler veri tabanına yansır fakat CT yapılanması başka zaman da getirebilmemiz için verileri izlemeye devam edecektir.
								
connectionDb.ChangeTracker.AcceptAllChanges();  //Değişiklikler yapılmasına rağmen takip edilen verilerin takibi bırakılmasını istersek AcceptAllChanges metodunu çağırmamız gerekecektir.
//HasNoChanges metodu takip edilen veriler içerisinde değişikliğe uğrayan veri varsa bool tipinde değer döndüren biğr metottur.
var result = connectionDb.ChangeTracker.HasChanges();
#endregion
#region Entity State
//Chanhe tracker ile izlenen nesnelerin durumlarıdır.Belirli birkaç farklı durum vardır.
Personal p = new Personal();
p.Name = "aa";
Console.WriteLine(connectionDb.Entry(p).State);
//Yukarıda konsola Detached olarak basılacaktır.Bunun nedeni change tracker ile izlenmeyen nesneleri stateleri detached olur.Personal nesnesi de görüldüğü gibi track edilmiyor.
connectionDb.Add(p);
Console.WriteLine(connectionDb.Entry(p).State); // Burda entry state added olacaktır.Fakat burdaki yapı da chane tracker ile izlenmiyor gibi de olsa context ile ilişkilendirikdiğinden track edilmeye başlanır.
//Update işleminden sonra modified delete işleminden sonra ise deleted olarak entry stateler bulunacaktır.
var p2 = connectionDb.Personals.FirstOrDefault(p => p.Id == 2);
Console.WriteLine(connectionDb.Entry(p2).State);
//Yukarıda change tracker ile takip edilmesine rağmen bir değişiklik yapılmayan bir değişken var.Bu durumda Unchanged olarak işaretli olduğunu görürüz.
connectionDb.SaveChanges(true);
//Bütün track işlemleri Save Changes ile bırakılır.(true parametre aldığı durumlar için).Bu durumda artık tüm değişiklikler unchanged olacaktır.Eğer false parametresi ile track etmeyi bıraktırmazsak hala eski state kalacaktır.
#endregion
#region Entry Metodu ve Change Trackerı Interceptor olarak kullanmak
//Context içerisinde bulunan Entry modeli belirli özellikler sunan propertyleri barındırır.Bu özellikler tracker ile izlenip değiştirilen verinin anlık değerlerine ulaşabilirken değişmeden önceki değerlere de ulaşabiliyor.
var p3 = connectionDb.Personals.FirstOrDefault(p => p.Id == 4);
p3.Name = "aa";
//Aşağıda güncelleme yapmadan olan değeri bize verecektir.
var data1 = connectionDb.Entry(p3).OriginalValues.GetValue<string>(nameof(p3.Surname));
//Current values ise bize normal değerini verecektir.
//Change tracker yapısı override edilmiş SaveChanges metodu içerisinde belirli şartlarla kontrol edilebilir.Bu connection db sınıfı içerisinde basit bir örnekle gösterildi.
//Tüm takip edilen entity stateler ile belirli şekilde kontroller yapılıp ayarlanması change trackerın interceptor olarak kullanılmasıyla alakalıdır.
#endregion
#region AsNoTracking & AsNoTrackingWithIdentityNoResolution
//CT ile izlenen verileri irademizle izlenmemisini AsNoTrackink metoduyla yapabiliriz.CT bazı durumlarda gereksiz olabilir ve maliyeti artırabilir.
//CT ile izlenmeyen verilerle de her şey yapılabilir fakat Entity state durum değişiklikleriyle yapmka mümkün değildir.Ek olarak update işleminde de contexten gelen Update fonksiyonuyla yapılmazsa CT izlenmemesi işe yaramaz.
var personal4 = connectionDb.Personals.AsNoTracking().ToList();
//Yukarıda Ct izlenmeyen bir personal listesi elimizde olacaktır.Bu listedeki bazı personal nesnelerini güncelleyebilmek için Update fonksiyonu kullanılmalıdır.
//AsNoTrackingWithIdentityResolution
//Bu fonksiyon ile ilişkisel veri yapılarında maliyeti daha düşük seviyelere çekebiliriz.
//CT ile takip etmediğimiz veriler bize bazı durumlarda maliyet getirebilirler.Örneğin ilişkisel tablolarda her instance için yinlenen dahi olsa baştan instance üretirler.Bu da halilyle bir maliyet oluşturur.
//CT ile izlemeyip ve yinelenen instanclardan kurtulmak istediğimiz senaryoda AsNoTrackingWithIdentityNoResolution kullanırız.

#endregion
#region AsTracking & UseQueryTrackingBehavior
//AsTracking ChangeTrackerın default halidir.Yani context nesnesi üzerinde default olarak change tracker mekanizmasını tetikleyen fonksiyondur.Eğer manuel olarak context içerisinde disabled edilmediyse kullanılmasına gerek yoktur.
var personal5 = connectionDb.Personals.AsTracking().FirstOrDefault();
//Context içerisinde UseQueryTrackingBehavior kullanılarak Ef Core seviyesinde CT mekanizması ayarları yapılabilir.Örneğin tamamen kapatılabilir veya farklı konfigürasyonlar yapılabilir.
//OnConfigüring içerisinde örnek hali bulunabilir.
#endregion


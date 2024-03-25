#region ChangeTracker Nedir
//Context üzerinde gelen verileri izleyen mekanizmaya change tracker adı verilir.Gelen bütün veriler izleniyor diye bir kaide yoktur.
//Change tracker ile izlenen veriler üzerinde yapılan değişiklikleri veri tabanına yansıtmaya ise change tracking adı verilir.
//Change Tracker aslında DbContext classında erişebildiğimiz bir member'dır
using ChangeTracker;

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

#endregion


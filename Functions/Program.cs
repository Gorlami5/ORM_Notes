// See https://aka.ms/new-console-template for more information
using Functions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
using static Functions.ConnectionDb;

ConnectionDb connectionDb = new ConnectionDb();
#region Where
var personals = await connectionDb.Personals.Where(x => x.Id > 500).ToListAsync(); // Where kullanımı için en basit örnek.
var personals2 = await connectionDb.Personals.Where(x => x.Name.StartsWith("a")).ToListAsync(); // EndsWith veya Contains kullanılarak string aramaları yapılabilir.
//Yukarıdakiler birden çok veri getirme örnekleridir.
#endregion
#region OrderBy
//Sorguları belirli bir kurala göre sıralamamızı sağlar.
var personal3 = connectionDb.Personals.Where(x => x.Id > 500 || x.Name.EndsWith("a")).OrderBy(x=>x.Id).ToListAsync();
//yukarıda belirli bir sorugunun Id kolonuna göre sıralı halini alabiliriz.Default değer olarak ascending olacaktır.
var personal4 = from personal in personals
                where personal.Id > 500 || personal.Name.StartsWith("a")
                orderby personal.Id
                select personal;
//Query sytnax ile aynı sorugunun yazımı
//OrderByDescending() ile tam tersi sıralama yapabiliriz.
#endregion
#region ThenBy
//OrderBy ile belirlediğimiz kolona göre sıralama yaptıktan sonra hala tekrar eden veriler varsa bu sıralamayı başka kolonlara göre de yapmamızı sağlar.
var personal5 = await connectionDb.Personals.Where(x => x.Id > 500 || x.Name.EndsWith("a")).OrderBy(x => x.Id).ThenBy(x => x.Name).ThenBy(x => x.Surname).ToListAsync();
//Eğer ıd ile sıralanırken tekrarlanan veri varsa Name,Name tekrarlanıyorsa Surname ile sıralama yapılır.
//ThenByDescending() ile de diğer kolonlarda sıralama yapılabilir.
#endregion
#region SingleAsync & SingleOrDefaultAsync
var personal6 = await connectionDb.Personals.SingleAsync(x => x.Id == 7); //Single fonksiyonu birden fazla değer dönerse veya hiç değer dönmezse exception fırlatacaktır.Tekil veri dönderir.
var personal7 = await connectionDb.Personals.SingleOrDefaultAsync(x => x.Id == 7);//Default olarak belirttiği değer nulldır.Eğer hiç veri gelmezse default değeri,birden çok gelirse exception fırlatacaktır.
#endregion
#region FirstAsync & FirstOrDefaultAsync
var personal8 = await connectionDb.Personals.FirstAsync(x => x.Id == 7); //Elde edeceğimiz tekil veriyi gelen veri kümesinden ilk olanı alarak elde ederiz.Hiç veri gelmezse exception fırlatacaktır.
var personal9 = await connectionDb.Personals.FirstOrDefaultAsync(x => x.Id == 7); //Elde edeceğimiz tekil veriyi gelen veri kümesinden ilk olanı alarak elde ederiz.Hiç veri gelmezse null değer dönderecektir.
#endregion
#region FindAsync
//primary key ile bir sorgu yapılacağı zaman find kullanmak performanslı bir sonuç verir.
var personal10 = await connectionDb.Personals.FindAsync(7); //Primary key değerinin sebebi unique bir yapıda olmasından kaynaklıdır.
                                                            //Composite primary key yapısında olan tablolar için de birden fazla id ile sorgu yaparkan FindAsync() kullanılabilir.Composite primary key onmodelcreating yapısında özel olarak konfigüre edilmelidir.
#endregion
#region LastAsync & LastOrDefaultAsync
var personal11 = await connectionDb.Personals.OrderBy(a => a.Id).LastAsync(); //Gelen verilerin en sonundaki veriyi alır.Eğer bir veri gelmezse exception fırlatır.Sıralama yapmak mecburidir
var personal12 = await connectionDb.Personals.OrderBy(a => a.Id).LastOrDefaultAsync();//Gelen verilerin en sonundaki veriyi alır.Eğer bir veri gelmezse null değer dönderir.Sıralama yapmak mecburidir.
#endregion
#region CountAsync & LongCountAsync
var personal13 = await connectionDb.Personals.CountAsync();//Dönen listenin bize kaç eleman içerdiğini veren fonksiyondur.Long ile yapılırsa dönen değer tipi long olacaktır.CountAsync() içerisine expression fonksiyon ile şart eklenebiklir
                                                           //Sorgu execute edilip ardından sayısı alınabilir fakat bu gereksiz maliyet doğuracaktır.Sorgu CountAsync() ile de execute edilebilir ve execute edildikten sonra dönen değer tek bir int veya long olur.Eğer sorguyu ilk execute etsek
                                                           //in memorye gelen bir veri listesi olacaktı.
#endregion
#region Any
var personal14 = await connectionDb.Personals.AnyAsync(x => x.Id == 7);//Verinin olup olmadığına göre bool tipinde bir değer dönderen fonksiyondur.Direkt şartla birlikte kullanılabilir.Execute etmeyi AnyAsync() ile yapabiliyoruz.
                                                                       //in memorye sorguyu alıp ardından iyi any ile enumerable halindeyken de yapabiliriz fakat yanlış bir yaklaşım olacaktır.
                                                                       //şartsız kullanılmak istenirse tablonun boş mu dolu mu olduğu kontrolü yapılabilir.
#endregion
#region MaxAsync&MinAsync
var personal15 = await connectionDb.Personals.MaxAsync(x=> x.Id); //Belirtilen modellenmiş tabloda parametreye göre max ve min değerlerini tek bir şekilde ve parametre tipine göre dönderen fonksiyondur.
var personal16 = await connectionDb.Personals.MinAsync(x => x.Id);
#endregion
#region Distinct
var personal17 = await connectionDb.Personals.Distinct().ToListAsync();//Tekrarlı verileri tek hale getirip yeni bir listede geri dönen fonksiyondur.
#endregion
#region AllAsync & SumAsync
var personal18 = await connectionDb.Personals.AllAsync(x => x.Id > 10); //Tablodaki verilerin tamamı verilen şarta uyuyorsa true uymayan tek bir veri dahi varsa false döner.
var personal19 = await connectionDb.Personals.SumAsync(x => x.Id); // Verilen property için tüm değerleri toplar ve bu değeri geri döner.
#endregion
#region Contains & StartsWith & EndsWith
//Where yapısı içerisinde string bir değer için kullanılabilir.Yani tek başına execute etme veya bir IQueryable yapısı oluşturma olanağı yoktur.
var personal20 = await connectionDb.Personals.Where(x => x.Name.Contains("a")).ToListAsync();
var personal21 = await connectionDb.Personals.Where(x => x.Name.StartsWith("a")).ToListAsync();
#endregion
#region ToDictionaryAsync & ToArrayAsync
var personal22 = await connectionDb.Personals.ToDictionaryAsync(x => x.Id, x => x.Name); //ToDictinoary fonksiyonu ToList gibi execute etme özelliğine sahiptir.Fakat asıl özelliği tablodan seçilen 2 kolonu key ve value değerleri
//olarak dönderir.Yani dictionary formatında dönderecektir.
var personal23 = await connectionDb.Personals.ToArrayAsync(); //Sorguyu execute etme yani in memorye taşıma özelliğine sahip bir diğer fonksiyondur.Sorguyu Array yapısında dönderecektir.
#endregion
#region Select
//Generate edeilecek sorguların getirilecek kolonlarını ayarlamamızı sağlar.
var personal24 = await connectionDb.Personals.Select(x => new Personal //Personal tablosunda Id ve name kolonlarını generate eder ve bunları tip olarak Personal tipinde tutar.Yani listelenmiş her veri personal tipindedir.
{
    Id = x.Id,
    Name = x.Name, 
}).ToListAsync();
//select ile generate edilen kolonlar anonym tipli de olabilir.Aynı şekilde tek bir kolon çekilecekse veri tabanında tanımlanmış tipiyle de geri dönebilir.
#endregion
#region GropBy & Foreach
//Belirli kolonlara göre guruplama işlemleri yapabildiğimiz ve bunları farklı tablo ile listeleyebildiğimiz fonksiyondur.Gruplanan veriler noname bir tabloda oluşacağı için bunları modellemeliyiz.
//Modelleme işlemini geçici olarak yapmalıyız çünkü her noname tablo için bir entity tanımlayamayız.Select edilen belirsiz tabloya tipsiz bir şekilde model verip içeride kolon isimlerine karşılık değerleri aşağıda ki gibi yazarız.
//Count ve Id aslında bizim belirli bir tipi olmayan tablo modelimizin kolonları.Karşılık olarak gruplanmış tablo içerisindeki veriler.
//anonym tip ile çalışmak zorunda değiliz fakat ikili değer tutan tiplere ihtiyaçlarımız olacak.Bunun için en iyi tip tuple olacaktır.
var data1 = await connectionDb.Personals.GroupBy(x => x.Id).Select(g => new 
{
    Count = g.Count(),
    Id = g.Key,

}).ToListAsync();

var data2 = (from pers in connectionDb.Personals
             group pers by pers.Id
            into g
             select new
             {
                 Count = g.Count(),
                 Id = g.Key,
             }).ToList();  //query syntax ile yazımı bu şekildedir.

//foreach ile IQueryable tipindeki bir soruguyu execute edebiliyoruz.Aynı şekilde IEnumerable tipinde bir listede iterasyonlarla gezebiliriz.
data2.ForEach(x =>
{
    Console.WriteLine(x.Count);
});
var data3 = await connectionDb.Personals //Bu örnekte int tipinde iki property bulunduran bir classı gruplandırılmış isimsiz tabloyla nasıl ilişkilendirildiğini göstermiş olduk.Aslında tipsiz bir yapıyla bazı işlemleri gerçekleştirmediğimiz
    //için bunu yaptık.Class bir deneme içeriyor daha doğru bir örnek için her kolona bir tuple değeri atanarak çok dahha doğru ve kullanışlı bir yapı inşa edebiliriz.Bu sadece teorik bir örnektir.
    // new Tuple<int,int>(g.Count,g.Key) yapısıyla birlikte bunu yapabiliriz.
       .GroupBy(x => x.Id)
       .Select(g => new a(){
          MyProperty = g.Count(),
          MyProperty1 = g.Key,
        })
       .ToListAsync();
data3.ForEach(x =>
{
    x.MyProperty += 1;
});
class a
{
    public int MyProperty { get; set; }
    public int MyProperty1 { get; set; }
}
#endregion

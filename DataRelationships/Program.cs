#region Temel Kavramlar
//Principal Entity : Kendi başına var olablien entity'dir.
//Dependent Entity : Başka bir tabloya bağımlı olan entity'dir
//Foreign Key : Dependent Entity ile Foreign Key arasındaki bağlantıyı kuran key'dir.
//Principal Key : Principal Entitydeki Id kolonudur.
//Navigation Property : İki tablo arasında bağlantıyı entity katmanında sağlayan propertylerdir.
//Default Conventions & Data Annotations : Default Conventions varsayılan entity kurallarıyla ilişki yapılandırma yöntemleridir.Data Annotations ise belirli keyler ile ince ayar yapmamızı sağlar.
//Fluent Api : Entity ilişkilendirilmelerinde en uç detay konfigürasyonları yapmamızı sağlar.
//HasOne,HasMany,WithMany,WithOne :  İki entity arasındaki ilişkiyi başlatan metotlardır.HasOne ve HasMany başlangıç WithOne ve WithMAny son gibi düşünebiliriz.
#endregion
#region OneToOne 
//İki tablonun birbirlerine 1e1 olarak bağlı olmasıdır.Aşağıdaki class yapılarında bir çalışanın tek bir adresi olurken bir adresin tek bir çalışanı olacaktır.Eğer birinden biri eksik olursa constraintler devreye girecektir.
//Aşağıdaki yapıda constraint foreign keydir.Yani iki tablo arasındaki ilişkiyi sağlayan kolondur diyebiliriz.
//Default Convention ile çalışırken ek olarak kullnacağımız bir yapı bulunmaz çünkü zaten oluşturulan entity default olarak veritabanına gidecek yapııyı oluşturur.Migrationları oluşturduğumuzda default olarak neyin ne olacağını bilir.
//ForeignKey için PrincipalEntityName+Id dediğimizde ef core bunu default olarak bilir ve ona göre migrate oluşturur.
//Navigation Propertyler iki tablo arasında fiziksel bir ilişki kurulacağını bizlere gösterir.1e1 bir ilişki olacağından NP'ler tekildir.
//İki tablo arasında 1e1 ilişki olacağının garantisini veritabanı sağlayıcı index oluşturarak sağlar.Yani foreign key kolonunun unique bir değer olacağının garantisini sağlar bu index.
//Bu index bize ek olarak bi maliyet sağlayacak ve bundan kurtulmanın yolu DataAnnotations ile Dependent Entity'nin Id'sini Hem PK hem de FK olarak işaretlemektir.Aşağıda örneği bulunuyor.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//new PersonalAddress()
//{
//    Id = 1,
//    PersonalId = 1,
//    Definiation = "",

//};
//Personal personal = new Personal();
//class Personal //Principal Entity
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public string Email { get; set; }
//    public PersonalAddress PersonalAddress { get; set; } //Navigation Property
//}

//class PersonalAddress //Dependent Entity
//{
//    public int Id { get; set; }
//    public int PersonalId  { get; set; } //ForeignKey
//    public string Definiation { get; set; }
//    public Personal? Personal { get; set; } //Navigation Property
//}

//class Personal1
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public string Email { get; set; }
//    public PersonalAddress1 PersonalAddress { get; set; } //Navigation Property
//}

//class PersonalAddress1
//{
//    //[Key,ForeignKey("Personal")] 
//    public int Id { get; set; }

//    [ForeignKey("Personal1")] // Foreign Key Default Convention ile belirtecek özellikler taşımıyorsa(adı gibi) bu şekilde belirtilerek atanabilir.
//    public int PersonalId { get; set; } //ForeignKey
//    public string? Definiation { get; set; }
//    public Personal1? Personal { get; set; } //Navigation Property
//}
//Data Annotations ile de 1e1 ilişkisel tablo bağlantıları kurabiliriz.Bunu daha çok belirli attributelar ile yaparız.Örneğin[Key] [ForeignKey] gibi attributeler ile yönetiriz.
//1e1 ilişkilerde iki tablonunda tek olarak bağımlı olduğunun garantisini oluşan index yapısı ile alıyorduk.Ama istersek FK tanımlamadan Dependent Entity içerisinde Id kolununu hem primary key hem de foreign key ile işaretleyebiliriz.
//Yani Dependent Entity içideki Id değeri zaten unique olacağından ayrıca bunu indexle belirtmemize gerek kalmayacaktır.Bizi indexleme maliyetinden kurtaracaktır.


#endregion
#region OneToMany
//İki tablonun ilikisel olarak bire çok davranışı göstermesidir.Personal tablosunda personellerin sadece 1 tane departmanı varken Departman tablosundaki departmanın birden çok personeli olaiblir.
//Default Convention yönteminde navigation propertyler ile belirtilmiş tablo ilişkisi olduğu takdirde Foreign Key tanımlamaya ihtiyaç duyulmaz çünkü ef core bunu otomatik olarak yapacaktır.Yani entity nesnesi içinde bulunmasa da olur.
//Data Annotations kullanmımı sadece FK propunu farklı bir şekilde tanımlayacaksak kullanırız aksi durumda kullanmamamıza gerek yoktur.
//class Personal2 //Dependent Entity
//{
//    public int Id { get; set; }
//    public int DepartmanId { get; set; } //Foreign Key
//    public string Name { get; set; }
//    public string Email { get; set; }
//    public Departman Departman { get; set; } //Navigation Property
//}
//class Departman
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public List<Personal2> Personals2 { get; set; } //Navigation Property

//}
#endregion
#region ManyToMany
// İki tablo arasında çoka çok ilişki kurmamıza yarar.Örneğin bir kitabın birden çok yazarı olabilirken bir yazarın da birden çok kitabı olabilecektir.DEfault Convention yönetimini kullanıyorsak FK tanımlamayız.
//Bu iki tablo arasındaki ilişkiyi kurabilmek için ef core bir cross table oluşturacaktır.Bu tabloyu entity olarak tanımlamaya ihtiyaç duymayız.KitapYazar adında bir cross table tablosu bulunacaktır.
//Entityler bu cross table ile 1eçok bir ilişki içerisinde olacaklar ve bu şekilde ilişkiyi sağlayacaklar.Bir kitabın birden çok yazarına erişmek istediği durumu buna bir örnek olarak verebiliriz.
//İçerisinde sadece Kitap ve Yazar Id bulunduran bu tablo iki tablo arasında bize bir yol olur.Bir tablodan diğer tabloya gitmemizi sağlar.İki kolon da primary key olur.Bu yapıya composite primary key denir.
//class Kitap
//{
//    public int Id { get; set; }
//    public string KitapAdı { get; set; }
//    public List<Yazar> Yazarlar { get; set; }
//}
//class Yazar
//{
//    public int Id { get; set; }
//    public string YazarAdı { get; set; }
//    public List<Kitap> Kitaplar { get; set; }
//}
//Data Annotations ile çoka çok bir ilişki kurmak istediğimizde cross table'ı fiziksel olarak tanımlamaya ihtiyaç duyarız.Bu tabloda aslında diğer iki tablo arasında bire çok bir ilişki olduğunu görmek kolay.
//Cross table içerisinde bulunan foreign keyleri attribute'lar ile tanımlarız fakat tablo içinde iki primary key olduğunu tanımlamak için fluent API kullanmak gerekli.(İki primary key demek compsite Pk anlamına gelir.)
//class KitapYazar
//{
//    [ForeignKey("Personal")]
//    public int KitapId { get; set; }
//    [ForeignKey("Personal")]
//    public int YazarId { get; set; }
//    public Kitap Kitap { get; set; }
//    public Yazar Yazar { get; set; }
//}

#endregion
#region OneToOne ilişkilerde veri ekleme
//Veri ekleme durumlarında eklenecek verinin Id kolonunu belirtmeden eklemeler yaparız.Bunun nedeni Id kolonu veritabanında AI olarak işaretlenmesidir.Eğer belirli bir veri eklenecekse Id kolonu kullanılır.
//OneToOne ilişki halinde olan tablolara veri eklerken istersek principal entity üzerinden istersek de dependent entity üzerinden ekleme yapabiliyoruz.
//Personal personal = new Personal();
//personal.Email = "";
//personal.PersonalAddress.Definiation = "";

//_context.Personal.AddAsync(personal); Burda Principal entity referansı üzerinden veri ekleme yönetemini görürüz.Personal eklenirken ek olarak ilişkili olarak bir PersonalAddress de eklenir.
//new Personal()
//{
//Email = "",
//PersonalAddress = new PersonalAddress() { Definiation = "" }
//    _context.Personal.AddAsync(personal);
//}; // burda da object initilazor ile ekleme yönetmini görüyoruz.
//PersonalAddress personalAddress = new PersonalAddress()
//{
//Definiation = "",
//Personal = new Personal()
//{
//Email = "" // Görüldüğü üzere Id kolonuna bir değer vermiyoruz.Foreign Key değerini burda yeni eklenecek verinin Id'sini ef core bize otomatik olarak set edecek.
//}
//};
//_context.PersonalAddress.AddAsync(personalAddress); // Burda da Dependent Entity ile veri ekleme davranışını görüyoruz.Bu davranışta önemli olan Personal'i yani principal entitiyi null olarak set edemeyeceğimiz.Hata alacağızdır.
//Ama Principal entity ile ekleme işlemi yapmak istersek Dependent Entity nav Property'sini null gönderebiliriz.(Nullable işaretleme şartıyla).
//Önceden eklenmiş fakat ilişkilendirlmemiş bir Personalin sonradan PersonalAddressi nasıl eklenir?Bunun cevabı PErsonalAddress eklerken Navigation prop gönderilmez fakat FK olan personalId daha önce eklenmiş personalden alınmalıdır.
//Bu durumda Principal Entity'i göndermek zorunda olmadan işlemler yapılabilir.
//class Personal //Principal Entity
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public string Email { get; set; }
//    public PersonalAddress PersonalAddress { get; set; } //Navigation Property
//}

//class PersonalAddress //Dependent Entity
//{
//    public int Id { get; set; }
//    public int PersonalId { get; set; } //ForeignKey
//    public string Definiation { get; set; }
//    public Personal? Personal { get; set; } //Navigation Property
//}

#endregion
#region OneToMany ilişkilerde veri ekleme
//OneToMany ilişkisel tablolarda veri eklemek için belirli davranışlar vardır.Bunlar Principal Entity üzerinden,Depedent Entity üzerinden ve Foreign Key üzerinden ekleme olarak 3 durum vardır.
//Departman departman = new Departman()
//{
//    Name = "name",
//};
//departman.Personals2.Add(new Personal2() // Departman classınin constructoru içerisinde Personal tipinde bir liste nesnesi oluşturmamızın sebebi departman referansı üzerinden veri eklerken null reference hatası almaktan kaçınmak istememizdir.
//{
//    Name = ""
//});  //departman.Personals2 listesinin bir nesnesi olmayacağından bu listeye bir şey ekleyemeyiz.Ama class içerisinde bunun önüne geçtik.
//departman.Personals2.Add(new Personal2()
//{
//    Name = "a"
//});
////Principal Entity ile ekleme işlemi yukarıda görüldüğü gibi yapabiliriz.Referans dışında object initilazior ile de ekleme işemlemi yapabiliriz.
////Dependent Entity ile ekleme one to many ilişkilerinde doğru bir yaklaşım olmayacaktır.Bunun nedeni tek bir personalin tek bir departmana sahip olmasına neden olacaktır.Sonrasında foreign key ile eklemeler yapılabilir bunu unutmayalım.
////3. olarak Foreign Key ile ekleme yapma yöntemi bulunuyor.Foreign Key ile ekleme haliyle yine dependent entity üzerinden olacaktır.
//Personal2 personal = new Personal2()
//{
//    DepartmanId = 1, // 1 idli departmana eklenen personeli foreign key yoluyla bu şekilde ekleyebiliriz.Id'yi istersek bir parametere olarak istersek sorgu sonucu gelen departmanId'sini alarak yapabiliriz.
//    Name = ""
//};
////Add işlemi

//class Personal2 //Dependent Entity
//{

//    public int Id { get; set; }
//    public int DepartmanId { get; set; } //Foreign Key
//    public string Name { get; set; }
//    public string Email { get; set; }
//    public Departman Departman { get; set; } //Navigation Property
//}
//class Departman
//{
//    public Departman()
//    {
//        Personals2 = new List<Personal2>();
//    }
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public List<Personal2> Personals2 { get; set; } //Navigation Property

//}
#endregion
#region ManyToMany ilişkilerde veri ekleme
//Default convention ile migrate edilmiş mtm bir ilişkide cross table ef core tarafından oluşturuluyordu.Veri eklerken one to many gibi davranırız fakat veritabanında cross table içerisine de bir kayıt düşer ve bu şekilde çoklu ilişki devam eder.
//Book b = new Book()
//{
//    Name = "",
//    Authors = new List<Author>()
//    {
//        new Author(){Name = ""},
//        new Author(){Name = ""},
//        new Author(){Name = ""}  // Author içinde aynı şekilde veri ekleme yapılabilir. 
//    }
//};
//class Book
//{
//    public Book()
//    {
//        Authors = new List<Author>();
//    }
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public List<Author> Authors { get; set; }
//}

//class Author
//{
//    public Author()
//    {
//        Books = new List<Book>();
//    }
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public List<Book> Books { get; set; }
//}
#endregion
#region OneToOne ilişkilerde veri güncelleme
//OneToOne ilişkilerde verileri güncellerken iki farklı durumu ele alacağız.Bu durumlardan biri principal üzerinden güncelleme yapılırken diğeri depedent entity üzerinden yapılacak.
//var person = _context.Personal.Include(p=> p.Address).FirstOrDefault(p => p.Id == 2);
//_context.Personal.Remove(person.Address);

//person.personaladdress = new personaladdress()
//{
//    definiation = "Updated"
//};
//await _context.SaveChangesAsync(); // Burda principal entity ile adres güncellemesi yapıyoruz.Güncellemede farklı olan eski adresi silip tekrar yenisini eklememiz.Bu örnek olan bir durum.
//2. durumda ise direkt adres üzerinden bir personali güncelleriz.Eğer direkt aynı kullanıcının adresini güncelleme işlemini yapmak istiyorsak bunu kolaylıkla yapabiliriz fakat biz burda veriyi tamamen değiştireceğiz.
//var address = await _context.Address.FindAsync(1);
//_context.Address.Remove(address);
//_context.SaveChanges();

//var person2 = _context.Person.FindAsync(1);
//address.Person = person2;
//_context.Person.Add(person2);
//_context.SaveChanges();
//Yukarıda dependent entity ile bir adres güncellemesi yapıyoruz.İlk olarak adresi silip ardından yeni bir adres ekleyip istediğimiz kullanıcıyla ilişkilendiriyoruz.


//class personal //principal entity
//{
//    public int ıd { get; set; }
//    public string name { get; set; }
//    public string email { get; set; }
//    public personaladdress personaladdress { get; set; } //navigation property
//}

//class personaladdress //dependent entity
//{
//    public int ıd { get; set; }
//    public int personalıd { get; set; } //foreignkey
//    public string definiation { get; set; }
//    public personal? personal { get; set; } //navigation property
//}

#endregion
#region OneToMany ilişkilerde veri güncelleme
//OneToMany ilişkilerde güncelleme yaparken yine 2 farklı durumu ele alırız.Birinde principal diğerinde depedent entity ile güncelleme işlemi yapılır.
//Principal entity ile güncellerken principal entity olan tablodan veri çekilir ve bu veri içerisindeki yapılar güncellenerek tekrardan saveChanges metodu çağırılır.CT tarafından izlendiğini unutmayalım.
//var departman = _context.Departman.Include(p => p.Personal).FirstOrDefault(p => p.Id == 1);
//var removedPersonal = departman.Personal.FirstOrDefault(p=>p.Id == 1);
//departman.Personal.Remove(removedPersonal);
//departman.Personal.Add(new Personal2 { });
//departman.Personal.Add(new Personal2 { });
//_context.SaveChanges();
//Yukarıda departman içerisinden bazı personeller sildik ve ekledik.İstersek var olan bir personeli de kolayca güncelleme işlemine gönderebiliriz.
//var p = await _context.Personal2.FindAsync(2);
//p.Departman = new()
//{

//};
////Yukarıda zaten var olan bir kullanıcının departmanı güncellenir.Güncellenirken eklenen departman farklı bir veri olarak eklenir.Yani Personal güncellenirken Departman eklenmiş olur.
////Veritabanından generate ettiğpimiz bir departmanı başka bir kullanıcıya atarken de aynı şekilde bu yolu izler ve güncelleriz.
//class Personal2 //Dependent Entity
//{

//    public int Id { get; set; }
//    public int DepartmanId { get; set; } //Foreign Key
//    public string Name { get; set; }
//    public string Email { get; set; }
//    public Departman Departman { get; set; } //Navigation Property
//}
//class Departman
//{
//    public Departman()
//    {
//        Personals2 = new List<Personal2>();
//    }
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public List<Personal2> Personals2 { get; set; } //Navigation Property

//}

#endregion
#region ManyToMany ilişkilerde veri güncelleme
//ManyToMany ilişkileride veri güncellerken iki tablodan herhhangi biri tercih edilebilir.Zaten güncellenen veri genelde cross table üzerinden güncellenir.Sadece atomik bir veri içeriği güncellersek principal tablolardan güncellenir.
//Book book = await context.Books.Include(p=>p.Authors).FirstOrDefault(p=> p.Id == 2)
//Author s = book.Authors.FirstOrDefault(a=>a.Id==1)
//book.Authors.Remove(s);
//_context.SaveChanges();
//Yukarıda bir book için bir yazar siliniyor.Burda silinen yazar direkt yazar tablosundan silinmez gider cross table içerisinden ilişkii atandığı yer silinir.
//class Book
//{
//    public Book()
//    {
//        Authors = new List<Author>();
//    }
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public List<Author> Authors { get; set; }
//}

//class Author
//{
//    public Author()
//    {
//        Books = new List<Book>();
//    }
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public List<Book> Books { get; set; }
//}
#endregion
#region OneToOne ilişkilerde veri silme
//var departman = _context.Departman.Include(d=>d.Personal).FirstOrDefault(d=>d.Id == 1);
//_context.Address.Remove(departman.address);
//Yukarıda principal entity üzerinden dependent entityde silme işlemi örneğini görüyoruz.
//class Personal2 //Dependent Entity
//{
//    public int Id { get; set; }
//    public int DepartmanId { get; set; } //Foreign Key
//    public string Name { get; set; }
//    public string Email { get; set; }
//    public Departman Departman { get; set; } //Navigation Property
//}
//class Departman
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public List<Personal2> Personals2 { get; set; } //Navigation Property

//}
#endregion
#region OneToMany ilişkilerde veri silme
//Principal entity üzerinden dependent entity verilerine ulaşır ve burda silme işlemini tamamlarız.
//var departman = _context.Departman.Include(p => p.Personal).FirstOrDefault(p => p.Id == 1);
//var removedPersonal = departman.Personal.FirstOrDefault(p=>p.Id == 1);
//_context.Personal2.Remove(removedPersonal);
//_context.SaveChanges(); //Güncellerken principal tablodan yola çıktığımızı burda ise depedent tablodan sildiğimizin farkına varılmalı.
//class Personal2 //Dependent Entity
//{

//    public int Id { get; set; }
//    public int DepartmanId { get; set; } //Foreign Key
//    public string Name { get; set; }
//    public string Email { get; set; }
//    public Departman Departman { get; set; } //Navigation Property
//}
//class Departman
//{
//    public Departman()
//    {
//        Personals2 = new List<Personal2>();
//    }
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public List<Personal2> Personals2 { get; set; } //Navigation Property

//}
#endregion
#region ManyToMany ilişkilerde veri silme
//Book book = await context.Books.Include(p=>p.Authors).FirstOrDefault(p=> p.Id == 2)
//Author s = book.Authors.FirstOrDefault(a=>a.Id==1)
//_context.Books.Remove(s);
//_context.SaveChanges();
//Yukarıda cross table ile birbiri arasındaki ilişkiyi silme yöntemi gösteriliyor.Many to many ilişkilerde direkt olarak principal tablolardan veri silmek bazı noktalarda veri kaybına neden olabilir.Bu yüzden ilişki silme durumu daha doğrudur.
//_context.Authors.Remove(s); // Bu şekilde bir silmeye zorlarsak cross table üzerinde veri kaybına neden olacaktır.Hem cross table içerisinden ilişki silinirken hem de author kendi tablosundan silinir.
//Belirli durumlarda yapılacak olsa da pek tercih edilmeyebilir.
//class Book
//{
//    public Book()
//    {
//        Authors = new List<Author>();
//    }
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public List<Author> Authors { get; set; }
//}

//class Author
//{
//    public Author()
//    {
//        Books = new List<Book>();
//    }
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public List<Book> Books { get; set; }
//}
#endregion
#region Cascade Delete & SetNull Delete
//İlişkisel senaryolarda verilerin silinemelerinde bazı durumlara izin verilmez.Bu durumlarda cascade ve setnull yapılarını kullanmamıza ihtiyaç duyarız.
//Principal bir tablodan bir veri silinmek istendiğinde depedent entity'de kalacak veriler bir anlam ifade etmeyeceğinden kaynaklı silinmeye veritabanı seviyesinde izin verilmez veya hata dönderilir.
//Örneğin 5 tane blog'a sahip bir postu silmek istediğimizde geriye kalan postlar bir FK alamaycağından bir anlam ifade edilmez.Bu durumla karşılaşmak istemediğimizden dolayı cascade ve setnull kullanırız.
//CASCADE : Principal tablodan sildiğimiz veriyle birlikte bu veriye bağlı olan verilerin depedent tablolsundan da silinmesini sağlar.Database connection içerisinde konfigürasyon dosyasında Fluent API ile ayarlanır.
//SetNULL : Principal tablodan sildiğimiz veriyle birlikte bu veriye bağlı olan verilere Null değeri atanmasını sağlar.Yani veriler fiziksek olarak silinmez fakat FK değerleri null olarak atanacaktır.
//Fluent Api ile sağlanmış ilişkilere .OnDelete(DeleteBehavior) veya .OnDelete(DeleteBehavior.SetNull) diyerek işlemleri gerçekleştirebiliriz.
#endregion
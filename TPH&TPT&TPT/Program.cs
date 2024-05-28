
#region Table Per Hierachy (TPH)
//Biribirleri arasında kalıtım alan entity durumlarında herbir hiyerarşi için bir tablo oluşturulur.
//Eğer TPH davranışını veri tabanına yansıtmak istersek direkt default convention davranışı gibi migrate edebiliriz.EF Core bunu anlayacak ve ona göre bir tablol oluşturacaktır.
#endregion
#region Discriminator
//TPH davranışıyla oluşturduğumuz tablo içerisinde bulunan bir kolondur.Kümülatif tablo içerisinde verinin hangi entity'e ait olduğunu belirler.İçerisinde entity adlarını tutar.
//EF Core bunu migrate işemi ile otomatik olarak oluşturur.
//Özelleştirmeler yapabiliriz.
//Discriminator kolonun ismini fluent api ile değiştirebiliriz.Base Class'a ile HasDiscriminator() fonksiyonuyla bunu kolaylıkla yapabiliriz.
//Eğer Discriminator içindeki değerleri değiştirmek istersek yine fluent API ile bunu gerçekleştirebiliriz.HasDiscriminator().HasValue<>() ile yapabiliriz.
#endregion
#region TPH veri ekleme
//Veri eklerken farklı bir yaklaşım uygulamayız.Default veri ekleme işlemlerini nasıl yapıyorsak burda da aynı işlemleri yaparız.
#endregion
#region TPH veri silme ve güncelleme
//TPH yaklaşımında veri silebilmek için aynı entity üzerinde sorgu yapıp aynı entity üzerinden silme yapabiliriz.
//Yani discriminator kolonunda hangi entity olduğunu bilecek ve ona göre bir silme işlemi yapacaktır.
//Eğer silinmek istenen veri TPH tablosundan geldiğinde silinecek entity ile uyuşmuyorsa hata alınacaktır.Buna dikkat etmeliyiz.
//Güncelleme operasyonu da veri silmeyle aynı mantıkta çalışır.
#endregion
#region TPH veri sorgulama
//Sorgulama yapılırken DbSet propertysi üzerinden hangi entity'i istiyorsak ona göre sorgulamalar yaparız.
//Eğer Employee ile sorgularsak Technician'lar da gelecektir çünkü kalıtım ön plandadır.Aynısı person için de geçerlidir.
#endregion
#region Table Per Type (TPT)
//Birbirleri arasında kalıtım alan entitylerin her entity tipi için bir tablo oluşturma yaklaşımıdır.Fakat her tabloda sadece kendi entity propertylerine karşılık tablolar bulnuur.
//Yani Employee sınıfında sadece ID ve CompanyName vardır.Peki normalden farkları nedir diye sorarsak işte burda her entity aslında kendi arasında bire bir bir ilişki içerisinde olacaktır.
//Her tabloda bir Id kolonu var olacak ve bu Id'ler üzerinden birbirleriyle birebir ilişki içerisinde olacaktır.Bir customer için Name,Surname Person tablosunda tutulurken CompantyName Kendi tablosunda tutulur.
//Bu yapının default hali TPH olduğuından TPT yapılanmasına çevirebilmek için Fluent API ile konfigürasyonlar yapmamız gerekir.
// Her tablo için modelBuilderi.Entity<>().ToTable("") ile tablo isimleri belirtilerek TPT davranışı oluşturulabilir.
#endregion
#region Table Per Concrete Type(TPC)
//Kalıtımsal olarak ilişkili entityler içerisinde sadece concrete/somut olanlara karşılık bir tablo oluşturan yapıdır.
//Yani eğer soyut entityler varsa bunlara karşılık birer tablo oluşturacaktır.FluentAPI ile soyut entity üzerinden konfigürasyonları yapılabilir.UseTpcMappingStrategy() yapısı ile konfigüre edilir.
//Her concrete entity'de kalıtım aldığı sınıfın kolonları oluşacaktır.
#endregion
class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
}
class Employee:Person
{
    public string? Departmant { get; set; }
}
class Customer:Person
{
    public string? CompanyName { get; set; }
}
class Technician : Employee
{
    public string? Branch { get; set; }
}
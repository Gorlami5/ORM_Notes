#region BackingFields
//Tablolardaki kolonları propertyler ile değil de fieldlar ile tutmak istersek backingField yapısını kullanırız.
//class Person
//{
//    public int Id { get; set; }
//    public string name; //field
//    public string Name { get => name; set => name = value; }
//    public string Surname { get; set; }
//}
//Yukarıda artık hem field hem de property okunan veya yazılan değeri tutacaktır.Eğer prop içerisinde bir kapsülleme yaparsak artık bu property name değerinin manipule halini dönderecektir.Field ise ilk halini bize verecektir.
//Yani eğer bir BF var ve konfigürasyon yapıldıysa artık kolon yerini o field tutar.Bir veri eklenecekse de veya okunacaksa da name üzerinden işlem yapabiliriz.Prop içinde yapılan encapsulation kafamızı karıştırmasın.

//Bu field ile kullanım şeklini bir attribute ile de sağlayabiliyoruz.Yani belirlediğimiz property'e verielcek bir attribute bize bunu verecektir.
//Ek olarak backingfield özelliği fluentApi ile de ayarlanabilir.
//class Person2
//{
//    public int Id { get; set; }
//    public string name; //field
//    //[BackingField(nameof(name))] Artık name field'ı Name propertysi ile birlikte aynı değere sahip olacaktır.
//    public string Name { get => name; set => name = value; }
//    public string Surname { get; set; }
//}
//fluent api ile UsePropertyAccessMode ile enum yapısı içerisinden property'nin mi yoksa field'ın mı kullanılacağını seçeriz.
//Eğer entity içerisinde Property olmadan sadece field kullanmak istiyorsak yani Property'e erişimin olmasını istemiyorsak Fluent api ile modelBuilder.Entity<Person>().Property(nameof(Person.name)) diyerek Property'i kaldırabiliriz.

#endregion
#region Shadow Properties
//Tablo içerisinde var olan bir kolonu entity içerisinde fiziki olarak tanımlamak istemediğimiz durumlarda kullandığımız yapıdır.Bu yapıya en iyi örnek İlişkisel iki tablo arasında tanımlamasak bile veritabanında oluşan FK ditebiliriz.
//Entity içerisinde tanımlamasak bile tabloda olan bu kolona CT ile veya EF Core kendi yapısıyla erişme ve kullanma şansımız vardır.
using Microsoft.EntityFrameworkCore;

ConnectionDb c = new ConnectionDb();
var post = c.Posts.FirstOrDefaultAsync(p => p.Id == 1);
var shadowPost = c.Entry(post).Property("CreatedDate");
Console.WriteLine(shadowPost.CurrentValue); // Shadow propertylere CT ile erişme yolu

//var orderedList = c.Posts.OrderBy(p =>p.EF.Property<DateTime>(p,"CreatedDate")).ToList(); // Ef ile shadow property'e ulaşma yolu
class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Post> Posts { get; set; }
}
class Post
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Defination { get; set; }
    //public DateTime CreatedDate { get; set; } Shadow ile fiziksel tanımmlamaya ihtiyacımız kalmayan property
    public Blog Blog { get; set; }
}
class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>().Property<DateTime>("CreatedDate"); // Bu şekilde Fluent Api ile yapılacak konfigürasyon entity içerisinde tanımlammaıza yardımcı olacaktır.
        base.OnModelCreating(modelBuilder);
    }
}
#endregion
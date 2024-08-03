
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

#region In Memory'de Neden Çalışırız
//Yapılan projede test etmemiz gerken bileşenleri veritabanına ihtiyaç duymadan test etmemiz için bize sunulan bir özelliktir.
//Ef Core için gelen yenilikleri de veritabanında gereksiz veri oluşturmadan deneyebilmemizi de sağlayacaktır.
#endregion
#region Avantajları
//Test ortamı veya pre-prod veriabanlarına ihtiyaç duymadan test işlemlerimiz gerçekliştrmemize yarar sağlar.Bu şekilde lüzümsüz veritabanlarına ihtyacı ortadan kaldırabilir.
//Bütün çalışmaları bellekte gerçekleştireceğinden daha hızlı ve performanslı bir şekilde test işlemlerimizi gerçekleştirebiliriz.
#endregion
#region Dezavantajları
//Bellekte çalışmanın en büyük dezavantajı bellekte modellenmiş veri tabanında ilişkisel tabloların ilişkileri,constraintler geçerli değildir.Eğer böyle bir veritabanımız var ve test yapmak istersek tutarsız sonuçlar elde ederiz.
//Ek olarak bellekte çalıştığımız için uygulama her kapandığında tutulan veriler silinecektir.
#endregion
#region Örnek
ConnectionDb db =  new ConnectionDb();
new Person()
{
    Name = ""
};
await db.AddAsync(new Person());
await db.SaveChangesAsync();
await db.Persons.ToListAsync();
Console.WriteLine();
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
        //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
        optionsBuilder.UseInMemoryDatabase("testDatabase"); //Bir veritabanına bağlanıyormuş gibi konfigürasyon yaparız.Verilen isimde bir database yoksa bellekte modelleyecektir.
    }
    public DbSet<Person> Persons { get; set; }

}
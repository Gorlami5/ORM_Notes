

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Metadata;
#region Index Nedir
//Bir sütun üzerinden yapacağımız sorguları daha performanslı hale getirmek için kullanılan yapıdır.
#endregion
#region Index'leme nasıl yapılır
//PK,FK ve Alternate Key'ler otomatik olarak zaten bir indekse sahip olacaktır.Eğer farklı bir prop için indeks istiyorsak bunu konfigüre etmemiz gerekecektir.
//Indexleme işlemi class seviyesinde yapılır ve iki farklı şekilde yapılabilir.FLuent API ve Data Attribute ile yapılabilir.
#endregion
#region Composite Index
//Bir sorgu içerisinde birden çok sütun etkileniyorsa bu ikisi için de indexleme yaparsak daha verimli ve performanslı sonuçlar alırız.
#endregion
#region Birden fazla index işlemi
//Composit index tanımlayabildiğimiz gibi indexleri ayrı ayrı da tanımlayabiliriz.Composit indexle ayrı ayrı indexleme farklı durumlardır.Örneğin Name ve Surname composit olarak indexleniyor ve bir de bunları
//ayrı ayrı indexlediğimiz senaryoda aynı işleme karşılık gelmez.Composit olan where(name = ""& surname = "") iken diğer ikisi where(name = "")  where(surname = "") olarak işleme geçecektir.
//Fakat çok fazla indeks tanımlamak bir külfet getireceğinden sadece sorgularda fazlaca kullanılan sütunlar için kullanılması gereklidir.
#endregion
#region Index Uniqueness
//Indexlenmiş sütunun tekrar edebilen bir değer alamamasına yarar ve daha performanslı hale getirir.Gerektiği senaryolarda kullanılması önemlidir.
#endregion
#region IsDescending Metodu
//Indexleme işlemlerinde sıralama olması bizlere çok daha performanslı sonuçlar çıkarır.Bunu IsDescending metoduyla yaparız.Eğer Ascending olmasını istiyorsak da parametreyi false olarak işaretleyebiliriz.
//Composite yapılar için de bunlar geçerlidir.
#endregion
#region IndexFilter
//Index yapısı oluşturulduğunda hedef tablodan ayrı bir şekilde index tablosu oluşturulur.Bu tabloya alınacak verilerin hacmini düşürebilmek için bazı filtreler uygulayabiliriz bu yapıyla birlikte.
//SAdece Fluent API ile bunu yapabiliriz.
#endregion
#region Included Columns
//Tanımladığımız indekslerin bazı sorgularda yetersiz kalması durumunda yetersiz kalmasına neden olan property'de include ederek indexlemeden yararlanabiliriz.Bunu da IncludeProperties ile yaparız.
#endregion

/*[Index(nameof(Name))]*/ //Data Attribute ile indexleme bu şekilde yapılır.
[Index(nameof(Name),nameof(SurName))] // Composite Indexleme örneği
[Index(nameof(Name),IsUnique = true)] //Uniqueness örneği
class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SurName { get; set; }
    public string? Department { get; set; }
    public int Salary { get; set; }
}

class ConnectionDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
    }

    public DbSet<Employee> Employees { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasIndex(h => h.Name); // Fluent API ile indexleme.W
        //modelBuilder.Entity<Employee>().HasIndex(h => h.Name).IsUnique(); uniqueness örneği with Fluent API
        modelBuilder.Entity<Employee>().HasIndex(h => h.Name).IsDescending(/*false*/); // false olursa ascending davranışı sergiler.
        modelBuilder.Entity<Employee>().HasIndex(h => h.Name).HasFilter("[NAME] IS NOT NULL"); //filtreleme örneği
        //modelBuilder.Entity<Employee>().HasIndex(h => h.Name).IncludeProperties(k => k.SurName);
    }
}
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

ConnectionDb cdb = new ConnectionDb();
#region FromSqlInterpolated
//Yazacağımız sorguyu eğer LINQ ile ifade edemiyorsak veya LINQ ile yeterince optimize edemiyorsak FromSqlInterpolated fonksiyonunu kullanırız.
var employee = cdb.Employees.FromSqlInterpolated($"Select * From Persons").ToList();
//Kullanım örneği yukarıdadır.
#endregion
#region FromSql
//Ef Core 7.0 ile gelen bu fonksiyon FromSqlInterpolated ile aynı işi yapacaktır.
// Ek olarak birden çok özelliği de bulunmaktadır bunları aşağıda inceleyeceğiz.
//En önemli özelliklerinden biri parametreli sorgular yazabiliriz.Bunu SQL cümleciği içerisine gömeriz ve database bunu bir DBParameter olarak görecektir.
var employee2 = cdb.Employees.FromSqlInterpolated($"Select * From Persons Where PersonId = {employee[0].Id}").ToList();
//FromSql ile ek olarak storedProcedure'lar da execute edilir.Oluşturulmuş storedProcedure hhangi tablodan türetildilyse onun DBSeti üzerinden FromSql kullanılabilir.
#endregion
#region SqlQueryRaw
//Yapılacak sorgunun dinamik bir yapıda olduğu durumlarda yani alacağı parametreleri dinamik bir şekilde alacağız zaman sqlqueryraw fonksiyonu kullanılır.
string columnName = "PersonId";
SqlParameter value = new("PersonId", "3");
var employee3 = cdb.Employees.FromSqlRaw($"Select * From Persons Where {columnName} = @PersonId",value).ToList();
//Dinamik bir yapıda olduğu için ve dinamik olarak gelecek verinin direkt DB üzerinden çalışmasını göz önüne aldığımızda hassas valide işlemlerini yapmamız gerekir.
#endregion
#region SqlQuery
//Entity ile değil scalar bir sorgu çalıştırma durumumuzda SqlQuery kullanılır.
var data = cdb.Database.SqlQuery<int>($"Select * From Persons").ToList();
//Eğer bu sorguya bir de LINQ şartı eklemek istersek bu sorgu db tarafında bir subquery olarka görüneceğinde alias ataması yapmamız gerekir.Bunu EF Core'da value olarak atarız ve artık linq ile sorgu ekleyebiliriz.
//Alias olarak select ettiğimiz kolona karşılık bunu atarız.Direkt subquery ismi gibi düşünülmesi hata olacaktır.
var data2 = cdb.Database.SqlQuery<int>($"Select PersonId Value From Persons").ToList().Where(x=> x > 5).ToList();
#endregion
#region ExecuteSql
//Insert Update Delete işlemlerini ham Sql cümlecikleriyle yapmak istediğimizde ExecuteSql fonksiyonunu kullanırız.
cdb.Database.ExecuteSql($"Update Persons SET Name = 'a' Where PersonId = 3");
#endregion
//Not!! Bütün query fonksiyonları için dönen veriler entity'nin bütün proplarını kapsamak zorundadır.Yani select edilen spesifik bir kolon tek başına değer dönemez.


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
    }
}

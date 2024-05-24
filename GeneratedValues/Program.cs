//Generated valuelerin genel hedefi verilerin uygulama dışında default değerler elde edilmesidir.Bu değerlerle birlikte Identity değerlerini de manipule edebiliriz.
using System.ComponentModel.DataAnnotations.Schema;

class Person
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)] // Burda iradeli olarak Pk özelliğini kaldırıyoruz.Eğer ki kaldırmazsak migrate ederken hata alacağız.
    public int Id { get; set; }
    public string Name { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Artık OtherId bir PK değeri olacaktır.Fakat ilk id değerinden PK özelliğini almamız lazm.
    public int OtherId { get; set; }
}
//PK değerini başka bir kolon yapmak istediğimizde bunu bir attribute ile yapabiliriz.Fakat asıl olan PK değerini kaldırmak zorundayız.
//PK değerini sayısal olmayan bir değere atamak istediğimizde PK değerini iradeli bir şekilde kaldırmaya ihtiyaç duymayız.
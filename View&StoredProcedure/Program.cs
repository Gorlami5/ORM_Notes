#region View Nedir
//Bir veritabanı objesidir ve kompleks sorguları daha rahat kullanabilmemiz için kullandığımız bir yapıdır.
#endregion
#region Ef Core'da View nasıl oluşturulur.
//1) Boş bir migration oluşturulur.
//2) Up fonksiyonun migrationBuilder ile bir Sql fonksiyonu eklenir ve bu fonksiyon parametre olarak view oluşturma scriptini alır.Hangi db ile çalışıyorsak ona göre değişkenlik gösterecektir.
//3) Down fonksiyonuna ise yine Sql fonksiyonu ile View objesini drop eden script eklenmeli.
//4) Oluşan view modellenmeli ve DBSet<> ile tanımlanmalı.Alacağı değerlere göre modelleme yapılabilir.
//5) Fakat bu modelin EF Core'a bir view olduğu bildirlmeli.Bunu da Fluent API ile OnModelCreating içerisinde yapmalıyız.modelBuilder.Entity<PersonOrder>().ToView("oluşturulan_view_ismi").HasNoKey() ile bu konfigürasyon yapılır.
//6) Execute etmek için bir veritabanı sorgusu gibi davranılabilir.
#endregion
#region Stored Procedure Nedir
//Bir veritabanı objesidir ve kompleks sorguları daha rahat kullanabilmemiz için kullandığımız bir yapıdır.View'dan farkı tablo olarak tutulmaz.Fonksiyonel bir davranış sergileyecektir.
//1) Boş bir migration oluşturulur.
//2) Up fonksiyonun migrationBuilder ile bir Sql fonksiyonu eklenir ve bu fonksiyon parametre olarak stored procedure oluşturma scriptini alır.Hangi db ile çalışıyorsak ona göre değişkenlik gösterecektir.
//3) Down fonksiyonuna ise yine Sql fonksiyonu ile View objesini drop eden script eklenmeli.
//4) Oluşan stored procedure modellenmeli ve DBSet<> ile tanımlanmalı.Alacağı değerlere göre modelleme yapılabilir.
//5) Fakat bu modelin EF Core'a bir stored procedure olduğu bildirilmese de bir PK'sı olmadığını bilmemiz gerekecektir.Bunu da Fluent API ile OnModelCreating içerisinde yapmalıyız.modelBuilder.Entity<PersonOrder>().HasNoKey() ile bu konfigürasyon yapılır.
//6) Execute etmek için oluşturduğumuz modele giderek içerisinde FromSql() ile veritabanında execute edilme scriptini çağırırız.($"EXEC sp_PersonsOrder").ToListAsync()
#endregion

# YazGelProje
Batuhan KANBER
Mehmet Berk BAYTÜRK
Berkan NALBANT
Selim Metehan AKTAŞ

## Projenin Yapılışı
- git clone kodu ile proje local'e çekilir.
- Web.config dosyasındaki "connectionString="data source=" kısmına MSSQL sunucu adı yazılır.
- "Package Manager Console" açılarak `update-database` kodu çalıştırılır.
![VeriTabani](/README_FOTO/VeriTabani.png)  
- InternCases tablosuna yukarıdaki bilgiler eklenir.  
![VeriTabani](/README_FOTO/VeriTabani2.png)  
- SemesterStarts tablosuna yukarıdaki bilgiler eklenir.  
![VeriTabani](/README_FOTO/VeriTabani3.png)  
- Commissions tablosuna yukarıdaki bilgiler eklenir.
- Giriş yapabilmek için bir tane SuperAdmin tanımlanır.
- Projede Rebuild solution yapılır.
- Projede Clean solution yapılır.
- Proje çalıştırılır ve süper admin olarak giriş yapabilmek için url'ye `Login/SuperAdminLogin` eklenir.

# TaskManagement API

Kullanıcıların günlük, haftalık ve aylık görevlerini yönetebildiği, JWT ile korunan bir REST API.
.NET 8, Clean Architecture ve PostgreSQL ile geliştirildi (Piton staj projesi).

## Özellikler

- Kayıt olma ve giriş yapma (BCrypt ile şifre hash'leme, JWT ile kimlik doğrulama)
- Görev ekleme, listeleme, güncelleme ve silme (soft delete)
- Her kullanıcı yalnızca kendi görevlerine erişebilir
- Sayfalama ve filtreleme (periyot, durum, tarih aralığı)
- Rapor: toplam, tamamlanan, devam eden ve bekleyen görev sayıları ile tamamlanma oranı
- Standart hata formatı ve Swagger arayüzü (Bearer token ile test edilebilir)

## Teknolojiler

.NET 8, ASP.NET Core Web API, Entity Framework Core 8 + Npgsql, PostgreSQL 16,
FluentValidation, Mapster, BCrypt.Net, JWT Bearer, Swagger (Swashbuckle), Docker

## Çalıştırma

### Docker ile (önerilen)

Gereksinim: Docker Desktop (veya Docker Engine + Compose).

```bash
docker compose up --build
```

- Swagger: http://localhost:8080/swagger
- PostgreSQL (host'tan erişim için): `localhost:5433`
- Migration'lar API açılırken otomatik uygulanır.

Durdurmak için `docker compose down`, veritabanı verisini de silmek için `docker compose down -v`.

Varsayılan şifre ve JWT anahtarı yalnızca demo içindir. Kendi değerlerinizi kullanmak için
`.env.example` dosyasını `.env` olarak kopyalayıp düzenleyin.

### Yerel geliştirme (Docker yalnızca veritabanı için)

```bash
# 1. Sadece veritabanını başlat
docker compose up -d db

# 2. Gizli ayarları user-secrets ile tanımla
dotnet user-secrets init --project src/TaskManagement.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=TaskManagementDb;Username=postgres;Password=postgres" --project src/TaskManagement.API
dotnet user-secrets set "Jwt:Secret" "en-az-32-karakterlik-uzun-ve-rastgele-bir-anahtar" --project src/TaskManagement.API

# 3. Migration'ları uygula ve çalıştır
dotnet ef database update --project src/TaskManagement.Infrastructure --startup-project src/TaskManagement.API
dotnet run --project src/TaskManagement.API
```

Swagger, Development ortamında otomatik açılır.

## Swagger ile deneme

1. `POST /api/auth/register` ile kullanıcı oluşturun, yanıttaki `token` değerini kopyalayın.
2. Sağ üstteki **Authorize** butonuna token'ı yapıştırın (`Bearer ` ön ekini Swagger ekler).
3. `/api/tasks` uçlarını deneyin.

## API uç noktaları

| Metot | Yol | Açıklama | Yetki |
|-------|-----|----------|-------|
| POST | `/api/auth/register` | Kayıt ol, token al | Hayır |
| POST | `/api/auth/login` | Giriş yap, token al | Hayır |
| GET | `/api/tasks` | Görev listesi (sayfalama, `period`, `status`, `dateFrom`, `dateTo`) | Evet |
| GET | `/api/tasks/report` | Görev raporu (`period`, `dateFrom`, `dateTo`) | Evet |
| GET | `/api/tasks/{id}` | Görev detayı | Evet |
| POST | `/api/tasks` | Görev oluştur | Evet |
| PUT | `/api/tasks/{id}` | Görevi güncelle | Evet |
| DELETE | `/api/tasks/{id}` | Görevi sil (soft delete) | Evet |

Hata yanıtları tek formatta döner:

```json
{ "statusCode": 400, "message": "...", "errors": { "Title": ["..."] }, "traceId": "..." }
```

## Mimari

Bağımlılık yönü her zaman içeriye doğrudur:

```
API ──► Application ──► Domain
 │                        ▲
 └──► Infrastructure ─────┘ (Application üzerinden)
```

| Katman | Sorumluluk |
|--------|-----------|
| **Domain** | Entity'ler (`User`, `TaskItem`), enum'lar, repository arayüzleri. Hiçbir şeye bağımlı değil. |
| **Application** | İş mantığı (`AuthService`, `TaskService`), DTO'lar, validator'lar, Mapster eşlemeleri, ayar sınıfları, ortak sabitler. Sadece Domain'e bağımlı. |
| **Infrastructure** | EF Core `DbContext`, repository ve Unit of Work implementasyonları, BCrypt ve JWT implementasyonları. |
| **API** | İnce controller'lar, global exception middleware, JWT ve Swagger yapılandırması. İş mantığı içermez. |

## Mimari kararlar ve gerekçeleri

- **Clean Architecture:** İş kuralları framework ve veritabanından bağımsız kalır, katmanlar ayrı ayrı test edilebilir.
- **BCrypt ve JWT arayüzlerle soyutlandı:** `IPasswordHasher` ve `IJwtTokenGenerator` Application'da, implementasyonları Infrastructure'da. Böylece Application, harici kütüphanelere bağımlı olmaz.
- **Repository + Unit of Work:** Veri erişimi tek noktada toplanır, servisler EF Core'u doğrudan bilmez, birden fazla işlem tek `SaveChanges` ile atomik kaydedilir.
- **Soft delete:** Silme işlemi `DbContext.SaveChangesAsync` içinde `IsDeleted = true` güncellemesine çevrilir, okumalarda global query filter (`IsDeleted == false`) uygulanır. `UpdatedDate` de aynı yerde otomatik doldurulur. Benzersiz e-posta/kullanıcı adı indeksleri yalnızca silinmemiş kayıtlar için tanımlıdır.
- **Doğrulama servis içinde (FluentValidation):** Servis hangi yoldan çağrılırsa çağrılsın kurallar uygulanır. Enum'lar 1'den başladığı için `IsInEnum` ile boş (`0`) değerler reddedilir.
- **Hata yönetimi exception + middleware ile:** Servisler anlamlı exception'lar fırlatır (`NotFound`, `Conflict`, `Unauthorized`, `ValidationException`), tek bir middleware bunları HTTP durum koduna ve standart formata çevirir. Beklenmeyen hatalarda istemciye ayrıntı sızdırılmaz, `traceId` ile log'dan bulunur.
- **Başkasının görevi için 404:** Yetkisiz erişimde 403 yerine 404 dönülür, böylece bir görevin var olup olmadığı bilgisi sızmaz.
- **Mapster:** AutoMapper'ın yeni sürümleri lisans anahtarı gerektirdiği için tercih edildi.
- **Options pattern:** JWT ve veritabanı ayarları `IOptions<T>` ile tek yerden okunur. Token üretimi ve doğrulaması aynı ayarları kullanır.
- **UTC:** Tüm tarihler UTC'ye normalize edilir (Npgsql `timestamptz` için gereklidir). Tarih filtresi gün bazlıdır ve bitiş günü dahildir.
- **Sabitler (Constants):** Validasyon limitleri, sayfalama değerleri ve hata mesajları tek yerde toplanır, "magic string/number" kullanılmaz.
- **Sayfalama ve rapor veritabanında:** `Skip/Take` ve `Count` sorguları SQL'de çalışır, veri belleğe çekilip kesilmez.

## Yapılandırma

Ortam değişkeni olarak verilirken iç içe anahtarlar `__` ile yazılır (`Jwt__Secret`).

| Anahtar | Açıklama |
|---------|----------|
| `ConnectionStrings:DefaultConnection` | PostgreSQL bağlantı cümlesi |
| `Jwt:Secret` | İmza anahtarı (en az 32 karakter) |
| `Jwt:Issuer`, `Jwt:Audience` | Token yayıncısı ve hedef kitlesi |
| `Jwt:ExpirationMinutes` | Token ömrü (dakika) |
| `Database:MigrateOnStartup` | `true` ise açılışta migration uygulanır |
| `Swagger:Enabled` | `true` ise Development dışında da Swagger açılır |

## Proje yapısı

```
src/
├── TaskManagement.Domain/          Entities, Enums, Interfaces
├── TaskManagement.Application/     DTOs, Services, Validators, Mappings, Common
├── TaskManagement.Infrastructure/  Persistence, Repositories, Security
└── TaskManagement.API/             Controllers, Middleware, Extensions
```
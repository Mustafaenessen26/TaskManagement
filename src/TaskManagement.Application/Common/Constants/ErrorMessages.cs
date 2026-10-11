namespace TaskManagement.Application.Common.Constants;

public static class ErrorMessages
{
    public const string EmailAlreadyExists = "Bu e-posta adresi zaten kayıtlı.";
    public const string UsernameAlreadyExists = "Bu kullanıcı adı zaten alınmış.";
    public const string InvalidCredentials = "E-posta veya şifre hatalı.";
    public const string TaskNotFound = "Görev bulunamadı.";
    public const string DateRangeInvalid = "Bitiş tarihi, başlangıç tarihinden önce olamaz.";
    public const string UsernameInvalidFormat = "Kullanıcı adı yalnızca harf, rakam, '_' ve '.' içerebilir.";
    public const string PasswordRequiresUppercase = "Şifre en az bir büyük harf içermelidir.";
    public const string PasswordRequiresLowercase = "Şifre en az bir küçük harf içermelidir.";
    public const string PasswordRequiresDigit = "Şifre en az bir rakam içermelidir.";
    public const string ConnectionStringMissing = "ConnectionStrings:DefaultConnection ayarı bulunamadı.";
    public const string ValidationFailed = "Bir veya daha fazla doğrulama hatası oluştu.";
    public const string UnexpectedError = "Beklenmeyen bir hata oluştu.";
    public const string Unauthorized = "Bu işlem için kimlik doğrulaması gerekli.";
    public const string InvalidToken = "Geçersiz kimlik bilgisi.";
}
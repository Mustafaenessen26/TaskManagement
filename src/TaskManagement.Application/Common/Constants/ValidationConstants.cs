namespace TaskManagement.Application.Common.Constants;

public static class ValidationConstants
{
    public const int UsernameMinLength = 3;
    public const int UsernameMaxLength = 50;
    public const string UsernamePattern = "^[a-zA-Z0-9_.]+$";
    public const int EmailMaxLength = 256;
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 72; // BCrypt 72 byte'tan sonrasını yok sayar
    public const int TaskTitleMaxLength = 200;
    public const int TaskDescriptionMaxLength = 2000;
}
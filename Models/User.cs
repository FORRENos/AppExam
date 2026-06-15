namespace AppExam.Models;

public sealed class User
{
    public int Id { get; set; }
    public string RoleName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";

    public bool IsAdministrator => RoleName == "Администратор";
    public bool CanManageProducts => RoleName is "Администратор";
}

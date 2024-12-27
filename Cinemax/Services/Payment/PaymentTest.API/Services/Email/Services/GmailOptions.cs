namespace Test.Email.Services;

public class GmailOptions
{
    public const string GmailOptionsKey = "GmailOptions";

    public string Host { get; set; }
    public int Port { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
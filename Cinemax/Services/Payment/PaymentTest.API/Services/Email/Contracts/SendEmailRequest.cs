namespace PaymentTest.API.Services.Email.Contracts;


public record SendEmailRequest(string From, string To, string Subject, string Body);


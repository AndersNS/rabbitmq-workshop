namespace RabbitMQWorkshop;

public class TokenIssuedEvent
{
    public string ClientId { get; set; } = string.Empty;
    public string[] ScopesRequested { get; set; } = [];
    public TokenType TokenType { get; set; }
    public DateTime IssuedAt { get; set; }
}

public enum TokenType
{
    Access,
    Refresh,
    Id
}

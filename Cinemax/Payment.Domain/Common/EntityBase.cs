namespace Payment.Domain.Common;

public class EntityBase
{
    public int Id { get; protected set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}
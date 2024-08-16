namespace cosmonauticaui.Server.Models
{
    public record Document(
        Guid id,
        string name,
        string file,
        string[] places,
        string[] counterParties,
        string[] products
    );
}

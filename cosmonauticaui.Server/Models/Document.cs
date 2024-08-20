namespace cosmonauticaui.Server.Models
{
    public record Document(
        Guid id,
        string name,
        string file,
        string[] places,
        string[] counterParties,
        string[] products
    )
    {
        public static implicit operator Document(FormCollection formCollection)
		{
            var id = Guid.NewGuid();
            var name = formCollection["name"];
			var file = formCollection.Files[0];
            var fileName = Path.GetFileName(file.FileName);
            string places = formCollection["places"];
            string counterParties = formCollection["counterParties"];
            string products = formCollection["products"];

			return new Document(
                id,
                name,
				fileName,
                places.Split(" "),
                counterParties.Split(" "),
				products.Split(" ")
			);
        }
    }
}

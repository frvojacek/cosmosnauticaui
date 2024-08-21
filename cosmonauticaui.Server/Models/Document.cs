namespace cosmonauticaui.Server.Models
{
    public class Document
	{
        public string id { get; set; }
        public string Name { get; set; }
		public List<DocumentVersion> Versions { get; set; }
		public string[] Places { get; set; }
        public string[] CounterParties { get; set; }
        public string[] Products { get; set; }

        public Document(string name, string[] places, string[] counterParties, string[] products)
        {
            id = Guid.NewGuid().ToString();
            Name = name;
            Versions = new List<DocumentVersion>();
            Places = places;
            CounterParties = counterParties;
            Products = products;
        }

		public static implicit operator Document(FormCollection formCollection)
		{
            var version = formCollection["version"];
            var name = formCollection["name"];
            string places = formCollection["places"];
            string counterParties = formCollection["counterParties"];
            string products = formCollection["products"];

			return new Document(
                name,
				places.Split(" "),
                counterParties.Split(" "),
				products.Split(" ")
			);
        }
    }

    public record DocumentVersion(
        string id,
		string fileId,
		string fileName,
        string validFrom,
        string validTo
    );
}

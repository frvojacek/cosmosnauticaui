namespace cosmonauticaui.Server.Models
{
    public class Document
	{
        public Guid id { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public string Version { get; set; }
        public string[] Places { get; set; }
        public string[] CounterParties { get; set; }
        public string[] Products { get; set; }

        public Document(string name, string file, string version, string[] places, string[] counterParties, string[] products)
        {
            id = Guid.NewGuid();
            Name = name;
            File = file;
            Version = version;
            Places = places;
            CounterParties = counterParties;
            Products = products;
        }

		public static implicit operator Document(FormCollection formCollection)
		{
            var version = formCollection["version"];
            var name = formCollection["name"];
			var file = formCollection.Files[0];
            var fileName = Path.GetFileName(file.FileName);
            string places = formCollection["places"];
            string counterParties = formCollection["counterParties"];
            string products = formCollection["products"];

			return new Document(
                name,
				fileName,
				version,
				places.Split(" "),
                counterParties.Split(" "),
				products.Split(" ")
			);
        }
    }
}

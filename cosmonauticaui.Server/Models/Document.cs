namespace cosmonauticaui.Server.Models
{
    public class Document
    {

        public string id;
        public string fileName;

        public Document(string id, string fileName)
        {
            this.id = id;
            this.fileName = fileName;
        }

    }
}

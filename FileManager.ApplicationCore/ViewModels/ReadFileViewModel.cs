using System.IO;

namespace FileManager.ApplicationCore.ViewModels
{
    public class ReadFileViewModel
    {       
            public string Uri { get; set; }
            public string Name { get; set; }
            public string ContentType { get; set; }
            public Stream Content { get; set; }        
    }
}

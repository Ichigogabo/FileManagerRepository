using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.ViewModels
{
    public class ReadFileViewModel
    {       
            public string Uri { get; set; }
            public string Name { get; set; }
            public string ContentType { get; set; }
            public Stream Content { get; set; }        
    }
}

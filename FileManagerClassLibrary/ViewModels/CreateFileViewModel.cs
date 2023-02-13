using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.ViewModels
{
    public class CreateFileViewModel
    {
        [DisplayName("Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The File is Requerid")]
        [DisplayName("File")]
        public IFormFile File { get; set; }
    }

    public class CreateFileResponseViewModel
    {      
        public string FileName { get; set; }
    
        public string FileUri { get; set; }
        public bool Success { get; set; }
    }


}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileManager.ViewModels.Field
{
    public class FileVM
    {
        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]
        [DisplayName("File")]
        public IFormFile File { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FileManager.ApplicationCore.ViewModels
{
    public class CreateFileViewModel
    {
        [Required(ErrorMessage = "Description is Requerid")]
        [DisplayName("Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "File is Requerid")]
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

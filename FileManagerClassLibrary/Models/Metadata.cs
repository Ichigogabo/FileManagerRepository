using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.Models
{
    public class Metadata
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "description")]
        [DisplayName("Description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "filename")]
        [DisplayName("File Name")]
        public string FileName { get; set; }
        [JsonProperty(PropertyName = "contenttype")]
        [DisplayName("Content-Type")]
        public string ContentType { get; set; }
        [JsonProperty(PropertyName = "filesize")]
        [DisplayName("File Size")]
        public long FileSize { get; set; }
        [JsonProperty(PropertyName = "uploadeddate")]
        [DisplayName("Uploaded Date")]
        public DateTime UploadedDate { get; set; }
        [JsonProperty(PropertyName = "uploadedby")]
        [DisplayName("Uploaded By")]
        public string UploadedBy { get; set; }
        [JsonProperty(PropertyName = "url")]
        [DisplayName("Name In Blob Storage")]
        public string NameInBlobStorage { get; set; }
    }
}

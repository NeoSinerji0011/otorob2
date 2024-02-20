using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options; 
using OtoRobotWeb2.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace  OtoRobotWeb2.Services
{
    public class StorageService
    {
       
    }
    public class GetBlobRequestDto
    {
        [Required]
        public string Name { get; set; }
    }
    public class BlobDto
    {
        public byte[] Content { get; set; }

        public string Name { get; set; }
    }
    public class SaveBlobInputDto
    {
        public byte[] Content { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
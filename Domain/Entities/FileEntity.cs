using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FileEntity : BaseEntity
    {
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public Byte[]? FileData { get; set; }
        public int? Version { get; set; }
    }
}

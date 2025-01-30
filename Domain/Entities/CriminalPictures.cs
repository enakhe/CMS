#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Entities
{
    public class CriminalPictures
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CriminalId { get; set; }
        public byte[] Mugshot { get; set; }
        public List<byte[]> AdditionalPictures { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public virtual Criminal Criminal { get; set; }
    }
}

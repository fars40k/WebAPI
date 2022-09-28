using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI_App.Data
{

    public class Credentials
    {

        [Key]
        [MaxLength(32)]
        public byte[] Entry { get; set; }

        public Credentials()
        {

        }

        public Credentials(byte[] newCredentials)
        {
            Entry = newCredentials;
        }

    }
}

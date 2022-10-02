using System.ComponentModel.DataAnnotations;

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

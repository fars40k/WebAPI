using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI_App.Data
{
    public class CredentialsWorker
    {
        private WinTaskContext _context;

        public CredentialsWorker(WinTaskContext context)
        {
            _context = context;

            if (context.Credentials.Count() == 0)
            {
                SHA256 sha256Hash = SHA256.Create();
                byte[] initialAccount = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes("Credentials.admin.12345.Admin"));

                context.Credentials.Add(new Credentials(initialAccount));
                context.SaveChanges();
            }
        }

        public void Insert(Credentials newCredentials)
        {
            _context.Credentials.Add(newCredentials);
        }

        public List<Credentials> FindAll()
        {
            return _context.Credentials.AsNoTracking().ToList();
        }
    }
}

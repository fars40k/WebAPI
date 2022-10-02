using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

        public List<Credentials> FindAll()
        {

            return _context.Credentials.AsNoTracking().ToList();

        }

        public void InsertAccount(string login, string password, string role)
        {

            SHA256 sha256Hash = SHA256.Create();
            byte[] account = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes($"Credentials.{login}.{password}.{role}"));

            _context.Credentials.Add(new Credentials(account));
            _context.SaveChanges();

        }

        public void Insert(Credentials newCredentials)
        {

            _context.Credentials.Add(newCredentials);
            _context.SaveChanges();

        }

        public void Update(string login, string password, string newPassword, string role)
        {
            try
            {

                SHA256 sha256Hash = SHA256.Create();
                byte[] oldAccount = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes($"Credentials.{login}.{password}.{role}"));

                Credentials oldForDeletion = _context.Credentials.FirstOrDefault(c => c.Entry.SequenceEqual(oldAccount));
                _context.Credentials.Remove(oldForDeletion);

                byte[] newAccount = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes($"Credentials.{login}.{newPassword}.{role}"));
                _context.Credentials.Add(new Credentials(newAccount));
                _context.SaveChanges();

            }
            catch (Exception ex)
            {

            }
        }

        public void Delete(string login, string password, string role)
        {
            try
            {

                SHA256 sha256Hash = SHA256.Create();
                byte[] oldAccount = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes($"Credentials.{login}.{password}.{role}"));

                foreach (Credentials item in _context.Credentials)
                {
                    if (item.Entry.SequenceEqual(oldAccount))
                    {
                        _context.Credentials.Remove(item);                        
                        break;
                    }
                }

                _context.SaveChanges();

            }
            catch (Exception ex)
            {

            }
        }

    }
}

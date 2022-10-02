using System;
using WebAPI_App.Data.Interfaces;

namespace WebAPI_App.Data
{
    /// <summary>
    /// Contains Repositories with basic database operations and worker for complex
    /// </summary>
    public class DataAccessObject : IDataAccessService
    {
        public WinTaskContext wtContext;
        public IRepository<Person> Personel;
        public IRepository<Project> Projects;
        public IRepository<Goal> Goals;
        public LinkedDataWorker LinkedData;
        public CredentialsWorker Credentials;

        public DataAccessObject(WinTaskContext context)
        {         
            wtContext = context;

            wtContext.Database.CreateIfNotExists();
            wtContext.Database.Connection.Open();

            UpdateContext();
            UpdateEntityModel();
        }     

        public bool DoesConnectedToDb()
        {
            try
            {
                wtContext.Database.Connection.Close();
                wtContext.Database.Connection.Open();
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void UpdateEntityModel()
        {

            Personel.FindAll();
            
        }

        public void SaveChanges()
        {
            Personel.SaveChanges();

            UpdateContext();
        }

        public void UpdateContext()
        { 

            Personel = new BaseRepository<Person>(wtContext);
            Projects = new BaseRepository<Project>(wtContext);
            Goals = new BaseRepository<Goal>(wtContext);
            LinkedData = new LinkedDataWorker(wtContext);
            Credentials = new CredentialsWorker(wtContext);

        }

    }
}

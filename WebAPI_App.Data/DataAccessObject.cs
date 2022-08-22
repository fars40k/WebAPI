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

        public DataAccessObject(WinTaskContext context)
        {
            wtContext = context;

            UpdateContextInRepositories();
            UpdateEntityModel();
        }     

        public void UpdateEntityModel()
        {

            Personel.FindAll();
            var personelToProjects = wtContext.Personel.Include("ProjectsWith");
            var personelToGoals = wtContext.Personel.Include("GoalsWith");
            var goalsToProjects = wtContext.Goals.Include("ProjectsWith");

        }

        public void SaveChanges()
        {
            Personel.SaveChanges();

            UpdateContextInRepositories();
        }

        public void UpdateContextInRepositories()
        { 

            Personel = new BaseRepository<Person>(wtContext);
            Projects = new BaseRepository<Project>(wtContext);
            Goals = new BaseRepository<Goal>(wtContext);
            LinkedData = new LinkedDataWorker(wtContext);

        }

    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace WebAPI_App.Data
{
    /// <summary>
    /// Contains navigation properties data work-logic
    /// </summary>
    public partial class LinkedDataWorker
    {
        private WinTaskContext _context;

        private bool _disposed = false;

        public LinkedDataWorker(WinTaskContext context)
        {
            this._context = context;
        }

        public void AddPersonToProject(Guid PersonGUID, Guid ProjectGUID)
        {
            var personelToProjects = _context.Personel.Include("ProjectsWith");

            var project = _context.Projects.Where(p => p.ProjectID.Equals(ProjectGUID)).FirstOrDefault<Project>();
            var person = _context.Personel.Where(r => r.PersonID.Equals(PersonGUID)).FirstOrDefault<Person>();

            Project projectDao = project;
            Person personDao = person;

            if ((projectDao != null) && (personDao != null) && (!projectDao.PersonelWith.Contains<Person>(personDao)))
            {
                projectDao.PersonelWith.Add(personDao);
            }
        }

        public void RemovePersonFromProject(Guid PersonGUID, Guid ProjectGUID)
        {
            var project = _context.Projects.Where(p => p.ProjectID.Equals(ProjectGUID)).FirstOrDefault<Project>();
            var person = _context.Personel.Where(r => r.PersonID.Equals(PersonGUID)).FirstOrDefault<Person>();

            var projectPersonel = _context.Personel.Include(p => p.ProjectsWith).ToList();

            Project projectDao = project;
            Person personDao = person;

            if ((projectDao != null) && (personDao != null) && (projectDao.PersonelWith.Contains<Person>(personDao)))
            {
                projectDao.PersonelWith.Remove(personDao);
            }
        }

        public void AddGoalToProject(Guid GoalGUID, Guid ProjectGUID)
        {
            var project = _context.Projects.Where(p => p.ProjectID.Equals(ProjectGUID)).FirstOrDefault<Project>();
            var goal = _context.Goals.Where(r => r.GoalID.Equals(GoalGUID)).FirstOrDefault<Goal>();

            Project projectDao = project;
            Goal goalDao = goal;

            if ((projectDao != null) && (goalDao != null) && (!projectDao.GoalsIn.Contains<Goal>(goalDao)))
            {
                goalDao.ProjectsWith.Add(projectDao);
                projectDao.GoalsIn.Add(goalDao);
            }
        }

        public void RemoveGoalFromProject(Guid GoalGUID, Guid ProjectGUID)
        {
            var project = _context.Projects.Where(p => p.ProjectID.Equals(ProjectGUID)).FirstOrDefault<Project>();
            var goal = _context.Goals.Where(r => r.GoalID.Equals(GoalGUID)).FirstOrDefault<Goal>();

            Project projectDao = project;
            Goal goalDao = goal;

            if ((projectDao != null) && (goalDao != null) && (projectDao.GoalsIn.Contains<Goal>(goalDao)))
            {
                goalDao.ProjectsWith.Remove(projectDao);
                projectDao.GoalsIn.Remove(goalDao);
            }
        }

        public void AddPersonToGoal(Guid PersonGUID, Guid GoalGUID)
        {
            var goal = _context.Goals.Where(p => p.GoalID.Equals(GoalGUID)).FirstOrDefault<Goal>();
            var person = _context.Personel.Where(r => r.PersonID.Equals(PersonGUID)).FirstOrDefault<Person>();

            Goal goalDao = goal;
            Person personDao = person;

            if ((goalDao != null) && (personDao != null) && (!goalDao.PersonelWith.Contains<Person>(personDao)))
            {
                goalDao.PersonelWith.Add(personDao);
            }
        }

        public void RemovePersonFromGoal(Guid PersonGUID, Guid GoalGUID)
        {
            var goal = _context.Goals.Where(p => p.GoalID.Equals(GoalGUID)).FirstOrDefault<Goal>();
            var person = _context.Personel.Where(r => r.PersonID.Equals(PersonGUID)).FirstOrDefault<Person>();

            Goal goalDao = goal;
            Person personDao = person;

            if ((goalDao != null) && (personDao != null) && (goalDao.PersonelWith.Contains<Person>(personDao)))
            {
                goalDao.PersonelWith.Remove(personDao);
            }
        }

        public IEnumerable<Goal> FindGoalsForProject(Guid ProjectID)
        {
            var project = _context.Projects.Where(p => p.ProjectID.Equals(ProjectID)).Include("GoalsIn").FirstOrDefault<Project>();

            List<Goal> goals = new List<Goal>();

            if (project != null)
            {
                foreach (Goal item in project.GoalsIn)
                {
                    goals.Add(item);
                }

                return goals;

            } else
            {
                return null;
            }
        }

        public IEnumerable<Person> FindAllPersonelWithLinks()
        {
            return _context.Personel.Include(p => p.GoalsWith.Select(w => w.ProjectsWith));
        }

        public IEnumerable<Person> FindPersonelForProject(Guid ProjectID)
        {
            var project = _context.Projects.Where(p => p.ProjectID.Equals(ProjectID)).Include("PersonelWith").FirstOrDefault();

            List<Person> personel = new List<Person>();

            if (project != null)
            {
                foreach (Person item in project.PersonelWith)
                {
                    personel.Add(item);
                }

                return personel;

            } else
            {
                return null;
            }
        }

        public IEnumerable<Person> FindPersonelForGoal(Guid GoalID)
        {
            var goal = _context.Goals.Where(p => p.GoalID.Equals(GoalID)).Include("PersonelWith").FirstOrDefault<Goal>();

            List<Person> personel = new List<Person>();

            foreach (Person item in goal.PersonelWith)
            {
                personel.Add(item);
            }

            return personel;
        }

        public Goal FindGoalwithProject(Guid GoalID)
        {
            Goal goal = _context.Goals.Where(g => g.GoalID == GoalID).Include("ProjectsWith").FirstOrDefault();

            return goal;
        }

        public EntityState CheckState(dynamic entity)
        {
            return _context.Entry(entity).State;
        }

        public void MakeModifiedStatus(dynamic entity)
        {        
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {

            }
            _disposed = true;
        }

        ~LinkedDataWorker()
        {
            Dispose(false);
        }

        public void ClearLinksForPerson(Guid personID)
        {
            var person = _context.Personel.Where(r => r.PersonID.Equals(personID)).FirstOrDefault<Person>();

            foreach(Project item in _context.Projects)
            {
                item.PersonelWith.Remove(person);
            }
            foreach (Goal item in _context.Goals)
            {
                item.PersonelWith.Remove(person);
            }

        }

    }

}


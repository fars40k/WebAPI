using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_App.Data;

namespace WebAPI_App.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private DataAccessObject _dataObject;

        public ProjectsController(DataAccessObject dataAccessObject)
        {
            _dataObject = dataAccessObject;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var list = _dataObject.Projects.FindAll();

            if (list.Count() == 0) return new JsonResult(null);

            foreach (Project item in list)
            {
                TrimProjectData(item);
            }

            var humbleList = MakeHumbleList(list);

            return new JsonResult(humbleList);
        }

        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            try
            {
                var projects = _dataObject.Projects.FindByID(Guid.Parse(id));

                if (projects == null) return new JsonResult(null) { StatusCode = 404 };

                TrimProjectData(projects);

                return new JsonResult(projects);
            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [HttpPut]
        public ActionResult Update(Project newProject)
        {
            return this.Insert(newProject);
        }


        [HttpPost]
        public ActionResult Insert(Project newProject)
        {
            try
            {
                if (_dataObject.Projects.FindByID(newProject.ProjectID) != null)
                {
                    // Updating existed entry

                    Project found = _dataObject.Projects.FindByID(newProject.ProjectID);

                    found.Name = newProject.Name;
                    found.Description = newProject.Description;
                    found.CreationDate = newProject.CreationDate;
                    found.ExpireDate = newProject.ExpireDate;
                    found.Percentage = newProject.Percentage;
                    found.StatusKey = newProject.StatusKey;

                    _dataObject.LinkedData.MakeModifiedStatus(found);

                    _dataObject.SaveChanges();
                }
                else
                {
                    // Adding new entry

                    _dataObject.Projects.Insert(newProject);

                }

                return new JsonResult(null);
            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            try
            {
                Project projectForDeletion = _dataObject.Projects.FindByID(Guid.Parse(id));

                if (projectForDeletion != null)
                {
                    var list = new List<Guid>();
                    
                    foreach (Goal item in projectForDeletion.GoalsIn)
                    {
                        list.Add(item.GoalID);
                    }

                    foreach (Guid item in list)
                    {
                        _dataObject.Goals.Delete(item);
                    }

                    _dataObject.Projects.Delete(Guid.Parse(id));
                    _dataObject.LinkedData.SaveChanges();

                }
                else
                {

                    return new JsonResult(null) { StatusCode = 404 };

                }

                return new JsonResult(null);
            }
            catch
            {
                return new JsonResult(null) { StatusCode = 400 };
            }
        }

        [Route("AddTaskTo/{ProjectId}/{GoalId}")]
        [HttpGet]
        public JsonResult GetAddTaskToProject(string ProjectId, string GoalId)
        {
            try
            {
                _dataObject.LinkedData.AddGoalToProject(Guid.Parse(GoalId), Guid.Parse(ProjectId));
                _dataObject.LinkedData.SaveChanges();
                return new JsonResult(null);

            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [Route("RemoveTaskFrom/{ProjectId}/{GoalId}")]
        [HttpGet]
        public JsonResult GetRemoveTaskFromProject(string ProjectId, string GoalId)
        {
            try
            {
                _dataObject.LinkedData.RemoveGoalFromProject(Guid.Parse(GoalId), Guid.Parse(ProjectId));
                _dataObject.LinkedData.SaveChanges();
                return new JsonResult(null);

            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        /// <summary>
        /// Trimming fields of the parameter object
        /// </summary>
        private void TrimProjectData(Project obj)
        {
            obj.Name= obj.Name.TrimEnd();
            obj.Description = obj.Description.Trim();
        }

        /// <summary>
        ///  Returns a copy of the parameter list without the navigation property collections
        /// </summary>
        private List<Project> MakeHumbleList(IEnumerable<Project> forCloning)
        {
            List<Project> outList = new List<Project>();

            foreach (Project item in forCloning)
            {
                Project cloned = new Project();

                cloned.ProjectID = item.ProjectID;
                cloned.Name = item.Name;
                cloned.Description = item.Description;
                cloned.CreationDate = item.CreationDate;
                cloned.ExpireDate = item.ExpireDate;
                cloned.Percentage = item.Percentage;
                cloned.StatusKey = item.StatusKey;
                cloned.GoalsIn = null;
                cloned.PersonelWith = null;

                outList.Add(cloned);
            }

            return outList;

        }
    }
}

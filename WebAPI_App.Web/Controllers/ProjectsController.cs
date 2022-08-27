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

        // GET: api/projects
        [HttpGet]
        public JsonResult Get()
        {
            var list = _dataObject.Projects.FindAll();

            if (list.Count() == 0) return new JsonResult(null);

            foreach (Project item in list)
            {
                TrimProjectData(item);
            }

            return new JsonResult(list);
        }

        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            try
            {
                var projects = _dataObject.Projects.FindByID(Guid.Parse(id));

                if (projects == null) return new JsonResult(null);

                TrimProjectData(projects);

                return new JsonResult(projects);
            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [HttpPost]
        public ActionResult Update(Project newProject)
        {
            try
            {
                if (_dataObject.Projects.FindByID(newProject.ProjectID) != null)
                {
                    // Updating existed entry

                    Project found = _dataObject.Projects.FindByID(newProject.ProjectID);

                    var s1 = _dataObject.LinkedData.CheckState(found);
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
                if (_dataObject.Projects.FindByID(Guid.Parse(id)) != null)
                {
                    _dataObject.LinkedData.ClearLinksForPerson(Guid.Parse(id));
                    _dataObject.Projects.Delete(Guid.Parse(id));
                }

                return new JsonResult(null);
            }
            catch
            {
                return new JsonResult(null) { StatusCode = 400 };
            }
        }

        [Route("AddTasks/{ProjectId}/{GoalId}")]
        [HttpGet]
        public JsonResult GetAddTasksToProject(string ProjectId, string GoalId)
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

        [Route("RemoveTasks/{ProjectId}/{GoalId}")]
        [HttpGet]
        public JsonResult GetRemoveTasksFromProject(string ProjectId, string GoalId)
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

        private void TrimProjectData(Project obj)
        {
            obj.Name= obj.Name.TrimEnd();
            obj.Description = obj.Description.Trim();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_App.Data;

namespace WebAPI_App.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalsController : ControllerBase
    {
        private DataAccessObject _dataObject;

        public GoalsController(DataAccessObject dataAccessObject)
        {
            _dataObject = dataAccessObject;
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public JsonResult Get()
        {
            var list = _dataObject.Goals.FindAll();

            if (list.Count() == 0) return new JsonResult(null);

            foreach (Goal item in list)
            {
                TrimGoalData(item);
            }

            var humbleList = MakeHumbleList(list);

            return new JsonResult(list);
        }

        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            try
            {
                var goal = _dataObject.Goals.FindByID(Guid.Parse(id));

                if (goal == null) return new JsonResult(null) { StatusCode = 404 };

                TrimGoalData(goal);

                return new JsonResult(goal);
            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [HttpPost]
        public ActionResult Update(Goal newGoal)
        {
            try
            {
                if (_dataObject.Goals.FindByID(newGoal.GoalID) != null)
                {
                    // Updating existed entry

                    Goal found = _dataObject.Goals.FindByID(newGoal.GoalID);

                    found.Name = newGoal.Name;
                    found.Description = newGoal.Description;
                    found.CreationDate = newGoal.CreationDate;
                    found.ExpireDate = newGoal.ExpireDate;
                    found.Percentage = newGoal.Percentage;
                    found.StatusKey = newGoal.StatusKey;

                    _dataObject.LinkedData.MakeModifiedStatus(found);

                    _dataObject.SaveChanges();
                }
                else
                {
                    // Adding new entry

                    _dataObject.Goals.Insert(newGoal);

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
                if (_dataObject.Goals.FindByID(Guid.Parse(id)) != null)
                {

                    _dataObject.Goals.Delete(Guid.Parse(id));

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

        [Route("FindForProject/{id}")]
        [HttpGet]
        public JsonResult GetForGoal(string id)
        {
            try
            {
                var list = _dataObject.LinkedData.FindGoalsForProject(Guid.Parse(id));

                if (list.Count() == 0) return new JsonResult(null);

                foreach (Goal item in list)
                {
                    TrimGoalData(item);
                }

                var humbleList = MakeHumbleList(list);

                return new JsonResult(list);

            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        // Trimming fields of the parameter object
        private void TrimGoalData(Goal obj)
        {
            obj.Name = obj.Name.TrimEnd();
            obj.Description = obj.Description.Trim();
        }

        // Returns a copy of the parameter list without the navigation property collections
        private List<Goal> MakeHumbleList(IEnumerable<Goal> forCloning)
        {
            List<Goal> outList = new List<Goal>();

            foreach (Goal item in forCloning)
            {
                Goal cloned = new Goal();

                cloned.GoalID = item.GoalID;
                cloned.Name = item.Name;
                cloned.Description = item.Description;
                cloned.CreationDate = item.CreationDate;
                cloned.ExpireDate = item.ExpireDate;
                cloned.Percentage = item.Percentage;
                cloned.Priority = item.Priority;
                cloned.StatusKey = item.StatusKey;
                cloned.ProjectsWith = null;
                cloned.PersonelWith = null;

                outList.Add(cloned);
            }

            return outList;

        }
    }
}

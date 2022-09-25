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
    public class PersonelController : ControllerBase
    {
        private DataAccessObject _dataObject;

        public PersonelController(DataAccessObject dataAccessObject)
        {
            _dataObject = dataAccessObject;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var list = _dataObject.Personel.FindAll();

            if (list.Count() == 0) return new JsonResult(null);

            foreach (Person item in list)
            {
                TrimPersonData(item);
            }

            var humbleList = MakeHumbleList(list);

            return new JsonResult(list);
        }

        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            try
            {
                var person = _dataObject.Personel.FindByID(Guid.Parse(id));

                if (person == null) return new JsonResult(null) { StatusCode = 404 };

                TrimPersonData(person);

                return new JsonResult(person);
            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [HttpPut]
        public ActionResult Update(Person newPerson)
        {
            return this.Insert(newPerson);
        }


        [HttpPost]
        public ActionResult Insert(Person newPerson)
        {
            try
            {

                if (_dataObject.Personel.FindByID(newPerson.PersonID) != null)
                {
                    // Updating existing entry

                    Person found = _dataObject.LinkedData.FindPersonWithLinks(newPerson.PersonID);

                    found.FirstName = newPerson.FirstName;
                    found.SurName = newPerson.SurName;
                    found.LastName = newPerson.LastName;
                    found.Division = newPerson.Division;
                    found.Occupation = newPerson.Occupation;

                    _dataObject.Personel.SaveChanges();
                }
                else
                {
                    // Adding new entry

                    _dataObject.Personel.Insert(newPerson);

                    _dataObject.Personel.SaveChanges();
                }

                return new JsonResult(null);
            }
            catch (Exception ex)
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            try
            {
                if (_dataObject.Personel.FindByID(Guid.Parse(id)) != null)
                {

                    _dataObject.LinkedData.ClearLinksForPerson(Guid.Parse(id));
                    _dataObject.Personel.Delete(Guid.Parse(id));

                } else
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

        [Route("ForProject/{id}")]
        [HttpGet]
        public JsonResult GetForProject(string id)
        {
            try
            {
                var list = _dataObject.LinkedData.FindPersonelForProject(Guid.Parse(id));

                if (list.Count() == 0) return new JsonResult(null);

                foreach (Person item in list)
                {
                    TrimPersonData(item);       
                }

                var humbleList = MakeHumbleList(list);

                return new JsonResult(humbleList);
            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [Route("ForGoal/{id}")]
        [HttpGet]
        public JsonResult GetForGoal(string id)
        {
            try
            {
                var list = _dataObject.LinkedData.FindPersonelForGoal(Guid.Parse(id));

                if (list.Count() == 0) return new JsonResult(null);

                foreach (Person item in list)
                {
                    TrimPersonData(item);
                }

                var humbleList = MakeHumbleList(list);

                return new JsonResult(humbleList);

            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [Route("Assign/{PersonId}/{ProjectId}")]
        [HttpGet]
        public JsonResult GetAssignToProject(string PersonId, string ProjectId)
        {
            try
            {
                _dataObject.LinkedData.AddPersonToProject(Guid.Parse(PersonId), Guid.Parse(ProjectId));
                _dataObject.LinkedData.SaveChanges();
                return new JsonResult(null);

            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [Route("Unassign/{PersonId}/{ProjectId}")]
        [HttpGet]
        public JsonResult GetUnassignToProject(string PersonId, string ProjectId)
        {
            try
            {
                _dataObject.LinkedData.RemovePersonFromProject(Guid.Parse(PersonId), Guid.Parse(ProjectId));
                var list = _dataObject.LinkedData.FindGoalsForProject(Guid.Parse(ProjectId));

                foreach(Goal item in list)
                {
                    _dataObject.LinkedData.RemovePersonFromProject(Guid.Parse(PersonId), Guid.Parse(item.GoalID.ToString()));
                }

                _dataObject.LinkedData.SaveChanges();
                return new JsonResult(null);

            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [Route("Appoint/{PersonId}/{GoalId}")]
        [HttpGet]
        public JsonResult GetAppointToProject(string PersonId, string GoalId)
        {
            try
            {
                _dataObject.LinkedData.AddPersonToGoal(Guid.Parse(PersonId), Guid.Parse(GoalId));
                _dataObject.LinkedData.SaveChanges();
                return new JsonResult(null);

            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [Route("Dismiss/{PersonId}/{GoalId}")]
        [HttpGet]
        public JsonResult GetDismissToProject(string PersonId, string GoalId)
        {
            try
            {
                _dataObject.LinkedData.RemovePersonFromGoal(Guid.Parse(PersonId), Guid.Parse(GoalId));
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
        private void TrimPersonData(Person obj)
        {
            obj.FirstName = obj.FirstName.TrimEnd();
            obj.SurName = obj.SurName.Trim();
            obj.LastName = obj.LastName.Trim();
            obj.Division = obj.Division.Trim();
            obj.Occupation = obj.Occupation.Trim();
        }


        /// <summary>
        ///  Returns a copy of the parameter list without the navigation property collections
        /// </summary>
        private List<Person> MakeHumbleList(IEnumerable<Person> forCloning)
        {
            List<Person> outList = new List<Person>();

            foreach (Person item in forCloning)
            {
                Person cloned = new Person();

                cloned.PersonID = item.PersonID;
                cloned.FirstName = item.FirstName;
                cloned.SurName = item.SurName;
                cloned.LastName = item.LastName;
                cloned.Division = item.Division;
                cloned.Occupation = item.Occupation;
                cloned.ProjectsWith = null;
                cloned.GoalsWith = null;

                outList.Add(cloned);
            }

            return outList;

        }
    }
}

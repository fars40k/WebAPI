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

        // GET: api/personel
        [HttpGet]
        public JsonResult Get()
        {
            var list = _dataObject.Personel.FindAll();

            if (list.Count() == 0) return new JsonResult(null);

            foreach (Person item in list)
            {
                TrimPersonData(item);
            }

            return new JsonResult(list);
        }

        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            try
            {
                var person = _dataObject.Personel.FindByID(Guid.Parse(id));

                if (person == null) return new JsonResult(null);

                TrimPersonData(person);

                return new JsonResult(person);
            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }

        [HttpPost]
        public ActionResult Update(Person newPerson)
        {
            try
            {
                if (_dataObject.Personel.FindByID(newPerson.PersonID) != null)
                {
                    // Updating existed entry

                    _dataObject.Personel.Update(newPerson);

                }
                else
                {
                    // Adding new entry

                    _dataObject.Personel.Insert(newPerson);

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
                if (_dataObject.Personel.FindByID(Guid.Parse(id)) != null)
                {
                    _dataObject.LinkedData.ClearLinksForPerson(Guid.Parse(id));
                    _dataObject.Personel.Delete(Guid.Parse(id));
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
                    item.ProjectsWith = null;
                    item.GoalsWith = null;
                }

                return new JsonResult(list);
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
                    item.ProjectsWith = null;
                    item.GoalsWith = null;
                }

                return new JsonResult(list);

            }
            catch
            {

                return new JsonResult(null) { StatusCode = 400 };

            }
        }


        private void TrimPersonData(Person obj)
        {
            obj.FirstName = obj.FirstName.TrimEnd();
            obj.SurName = obj.SurName.Trim();
            obj.LastName = obj.LastName.Trim();
            obj.Division = obj.Division.Trim();
            obj.Occupation = obj.Occupation.Trim();
        }
    }
}

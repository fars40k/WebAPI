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

            foreach(Person item in list)
            {
                TrimPersonData(item);
            }

            JsonResult jsonResult = new JsonResult(list);

            return jsonResult;
        }

        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            Person person = _dataObject.Personel.FindByID(Guid.Parse(id));

            TrimPersonData(person);

            return new JsonResult(person);
        }

        [HttpPost]
        public void Update(Person newPerson)
        {
            if (_dataObject.Personel.FindByID(newPerson.PersonID) != null)
            {
                // Updating existed entry

                _dataObject.Personel.Update(newPerson);

            } else
            {
                // Adding new entry

                _dataObject.Personel.Insert(newPerson);

            }
        }

        [HttpDelete]
        public void Delete(string id)
        {
            if (_dataObject.Personel.FindByID(Guid.Parse(id)) != null)
            {
                _dataObject.LinkedData.ClearLinksForPerson(Guid.Parse(id));
                _dataObject.Personel.Delete(Guid.Parse(id));
            }
        }

        [Route("ForProject/{id}")]
        [HttpGet]
        public JsonResult GetForProject(string id)
        {
            var list = _dataObject.LinkedData.FindPersonelForProject(Guid.Parse(id));

            foreach (Person item in list)
            {
                TrimPersonData(item);
            }

            JsonResult jsonResult = new JsonResult(list);

            return jsonResult;
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

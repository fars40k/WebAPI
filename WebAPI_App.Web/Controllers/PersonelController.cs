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
        private WinTaskContext _winTaskContext;

        public PersonelController(WinTaskContext context)
        {
            _winTaskContext = context;
        }

        // GET: api/personel
        [HttpGet]
        public JsonResult Get()
        {
            var list = _winTaskContext.Personel.ToList<Person>();

            foreach(Person item in list)
            {
                item.FirstName.TrimEnd();
                item.SurName.TrimEnd();
                item.LastName.TrimEnd();
                item.Division.TrimEnd();
                item.Occupation.TrimEnd();
            }

            return new JsonResult(list);
        }


    }
}

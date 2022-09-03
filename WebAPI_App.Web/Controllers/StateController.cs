using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_App.Data;
using WebAPI_App.Data.Interfaces;

namespace WebAPI_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private DataAccessObject _dataObject;

        public StateController(DataAccessObject dataAccessObject)
        {

            _dataObject = dataAccessObject;

        }

        // GET: api/state
        [HttpGet]
        public bool GetServerState()
        {

            return _dataObject.DoesConnectedToDb();

        }

    }
}

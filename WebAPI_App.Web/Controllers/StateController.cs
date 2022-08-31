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
        public DatabaseWorker DatabaseWorker;

        public StateController(DatabaseWorker worker)
        {
            // Produses major bug (softlock)
            DatabaseWorker = worker;            
        }

        // GET: api/state
        [HttpGet]
        public bool GetServerState()
        {
            return DatabaseWorker.IsConnectionEstablished;
        }

    }
}

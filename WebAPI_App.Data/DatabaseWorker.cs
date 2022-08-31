using System;
using System.Threading.Tasks;
using System.Timers;
using WebAPI_App.Data.Interfaces;

namespace WebAPI_App.Data
{
    /// <summary>
    /// Contains database access logic
    /// </summary>
    public class DatabaseWorker : IDatabaseService
    {
        public DataAccessObject DataAccessObject { get; set; }

        public System.Timers.Timer UpdateTimer;

        public event Action<bool> StatusChangedEvent;
        public event Action<bool> TryUpdateEvent;
        public event Action UpdatedDataLoadedEvent;

        private bool _isConnectionEstablished;
        public bool IsConnectionEstablished
        {
            get => _isConnectionEstablished;
            private set
            {

                if ((StatusChangedEvent != null)&&(value != _isConnectionEstablished)) StatusChangedEvent.Invoke(value);
                _isConnectionEstablished = value;
                
            }
        }

        public DatabaseWorker()
        {
            using (WinTaskContext wtContext = new WinTaskContext())
            {
                wtContext.Database.CreateIfNotExists();
                wtContext.Database.Connection.Open();

                DataAccessObject.UpdateContextInRepositories();
                DataAccessObject.UpdateEntityModel();

                IsConnectionEstablished = true;
            };
        }

        
    }
}

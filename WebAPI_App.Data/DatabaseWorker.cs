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
                            
        }

        public void DatabaseWorkerInit(DataAccessObject newDataAccessObject)
        {
            DataAccessObject = (DataAccessObject == null) ? newDataAccessObject 
                                                          : DataAccessObject;

            IsConnectionEstablished = false;
        }

        public void ConnectionInit()
        {

            Task.Factory.StartNew(() =>
            {
                try
                {
                    
                    UpdateTimer = new System.Timers.Timer(5000);
                    UpdateTimer.AutoReset = true;
                    UpdateTimer.Elapsed += delegate
                    {
                        throw new ArgumentException();
                    };
                  
                    using (WinTaskContext wtContext = new WinTaskContext())
                    {
                        wtContext.Database.CreateIfNotExists();
                        wtContext.Database.Connection.Open();

                        DataAccessObject.UpdateContextInRepositories();
                        DataAccessObject.UpdateEntityModel();

                        IsConnectionEstablished = true;                       
                    };

                    UpdateTimer.Stop();

                    CreateContiniousUpdatingTask();
                }
                catch (ArgumentException)
                {

                    if (StatusChangedEvent != null) StatusChangedEvent.Invoke(IsConnectionEstablished);
                    if (TryUpdateEvent != null) TryUpdateEvent.Invoke(IsConnectionEstablished);
                
                }
                catch (Exception)
                {
                    IsConnectionEstablished = false;
                }
            });
        }


        /// <summary>
        /// Creates task which every 10 sec task calls data from the database.
        /// </summary>
        public void CreateContiniousUpdatingTask()
        {
            // Every 10 sec task calls data from db 

            UpdateTimer = new System.Timers.Timer(10000);
            UpdateTimer.AutoReset = true;
            UpdateTimer.Elapsed += UpdateTimerElapsedAsync;
            UpdateTimer.Start();
        }

        /// <summary>
        /// Elapse timer connection check event handler.
        /// </summary>
        private void UpdateTimerElapsedAsync(object sender, ElapsedEventArgs e)
        {          
            Task.Factory.StartNew(() =>
            {
                if (TryUpdateEvent != null) TryUpdateEvent.Invoke(IsConnectionEstablished);

                try
                {
                    System.Timers.Timer timerShort = new System.Timers.Timer(2000);
                    timerShort.Elapsed += delegate
                    {
                        throw new InvalidOperationException();
                    };

                    DataAccessObject.UpdateEntityModel();

                    IsConnectionEstablished = true;

                    timerShort.Dispose();

                    if (UpdatedDataLoadedEvent != null) UpdatedDataLoadedEvent.Invoke();

                }
                catch (Exception ex)
                {

                    IsConnectionEstablished = false;
                    UpdateTimer.Stop();
                    UpdateTimer.Start();

                }
            });
        }

        private void ApplicationCloseRequested()
        {
            UpdateTimer.Dispose();
        }

        public void SaveChanges()
        {
            using (WinTaskContext wtContext = new WinTaskContext())
            {
                wtContext.SaveChanges();
            }
        }
    }
}

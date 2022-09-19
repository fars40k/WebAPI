using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_App.Data.Dto
{
    class GoalsToProjects
    {
        public Guid GoalID { get; set; }
        public Guid ProjectID { get; set; }

        public virtual Goal Goal { get; set; }
        public virtual Project Project { get; set; }
    }
}

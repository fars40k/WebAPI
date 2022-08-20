using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_App.Data
{

    [Table("Personel")]
    public partial class Person
    {
        [Key]
        public Guid PersonID { get; set; }

        [StringLength(30)]
        public string FirstName { get; set; }

        [StringLength(30)]
        public string SurName { get; set; }

        [StringLength(30)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Division { get; set; }

        [StringLength(30)]
        public string Occupation { get; set; }

        public ICollection<Project> ProjectsWith { get; set; }
        public ICollection<Goal> GoalsWith { get; set; }

        public Person()
        {
            ProjectsWith = new List<Project>();
            GoalsWith = new List<Goal>();
        }
    }
}

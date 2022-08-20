using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_App.Data
{

    public partial class Goal
    {
        public Guid GoalID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(4000)]
        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ExpireDate { get; set; }

        public byte Percentage { get; set; }

        public int Priority { get; set; }

        public int StatusKey { get; set; }

        public ICollection<Project> ProjectsWith { get; set; }
        public ICollection<Person> PersonelWith { get; set; }

        public Goal()
        {
            PersonelWith = new List<Person>();
            ProjectsWith = new List<Project>();
        }
        
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.Models
{

    public class ConcentrationChange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public double Change { get; set; }
        public double Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
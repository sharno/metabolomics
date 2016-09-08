using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabol.DbModels.Models;

namespace Metabol.DbModels.ViewModels
{
    public class AnalysisViewModel
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public ConcentrationChange[] ConcentrationChanges { get; set; }

    }
}

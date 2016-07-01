using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AnalysisModels
    {
        [Key]
        public string SessionKey { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        [Required]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
     

        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<IterationModels> Iterations { get; set; }
        public virtual ICollection<ConcentrationChange> ConcentrationChanges { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}
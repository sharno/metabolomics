using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Metabol.Api.Models;

namespace Metabol.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AnalysisModels
    {
        /// <summary>
        /// 
        /// </summary> 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AnalysisId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LastIteration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SessionKey { get; set; }

        /// <summary>
        /// 
        /// </summary>

        public virtual ApplicationUser User { get; set; }

    }
}
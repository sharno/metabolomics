using Metabol.DbModels.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Metabol.Api.Models
{
    public class SubsystemViewModel
    {
    }

    public class ConnectedSubsystemRawViewModel
    {
        public string Subsystem{ get; set; }
        public string SbmlId { get; set; }
        public string Name { get; set; }
    }

    public class ConnectedSubsystemViewModel
    {
        public string Subsystem { get; set; }

        public List<ConnectedSubsystemItemViewModel> Metabolites { get; set; }
    }


    public class ConnectedSubsystemItemViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
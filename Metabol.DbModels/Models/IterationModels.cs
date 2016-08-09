using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace Metabol.DbModels.Models
{
    public class IterationModels
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        private static IterationModels _empty;
        [NotMapped]
        public static IterationModels Empty => _empty ?? (_empty = new IterationModels(-1));

        public int IterantionNumber { get; set; }
        public int Fba { get; set; }
        public double Time { get; set; }

        [NotMapped]
        public IDictionary<string, double> Fluxes { get; set; }
        [NotMapped]
        public IEnumerable<string> Constraints { get; set; }
        [NotMapped]
        public IEnumerable<dynamic> Nodes { get; set; }
        [NotMapped]
        public IEnumerable<dynamic> Links { get; set; }

        [JsonIgnore]
        public string FluxesJson
        {
            get { return JsonConvert.SerializeObject(Fluxes); }
            set { Fluxes = JsonConvert.DeserializeObject<IDictionary<string, double>>(value); }
        }

        [JsonIgnore]
        public string ConstraintsJson
        {
            get { return JsonConvert.SerializeObject(Constraints); }
            set { Constraints = JsonConvert.DeserializeObject<IEnumerable<string>>(value); }
        }

        [JsonIgnore]
        public string NodesJson
        {
            get { return JsonConvert.SerializeObject(Nodes); }
            set { Nodes = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(value); }
        }

        [JsonIgnore]
        public string LinksJson
        {
            get { return JsonConvert.SerializeObject(Links); }
            set { Links = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(value); }
        }

        //[NotMapped]
        //public HyperGraph MetabolicNetwork { get; set; }

        //public string MetabolicNetworkJson
        //{
        //    get { return JsonConvert.SerializeObject(MetabolicNetwork); }
        //    set { MetabolicNetwork = JsonConvert.DeserializeObject<HyperGraph>(value); }
        //}

        public IterationModels()
        {

        }

        public IterationModels(int iteration)
        {
            IterantionNumber = iteration;
            Constraints = new List<string>();
        }
    }
}

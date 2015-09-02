namespace MetabolApi.Hubs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Metabol;

    using Microsoft.AspNet.SignalR;

    using Newtonsoft.Json;

    public class FbaHub : Hub
    {
        public void Init()
        {
            var model = new TheAlgorithm();
            model.Start();
            var sm = JsonConvert.SerializeObject(
                model,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            Clients.Caller.init(sm);
        }

        public void Init(MetaboliteChangeModel init)
        {
            var model = new TheAlgorithm();
            model.Init(init.MetaboliteChange);
            Clients.Caller.init(model);
        }

        public void Step(TheAlgorithm model)
        {
            Task.Run(() =>
                {
                    model.Step();
                    var sm = JsonConvert.SerializeObject(
                        model,
                        new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    Clients.Caller.updateGraph(sm);
                });
        }
    }

    public class MetaboliteChangeModel
    {
        [JsonProperty("change")]
        public Dictionary<string, int> MetaboliteChange { get; set; }
    }

}
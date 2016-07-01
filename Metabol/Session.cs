using System;

namespace Metabol
{
    public partial class Session 
    {
        //public string Id;

        public TheAlgorithm2 Worker;

        public Session(string id)
        {
            //Id = id;
            Worker = new TheAlgorithm2();
            //Init();
        }

        public void Init()
        {
        
        }

        public void Dispose()
        {
            //Worker.Stop();
        }

      
    }
}
using System;
using EngineIO;

namespace Controllers.Scenes
{
    class SupervisoryControl
    {
        //private readonly McStatus mc0Status;
        //private readonly McStatus mc1Status;
        //private readonly EventsMc0 eventsMc0;
        //private readonly EventsMc1 eventsMc1;

        public SupervisoryControl()//(McStatus mc0Status, McStatus mc1Status, EventsMc0 eventsMc0, EventsMc1 eventsMc1)
        {
            //this.mc0Status = mc0Status;
            //this.mc1Status = mc1Status;
            //this.eventsMc0 = eventsMc0;
            //this.eventsMc1 = eventsMc1;
            
        }

        public bool On(McStatus mc0Status, McStatus mc1Status)
        {
            Console.WriteLine("This is printed from SupervisoryControl Class. mc0Status = " + mc0Status);
            Console.WriteLine("This is printed from SupervisoryControl Class. mc1Status = " + mc1Status);
            return (true);
        }




    }
}

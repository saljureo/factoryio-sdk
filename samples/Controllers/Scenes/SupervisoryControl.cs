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
        int stateCode;
        int previousStateCode;
        SupervisorState supervisorState;
        SupervisorState previousState;
        public enum SupervisorState
        {
            OK_E_i1_i2,
            OK_E_w1_i2,
            OK_F_i1_i2
        }
        public SupervisoryControl()//(McStatus mc0Status, McStatus mc1Status, EventsMc0 eventsMc0, EventsMc1 eventsMc1)
        {
            //this.mc0Status = mc0Status;
            //this.mc1Status = mc1Status;
            //this.eventsMc0 = eventsMc0;
            //this.eventsMc1 = eventsMc1;
            supervisorState = SupervisorState.OK_E_i1_i2;//Initial State
            previousState = SupervisorState.OK_E_w1_i2;
        }

        public bool On(McStatus mc1Status, McStatus mc2Status, BufferStatus bufferStatus, BreakdownM2 breakdownM2,
            EventsMc1 eventsMc1, EventsMc2 eventsMc2)
        {
            if (mc1Status == McStatus.IDLE && mc2Status == McStatus.IDLE && 
                bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.OK)//stateCode = 0
            {
                supervisorState = SupervisorState.OK_E_i1_i2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }
                if (eventsMc1 == EventsMc1.s1)
                {
                    return (true);
                }
                else if (eventsMc1 == EventsMc1.r1)
                {
                    return (false);
                }
            }
            else if (mc1Status == McStatus.WORKING && mc2Status == McStatus.IDLE &&
                bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.OK)//stateCode = 1
            {
                supervisorState = SupervisorState.OK_E_w1_i2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }
                if (eventsMc1 == EventsMc1.s1)
                {
                    Console.WriteLine("Event " + eventsMc1 + " approved.");
                    return (true);
                }
                else if (eventsMc1 == EventsMc1.r1)
                {
                    return (false);
                }
            }
            return (false);
        }




    }
}

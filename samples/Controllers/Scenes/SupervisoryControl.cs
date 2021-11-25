using System;
using EngineIO;

namespace Controllers.Scenes
{
    class SupervisoryControl
    {
        SupervisorState supervisorState;
        SupervisorState previousState;
        public enum SupervisorState
        {
            OK_E_i1_i2,
            OK_E_w1_i2,
            OK_F_i1_i2
        }
        public SupervisoryControl()
        {
            supervisorState = SupervisorState.OK_E_i1_i2;//Initial State
            previousState = SupervisorState.OK_E_w1_i2;
        }

        public bool On(McStatus mc1Status, McStatus mc2Status, BufferStatus bufferStatus, BreakdownM2 breakdownM2,
            Events eventsMc1, Events eventsMc2)
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
                if (eventsMc1 == Events.s1)
                {
                    Console.WriteLine("Event " + eventsMc1 + " approved.");
                    return (true);
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
            }
            else if (mc1Status == McStatus.WORKING && mc2Status == McStatus.IDLE &&
                bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.OK)//stateCode = 2
            {
                supervisorState = SupervisorState.OK_F_i1_i2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }

                if (eventsMc1 == Events.s1)
                {
                    Console.WriteLine("Event " + eventsMc1 + " forbidden.");
                    return (false);
                }
                else if (eventsMc2 == Events.s2)
                {

                }
            }
            return (false);
        }




    }
}

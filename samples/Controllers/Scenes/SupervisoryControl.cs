using System;
using EngineIO;
using System.Threading;

namespace Controllers.Scenes
{
    class SupervisoryControl
    {
        SupervisorState supervisorState;
        SupervisorState previousState;

        public enum SupervisorState
        {
            OK_E_i1_i2,//1
            OK_E_w1_i2,//2
            OK_F_i1_i2,//3
            OK_E_i1_w2,//4
            KO_E_i1_d2,//5
            OK_E_d1_i2,//6
            OK_E_d1_w2,//7
            KO_E_d1_d2,//8
            KO_E_w1_d2,//9
            KO_F_i1_d2,//10
            OK_F_i1_w2,//11
            OK_E_w1_w2//12
        }
        public SupervisoryControl()
        {
            supervisorState = SupervisorState.OK_E_i1_i2;//Initial State
            previousState = SupervisorState.OK_E_w1_i2;
        }

        public bool On(McStatus mc1Status, McStatus mc2Status, BufferStatus bufferStatus, BreakdownM2 breakdownM2,
            Events eventsMc)
        {
            if (mc1Status == McStatus.IDLE && mc2Status == McStatus.IDLE && 
                bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.OK)//stateCode = 1 OK_E_i1_i2
            {
                supervisorState = SupervisorState.OK_E_i1_i2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }
                if (eventsMc == Events.s1)
                {
                    Console.WriteLine("Event " + eventsMc + " approved.");
                    return (true);
                }
            }
            else if (mc1Status == McStatus.WORKING && mc2Status == McStatus.IDLE &&
                bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.OK)//stateCode = 2 OK_E_w1_i2
            {
                supervisorState = SupervisorState.OK_E_w1_i2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }
            }
            else if (mc1Status == McStatus.IDLE && mc2Status == McStatus.IDLE &&
                bufferStatus == BufferStatus.FULL && breakdownM2 == BreakdownM2.OK)//stateCode = 3 OK_F_i1_i2
            {
                supervisorState = SupervisorState.OK_F_i1_i2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }

                if (eventsMc == Events.s1)
                {
                    Console.WriteLine("Event " + eventsMc + " blocked.");
                    Thread.Sleep(800);
                    return (false);
                }
                else if (eventsMc == Events.s2)
                {
                    Console.WriteLine("Here?");
                    Console.WriteLine("Event " + eventsMc + " approved.");
                    return (true);
                }
            }
            else if (mc1Status == McStatus.IDLE && mc2Status == McStatus.WORKING &&
               bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.OK)//stateCode = 4 OK_E_i1_w2
            {
                supervisorState = SupervisorState.OK_E_i1_w2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }

                if (eventsMc == Events.s1)
                {
                    Console.WriteLine("Event " + eventsMc + " approved.");
                    return (true);
                }
            }
            else if (mc1Status == McStatus.IDLE && mc2Status == McStatus.DOWN &&
               bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.KO)//stateCode = 5 KO_E_i1_d2
            {
                supervisorState = SupervisorState.KO_E_i1_d2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }
                
                if (eventsMc == Events.s1)
                {
                    Console.WriteLine("Event " + eventsMc + " approved.");
                    return (true);
                }
                else if (eventsMc == Events.r2)
                {
                    Console.WriteLine("Event " + eventsMc + " approved.");
                    return (true);
                }
            }
            else if (mc1Status == McStatus.DOWN && mc2Status == McStatus.IDLE &&
               bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.OK)//stateCode = 6 OK_E_d1_i2
            {
                supervisorState = SupervisorState.OK_E_d1_i2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }

                if (eventsMc == Events.r1)
                {
                    Console.WriteLine("Event " + eventsMc + " approved.");
                    return (true);
                }
            }
            else if (mc1Status == McStatus.DOWN && mc2Status == McStatus.WORKING &&
               bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.OK)//stateCode = 7 OK_E_d1_w2
            {
                supervisorState = SupervisorState.OK_E_d1_w2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }

                if (eventsMc == Events.r1)
                {
                    Console.WriteLine("Event " + eventsMc + " approved.");
                    return (true);
                }
            }
            else if (mc1Status == McStatus.DOWN && mc2Status == McStatus.DOWN &&
               bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.KO)//stateCode = 8 KO_E_d1_d2
            {
                supervisorState = SupervisorState.KO_E_d1_d2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }

                if (eventsMc == Events.r2)
                {
                    Console.WriteLine("Event " + eventsMc + " approved.");
                    return (true);
                }
            }
            else if (mc1Status == McStatus.WORKING && mc2Status == McStatus.DOWN &&
               bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.KO)//stateCode = 9 KO_E_w1_d2
            {
                supervisorState = SupervisorState.KO_E_w1_d2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }

                if (eventsMc == Events.r2)
                {
                    Console.WriteLine("Event " + eventsMc + " approved.");
                    return (true);
                }
            }
            else if (mc1Status == McStatus.IDLE && mc2Status == McStatus.DOWN &&
               bufferStatus == BufferStatus.FULL && breakdownM2 == BreakdownM2.KO)//stateCode = 10 KO_F_i1_d2
            {
                supervisorState = SupervisorState.KO_F_i1_d2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }

                if (eventsMc == Events.s1)
                {
                    Console.WriteLine("Event " + eventsMc + " blocked.");
                    Thread.Sleep(800);
                    return (false);
                }

                if (eventsMc == Events.r2)
                {
                    Console.WriteLine("Event " + eventsMc + " approved.");
                    return (true);
                }
            }
            else if (mc1Status == McStatus.IDLE && mc2Status == McStatus.WORKING &&
               bufferStatus == BufferStatus.FULL && breakdownM2 == BreakdownM2.OK)//stateCode = 11 OK_F_i1_w2
            {
                supervisorState = SupervisorState.OK_F_i1_w2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }

                if (eventsMc == Events.s1)
                {
                    Console.WriteLine("Event " + eventsMc + " blocked.");
                    Thread.Sleep(800);
                    return (false);
                }
            }
            else if (mc1Status == McStatus.WORKING && mc2Status == McStatus.WORKING &&
               bufferStatus == BufferStatus.EMPTY && breakdownM2 == BreakdownM2.OK)//stateCode = 12 OK_E_w1_w2
            {
                supervisorState = SupervisorState.OK_E_w1_w2;
                if (supervisorState != previousState)
                {
                    Console.WriteLine("State: " + supervisorState);
                    previousState = supervisorState;
                }
            }
            Console.WriteLine("This should NEVER appear. If it does, there's a state missing in SupervisoryControl.cs");
            return (false);
        }




    }
}

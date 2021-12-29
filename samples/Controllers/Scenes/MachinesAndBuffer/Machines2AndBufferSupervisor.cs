using System;
using System.Collections.Generic;
using System.Threading;

namespace Controllers.Scenes
{
    class Machines2AndBufferSupervisor
    {

        // #### VARIABLE CREATION TO ALLOCATE IN MEMORY ####
        private int currentState;
        private int evento;
        private Dictionary<(int, int), int> transiciones;
        private Dictionary<string, int> eventLabels;

        private Dictionary<int, string> stateLabels;

        public void CreateController()
        {
            transiciones = new Dictionary<(int, int), int>();
            eventLabels = new Dictionary<string, int>();
            stateLabels = new Dictionary<int, string>();

            currentState = 5;
            //#########  TRANSICIONES START ############

            transiciones.Add((0, 5), 1);
            transiciones.Add((1, 4), 5);
            transiciones.Add((2, 1), 0);
            transiciones.Add((2, 3), 1);
            transiciones.Add((2, 4), 7);
            transiciones.Add((3, 5), 5);
            transiciones.Add((3, 6), 9);
            transiciones.Add((4, 5), 6);
            transiciones.Add((5, 6), 10);
            transiciones.Add((6, 7), 7);
            transiciones.Add((7, 1), 3);
            transiciones.Add((7, 3), 5);
            transiciones.Add((7, 6), 11);
            transiciones.Add((8, 1), 4);
            transiciones.Add((8, 3), 6);
            transiciones.Add((9, 0), 0);
            transiciones.Add((9, 2), 4);
            transiciones.Add((9, 5), 10);
            transiciones.Add((10, 0), 1);
            transiciones.Add((10, 2), 6);
            transiciones.Add((11, 0), 2);
            transiciones.Add((11, 1), 9);
            transiciones.Add((11, 2), 8);
            transiciones.Add((11, 3), 10);

            //#########  TRANSICIONES END ############

            //#########  EVENTLABEL START ############

            eventLabels.Add("b1", 0);
            eventLabels.Add("b2", 1);
            eventLabels.Add("f1", 2);
            eventLabels.Add("f2", 3);
            eventLabels.Add("r1", 4);
            eventLabels.Add("r2", 5);
            eventLabels.Add("s1", 6);
            eventLabels.Add("s2", 7);

            //#########  EVENTLABEL END ############


            //#########  STATELABEL START ############

            stateLabels.Add(0, "d1.d2.EmptyB1.KO2");
            stateLabels.Add(1, "d1.i2.EmptyB1.OK2");
            stateLabels.Add(2, "d1.w2.EmptyB1.OK2");
            stateLabels.Add(3, "i1.d2.EmptyB1.KO2");
            stateLabels.Add(4, "i1.d2.FullB1.KO2");
            stateLabels.Add(5, "i1.i2.EmptyB1.OK2");
            stateLabels.Add(6, "i1.i2.FullB1.OK2");
            stateLabels.Add(7, "i1.w2.EmptyB1.OK2");
            stateLabels.Add(8, "i1.w2.FullB1.OK2");
            stateLabels.Add(9, "w1.d2.EmptyB1.KO2");
            stateLabels.Add(10, "w1.i2.EmptyB1.OK2");
            stateLabels.Add(11, "w1.w2.EmptyB1.OK2");

            //#########  STATELABEL END ############

            Console.WriteLine("\nCurrent state is: " + stateLabels[currentState] + "\n");
        }

        public bool On(string eventoLabel)
        {
            evento = eventLabels[eventoLabel];
            if (transiciones.ContainsKey((currentState, evento)))
            {
                currentState = transiciones[(currentState, evento)];
                if (evento != 0 && evento != 1 && evento != 2 && evento != 3)
                {
                    Console.WriteLine(eventoLabel + " event approved");
                }
                else
                {
                    Console.WriteLine(eventoLabel + " event is uncontrollable and must be enabled");
                }
                Console.WriteLine("Current state is: " + stateLabels[currentState] + "\n");
                return true;
            } else
            {
                Console.WriteLine(eventoLabel + " event blocked");
                Thread.Sleep(800);
                return false;
            }
        }
    }
}
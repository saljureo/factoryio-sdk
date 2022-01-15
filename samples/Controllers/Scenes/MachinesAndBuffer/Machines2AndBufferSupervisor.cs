using System;
using System.Collections.Generic;
using System.Threading;

namespace Controllers.Scenes.MachinesAndBuffer
{
    class Machines2AndBufferSupervisor
    {

        // #### VARIABLE CREATION TO ALLOCATE IN MEMORY ####
        private int currentState;
        private int evento;
        private Dictionary<(int, int), int> transiciones;
        private Dictionary<string, int> eventLabels;
        private Dictionary<int, (string, string)> eventLabelsInverse;
        private Dictionary<int, string> stateLabels;

        private bool activeEventsAvailable;

        public void CreateController()
        {
            transiciones = new Dictionary<(int, int), int>();
            eventLabels = new Dictionary<string, int>();
            eventLabelsInverse = new Dictionary<int, (string, string)>();
            stateLabels = new Dictionary<int, string>();
            activeEventsAvailable = false;

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

            eventLabelsInverse.Add(0, ("b1","nc"));
            eventLabelsInverse.Add(1, ("b2","nc"));
            eventLabelsInverse.Add(2, ("f1","nc"));
            eventLabelsInverse.Add(3, ("f2","nc"));
            eventLabelsInverse.Add(4, ("r1","c"));
            eventLabelsInverse.Add(5, ("r2","c"));
            eventLabelsInverse.Add(6, ("s1","c"));
            eventLabelsInverse.Add(7, ("s2","c"));

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
            Console.WriteLine("List of active events. Choose one and press enter: \n");
            for (int i = 0; i < eventLabels.Count; i++)
            {
                if (transiciones.ContainsKey((currentState, i)) && eventLabelsInverse[i].Item2 == "c")
                {
                    Console.WriteLine(i + ": " + eventLabelsInverse[i].Item1 + "\n");
                }
            }
            Console.WriteLine("Type event number and press enter to execute or press button on Factory I/O interface:\n");
        }

        public bool IsInActiveEvents(int newState)
        {
            if (transiciones.ContainsKey((currentState, newState)) && eventLabelsInverse[newState].Item2 == "c")
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }
        public void ListOfActiveEvents()
        {
            Console.WriteLine("\nList of active events. Choose one and press enter or wait:\n");

            for (int i = 0; i < eventLabels.Count; i++)
            {
                if (transiciones.ContainsKey((currentState, i)) && eventLabelsInverse[i].Item2 == "c")
                {
                    Console.WriteLine(i + ": " + eventLabelsInverse[i].Item1);
                }
            }
        }
        public string StateName(int eventNumber)
        {
            if (eventLabelsInverse.ContainsKey(eventNumber))
            {
                return (eventLabelsInverse[eventNumber].Item1);
            }
            else
            {
                Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                Console.WriteLine("\nEvent number pressed does not exist. Try again.\n");
                Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                ListOfActiveEvents();
                return ("Event number pressed does not exist");
            }
        }
        public bool On(string eventoLabel)
        {
            evento = eventLabels[eventoLabel];
            if (transiciones.ContainsKey((currentState, evento)))
            {
                currentState = transiciones[(currentState, evento)];
                if (evento != 0 && evento != 1 && evento != 2 && evento != 3)
                {
                    Console.WriteLine("oooooooooooooooooooooooooooooooooooooooo\n");
                    Console.WriteLine(eventoLabel + " event approved");
                    Console.WriteLine("Current state is: " + stateLabels[currentState]);
                    Console.WriteLine("\noooooooooooooooooooooooooooooooooooooooo");
                    ListOfActiveEvents();
                }
                else
                {
                    Console.WriteLine(eventoLabel + " event is uncontrollable and must be enabled");
                    Console.WriteLine("Current state is: " + stateLabels[currentState]);
                    ListOfActiveEvents();
                }
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
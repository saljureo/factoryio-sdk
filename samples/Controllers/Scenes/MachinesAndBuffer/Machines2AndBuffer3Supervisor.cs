using System;
using System.Collections.Generic;
using System.Threading;

namespace Controllers.Scenes
{
    class Machines2AndBuffer3Supervisor
    {

        // #### VARIABLE CREATION TO ALLOCATE IN MEMORY ####
        private int currentState;
        private int evento;
        private Dictionary<(int, int), int> transiciones;
        private Dictionary<string, int> eventLabels;

        private Dictionary<int, (string, string)> eventLabelsInverse;

        private Dictionary<int, string> stateLabels;

        public void CreateController()
        {
            transiciones = new Dictionary<(int, int), int>();
            eventLabels = new Dictionary<string, int>();
            eventLabelsInverse = new Dictionary<int, (string, string)>();
            stateLabels = new Dictionary<int, string>();

            currentState = 13;
            //#########  TRANSICIONES START ############

            transiciones.Add((0, 5), 3);
            transiciones.Add((1, 5), 4);
            transiciones.Add((2, 5), 5);
            transiciones.Add((3, 4), 13);
            transiciones.Add((4, 4), 14);
            transiciones.Add((4, 7), 6);
            transiciones.Add((5, 4), 16);
            transiciones.Add((5, 7), 7);
            transiciones.Add((6, 1), 0);
            transiciones.Add((6, 3), 3);
            transiciones.Add((6, 4), 17);
            transiciones.Add((7, 1), 1);
            transiciones.Add((7, 3), 4);
            transiciones.Add((7, 4), 18);
            transiciones.Add((8, 1), 2);
            transiciones.Add((8, 3), 5);
            transiciones.Add((8, 4), 20);
            transiciones.Add((9, 5), 13);
            transiciones.Add((9, 6), 21);
            transiciones.Add((10, 5), 14);
            transiciones.Add((10, 6), 22);
            transiciones.Add((11, 5), 15);
            transiciones.Add((12, 5), 16);
            transiciones.Add((12, 6), 23);
            transiciones.Add((13, 6), 24);
            transiciones.Add((14, 6), 25);
            transiciones.Add((14, 7), 17);
            transiciones.Add((15, 7), 20);
            transiciones.Add((16, 6), 26);
            transiciones.Add((16, 7), 18);
            transiciones.Add((17, 1), 9);
            transiciones.Add((17, 3), 13);
            transiciones.Add((17, 6), 27);
            transiciones.Add((18, 1), 10);
            transiciones.Add((18, 3), 14);
            transiciones.Add((18, 6), 28);
            transiciones.Add((19, 1), 11);
            transiciones.Add((19, 3), 15);
            transiciones.Add((20, 1), 12);
            transiciones.Add((20, 3), 16);
            transiciones.Add((20, 6), 29);
            transiciones.Add((21, 0), 0);
            transiciones.Add((21, 2), 10);
            transiciones.Add((21, 5), 24);
            transiciones.Add((22, 0), 1);
            transiciones.Add((22, 2), 12);
            transiciones.Add((22, 5), 25);
            transiciones.Add((23, 0), 2);
            transiciones.Add((23, 2), 11);
            transiciones.Add((23, 5), 26);
            transiciones.Add((24, 0), 3);
            transiciones.Add((24, 2), 14);
            transiciones.Add((25, 0), 4);
            transiciones.Add((25, 2), 16);
            transiciones.Add((25, 7), 27);
            transiciones.Add((26, 0), 5);
            transiciones.Add((26, 2), 15);
            transiciones.Add((26, 7), 28);
            transiciones.Add((27, 0), 6);
            transiciones.Add((27, 1), 21);
            transiciones.Add((27, 2), 18);
            transiciones.Add((27, 3), 24);
            transiciones.Add((28, 0), 7);
            transiciones.Add((28, 1), 22);
            transiciones.Add((28, 2), 20);
            transiciones.Add((28, 3), 25);
            transiciones.Add((29, 0), 8);
            transiciones.Add((29, 1), 23);
            transiciones.Add((29, 2), 19);
            transiciones.Add((29, 3), 26);

            //#########  TRANSICIONES END ############


            //#########  STATELABEL START ############

            stateLabels.Add(0, "d1.d2.EmptyB1.KO2");
            stateLabels.Add(1, "d1.d2.OneB1.KO2");
            stateLabels.Add(2, "d1.d2.TwoB1.KO2");
            stateLabels.Add(3, "d1.i2.EmptyB1.OK2");
            stateLabels.Add(4, "d1.i2.OneB1.OK2");
            stateLabels.Add(5, "d1.i2.TwoB1.OK2");
            stateLabels.Add(6, "d1.w2.EmptyB1.OK2");
            stateLabels.Add(7, "d1.w2.OneB1.OK2");
            stateLabels.Add(8, "d1.w2.TwoB1.OK2");
            stateLabels.Add(9, "i1.d2.EmptyB1.KO2");
            stateLabels.Add(10, "i1.d2.OneB1.KO2");
            stateLabels.Add(11, "i1.d2.ThreeB1.KO2");
            stateLabels.Add(12, "i1.d2.TwoB1.KO2");
            stateLabels.Add(13, "i1.i2.EmptyB1.OK2");
            stateLabels.Add(14, "i1.i2.OneB1.OK2");
            stateLabels.Add(15, "i1.i2.ThreeB1.OK2");
            stateLabels.Add(16, "i1.i2.TwoB1.OK2");
            stateLabels.Add(17, "i1.w2.EmptyB1.OK2");
            stateLabels.Add(18, "i1.w2.OneB1.OK2");
            stateLabels.Add(19, "i1.w2.ThreeB1.OK2");
            stateLabels.Add(20, "i1.w2.TwoB1.OK2");
            stateLabels.Add(21, "w1.d2.EmptyB1.KO2");
            stateLabels.Add(22, "w1.d2.OneB1.KO2");
            stateLabels.Add(23, "w1.d2.TwoB1.KO2");
            stateLabels.Add(24, "w1.i2.EmptyB1.OK2");
            stateLabels.Add(25, "w1.i2.OneB1.OK2");
            stateLabels.Add(26, "w1.i2.TwoB1.OK2");
            stateLabels.Add(27, "w1.w2.EmptyB1.OK2");
            stateLabels.Add(28, "w1.w2.OneB1.OK2");
            stateLabels.Add(29, "w1.w2.TwoB1.OK2");

            //#########  STATELABEL END ############

            //#########  EVENTLABEL START ############

            eventLabels.Add("b1", 0);
            eventLabels.Add("b2", 1);
            eventLabels.Add("f1", 2);
            eventLabels.Add("f2", 3);
            eventLabels.Add("r1", 4);
            eventLabels.Add("r2", 5);
            eventLabels.Add("s1", 6);
            eventLabels.Add("s2", 7);

            eventLabelsInverse.Add(0, ("b1", "nc"));
            eventLabelsInverse.Add(1, ("b2", "nc"));
            eventLabelsInverse.Add(2, ("f1", "nc"));
            eventLabelsInverse.Add(3, ("f2", "nc"));
            eventLabelsInverse.Add(4, ("r1", "c"));
            eventLabelsInverse.Add(5, ("r2", "c"));
            eventLabelsInverse.Add(6, ("s1", "c"));
            eventLabelsInverse.Add(7, ("s2", "c"));

            //#########  EVENTLABEL END ############

            Console.WriteLine("\nCurrent state is: " + stateLabels[currentState] + "\n");
            Console.WriteLine("List of active events. Choose one and press enter: \n");
            for (int i = 0; i < eventLabels.Count; i++)
            {
                if (transiciones.ContainsKey((currentState, i)) && eventLabelsInverse[i].Item2 == "c")
                {
                    Console.WriteLine(i + ": " + eventLabelsInverse[i].Item1 + "\n");
                }
            }
            Console.WriteLine("Type event number and press enter to execute or press button on Factory I/O interface:\n");        }

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
            Console.WriteLine("----------------------------------------\n");
            Console.WriteLine("\nList of active events. Choose one and press enter or wait:\n");

            for (int i = 0; i < eventLabels.Count; i++)
            {
                if (transiciones.ContainsKey((currentState, i)) && eventLabelsInverse[i].Item2 == "c")
                {
                    Console.WriteLine(i + ": " + eventLabelsInverse[i].Item1);
                }
            }
            Console.WriteLine("\n----------------------------------------");
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
                if (evento != 2 && evento != 14 && evento != 15 && evento != 16 && evento != 17 && evento != 18 && evento != 19 && evento != 20 && evento != 21 && evento != 22 && evento != 0 && evento != 1 && evento != 2 && evento != 3 && evento != 4 && evento != 5 && evento != 0 && evento != 1 && evento != 2 && evento != 3 && evento != 4 && evento != 5 && evento != 0 && evento != 1 && evento != 2 && evento != 3)
                {
                    Console.WriteLine("oooooooooooooooooooooooooooooooooooooooo\n");
                    Console.WriteLine(eventoLabel + " event approved");
                    Console.WriteLine("Current state is: " + stateLabels[currentState]);
                    Console.WriteLine("\noooooooooooooooooooooooooooooooooooooooo");
                    ListOfActiveEvents();
                }
                else
                {
                    Console.WriteLine("oooooooooooooooooooooooooooooooooooooooo\n");
                    Console.WriteLine(eventoLabel + " event is uncontrollable and must be enabled");
                    Console.WriteLine("Current state is: " + stateLabels[currentState]);
                    Console.WriteLine("\noooooooooooooooooooooooooooooooooooooooo");
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
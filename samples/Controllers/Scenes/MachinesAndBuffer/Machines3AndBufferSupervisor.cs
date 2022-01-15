using System;
using System.Collections.Generic;
using System.Threading;

namespace Controllers.Scenes
{
    class Machines3AndBufferSupervisor
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

            currentState = 22;
            //#########  TRANSICIONES START ############

            transiciones.Add((0, 8), 1);
            transiciones.Add((1, 7), 5);
            transiciones.Add((2, 2), 0);
            transiciones.Add((2, 5), 1);
            transiciones.Add((2, 7), 7);
            transiciones.Add((3, 8), 5);
            transiciones.Add((4, 8), 6);
            transiciones.Add((5, 6), 22);
            transiciones.Add((6, 6), 23);
            transiciones.Add((6, 11), 7);
            transiciones.Add((7, 2), 3);
            transiciones.Add((7, 5), 5);
            transiciones.Add((7, 6), 26);
            transiciones.Add((8, 2), 4);
            transiciones.Add((8, 5), 6);
            transiciones.Add((8, 6), 27);
            transiciones.Add((9, 1), 0);
            transiciones.Add((9, 4), 4);
            transiciones.Add((9, 8), 10);
            transiciones.Add((10, 1), 1);
            transiciones.Add((10, 4), 6);
            transiciones.Add((10, 6), 32);
            transiciones.Add((11, 1), 2);
            transiciones.Add((11, 2), 9);
            transiciones.Add((11, 4), 8);
            transiciones.Add((11, 5), 10);
            transiciones.Add((11, 6), 34);
            transiciones.Add((12, 8), 14);
            transiciones.Add((12, 9), 36);
            transiciones.Add((13, 8), 15);
            transiciones.Add((14, 7), 22);
            transiciones.Add((14, 9), 37);
            transiciones.Add((15, 7), 24);
            transiciones.Add((16, 2), 12);
            transiciones.Add((16, 5), 14);
            transiciones.Add((16, 7), 26);
            transiciones.Add((16, 9), 38);
            transiciones.Add((17, 2), 13);
            transiciones.Add((17, 5), 15);
            transiciones.Add((17, 7), 28);
            transiciones.Add((18, 8), 22);
            transiciones.Add((18, 9), 39);
            transiciones.Add((19, 8), 23);
            transiciones.Add((19, 9), 40);
            transiciones.Add((20, 8), 24);
            transiciones.Add((20, 10), 30);
            transiciones.Add((21, 8), 25);
            transiciones.Add((22, 9), 41);
            transiciones.Add((23, 9), 42);
            transiciones.Add((23, 11), 26);
            transiciones.Add((24, 10), 32);
            transiciones.Add((25, 11), 28);
            transiciones.Add((26, 2), 18);
            transiciones.Add((26, 5), 22);
            transiciones.Add((26, 9), 43);
            transiciones.Add((27, 2), 19);
            transiciones.Add((27, 5), 23);
            transiciones.Add((27, 9), 44);
            transiciones.Add((28, 2), 20);
            transiciones.Add((28, 5), 24);
            transiciones.Add((28, 10), 34);
            transiciones.Add((29, 2), 21);
            transiciones.Add((29, 5), 25);
            transiciones.Add((30, 1), 12);
            transiciones.Add((30, 4), 19);
            transiciones.Add((30, 8), 32);
            transiciones.Add((30, 9), 45);
            transiciones.Add((31, 1), 13);
            transiciones.Add((31, 4), 21);
            transiciones.Add((31, 8), 33);
            transiciones.Add((32, 1), 14);
            transiciones.Add((32, 4), 23);
            transiciones.Add((32, 9), 46);
            transiciones.Add((33, 1), 15);
            transiciones.Add((33, 4), 25);
            transiciones.Add((34, 1), 16);
            transiciones.Add((34, 2), 30);
            transiciones.Add((34, 4), 27);
            transiciones.Add((34, 5), 32);
            transiciones.Add((34, 9), 47);
            transiciones.Add((35, 1), 17);
            transiciones.Add((35, 2), 31);
            transiciones.Add((35, 4), 29);
            transiciones.Add((35, 5), 33);
            transiciones.Add((36, 0), 0);
            transiciones.Add((36, 3), 13);
            transiciones.Add((36, 8), 37);
            transiciones.Add((37, 0), 1);
            transiciones.Add((37, 3), 15);
            transiciones.Add((37, 7), 41);
            transiciones.Add((38, 0), 2);
            transiciones.Add((38, 2), 36);
            transiciones.Add((38, 3), 17);
            transiciones.Add((38, 5), 37);
            transiciones.Add((38, 7), 43);
            transiciones.Add((39, 0), 3);
            transiciones.Add((39, 3), 20);
            transiciones.Add((39, 8), 41);
            transiciones.Add((40, 0), 4);
            transiciones.Add((40, 3), 21);
            transiciones.Add((40, 8), 42);
            transiciones.Add((41, 0), 5);
            transiciones.Add((41, 3), 24);
            transiciones.Add((42, 0), 6);
            transiciones.Add((42, 3), 25);
            transiciones.Add((42, 11), 43);
            transiciones.Add((43, 0), 7);
            transiciones.Add((43, 2), 39);
            transiciones.Add((43, 3), 28);
            transiciones.Add((43, 5), 41);
            transiciones.Add((44, 0), 8);
            transiciones.Add((44, 2), 40);
            transiciones.Add((44, 3), 29);
            transiciones.Add((44, 5), 42);
            transiciones.Add((45, 0), 9);
            transiciones.Add((45, 1), 36);
            transiciones.Add((45, 3), 31);
            transiciones.Add((45, 4), 40);
            transiciones.Add((45, 8), 46);
            transiciones.Add((46, 0), 10);
            transiciones.Add((46, 1), 37);
            transiciones.Add((46, 3), 33);
            transiciones.Add((46, 4), 42);
            transiciones.Add((47, 0), 11);
            transiciones.Add((47, 1), 38);
            transiciones.Add((47, 2), 45);
            transiciones.Add((47, 3), 35);
            transiciones.Add((47, 4), 44);
            transiciones.Add((47, 5), 46);

            //#########  TRANSICIONES END ############


            //#########  STATELABEL START ############

            stateLabels.Add(0, "d1.d2.d3.EmptyB1.EmptyB2.KO2.KO3");
            stateLabels.Add(1, "d1.d2.i3.EmptyB1.EmptyB2.KO2.OK3");
            stateLabels.Add(2, "d1.d2.w3.EmptyB1.EmptyB2.KO2.OK3");
            stateLabels.Add(3, "d1.i2.d3.EmptyB1.EmptyB2.OK2.KO3");
            stateLabels.Add(4, "d1.i2.d3.EmptyB1.FullB2.OK2.KO3");
            stateLabels.Add(5, "d1.i2.i3.EmptyB1.EmptyB2.OK2.OK3");
            stateLabels.Add(6, "d1.i2.i3.EmptyB1.FullB2.OK2.OK3");
            stateLabels.Add(7, "d1.i2.w3.EmptyB1.EmptyB2.OK2.OK3");
            stateLabels.Add(8, "d1.i2.w3.EmptyB1.FullB2.OK2.OK3");
            stateLabels.Add(9, "d1.w2.d3.EmptyB1.EmptyB2.OK2.KO3");
            stateLabels.Add(10, "d1.w2.i3.EmptyB1.EmptyB2.OK2.OK3");
            stateLabels.Add(11, "d1.w2.w3.EmptyB1.EmptyB2.OK2.OK3");
            stateLabels.Add(12, "i1.d2.d3.EmptyB1.EmptyB2.KO2.KO3");
            stateLabels.Add(13, "i1.d2.d3.FullB1.EmptyB2.KO2.KO3");
            stateLabels.Add(14, "i1.d2.i3.EmptyB1.EmptyB2.KO2.OK3");
            stateLabels.Add(15, "i1.d2.i3.FullB1.EmptyB2.KO2.OK3");
            stateLabels.Add(16, "i1.d2.w3.EmptyB1.EmptyB2.KO2.OK3");
            stateLabels.Add(17, "i1.d2.w3.FullB1.EmptyB2.KO2.OK3");
            stateLabels.Add(18, "i1.i2.d3.EmptyB1.EmptyB2.OK2.KO3");
            stateLabels.Add(19, "i1.i2.d3.EmptyB1.FullB2.OK2.KO3");
            stateLabels.Add(20, "i1.i2.d3.FullB1.EmptyB2.OK2.KO3");
            stateLabels.Add(21, "i1.i2.d3.FullB1.FullB2.OK2.KO3");
            stateLabels.Add(22, "i1.i2.i3.EmptyB1.EmptyB2.OK2.OK3");
            stateLabels.Add(23, "i1.i2.i3.EmptyB1.FullB2.OK2.OK3");
            stateLabels.Add(24, "i1.i2.i3.FullB1.EmptyB2.OK2.OK3");
            stateLabels.Add(25, "i1.i2.i3.FullB1.FullB2.OK2.OK3");
            stateLabels.Add(26, "i1.i2.w3.EmptyB1.EmptyB2.OK2.OK3");
            stateLabels.Add(27, "i1.i2.w3.EmptyB1.FullB2.OK2.OK3");
            stateLabels.Add(28, "i1.i2.w3.FullB1.EmptyB2.OK2.OK3");
            stateLabels.Add(29, "i1.i2.w3.FullB1.FullB2.OK2.OK3");
            stateLabels.Add(30, "i1.w2.d3.EmptyB1.EmptyB2.OK2.KO3");
            stateLabels.Add(31, "i1.w2.d3.FullB1.EmptyB2.OK2.KO3");
            stateLabels.Add(32, "i1.w2.i3.EmptyB1.EmptyB2.OK2.OK3");
            stateLabels.Add(33, "i1.w2.i3.FullB1.EmptyB2.OK2.OK3");
            stateLabels.Add(34, "i1.w2.w3.EmptyB1.EmptyB2.OK2.OK3");
            stateLabels.Add(35, "i1.w2.w3.FullB1.EmptyB2.OK2.OK3");
            stateLabels.Add(36, "w1.d2.d3.EmptyB1.EmptyB2.KO2.KO3");
            stateLabels.Add(37, "w1.d2.i3.EmptyB1.EmptyB2.KO2.OK3");
            stateLabels.Add(38, "w1.d2.w3.EmptyB1.EmptyB2.KO2.OK3");
            stateLabels.Add(39, "w1.i2.d3.EmptyB1.EmptyB2.OK2.KO3");
            stateLabels.Add(40, "w1.i2.d3.EmptyB1.FullB2.OK2.KO3");
            stateLabels.Add(41, "w1.i2.i3.EmptyB1.EmptyB2.OK2.OK3");
            stateLabels.Add(42, "w1.i2.i3.EmptyB1.FullB2.OK2.OK3");
            stateLabels.Add(43, "w1.i2.w3.EmptyB1.EmptyB2.OK2.OK3");
            stateLabels.Add(44, "w1.i2.w3.EmptyB1.FullB2.OK2.OK3");
            stateLabels.Add(45, "w1.w2.d3.EmptyB1.EmptyB2.OK2.KO3");
            stateLabels.Add(46, "w1.w2.i3.EmptyB1.EmptyB2.OK2.OK3");
            stateLabels.Add(47, "w1.w2.w3.EmptyB1.EmptyB2.OK2.OK3");

            //#########  STATELABEL END ############

            //#########  EVENTLABEL START ############

            eventLabels.Add("b1", 0);
            eventLabels.Add("b2", 1);
            eventLabels.Add("b3", 2);
            eventLabels.Add("f1", 3);
            eventLabels.Add("f2", 4);
            eventLabels.Add("f3", 5);
            eventLabels.Add("r1", 6);
            eventLabels.Add("r2", 7);
            eventLabels.Add("r3", 8);
            eventLabels.Add("s1", 9);
            eventLabels.Add("s2", 10);
            eventLabels.Add("s3", 11);

            eventLabelsInverse.Add(0, ("b1", "nc"));
            eventLabelsInverse.Add(1, ("b2", "nc"));
            eventLabelsInverse.Add(2, ("b3", "nc"));
            eventLabelsInverse.Add(3, ("f1", "nc"));
            eventLabelsInverse.Add(4, ("f2", "nc"));
            eventLabelsInverse.Add(5, ("f3", "nc"));
            eventLabelsInverse.Add(6, ("r1", "c"));
            eventLabelsInverse.Add(7, ("r2", "c"));
            eventLabelsInverse.Add(8, ("r3", "c"));
            eventLabelsInverse.Add(9, ("s1", "c"));
            eventLabelsInverse.Add(10, ("s2", "c"));
            eventLabelsInverse.Add(11, ("s3", "c"));

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
                if (evento != 2 && evento != 14 && evento != 15 && evento != 16 && evento != 17 && evento != 18 && evento != 19 && evento != 20 && evento != 21 && evento != 22 && evento != 0 && evento != 1 && evento != 2 && evento != 3 && evento != 4 && evento != 5 && evento != 0 && evento != 1 && evento != 2 && evento != 3 && evento != 4 && evento != 5)
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
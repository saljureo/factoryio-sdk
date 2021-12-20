using System;
using System.Collections.Generic;
using System.Threading;

namespace Controllers.Scenes
{
    class SupervisoryControl
    {
        bool booleano = true;
        private string currentState;
        private Dictionary<(string, string), string> diccionario;

        public void CreateController()
        {
            diccionario = new Dictionary<(string, string), string>();
            diccionario.Add(("OK_E_i1_i2", "s1"), "OK_E_w1_i2");//Controllable
            currentState = "OK_E_i1_i2";//Initial state
            Console.WriteLine("Current state is: " + currentState);
            diccionario.Add(("OK_E_w1_i2", "b1"), "OK_E_d1_i2");//Uncontrollable
            diccionario.Add(("OK_E_w1_i2", "f1"), "OK_F_i1_i2");//Uncontrollable
            diccionario.Add(("OK_F_i1_i2", "s2"), "OK_E_i1_w2");//Controllable
            diccionario.Add(("OK_E_i1_w2", "s1"), "OK_E_w1_w2");//Controllable
            diccionario.Add(("OK_E_i1_w2", "f2"), "OK_E_i1_i2");//Uncontrollable
            diccionario.Add(("OK_E_i1_w2", "b2"), "KO_E_i1_d2");//Uncontrollable
            diccionario.Add(("KO_E_i1_d2", "r2"), "OK_E_i1_i2");//Controllable
            diccionario.Add(("KO_E_i1_d2", "s1"), "KO_E_w1_d2");//Controllable
            diccionario.Add(("OK_E_d1_i2", "r1"), "OK_E_i1_i2");//Controllable
            diccionario.Add(("OK_E_d1_w2", "r1"), "OK_E_i1_w2");//Controllable
            diccionario.Add(("OK_E_d1_w2", "b2"), "KO_E_d1_d2");//Uncontrollable
            diccionario.Add(("OK_E_d1_w2", "f2"), "OK_E_d1_i2");//Uncontrollable
            diccionario.Add(("KO_E_d1_d2", "r2"), "OK_E_d1_i2");//Controllable
            diccionario.Add(("KO_E_w1_d2", "r2"), "OK_E_w1_i2");//Controllable
            diccionario.Add(("KO_E_w1_d2", "b1"), "KO_E_d1_d2");//Uncontrollable
            diccionario.Add(("KO_E_w1_d2", "f1"), "KO_F_i1_d2");//Uncontrollable
            diccionario.Add(("KO_F_i1_d2", "r2"), "OK_F_i1_i2");//Controllable
            diccionario.Add(("OK_F_i1_w2", "b2"), "KO_F_i1_d2");//Uncontrollable
            diccionario.Add(("OK_F_i1_w2", "f2"), "OK_F_i1_i2");//Uncontrollable
            diccionario.Add(("OK_E_w1_w2", "f1"), "OK_F_i1_w2");//Uncontrollable
            diccionario.Add(("OK_E_w1_w2", "b2"), "KO_E_w1_d2");//Uncontrollable
            diccionario.Add(("OK_E_w1_w2", "f2"), "OK_E_w1_i2");//Uncontrollable
            diccionario.Add(("OK_E_w1_w2", "b1"), "OK_E_d1_w2");//Uncontrollable
        }

        public bool On2(string evento)
        {
            
            if (diccionario.ContainsKey((currentState, evento)))
            {
                currentState = diccionario[(currentState, evento)];
                if (evento != "f1" && evento != "f2" && evento != "b1" && evento != "b2")
                {
                    Console.WriteLine(evento + " event approved");
                }
                else
                {
                    Console.WriteLine(evento + " event is uncontrollable and must be enabled");
                }
                Console.WriteLine("Current state is: " + currentState + "\n");
                return true;
            } else
            {
                Console.WriteLine(evento + " event blocked");
                Thread.Sleep(800);
                return false;
            }
        }
    }
}

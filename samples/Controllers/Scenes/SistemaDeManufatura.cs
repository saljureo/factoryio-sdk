using Controllers.Scenes;
using EngineIO;
using System;
using System.Diagnostics;

namespace Controllers
{
    public class SistemaDeManufatura : Controller
    {
        //ESTEIRAS E1 E2
        readonly MemoryBit conveyorE1;
        readonly MemoryBit sensorEndE1;
        readonly MemoryInt sensorColor;
        readonly MemoryBit stopbladeEndE1;
        //Emitter
        readonly MemoryBit sensorPreEmitter;
        RTRIG rtSensorPreEmitter;
        readonly MemoryBit emitter;
        //E2
        readonly MemoryBit sensorStartE2;
        RTRIG rtSensorStartE2;
        readonly MemoryBit conveyor0E2;
        readonly MemoryBit conveyor1E2;
        readonly MemoryBit conveyor2E2;
        readonly MemoryBit conveyor3E2;
        readonly MemoryBit conveyor4E2;
        readonly MemoryBit conveyor5E2;
        readonly MemoryBit conveyor6E2;
        readonly MemoryBit conveyor7E2;
        readonly MemoryBit conveyor8E2;
        readonly MemoryBit conveyor9E2;
        readonly MemoryBit conveyor10E2;
        readonly MemoryBit conveyor11E2;
        readonly MemoryBit stopbladeEndE2;
        private enum BufferE2
        {
            ZERO,
            ONE,
            TWO,
            THREE,
            FOUR,
            FIVE
        }
        BufferE2 bufferE2;

        //ROBÔ
        readonly MemoryFloat robo0X;
        readonly MemoryFloat robo0Y;
        readonly MemoryFloat robo0Z;
        readonly MemoryFloat robo0XPos;
        readonly MemoryFloat robo0YPos;
        readonly MemoryFloat robo0ZPos;
        readonly MemoryBit robo0Grab;
        readonly MemoryBit robo0ToE2start;
        readonly MemoryBit robo0Grabbed;
        
        private enum RoboSteps
        {
            IDLE,
            DOWN_FOR_PIECE,
            GRAB_PIECE,
            UP_WITH_PIECE,
            TO_E2,
            DOWN_WITH_PIECE,
            RELEASE_PIECE,
            UP_WITHOUT_PIECE
        }
        RoboSteps robo0Steps;

        //Messages only once
        bool messageOnlyOnce;
        public SistemaDeManufatura()
        {
            //ESTEIRAS E1 E2
            conveyorE1 = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 0", MemoryType.Output);
            conveyorE1.Value = true;
            sensorEndE1 = MemoryMap.Instance.GetBit("Diffuse Sensor 0", MemoryType.Input);
            sensorColor = MemoryMap.Instance.GetInt("Vision Sensor 0 (Value)", MemoryType.Input);
            stopbladeEndE1 = MemoryMap.Instance.GetBit("Stop Blade 1", MemoryType.Output);
            stopbladeEndE1.Value = true;
            //Emitter
            emitter = MemoryMap.Instance.GetBit("Emitter 0 (Emit)", MemoryType.Output);
            sensorPreEmitter = MemoryMap.Instance.GetBit("Diffuse Sensor 3", MemoryType.Input);
            rtSensorPreEmitter = new RTRIG();
            //E2
            sensorStartE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 1", MemoryType.Input);
            rtSensorStartE2 = new RTRIG();
            conveyor0E2 = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 0", MemoryType.Output);
            conveyor0E2.Value = false;
            conveyor1E2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 2 CW", MemoryType.Output);
            conveyor2E2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 3 CCW", MemoryType.Output);
            conveyor3E2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 4 CW", MemoryType.Output);
            conveyor4E2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 0 CW", MemoryType.Output);
            conveyor5E2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 5 CCW", MemoryType.Output);
            conveyor6E2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 6 CCW", MemoryType.Output);
            conveyor7E2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 7 CW", MemoryType.Output);
            conveyor8E2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 8 CW", MemoryType.Output);
            conveyor9E2 = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 2", MemoryType.Output);
            conveyor10E2 = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 1", MemoryType.Output);
            conveyor11E2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 1 CW", MemoryType.Output);
            bufferE2 = BufferE2.ZERO;
            stopbladeEndE2 = MemoryMap.Instance.GetBit("Stop Blade 0", MemoryType.Output);
            stopbladeEndE2.Value = true;

            //ROBÔ0
            robo0X = MemoryMap.Instance.GetFloat("Pick & Place 0 X Set Point (V)", MemoryType.Output);
            robo0Y = MemoryMap.Instance.GetFloat("Pick & Place 0 Y Set Point(V)", MemoryType.Output);
            robo0Z = MemoryMap.Instance.GetFloat("Pick & Place 0 Z Set Point (V)", MemoryType.Output);
            robo0XPos = MemoryMap.Instance.GetFloat("Pick & Place 0 X Position (V)", MemoryType.Input);
            robo0YPos = MemoryMap.Instance.GetFloat("Pick & Place 0 Y Position (V)", MemoryType.Input);
            robo0ZPos = MemoryMap.Instance.GetFloat("Pick & Place 0 Z Position (V)", MemoryType.Input);
            robo0X.Value = 0.9f;//0.9f
            robo0Y.Value = 0.7f;//0.7f
            robo0Y.Value = 0.0f;
            robo0Grab = MemoryMap.Instance.GetBit("Pick & Place 0 (Grab)", MemoryType.Output);
            robo0ToE2start = MemoryMap.Instance.GetBit("Start Button 0", MemoryType.Input);
            robo0Steps = RoboSteps.IDLE;
            robo0Grabbed = MemoryMap.Instance.GetBit("Pick & Place 0 (Box Detected)", MemoryType.Input);

            //Messages only once
            messageOnlyOnce = true;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            //Console.WriteLine("BufferE2 = " + bufferE2);
            Console.WriteLine("Color sensor = " + sensorColor.Value);
            //%%%%%%%%%%%%%%%%%%%% ESTEIRA START %%%%%%%%%%%%%%%%%%%%
            if (sensorEndE1.Value == true)
            {
                conveyorE1.Value = false;
                if (messageOnlyOnce)
                {
                    Console.WriteLine("Part number is: " + sensorColor.Value);
                    messageOnlyOnce = false;
                }
            }
            else
            {
                messageOnlyOnce = true;
            }

            //E2
            rtSensorStartE2.CLK(sensorStartE2.Value);
            rtSensorPreEmitter.CLK(sensorPreEmitter.Value);

            if (bufferE2 == BufferE2.ZERO)
            {
                conveyor0E2.Value = false;
                conveyor1E2.Value = false;
                conveyor2E2.Value = false;
                conveyor3E2.Value = false;
                conveyor4E2.Value = false;
                conveyor5E2.Value = false;
                conveyor6E2.Value = false;
                conveyor7E2.Value = false;
                conveyor8E2.Value = false;
                conveyor9E2.Value = false;
                conveyor10E2.Value = false;
                conveyor11E2.Value = false;
                if (rtSensorStartE2.Q == true)
                {
                    bufferE2 = BufferE2.ONE;
                }
            }
            else if (bufferE2 == BufferE2.ONE)
            {
                conveyor0E2.Value = true;
                conveyor1E2.Value = true;
                conveyor2E2.Value = true;
                conveyor3E2.Value = true;
                conveyor4E2.Value = true;
                conveyor5E2.Value = true;
                conveyor6E2.Value = true;
                conveyor7E2.Value = true;
                conveyor8E2.Value = true;
                conveyor9E2.Value = true;
                conveyor10E2.Value = true;
                conveyor11E2.Value = true;
                if (rtSensorStartE2.Q == true)
                {
                    bufferE2 = BufferE2.TWO;
                }
                if (rtSensorPreEmitter.Q == true)
                {
                    bufferE2 = BufferE2.ZERO;
                }
            }
            else if (bufferE2 == BufferE2.TWO)
            {
                if (rtSensorStartE2.Q == true)
                {
                    bufferE2 = BufferE2.THREE;
                }
                if (rtSensorPreEmitter.Q == true)
                {
                    bufferE2 = BufferE2.ONE;
                }
            }
            else if (bufferE2 == BufferE2.THREE)
            {
                if (rtSensorStartE2.Q == true)
                {
                    bufferE2 = BufferE2.FOUR;
                }
                if (rtSensorPreEmitter.Q == true)
                {
                    bufferE2 = BufferE2.TWO;
                }
            }
            else if (bufferE2 == BufferE2.FOUR)
            {
                if (rtSensorStartE2.Q == true)
                {
                    bufferE2 = BufferE2.FIVE;
                }
                if (rtSensorPreEmitter.Q == true)
                {
                    bufferE2 = BufferE2.THREE;
                }
            }
            else if (bufferE2 == BufferE2.FIVE)
            {
                if (rtSensorPreEmitter.Q == true)
                {
                    bufferE2 = BufferE2.FOUR;
                }
            }


            //%%%%%%%%%%%%%%%%%%%% ESTEIRA ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%% ROBÔ STARTS %%%%%%%%%%%%%%%%%%%%

            //Piece to E2
            if (robo0Steps == RoboSteps.IDLE)
            {
                robo0X.Value = 0.9f;
                robo0Y.Value = 0.7f;
                robo0Z.Value = 0.0f;
                if (robo0ToE2start.Value)
                {
                    robo0Steps = RoboSteps.DOWN_FOR_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.DOWN_FOR_PIECE)
            {
                robo0Z.Value = 8.5f;
                Console.WriteLine("robo0Grabbed = " + robo0Grabbed.Value);
                if (robo0ZPos.Value > 9.0f)
                {
                    robo0Steps = RoboSteps.GRAB_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.GRAB_PIECE)
            {
                robo0Grab.Value = true;
                if (robo0Grabbed.Value == true)
                {
                    robo0Steps = RoboSteps.UP_WITH_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.UP_WITH_PIECE)
            {
                robo0Z.Value = 4.0f;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.TO_E2;
                }
            }
            else if (robo0Steps == RoboSteps.TO_E2)
            {
                robo0X.Value = 9.25f;
                robo0Y.Value = 2.3f;
                if (robo0XPos.Value > 8.0f && robo0YPos.Value > 2.0f)
                {
                    robo0Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                robo0Z.Value = 8.4f;
                if (robo0ZPos.Value > 8.1f)
                {
                    robo0Steps = RoboSteps.RELEASE_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.RELEASE_PIECE)
            {
                robo0Grab.Value = false;
                robo0Steps = RoboSteps.UP_WITHOUT_PIECE;
            }
            else if (robo0Steps == RoboSteps.UP_WITHOUT_PIECE)
            {
                robo0Z.Value = 4.0f;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.IDLE;
                }
            }
            //%%%%%%%%%%%%%%%%%%%% ROBÔ ENDS %%%%%%%%%%%%%%%%%%%%
        }
    }
}

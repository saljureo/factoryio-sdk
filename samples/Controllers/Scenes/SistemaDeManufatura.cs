using Controllers.Scenes;
using EngineIO;
using System;

namespace Controllers
{
    public class SistemaDeManufatura : Controller
    {
        //ESTEIRAS E1 E2
        //E1
        readonly MemoryBit conveyorE1;
        readonly MemoryBit conveyorE1Start;
        readonly MemoryBit conveyorE1Stop;
        readonly MemoryBit sensorEndE1;
        readonly MemoryInt sensorColor;
        readonly MemoryBit stopbladeEndE1;
        private enum E1ConveyorState
        {
            EMITTING,
            EMITTED,
            GOING_TO_ROBO0,
            REACHED_ROBO0,
            E2_TO_E1,
        }
        E1ConveyorState e1ConveyorState;
        private enum E2toE1Steps
        {
            GOING_TO_E2,
            DOWN_LOOKING_FOR_PIECE,
            GRABBING_PIECE,
            UP_WITH_PIECE,
            GOING_TO_E1_FIRST_HALF,
            GOING_TO_E1_SECOND_HALF,
            DOWN_WITH_PIECE,
            UNGRABBING_PIECE,
            UP_WITHOUT_PIECE,
            UNROTATE_FIRST_HALF,
            UNROTATE_SECOND_HALF
        }
        E2toE1Steps e2toE1Steps;

        //Emitter
        readonly MemoryBit emitter;
        readonly MemoryBit sensorEmitter;
        //E2
        readonly MemoryBit sensorStartE2;
        RTRIG rtSensorStartE2;
        readonly MemoryBit conveyorStartE2;
        readonly MemoryBit conveyorFirstCornerE2;
        readonly MemoryBit conveyorMiddleE2;
        readonly MemoryBit conveyorSecondCornerE2;
        readonly MemoryBit conveyorEndE2;
        readonly MemoryBit sensorEndE2;
        FTRIG ftSensorEndE2;
        readonly MemoryBit sensorSecondSpotE2;
        readonly MemoryBit sensorThirdSpotE2;
        readonly MemoryBit sensorFourthSpotE2;
        private enum BufferE2
        {
            ZERO,
            ONE,
            TWO,
            THREE,
            FOUR,
        }
        BufferE2 bufferE2;
        //GripperArm
        readonly MemoryFloat armX;
        readonly MemoryFloat armXpos;
        readonly MemoryFloat armZ;
        readonly MemoryFloat armZpos;
        readonly MemoryBit armGrab;
        readonly MemoryBit armRotate;
        readonly MemoryBit armUnrotate;
        readonly MemoryBit armPieceRotate;
        readonly MemoryBit armRotating;
        readonly MemoryBit armPieceDetected;

        //ROBÔ
        SistemaDeManufatura_Robo0 robo0;
        MemoryBit startC1toB1;
        MemoryBit startC2toB1;
        MemoryBit startC2toB2;
        MemoryBit startC3toB3;
        MemoryBit startC1toE2;
        MemoryBit startC2toE2;
        MemoryBit startC3toE2;
        private enum Robo0State
        {
            IDLE,
            E1toE2,
            E1toB1,
            E1toB2,
            E1toB3
        }
        Robo0State robo0State;

        //Messages only once
        bool messageOnlyOnce;
        bool initialMessage;
        bool colorMessage;
        bool colorMessage2;
        public SistemaDeManufatura()
        {
            //ESTEIRAS E1 E2
            //E1
            conveyorE1 = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 2", MemoryType.Output);
            conveyorE1Start = MemoryMap.Instance.GetBit("Liga esteira E1", MemoryType.Input);
            conveyorE1Stop = MemoryMap.Instance.GetBit("Desliga esteira E1", MemoryType.Input);
            sensorEndE1 = MemoryMap.Instance.GetBit("Diffuse Sensor 0", MemoryType.Input);
            sensorColor = MemoryMap.Instance.GetInt("Vision Sensor 0 (Value)", MemoryType.Input);
            stopbladeEndE1 = MemoryMap.Instance.GetBit("Stop Blade 1", MemoryType.Output);
            stopbladeEndE1.Value = true;
            e1ConveyorState = E1ConveyorState.EMITTING;
            e2toE1Steps = E2toE1Steps.GOING_TO_E2;

            //Emitter
            emitter = MemoryMap.Instance.GetBit("Emitter 0 (Emit)", MemoryType.Output);
            sensorEmitter = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);

            //E2
            sensorStartE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 1", MemoryType.Input);
            rtSensorStartE2 = new RTRIG();
            conveyorStartE2 = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 0", MemoryType.Output);
            conveyorFirstCornerE2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 0 CW", MemoryType.Output);
            conveyorMiddleE2 = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 0", MemoryType.Output);
            conveyorSecondCornerE2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 1 CW", MemoryType.Output);
            conveyorEndE2 = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 1", MemoryType.Output);
            bufferE2 = BufferE2.ZERO;
            sensorEndE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 2", MemoryType.Input);
            ftSensorEndE2 = new FTRIG();
            sensorSecondSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 12", MemoryType.Input);
            sensorThirdSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 13", MemoryType.Input);
            sensorFourthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 14", MemoryType.Input);

            //GripperArm
            armX = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Set Point (V)", MemoryType.Output);
            armX.Value = 2.3f;
            armXpos = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Position (V)", MemoryType.Input);
            armZ = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Set Point (V)", MemoryType.Output);
            armZ.Value = 6.0f;
            armZpos = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Position (V)", MemoryType.Input);
            armGrab = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Grab)", MemoryType.Output);
            armRotate = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 Rotate CCW", MemoryType.Output);
            armUnrotate = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 Rotate CW", MemoryType.Output);
            armPieceRotate = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 Gripper CCW", MemoryType.Output);
            armRotating = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Rotating)", MemoryType.Input);
            armPieceDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Item Detected)", MemoryType.Input);


            //ROBÔ0
            startC1toB1 = MemoryMap.Instance.GetBit("Robô C1 to B1", MemoryType.Input);
            startC2toB1 = MemoryMap.Instance.GetBit("Robô C2 to B1", MemoryType.Input);
            startC2toB2 = MemoryMap.Instance.GetBit("Robô C2 to B2", MemoryType.Input);
            startC3toB3 = MemoryMap.Instance.GetBit("Robô C3 to B3", MemoryType.Input);
            startC1toE2 = MemoryMap.Instance.GetBit("Robô C1 to E2", MemoryType.Input);
            startC2toE2 = MemoryMap.Instance.GetBit("Robô C2 to E2", MemoryType.Input);
            startC3toE2 = MemoryMap.Instance.GetBit("Robô C3 to E2", MemoryType.Input);
            robo0State = Robo0State.IDLE;

            robo0 = new SistemaDeManufatura_Robo0(
                MemoryMap.Instance.GetFloat("Pick & Place 0 X Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 0 X Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Pick & Place 0 Y Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 0 Y Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Pick & Place 0 Z Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 0 Z Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetBit("Pick & Place 0 (Grab)", MemoryType.Output),
                MemoryMap.Instance.GetBit("Pick & Place 0 (Box Detected)", MemoryType.Input),
                MemoryMap.Instance.GetBit("Pick & Place 0 C(+)", MemoryType.Output),
                startC1toB1, startC2toB1, startC2toB2, startC3toB3, startC1toE2, startC2toE2, startC3toE2);
                

            //Messages only once
            messageOnlyOnce = true;
            initialMessage = true;
            colorMessage = true;
            colorMessage2 = true;
        }

        public override void Execute(int elapsedMilliseconds)
        {

            if (initialMessage)
            {
                Console.WriteLine("\n");
                Console.WriteLine("Green is C1, Metal is C2, and Blue is C3");
                Console.WriteLine("\n");
                initialMessage = false;
            }
            
            //%%%%%%%%%%%%%%%%%%%% ESTEIRA START %%%%%%%%%%%%%%%%%%%%

            rtSensorStartE2.CLK(sensorStartE2.Value);
            ftSensorEndE2.CLK(sensorEndE2.Value);

            //E1
            if (conveyorE1Start.Value)
            {
                conveyorE1.Value = true;
            }
            else if (!conveyorE1Stop.Value)
            {
                conveyorE1.Value = false;
            }

            if (e1ConveyorState == E1ConveyorState.EMITTING)
            {
                emitter.Value = true;
                if (sensorEmitter.Value)
                {
                    e1ConveyorState = E1ConveyorState.EMITTED;
                }
            }
            else if (e1ConveyorState == E1ConveyorState.EMITTED)
            {
                emitter.Value = false;
                if (sensorEmitter.Value == false)
                {
                    e1ConveyorState = E1ConveyorState.GOING_TO_ROBO0;
                }
            }
            else if (e1ConveyorState == E1ConveyorState.GOING_TO_ROBO0)
            {
                if (sensorEndE1.Value)
                {
                    e1ConveyorState = E1ConveyorState.REACHED_ROBO0;
                }
            }
            else if (e1ConveyorState == E1ConveyorState.REACHED_ROBO0)
            {
                if (!sensorEndE1.Value && !sensorEndE2.Value)
                {
                    e1ConveyorState = E1ConveyorState.EMITTING;
                }
                else if (!sensorEndE1.Value && sensorEndE2.Value)
                {
                    e1ConveyorState = E1ConveyorState.E2_TO_E1;
                    e2toE1Steps = E2toE1Steps.GOING_TO_E2;
                }
            }
            else if (e1ConveyorState == E1ConveyorState.E2_TO_E1) //%%%%%%%%%%%% ARM
            {
                if (e2toE1Steps == E2toE1Steps.GOING_TO_E2)
                {
                    armX.Value = 2.3f;
                    if (armXpos.Value > 2.1f)
                    {
                        e2toE1Steps = E2toE1Steps.DOWN_LOOKING_FOR_PIECE;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.DOWN_LOOKING_FOR_PIECE)
                {
                    armZ.Value = 9.0f;
                    if (armZpos.Value > 8.5f)
                    {
                        e2toE1Steps = E2toE1Steps.GRABBING_PIECE;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.GRABBING_PIECE)
                {
                    armGrab.Value = true;
                    conveyorEndE2.Value = false;
                    if (armPieceDetected.Value && armZpos.Value > 8.8f)
                    {
                        e2toE1Steps = E2toE1Steps.UP_WITH_PIECE;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.UP_WITH_PIECE)
                {
                    armZ.Value = 4.0f;
                    if (armZpos.Value < 4.4f)
                    {
                        e2toE1Steps = E2toE1Steps.GOING_TO_E1_FIRST_HALF;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.GOING_TO_E1_FIRST_HALF)
                {
                    armRotate.Value = true;
                    armPieceRotate.Value = true;
                    armX.Value = 4.0f;
                    if (armXpos.Value > 3.9f)
                    {
                        armRotate.Value = false;
                        armPieceRotate.Value = false;
                    }
                    if (!armRotating.Value && armXpos.Value > 3.9f)
                    {
                        e2toE1Steps = E2toE1Steps.GOING_TO_E1_SECOND_HALF;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.GOING_TO_E1_SECOND_HALF)
                {
                    armRotate.Value = true;
                    armX.Value = 5.0f;
                    if (!armRotating.Value && armXpos.Value > 4.9f)
                    {
                        armRotate.Value = false;
                        e2toE1Steps = E2toE1Steps.DOWN_WITH_PIECE;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.DOWN_WITH_PIECE)
                {
                    armZ.Value = 9.0f;
                    if (armZpos.Value > 8.9f)
                    {
                        e2toE1Steps = E2toE1Steps.UNGRABBING_PIECE;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.UNGRABBING_PIECE)
                {
                    armGrab.Value = false;
                    e2toE1Steps = E2toE1Steps.UP_WITHOUT_PIECE;
                }
                else if (e2toE1Steps == E2toE1Steps.UP_WITHOUT_PIECE)
                {
                    armZ.Value = 5.5f;
                    if (armZpos.Value < 5.6f)
                    {
                        e2toE1Steps = E2toE1Steps.UNROTATE_FIRST_HALF;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.UNROTATE_FIRST_HALF)
                {
                    armUnrotate.Value = true;
                    armZ.Value = 4.5f;
                    if (!armRotating.Value && armZpos.Value < 4.6f)
                    {
                        armUnrotate.Value = false;
                        e2toE1Steps = E2toE1Steps.UNROTATE_SECOND_HALF;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.UNROTATE_SECOND_HALF)
                {
                    armUnrotate.Value = true;
                    armZ.Value = 3.5f;
                    if (!armRotating.Value && armZpos.Value < 3.6f)
                    {
                        armUnrotate.Value = false;
                        e1ConveyorState = E1ConveyorState.EMITTED;
                        armX.Value = 2.3f;
                        armZ.Value = 6.0f;
                    }
                }
            }

            if (sensorEmitter.Value)
            {
                emitter.Value = false;
            }

            

            //E2
            if (bufferE2 == BufferE2.ZERO)
            {
                conveyorStartE2.Value = false;
                conveyorFirstCornerE2.Value = false;
                conveyorMiddleE2.Value = false;
                conveyorSecondCornerE2.Value = false;
                conveyorEndE2.Value = false;
                if (rtSensorStartE2.Q == true)
                {
                    Console.WriteLine("Buffer One");
                    conveyorStartE2.Value = true;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = true;
                    conveyorSecondCornerE2.Value = true;
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.ONE;
                }
            }
            else if (bufferE2 == BufferE2.ONE)
            {
                if (rtSensorStartE2.Q == true)
                {
                    Console.WriteLine("Buffer Two");
                    conveyorStartE2.Value = true;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = true;
                    conveyorSecondCornerE2.Value = true;
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.TWO;
                }
                if (ftSensorEndE2.Q == true)
                {
                    Console.WriteLine("Buffer Zero");
                    bufferE2 = BufferE2.ZERO;
                }
                if (sensorEndE2.Value)
                {
                    conveyorStartE2.Value = false;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = false;
                    conveyorSecondCornerE2.Value = false;
                    conveyorEndE2.Value = false;
                }
            }
            else if (bufferE2 == BufferE2.TWO)
            {
                if (rtSensorStartE2.Q == true)
                {
                    Console.WriteLine("Buffer Three");
                    conveyorStartE2.Value = true;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = true;
                    conveyorSecondCornerE2.Value = true;
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.THREE;
                }
                if (ftSensorEndE2.Q == true)
                {
                    Console.WriteLine("Buffer One");
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.ONE;
                }
                if (sensorEndE2.Value && sensorSecondSpotE2.Value && !sensorThirdSpotE2.Value && !sensorFourthSpotE2.Value)
                {
                    conveyorStartE2.Value = false;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = false;
                    conveyorSecondCornerE2.Value = false;
                    conveyorEndE2.Value = false;
                }
            }
            else if (bufferE2 == BufferE2.THREE)
            {
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = true;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = true;
                    conveyorSecondCornerE2.Value = true;
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.FOUR;
                }
                if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.TWO;
                }
                if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && !sensorFourthSpotE2.Value)
                {
                    conveyorStartE2.Value = false;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = false;
                    conveyorSecondCornerE2.Value = false;
                    conveyorEndE2.Value = false;
                }
            }
            else if (bufferE2 == BufferE2.FOUR)
            {
                if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.THREE;
                }
                if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value)
                {
                    conveyorStartE2.Value = false;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = false;
                    conveyorSecondCornerE2.Value = false;
                    conveyorEndE2.Value = false;
                }
            }

            //%%%%%%%%%%%%%%%%%%%% ESTEIRA ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%% ROBÔ STARTS %%%%%%%%%%%%%%%%%%%%
            if (startC1toE2.Value || startC2toE2.Value || startC3toE2.Value)
            {
                robo0State = Robo0State.E1toE2;
            }
            else if (startC1toB1.Value || startC2toB1.Value)
            {
                robo0State = Robo0State.E1toB1;
            }
            else if (startC2toB2.Value)
            {
                robo0State = Robo0State.E1toB2;
            }
            else if (startC3toB3.Value)
            {
                robo0State = Robo0State.E1toB3;
            }

            if (robo0State == Robo0State.E1toE2)
            {
                robo0.E1toE2();
            }
            else if (robo0State == Robo0State.E1toB1)
            {
                robo0.E1toB1();
            }
            else if (robo0State == Robo0State.E1toB2)
            {
                robo0.E1toB2();
            }
            else if (robo0State == Robo0State.E1toB3)
            {
                robo0.E1toB3();
            }

            //%%%%%%%%%%%%%%%%%%%% ROBÔ ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%% COLOR SENSOR STARTS %%%%%%%%%%%%%%%%%%%%

            if (sensorColor.Value == 5)
            {
                if (colorMessage)
                {
                    Console.WriteLine("evento s_C1");
                    colorMessage = false;
                    colorMessage2 = true;
                }
            }
            else if (sensorColor.Value == 9)
            {
                if (colorMessage)
                {
                    Console.WriteLine("evento s_C2");
                    colorMessage = false;
                    colorMessage2 = true;
                }
            }
            else if (sensorColor.Value == 2)
            {
                if (colorMessage)
                {
                    Console.WriteLine("evento s_C3");
                    colorMessage = false;
                    colorMessage2 = true;
                }
            }
            else if (sensorColor.Value == 0)
            {
                if (colorMessage2)
                {
                    Console.WriteLine("evento s_des");
                    colorMessage2 = false;
                }
                colorMessage = true;
            }

            //%%%%%%%%%%%%%%%%%%%% COLOR SENSOR ENDS %%%%%%%%%%%%%%%%%%%%
        }
    }
}

using EngineIO;
using System;
using System.Threading;

namespace Controllers.Scenes.SistemaDeManufatura
{
    public class SistemaDeManufatura : Controller
    {
        //SUPERVISOR
        bool supervisoryApproval;
        readonly SistemaDeManufaturaSupervisor sistemaDeManufaturaSupervisor;
        int e1Counter;

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

        //Emitter Remover
        readonly MemoryBit emitter;
        readonly MemoryBit remover;
        readonly MemoryBit sensorEmitter;
        //E2
        readonly MemoryBit sensorStartE2;
        readonly RTRIG rtSensorStartE2;
        readonly MemoryFloat conveyorStartE2;
        readonly MemoryBit conveyorFirstCornerE2;
        readonly MemoryFloat conveyorMiddleE2;
        readonly MemoryBit conveyorSecondCornerE2;
        readonly MemoryFloat conveyorPreEndE2;
        readonly MemoryFloat conveyorEndE2;
        readonly MemoryBit sensorEndE2;
        readonly FTRIG ftSensorEndE2;
        readonly MemoryBit sensorSecondSpotE2;
        readonly MemoryBit sensorThirdSpotE2;
        readonly MemoryBit sensorFourthSpotE2;
        readonly MemoryBit sensorFifthSpotE2;
        readonly MemoryBit sensorSixthSpotE2;
        readonly MemoryBit sensorSeventhSpotE2;
        readonly MemoryBit sensorEighthSpotE2;
        readonly MemoryBit sensorNinthSpotE2;
        readonly MemoryBit sensorTenthSpotE2;
        readonly MemoryBit sensorEleventhSpotE2;
        readonly MemoryBit sensorTwelvethSpotE2;
        private enum BufferE2
        {
            ZERO,
            ONE,
            TWO,
            THREE,
            FOUR,
            FIVE,
            SIX,
            SEVEN,
            EIGHT,
            NINE,
            TEN,
            ELEVEN,
            TWELVE
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
        bool rotationBool;

        //ROBÔ
        readonly SistemaDeManufatura_Robo0 robo0;
        readonly MemoryBit startC1toB1;
        readonly MemoryBit startC2toB1;
        readonly MemoryBit startC2toB2;
        readonly MemoryBit startC3toB3;
        readonly MemoryBit startC1toE2;
        readonly MemoryBit startC2toE2;
        readonly MemoryBit startC3toE2;
        int roboCounter;
        string color;
        bool roboFinished;
        private enum Robo0State
        {
            IDLE,
            E1toE2,
            E1toB1,
            E1toB2,
            E1toB3
        }
        Robo0State robo0State;

        //M1
        readonly MemoryBit startC2fromB2toM1;
        readonly MemoryBit startC2fromB1toM1;
        readonly MemoryBit startC1fromB1toM1;
        readonly MemoryBit startC3fromB3toM1;
        readonly MemoryBit sensorM1end;
        int m1Counter;
        bool roboM1Finished;
        private enum M1states
        {
            IDLE,
            C1fromB1toM1,
            C2fromB1toM1,
            C2fromB2toM1,
            C3fromB3toM1
        }
        M1states m1states;
        readonly SistemaDeManufatura_M1 roboM1;

        //Messages only once
        bool initialMessage;
        bool colorMessage;
        bool colorMessage2;
        public SistemaDeManufatura()
        {
            //SUPERVISOR
            supervisoryApproval = false;
            sistemaDeManufaturaSupervisor = new SistemaDeManufaturaSupervisor();
            e1Counter = 0;

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

            //Emitter Remover
            emitter = MemoryMap.Instance.GetBit("Emitter 0 (Emit)", MemoryType.Output);
            remover = MemoryMap.Instance.GetBit("Remover 0 (Remove)", MemoryType.Output);
            sensorEmitter = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);

            //E2
            sensorStartE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 1", MemoryType.Input);
            rtSensorStartE2 = new RTRIG();
            conveyorStartE2 = MemoryMap.Instance.GetFloat("Belt Conveyor (6m) 0 (V)", MemoryType.Output);
            conveyorStartE2.Value = 0;
            conveyorFirstCornerE2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 0 CW", MemoryType.Output);
            conveyorMiddleE2 = MemoryMap.Instance.GetFloat("Belt Conveyor (4m) 0 (V)", MemoryType.Output);
            conveyorSecondCornerE2 = MemoryMap.Instance.GetBit("Curved Belt Conveyor 1 CW", MemoryType.Output);
            conveyorPreEndE2 = MemoryMap.Instance.GetFloat("Belt Conveyor (4m) 4 (V)", MemoryType.Output);
            conveyorEndE2 = MemoryMap.Instance.GetFloat("Belt Conveyor (4m) 1 (V)", MemoryType.Output);
            bufferE2 = BufferE2.ZERO;
            sensorEndE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 2", MemoryType.Input);
            ftSensorEndE2 = new FTRIG();
            sensorSecondSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 12", MemoryType.Input);
            sensorThirdSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 13", MemoryType.Input);
            sensorFourthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 14", MemoryType.Input);
            sensorFifthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 3", MemoryType.Input);
            sensorSixthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 16", MemoryType.Input);
            sensorSeventhSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 15", MemoryType.Input);
            sensorEighthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 17", MemoryType.Input);
            sensorNinthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 19", MemoryType.Input);
            sensorTenthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 21", MemoryType.Input);
            sensorEleventhSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 18", MemoryType.Input);
            sensorTwelvethSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 20", MemoryType.Input);

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
            rotationBool = false;


            //ROBÔ0
            startC1toB1 = MemoryMap.Instance.GetBit("Robô C1 to B1", MemoryType.Input);
            startC2toB1 = MemoryMap.Instance.GetBit("Robô C2 to B1", MemoryType.Input);
            startC2toB2 = MemoryMap.Instance.GetBit("Robô C2 to B2", MemoryType.Input);
            startC3toB3 = MemoryMap.Instance.GetBit("Robô C3 to B3", MemoryType.Input);
            startC1toE2 = MemoryMap.Instance.GetBit("Robô C1 to E2", MemoryType.Input);
            startC2toE2 = MemoryMap.Instance.GetBit("Robô C2 to E2", MemoryType.Input);
            startC3toE2 = MemoryMap.Instance.GetBit("Robô C3 to E2", MemoryType.Input);
            robo0State = Robo0State.IDLE;
            roboCounter = 0;
            color = "";
            roboFinished = false;

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
                startC1toB1, startC2toB1, startC2toB2, startC3toB3, startC1toE2, startC2toE2, startC3toE2,sistemaDeManufaturaSupervisor);

            //M1
            startC2fromB2toM1 = MemoryMap.Instance.GetBit("C2fromB2toM1", MemoryType.Input);
            startC2fromB1toM1 = MemoryMap.Instance.GetBit("C2fromB1toM1", MemoryType.Input);
            startC1fromB1toM1 = MemoryMap.Instance.GetBit("C1fromB1toM1", MemoryType.Input);
            startC3fromB3toM1 = MemoryMap.Instance.GetBit("C3fromB3toM1", MemoryType.Input);
            m1states = M1states.IDLE;
            sensorM1end = MemoryMap.Instance.GetBit("Diffuse Sensor 22", MemoryType.Input);
            roboM1 = new SistemaDeManufatura_M1(
                MemoryMap.Instance.GetFloat("Pick & Place 1 X Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 1 X Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Pick & Place 1 Y Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 1 Y Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Pick & Place 1 Z Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Pick & Place 1 Z Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetBit("Pick & Place 1 (Grab)", MemoryType.Output),
                MemoryMap.Instance.GetBit("Pick & Place 1 (Box Detected)", MemoryType.Input),
                MemoryMap.Instance.GetBit("Belt Conveyor (2m) 5", MemoryType.Output),
                MemoryMap.Instance.GetBit("Belt Conveyor (2m) 4", MemoryType.Output),
                MemoryMap.Instance.GetBit("Belt Conveyor (2m) 3", MemoryType.Output),
                MemoryMap.Instance.GetBit("Belt Conveyor (4m) 3", MemoryType.Output),
                MemoryMap.Instance.GetBit("Diffuse Sensor 4", MemoryType.Input),
                MemoryMap.Instance.GetBit("Diffuse Sensor 5", MemoryType.Input),
                MemoryMap.Instance.GetBit("Diffuse Sensor 6", MemoryType.Input),
                MemoryMap.Instance.GetBit("Diffuse Sensor 9", MemoryType.Input),
                MemoryMap.Instance.GetBit("Diffuse Sensor 8", MemoryType.Input),
                MemoryMap.Instance.GetBit("Diffuse Sensor 7", MemoryType.Input),
                sensorM1end,
                startC1fromB1toM1, startC2fromB1toM1, startC2fromB2toM1, startC3fromB3toM1);
            m1Counter = 0;
            roboM1Finished = false;

            //Messages only once
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
                Console.WriteLine("Product A is C2 and C1. Product B is C2 and C3.");
                Console.WriteLine("\n");
                sistemaDeManufaturaSupervisor.CreateController();
                initialMessage = false;
            }
            
            //%%%%%%%%%%%%%%%%%%%% ESTEIRA START %%%%%%%%%%%%%%%%%%%%

            rtSensorStartE2.CLK(sensorStartE2.Value);
            ftSensorEndE2.CLK(sensorEndE2.Value);

            //E1
            if (conveyorE1Start.Value)
            {
                if (e1Counter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("e1_lig");
                    if (supervisoryApproval)
                    {
                        conveyorE1.Value = true;
                        e1Counter++;
                    }
                }
            }
            else if (!conveyorE1Stop.Value)
            {   if (e1Counter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("e1_des");
                    if (supervisoryApproval)
                    {
                        conveyorE1.Value = false;
                        e1Counter++;
                    }
                }
            }
            else
            {
                e1Counter = 0;
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
                    Console.WriteLine("Emitting new piece");
                    e1ConveyorState = E1ConveyorState.EMITTING;
                }
                else if (!sensorEndE1.Value && sensorEndE2.Value)
                {
                    Console.WriteLine("using arm");
                    e1ConveyorState = E1ConveyorState.E2_TO_E1;
                    e2toE1Steps = E2toE1Steps.GOING_TO_E2;
                }
            }
            else if (e1ConveyorState == E1ConveyorState.E2_TO_E1) //%%%%%%%%%%%% ARM
            {
                if (e2toE1Steps == E2toE1Steps.GOING_TO_E2)
                {
                    Console.WriteLine("Going to E2");
                    armX.Value = 2.3f;
                    if (armXpos.Value > 2.2f && armXpos.Value < 2.4f)
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
                        conveyorEndE2.Value = 1;
                        e2toE1Steps = E2toE1Steps.GOING_TO_E1_FIRST_HALF;
                        rotationBool = false;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.GOING_TO_E1_FIRST_HALF)
                {
                    armRotate.Value = true;
                    armPieceRotate.Value = true;
                    armX.Value = 4.0f;
                    if (rotationBool)
                    {
                        Thread.Sleep(200);
                        armRotate.Value = false;
                        armPieceRotate.Value = false;
                    }
                    else
                    {
                        rotationBool = true;
                    }
                    if (!armRotating.Value && armXpos.Value > 3.9f && rotationBool)
                    {
                        e2toE1Steps = E2toE1Steps.GOING_TO_E1_SECOND_HALF;
                        rotationBool = false;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.GOING_TO_E1_SECOND_HALF)
                {
                    armRotate.Value = true;
                    armX.Value = 5.0f;
                    if (!armRotating.Value && armXpos.Value > 4.9f && rotationBool)
                    {
                        Thread.Sleep(200);
                        armRotate.Value = false;
                        e2toE1Steps = E2toE1Steps.DOWN_WITH_PIECE;
                    }
                    else
                    {
                        rotationBool = true;
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
                        rotationBool = false;                    
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.UNROTATE_FIRST_HALF)
                {
                    armUnrotate.Value = true;
                    armZ.Value = 4.5f;
                    if (!armRotating.Value && armZpos.Value < 4.6f && armZpos.Value > 4.4f && rotationBool)
                    {
                        armUnrotate.Value = false;
                        e2toE1Steps = E2toE1Steps.UNROTATE_SECOND_HALF;
                        rotationBool = false;
                    }
                    else
                    {
                        Thread.Sleep(200);
                        rotationBool = true;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.UNROTATE_SECOND_HALF)
                {
                    armUnrotate.Value = true;
                    armZ.Value = 3.5f;
                    if (!armRotating.Value && armZpos.Value < 3.6f && armZpos.Value > 3.4f && rotationBool)
                    {
                        armUnrotate.Value = false;
                        e1ConveyorState = E1ConveyorState.EMITTED;
                        armX.Value = 2.3f;
                        armZ.Value = 6.0f;
                    }
                    else
                    {
                        Thread.Sleep(200);
                        rotationBool = true;
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
                conveyorStartE2.Value = 0;
                conveyorFirstCornerE2.Value = false;
                conveyorMiddleE2.Value = 0;
                conveyorSecondCornerE2.Value = false;
                conveyorPreEndE2.Value = 0;
                conveyorEndE2.Value = 0;
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 1;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 1;
                    conveyorEndE2.Value = 1;
                    bufferE2 = BufferE2.ONE;
                }
            }
            else if (bufferE2 == BufferE2.ONE)
            {
                if (ftSensorEndE2.Q == true)
                {
                    bufferE2 = BufferE2.ZERO;
                }
                else if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 0.5f;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 0.5f;
                    conveyorEndE2.Value = 0.5f;
                    bufferE2 = BufferE2.TWO;
                }
                else if (sensorEndE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2 == BufferE2.TWO)
            {
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 0.5f;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 0.5f;
                    conveyorEndE2.Value = 0.5f;
                    bufferE2 = BufferE2.THREE;
                }
                else if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = 1;
                    conveyorPreEndE2.Value = 1;
                    bufferE2 = BufferE2.ONE;
                }
                else if (sensorEndE2.Value && sensorSecondSpotE2.Value && !sensorThirdSpotE2.Value && !sensorFourthSpotE2.Value && !sensorFifthSpotE2.Value && !sensorSixthSpotE2.Value && !sensorSeventhSpotE2.Value && !sensorEighthSpotE2.Value && !sensorNinthSpotE2.Value && !sensorTenthSpotE2.Value && !sensorEleventhSpotE2.Value && !sensorTwelvethSpotE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2 == BufferE2.THREE)
            {
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 0.5f;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 0.5f;
                    conveyorEndE2.Value = 0.5f;
                    bufferE2 = BufferE2.FOUR;
                }
                else if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = 1;
                    conveyorPreEndE2.Value = 1;
                    bufferE2 = BufferE2.TWO;
                }
                else if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && !sensorFourthSpotE2.Value && !sensorFifthSpotE2.Value && !sensorSixthSpotE2.Value && !sensorSeventhSpotE2.Value && !sensorEighthSpotE2.Value && !sensorNinthSpotE2.Value && !sensorTenthSpotE2.Value && !sensorEleventhSpotE2.Value && !sensorTwelvethSpotE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2 == BufferE2.FOUR)
            {
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 0.5f;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 0.5f;
                    conveyorEndE2.Value = 0.5f;
                    bufferE2 = BufferE2.FIVE;
                }
                else if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = 1;
                    conveyorPreEndE2.Value = 1;
                    bufferE2 = BufferE2.THREE;
                }
                else if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && !sensorFifthSpotE2.Value && !sensorSixthSpotE2.Value && !sensorSeventhSpotE2.Value && !sensorEighthSpotE2.Value && !sensorNinthSpotE2.Value && !sensorTenthSpotE2.Value && !sensorEleventhSpotE2.Value && !sensorTwelvethSpotE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2 == BufferE2.FIVE)
            {
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 0.5f;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 0.5f;
                    conveyorEndE2.Value = 0.5f;
                    bufferE2 = BufferE2.SIX;
                }
                else if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = 1;
                    conveyorPreEndE2.Value = 1;
                    bufferE2 = BufferE2.FOUR;
                }
                else if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value && !sensorSixthSpotE2.Value && !sensorSeventhSpotE2.Value && !sensorEighthSpotE2.Value && !sensorNinthSpotE2.Value && !sensorTenthSpotE2.Value && !sensorEleventhSpotE2.Value && !sensorTwelvethSpotE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2 == BufferE2.SIX)
            {
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 0.5f;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 0.5f;
                    conveyorEndE2.Value = 0.5f;
                    bufferE2 = BufferE2.SEVEN;
                }
                else if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = 1;
                    conveyorPreEndE2.Value = 1;
                    bufferE2 = BufferE2.FIVE;
                }
                else if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && !sensorSeventhSpotE2.Value && !sensorEighthSpotE2.Value && !sensorNinthSpotE2.Value && !sensorTenthSpotE2.Value && !sensorEleventhSpotE2.Value && !sensorTwelvethSpotE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2 == BufferE2.SEVEN)
            {
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 0.5f;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 0.5f;
                    conveyorEndE2.Value = 0.5f;
                    bufferE2 = BufferE2.EIGHT;
                }
                else if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = 1;
                    conveyorPreEndE2.Value = 1;
                    bufferE2 = BufferE2.SIX;
                }
                else if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && !sensorEighthSpotE2.Value && !sensorNinthSpotE2.Value && !sensorTenthSpotE2.Value && !sensorEleventhSpotE2.Value && !sensorTwelvethSpotE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2 == BufferE2.EIGHT)
            {
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 0.5f;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 0.5f;
                    conveyorEndE2.Value = 0.5f;
                    bufferE2 = BufferE2.NINE;
                }
                else if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = 1;
                    conveyorPreEndE2.Value = 1;
                    bufferE2 = BufferE2.SEVEN;
                }
                else if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && sensorEighthSpotE2.Value && !sensorNinthSpotE2.Value && !sensorTenthSpotE2.Value && !sensorEleventhSpotE2.Value && !sensorTwelvethSpotE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2 == BufferE2.NINE)
            {
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 0.5f;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 0.5f;
                    conveyorEndE2.Value = 0.5f;
                    bufferE2 = BufferE2.TEN;
                }
                else if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = 1;
                    conveyorPreEndE2.Value = 1;
                    bufferE2 = BufferE2.EIGHT;
                }
                else if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && sensorEighthSpotE2.Value && sensorNinthSpotE2.Value && !sensorTenthSpotE2.Value && !sensorEleventhSpotE2.Value && !sensorTwelvethSpotE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2 == BufferE2.TEN)
            {
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 0.5f;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 0.5f;
                    conveyorEndE2.Value = 0.5f;
                    bufferE2 = BufferE2.ELEVEN;
                }
                else if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = 1;
                    conveyorPreEndE2.Value = 1;
                    bufferE2 = BufferE2.NINE;
                }
                else if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && sensorEighthSpotE2.Value && sensorNinthSpotE2.Value && sensorTenthSpotE2.Value && !sensorEleventhSpotE2.Value && !sensorTwelvethSpotE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2 == BufferE2.ELEVEN)
            {
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = 0.5f;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = 0.5f;
                    conveyorSecondCornerE2.Value = true;
                    conveyorPreEndE2.Value = 0.5f;
                    conveyorEndE2.Value = 0.5f;
                    bufferE2 = BufferE2.TWELVE;
                }
                else if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = 1;
                    conveyorPreEndE2.Value = 1;
                    bufferE2 = BufferE2.TEN;
                }
                else if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && sensorEighthSpotE2.Value && sensorNinthSpotE2.Value && sensorTenthSpotE2.Value && sensorEleventhSpotE2.Value && !sensorTwelvethSpotE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }
            else if (bufferE2 == BufferE2.TWELVE)
            {
                if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = 1;
                    conveyorPreEndE2.Value = 1;
                    bufferE2 = BufferE2.ELEVEN;
                }
                else if (sensorEndE2.Value && sensorSecondSpotE2.Value && sensorThirdSpotE2.Value && sensorFourthSpotE2.Value && sensorFifthSpotE2.Value && sensorSixthSpotE2.Value && sensorSeventhSpotE2.Value && sensorEighthSpotE2.Value && sensorNinthSpotE2.Value && sensorTenthSpotE2.Value && sensorEleventhSpotE2.Value && sensorTwelvethSpotE2.Value)
                {
                    conveyorStartE2.Value = 0;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = 0;
                    conveyorSecondCornerE2.Value = false;
                    conveyorPreEndE2.Value = 0;
                    conveyorEndE2.Value = 0;
                }
            }

            //%%%%%%%%%%%%%%%%%%%% ESTEIRA ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%% ROBÔ STARTS %%%%%%%%%%%%%%%%%%%%
            if (startC1toE2.Value)
            {
                if (roboCounter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("r_c1e2");
                    if (supervisoryApproval)
                    {
                        robo0State = Robo0State.E1toE2;
                        roboCounter++;
                    }
                }
            }
            else if (startC2toE2.Value)
            {
                if (roboCounter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("r_c2e2");
                    if (supervisoryApproval)
                    {
                        robo0State = Robo0State.E1toE2;
                        roboCounter++;
                    }
                }
            }
            else if (startC3toE2.Value)
            {
                if (roboCounter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("r_c3e2");
                    if (supervisoryApproval)
                    {
                        robo0State = Robo0State.E1toE2;
                        roboCounter++;
                    }
                }
            }
            else if (startC1toB1.Value)
            {
                if (roboCounter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("r_c1b1");
                    if (supervisoryApproval)
                    {
                        robo0State = Robo0State.E1toB1;
                        roboCounter++;
                        color = "c1";
                    }
                }
            }
            else if (startC2toB1.Value)
            {
                if (roboCounter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("r_c2b1");
                    if (supervisoryApproval)
                    {
                        robo0State = Robo0State.E1toB1;
                        roboCounter++;
                        color = "c2";
                    }
                }
            }
            else if (startC2toB2.Value)
            {
                if (roboCounter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("r_c2b2");
                    if (supervisoryApproval)
                    {
                        robo0State = Robo0State.E1toB2;
                        roboCounter++;
                    }
                }
            }
            else if (startC3toB3.Value)
            {
                if (roboCounter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("r_c3b3");
                    if (supervisoryApproval)
                    {
                        robo0State = Robo0State.E1toB3;
                        roboCounter++;
                    }
                }
            }
            else
            {
                roboCounter = 0;
            }

            if (robo0State == Robo0State.E1toE2)
            {
                roboFinished = robo0.E1toE2();
                if (roboFinished)
                {
                    robo0State = Robo0State.IDLE;
                }
            }
            else if (robo0State == Robo0State.E1toB1)
            {
                roboFinished = robo0.E1toB1(color);
                if (roboFinished)
                {
                    robo0State = Robo0State.IDLE;
                }
            }
            else if (robo0State == Robo0State.E1toB2)
            {
                roboFinished = robo0.E1toB2();
                if (roboFinished)
                {
                    robo0State = Robo0State.IDLE;
                }
            }
            else if (robo0State == Robo0State.E1toB3)
            {
                roboFinished = robo0.E1toB3();
                if (roboFinished)
                {
                    robo0State = Robo0State.IDLE;
                }
            }
            else if (robo0State == Robo0State.IDLE)
            {
                robo0.Idle();
            }

            //%%%%%%%%%%%%%%%%%%%% ROBÔ ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%% COLOR SENSOR STARTS %%%%%%%%%%%%%%%%%%%%

            if (sensorColor.Value == 5)
            {
                if (colorMessage)
                {
                    sistemaDeManufaturaSupervisor.On("s_c1");
                    colorMessage = false;
                    colorMessage2 = true;
                }
            }
            else if (sensorColor.Value == 9)
            {
                if (colorMessage)
                {
                    sistemaDeManufaturaSupervisor.On("s_c2");
                    colorMessage = false;
                    colorMessage2 = true;
                }
            }
            else if (sensorColor.Value == 2)
            {
                if (colorMessage)
                {
                    sistemaDeManufaturaSupervisor.On("s_c3");
                    colorMessage = false;
                    colorMessage2 = true;
                }
            }
            else if (sensorColor.Value == 0)
            {
                if (colorMessage2)
                {
                    sistemaDeManufaturaSupervisor.On("s_des");
                    colorMessage2 = false;
                }
                colorMessage = true;
            }
            //%%%%%%%%%%%%%%%%%%%% COLOR SENSOR ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%% M1 STARTS %%%%%%%%%%%%%%%%%%%%

            
            if (startC1fromB1toM1.Value)
            {
                if (m1Counter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("m1_ini_c_c1");
                    if (supervisoryApproval)
                    {
                        m1states = M1states.C1fromB1toM1;
                        m1Counter++;
                    }
                }
            }
            else if (startC2fromB1toM1.Value)
            {
                if (m1Counter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("m1_ini_c_c2");
                    if (supervisoryApproval)
                    {
                        m1states = M1states.C2fromB1toM1;
                        m1Counter++;
                    }
                }
            }
            else if (startC2fromB2toM1.Value)
            {
                if (m1Counter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("m1_ini_b");
                    if (supervisoryApproval)
                    {
                        m1states = M1states.C2fromB2toM1;
                        m1Counter++;
                    }
                }
            }
            else if (startC3fromB3toM1.Value)
            {
                if (m1Counter == 0)
                {
                    supervisoryApproval = sistemaDeManufaturaSupervisor.On("m1_ini_a");
                    if (supervisoryApproval)
                    {
                        m1states = M1states.C3fromB3toM1;
                        m1Counter++;
                    }
                }
            }
            else
            {
                m1Counter = 0;
            }

            if (m1states == M1states.IDLE)
            {
                roboM1.Idle();
            }
            else if (m1states == M1states.C1fromB1toM1)
            {
                roboM1Finished = roboM1.C1fromB1toM1();
                if (roboM1Finished)
                {
                    m1states = M1states.IDLE;
                    sistemaDeManufaturaSupervisor.On("m1_fim");
                }
            }
            else if (m1states == M1states.C2fromB1toM1)
            {
                roboM1Finished = roboM1.C2fromB1toM1();
                if (roboM1Finished)
                {
                    m1states = M1states.IDLE;
                }
            }
            else if (m1states == M1states.C2fromB2toM1)
            {
                roboM1Finished = roboM1.C2fromB2toM1();
                if (roboM1Finished)
                {
                    m1states = M1states.IDLE;
                }
            }
            else if (m1states == M1states.C3fromB3toM1)
            {
                roboM1Finished = roboM1.C3fromB3toM1();
                if (roboM1Finished)
                {
                    m1states = M1states.IDLE;
                    sistemaDeManufaturaSupervisor.On("m1_fim");
                }
            }

            if (sensorM1end.Value)
            {
                remover.Value = true;
            }
            else
            {
                remover.Value = false;
            }

            //%%%%%%%%%%%%%%%%%%%% M1 ENDS %%%%%%%%%%%%%%%%%%%%
        }
    }
}

using Controllers.Scenes;
using EngineIO;
using System;
using System.Diagnostics;

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
            IDLE,
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
        readonly MemoryBit sensorFifthSpotE2;
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
        readonly MemoryFloat robo0X;
        readonly MemoryFloat robo0Y;
        readonly MemoryFloat robo0Z;
        readonly MemoryFloat robo0XPos;
        readonly MemoryFloat robo0YPos;
        readonly MemoryFloat robo0ZPos;
        readonly MemoryBit robo0Grab;
        readonly MemoryBit robo0ToE2start;
        readonly MemoryBit robo0Grabbed;
        float searchingForPieceYvalue;
        float lowestYinSearch;
        float highestYinSearch;
        float tempLowestYinSearch;
        bool loweestYinSearchFound;

        private enum RoboSteps
        {
            IDLE,
            DOWN_FOR_PIECE,
            SEARCHING_FOR_PIECE,
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
            //E1
            conveyorE1 = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 2", MemoryType.Output);
            conveyorE1Start = MemoryMap.Instance.GetBit("Liga esteira E1", MemoryType.Input);
            conveyorE1Stop = MemoryMap.Instance.GetBit("Desliga esteira E1", MemoryType.Input);
            sensorEndE1 = MemoryMap.Instance.GetBit("Diffuse Sensor 0", MemoryType.Input);
            sensorColor = MemoryMap.Instance.GetInt("Vision Sensor 0 (Value)", MemoryType.Input);
            stopbladeEndE1 = MemoryMap.Instance.GetBit("Stop Blade 1", MemoryType.Output);
            stopbladeEndE1.Value = true;
            e1ConveyorState = E1ConveyorState.EMITTING;
            e2toE1Steps = E2toE1Steps.IDLE;

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
            sensorFifthSpotE2 = MemoryMap.Instance.GetBit("Diffuse Sensor 15", MemoryType.Input);

            //GripperArm
            armX = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Set Point (V)", MemoryType.Output);
            armXpos = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Position (V)", MemoryType.Input);
            armZ = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Set Point (V)", MemoryType.Output);
            armZpos = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Position (V)", MemoryType.Input);
            armGrab = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Grab)", MemoryType.Output);
            armRotate = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 Rotate CCW", MemoryType.Output);
            armUnrotate = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 Rotate CW", MemoryType.Output);
            armPieceRotate = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 Gripper CCW", MemoryType.Output);
            armRotating = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Rotating)", MemoryType.Input);
            armPieceDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Item Detected)", MemoryType.Input);


            //ROBÔ0
            robo0X = MemoryMap.Instance.GetFloat("Pick & Place 0 X Set Point (V)", MemoryType.Output);
            robo0Y = MemoryMap.Instance.GetFloat("Pick & Place 0 Y Set Point(V)", MemoryType.Output);
            robo0Z = MemoryMap.Instance.GetFloat("Pick & Place 0 Z Set Point (V)", MemoryType.Output);
            robo0XPos = MemoryMap.Instance.GetFloat("Pick & Place 0 X Position (V)", MemoryType.Input);
            robo0YPos = MemoryMap.Instance.GetFloat("Pick & Place 0 Y Position (V)", MemoryType.Input);
            robo0ZPos = MemoryMap.Instance.GetFloat("Pick & Place 0 Z Position (V)", MemoryType.Input);
            robo0X.Value = 0.9f;//0.9f initial state
            robo0Y.Value = 0.7f;//0.7f initial state
            robo0Y.Value = 0.0f;//0.0f initial state
            robo0Grab = MemoryMap.Instance.GetBit("Pick & Place 0 (Grab)", MemoryType.Output);
            robo0ToE2start = MemoryMap.Instance.GetBit("Robô E1 to E2", MemoryType.Input);
            robo0Steps = RoboSteps.IDLE;
            robo0Grabbed = MemoryMap.Instance.GetBit("Pick & Place 0 (Box Detected)", MemoryType.Input);
            searchingForPieceYvalue = 0.0f;
            lowestYinSearch = 0.0f;
            highestYinSearch = 3.0f;
            tempLowestYinSearch = 0.0f;
            loweestYinSearchFound = false;

            //Messages only once
            messageOnlyOnce = true;
        }

        public override void Execute(int elapsedMilliseconds)
        {
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
                }
            }
            else if (e1ConveyorState == E1ConveyorState.E2_TO_E1)
            {
                if (e2toE1Steps == E2toE1Steps.IDLE)
                {
                    armX.Value = 0.0f;
                    armZ.Value = 4.0f;
                    e2toE1Steps = E2toE1Steps.GOING_TO_E2;
                }
                else if (e2toE1Steps == E2toE1Steps.GOING_TO_E2)
                {
                    armX.Value = 2.3f;
                    if (armXpos.Value > 2.0f)
                    {
                        e2toE1Steps = E2toE1Steps.DOWN_LOOKING_FOR_PIECE;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.DOWN_LOOKING_FOR_PIECE)
                {
                    armZ.Value = 9.2f;
                    if (armZpos.Value > 8.5f)
                    {
                        e2toE1Steps = E2toE1Steps.GRABBING_PIECE;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.GRABBING_PIECE)
                {
                    armGrab.Value = true;
                    if (armPieceDetected.Value)
                    {
                        e2toE1Steps = E2toE1Steps.UP_WITH_PIECE;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.UP_WITH_PIECE)
                {
                    armZ.Value = 4.0f;
                    if (armZpos.Value < 6.0f)
                    {
                        e2toE1Steps = E2toE1Steps.GOING_TO_E1_FIRST_HALF;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.GOING_TO_E1_FIRST_HALF)
                {
                    armRotate.Value = true;
                    armX.Value = 5.9f;
                    armPieceRotate.Value = true;
                    if (!armRotating.Value)
                    {
                        armRotate.Value = false;
                        armPieceRotate.Value = false;
                        e2toE1Steps = E2toE1Steps.GOING_TO_E1_SECOND_HALF;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.GOING_TO_E1_SECOND_HALF)
                {
                    armRotate.Value = true;
                    if (!armRotating.Value && armXpos.Value > 4.9f)
                    {
                        armRotate.Value = false;
                        e2toE1Steps = E2toE1Steps.DOWN_WITH_PIECE;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.DOWN_WITH_PIECE)
                {
                    armZ.Value = 9.2f;
                    if (armZpos.Value > 9.0f)
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
                    armZ.Value = 4.0f;
                    if (armZpos.Value < 6.0f)
                    {
                        e2toE1Steps = E2toE1Steps.UNROTATE_FIRST_HALF;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.UNROTATE_FIRST_HALF)
                {
                    armUnrotate.Value = true;
                    if (!armRotating.Value)
                    {
                        e2toE1Steps = E2toE1Steps.UNROTATE_SECOND_HALF;
                    }
                }
                else if (e2toE1Steps == E2toE1Steps.UNROTATE_SECOND_HALF)
                {
                    armUnrotate.Value = true;
                    if (!armRotating.Value)
                    {
                        e2toE1Steps = E2toE1Steps.IDLE;
                    }
                }
                
                if (sensorEmitter.Value)
                {
                    emitter.Value = false;
                }
            }
            if (sensorEndE1.Value == true)
            {
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
            if (bufferE2 == BufferE2.ZERO)
            {
                conveyorStartE2.Value = false;
                conveyorFirstCornerE2.Value = false;
                conveyorMiddleE2.Value = false;
                conveyorSecondCornerE2.Value = false;
                conveyorEndE2.Value = false;
                if (rtSensorStartE2.Q == true)
                {
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
                    conveyorStartE2.Value = true;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = true;
                    conveyorSecondCornerE2.Value = true;
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.TWO;
                }
                if (ftSensorEndE2.Q == true)
                {
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
                    conveyorStartE2.Value = true;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = true;
                    conveyorSecondCornerE2.Value = true;
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.THREE;
                }
                if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.ONE;
                }
                if (sensorEndE2.Value && sensorSecondSpotE2.Value)
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
                if (sensorEndE2.Value && sensorSecondSpotE2.Value & sensorThirdSpotE2.Value)
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
                if (rtSensorStartE2.Q == true)
                {
                    conveyorStartE2.Value = true;
                    conveyorFirstCornerE2.Value = true;
                    conveyorMiddleE2.Value = true;
                    conveyorSecondCornerE2.Value = true;
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.FIVE;
                }
                if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.THREE;
                }
                if (sensorEndE2.Value && sensorSecondSpotE2.Value & sensorThirdSpotE2.Value & sensorFourthSpotE2.Value)
                {
                    conveyorStartE2.Value = false;
                    conveyorFirstCornerE2.Value = false;
                    conveyorMiddleE2.Value = false;
                    conveyorSecondCornerE2.Value = false;
                    conveyorEndE2.Value = false;
                }
            }
            else if (bufferE2 == BufferE2.FIVE)
            {
                if (ftSensorEndE2.Q == true)
                {
                    conveyorEndE2.Value = true;
                    bufferE2 = BufferE2.FOUR;
                }
                if (sensorEndE2.Value && sensorSecondSpotE2.Value & sensorThirdSpotE2.Value & sensorFourthSpotE2.Value & sensorFifthSpotE2.Value)
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
                robo0Z.Value = 8.0f;
                if (robo0ZPos.Value > 7.7f)
                {
                    robo0Steps = RoboSteps.SEARCHING_FOR_PIECE;
                    robo0Y.Value = 0.0f;
                }
            }
            else if (robo0Steps == RoboSteps.SEARCHING_FOR_PIECE)
            {
                //for (searchingForPieceYvalue = 0.1f; searchingForPieceYvalue <= 5.5f; searchingForPieceYvalue += 0.1f)
                //{
                //    robo0Y.Value = searchingForPieceYvalue;
                //    while (robo0YPos.Value - robo0Y.Value > 0.05)
                //    {
                //        Console.WriteLine("Waiting for robot to reach position");
                //    }
                //    if (!robo0Grabbed.Value && !loweestYinSearchFound)
                //    {
                //        tempLowestYinSearch = searchingForPieceYvalue + 0.1f;
                //    }
                //    else if (robo0Grabbed.Value)
                //    {
                //        loweestYinSearchFound = true;
                //        lowestYinSearch = tempLowestYinSearch;
                //        highestYinSearch = searchingForPieceYvalue;
                //    }
                //}
                //Console.WriteLine("lowestYinSearch = " + lowestYinSearch);
                //Console.WriteLine("highestYinSearch = " + highestYinSearch);
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

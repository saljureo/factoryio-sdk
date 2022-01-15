using EngineIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Controllers.Scenes.SistemaDeManufatura
{
    class SistemaDeManufatura_M1
    {
        private enum RoboSteps
        {
            IDLE,
            WAITING_FOR_PIECE,
            DOWN_FOR_PIECE,
            SEARCHING_FOR_PIECE,
            GRAB_PIECE,
            UP_WITH_PIECE,
            TO_DESTINATION,
            DOWN_WITH_PIECE,
            RELEASE_PIECE,
            UP_WITHOUT_PIECE,
            TAKE_PIECE_AWAY
        }
        RoboSteps roboM1Steps;
        readonly MemoryFloat roboM1X;
        readonly MemoryFloat roboM1XPos;
        readonly MemoryFloat roboM1Y;
        readonly MemoryFloat roboM1YPos;
        readonly MemoryFloat roboM1Z;
        readonly MemoryFloat roboM1ZPos;
        readonly MemoryBit roboM1Grab;
        readonly MemoryBit roboM1Grabbed;
        readonly MemoryBit conveyorB1;
        readonly MemoryBit conveyorB2;
        readonly MemoryBit conveyorB3;
        readonly MemoryBit conveyorM1;
        readonly MemoryBit sensorB1start;
        readonly MemoryBit sensorB2start;
        readonly MemoryBit sensorB3start;
        readonly MemoryBit sensorB1end;
        readonly MemoryBit sensorB2end;
        readonly MemoryBit sensorB3end;
        readonly MemoryBit sensorM1end;
        readonly MemoryBit startC1fromB1toM1;
        readonly MemoryBit startC2fromB1toM1;
        readonly MemoryBit startC2fromB2toM1;
        readonly MemoryBit startC3fromB3toM1;
        bool pieceFound;
        public SistemaDeManufatura_M1(MemoryFloat roboM1X, MemoryFloat roboM1XPos, MemoryFloat roboM1Y, 
            MemoryFloat roboM1YPos, MemoryFloat roboM1Z, MemoryFloat roboM1ZPos, MemoryBit roboM1Grab, 
            MemoryBit roboM1Grabbed, MemoryBit conveyorB1, MemoryBit conveyorB2, MemoryBit conveyorB3, MemoryBit conveyorM1,
            MemoryBit sensorB1start, MemoryBit sensorB2start, MemoryBit sensorB3start, MemoryBit sensorB1end, MemoryBit sensorB2end, MemoryBit sensorB3end, MemoryBit sensorM1end,
            MemoryBit startC1fromB1toM1, MemoryBit startC2fromB1toM1, MemoryBit startC2fromB2toM1, MemoryBit startC3fromB3toM1)
        {
            this.roboM1X = roboM1X;
            this.roboM1XPos = roboM1XPos;
            this.roboM1Y = roboM1Y;
            this.roboM1YPos = roboM1YPos;
            this.roboM1Z = roboM1Z;
            this.roboM1ZPos = roboM1ZPos;
            this.roboM1Grab = roboM1Grab;
            this.roboM1Grabbed = roboM1Grabbed;
            this.conveyorB1 = conveyorB1;
            this.conveyorB2 = conveyorB2;
            this.conveyorB3 = conveyorB3;
            this.conveyorM1 = conveyorM1;
            this.sensorB1start = sensorB1start;
            this.sensorB2start = sensorB2start;
            this.sensorB3start = sensorB3start;
            this.sensorB1end = sensorB1end;
            this.sensorB2end = sensorB2end;
            this.sensorB3end = sensorB3end;
            this.sensorM1end = sensorM1end;
            this.startC1fromB1toM1 = startC1fromB1toM1;
            this.startC2fromB1toM1 = startC2fromB1toM1;
            this.startC2fromB2toM1 = startC2fromB2toM1;
            this.startC3fromB3toM1 = startC3fromB3toM1;
            roboM1Steps = RoboSteps.IDLE;
            pieceFound = false;
        }

        public bool C1fromB1toM1()
        {
            if (roboM1Steps == RoboSteps.IDLE)
            {
                roboM1Grab.Value = false;
                roboM1Steps = RoboSteps.WAITING_FOR_PIECE;
            }
            else if (roboM1Steps == RoboSteps.WAITING_FOR_PIECE)
            {
                roboM1X.Value = 1.2f;
                if (sensorB1start.Value)
                {
                    conveyorB1.Value = true;
                    pieceFound = true;
                }
                else if ((!pieceFound || sensorB1end.Value) && Math.Abs(roboM1XPos.Value - roboM1X.Value) < 0.1f)
                {
                    conveyorB1.Value = false;
                    roboM1Steps = RoboSteps.DOWN_FOR_PIECE;
                    pieceFound = false;
                }
            }
            else if (roboM1Steps == RoboSteps.DOWN_FOR_PIECE)
            {
                roboM1Z.Value = 9.0f;
                if (roboM1ZPos.Value > 8.7f)
                {
                    roboM1Steps = RoboSteps.GRAB_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.GRAB_PIECE)
            {
                roboM1Grab.Value = true;
                MemoryMap.Instance.Update();
                Thread.Sleep(300);
                if (roboM1Grabbed.Value)
                {
                    roboM1Steps = RoboSteps.UP_WITH_PIECE;
                }
                else
                {
                    roboM1Steps = RoboSteps.IDLE;
                }
            }
            else if (roboM1Steps == RoboSteps.UP_WITH_PIECE)
            {
                roboM1Z.Value = 4.0f;
                if (roboM1ZPos.Value < 6.0f)
                {
                    roboM1Steps = RoboSteps.TO_DESTINATION;
                }
            }
            else if (roboM1Steps == RoboSteps.TO_DESTINATION)
            {
                roboM1X.Value = 5.0f;
                roboM1Y.Value = 9.5f;
                if (roboM1YPos.Value > 9.45f)
                {
                    roboM1Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                roboM1Z.Value = 7.6f;
                if (roboM1ZPos.Value > 7.5f)
                {
                    roboM1Steps = RoboSteps.RELEASE_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.RELEASE_PIECE)
            {
                roboM1Grab.Value = false;
                roboM1Steps = RoboSteps.UP_WITHOUT_PIECE;
            }
            else if (roboM1Steps == RoboSteps.UP_WITHOUT_PIECE)
            {
                roboM1Z.Value = 4.0f;
                if (roboM1ZPos.Value < 6.0f)
                {
                    roboM1Steps = RoboSteps.TAKE_PIECE_AWAY;
                    return (true);
                }
            }
            return (false);
        }

        public bool C2fromB1toM1()
        {
            if (roboM1Steps == RoboSteps.IDLE)
            {
                roboM1Grab.Value = false;
                roboM1Steps = RoboSteps.WAITING_FOR_PIECE;
            }
            else if (roboM1Steps == RoboSteps.WAITING_FOR_PIECE)
            {
                roboM1X.Value = 1.2f;
                if (sensorB1start.Value)
                {
                    conveyorB1.Value = true;
                    pieceFound = true;
                }
                else if ((!pieceFound || sensorB1end.Value) && Math.Abs(roboM1XPos.Value - roboM1X.Value) < 0.1f)
                {
                    conveyorB1.Value = false;
                    roboM1Steps = RoboSteps.DOWN_FOR_PIECE;
                    pieceFound = false;
                }
            }
            else if (roboM1Steps == RoboSteps.DOWN_FOR_PIECE)
            {
                roboM1Z.Value = 9.0f;
                if (roboM1ZPos.Value > 8.7f)
                {
                    roboM1Steps = RoboSteps.GRAB_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.GRAB_PIECE)
            {
                roboM1Grab.Value = true;
                MemoryMap.Instance.Update();
                Thread.Sleep(300);
                if (roboM1Grabbed.Value)
                {
                    roboM1Steps = RoboSteps.UP_WITH_PIECE;
                }
                else
                {
                    roboM1Steps = RoboSteps.IDLE;
                }
            }
            else if (roboM1Steps == RoboSteps.UP_WITH_PIECE)
            {
                roboM1Z.Value = 4.0f;
                if (roboM1ZPos.Value < 6.0f)
                {
                    roboM1Steps = RoboSteps.TO_DESTINATION;
                }
            }
            else if (roboM1Steps == RoboSteps.TO_DESTINATION)
            {
                roboM1X.Value = 5.0f;
                roboM1Y.Value = 9.5f;
                if (roboM1YPos.Value > 9.45f)
                {
                    roboM1Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                roboM1Z.Value = 8.1f;
                if (roboM1ZPos.Value > 8.0f)
                {
                    roboM1Steps = RoboSteps.RELEASE_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.RELEASE_PIECE)
            {
                roboM1Grab.Value = false;
                roboM1Steps = RoboSteps.UP_WITHOUT_PIECE;
            }
            else if (roboM1Steps == RoboSteps.UP_WITHOUT_PIECE)
            {
                roboM1Z.Value = 4.0f;
                if (roboM1ZPos.Value < 6.0f)
                {
                    roboM1Steps = RoboSteps.IDLE;
                    return (true);
                }
            }
            return (false);
        }

        public bool C2fromB2toM1()
        {
            if (roboM1Steps == RoboSteps.IDLE)
            {
                roboM1Grab.Value = false;
                roboM1Steps = RoboSteps.WAITING_FOR_PIECE;
            }
            else if (roboM1Steps == RoboSteps.WAITING_FOR_PIECE)
            {
                if (sensorB2start.Value)
                {
                    conveyorB2.Value = true;
                    pieceFound = true;
                }
                else if (!pieceFound || sensorB2end.Value)
                {
                    conveyorB2.Value = false;
                    roboM1Steps = RoboSteps.DOWN_FOR_PIECE;
                    pieceFound = false;
                }
            }
            else if (roboM1Steps == RoboSteps.DOWN_FOR_PIECE)
            {
                roboM1Z.Value = 9.0f;
                if (roboM1ZPos.Value > 8.7f)
                {
                    roboM1Steps = RoboSteps.GRAB_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.GRAB_PIECE)
            {
                roboM1Grab.Value = true;
                MemoryMap.Instance.Update();
                Thread.Sleep(300);
                if (roboM1Grabbed.Value)
                {
                    roboM1Steps = RoboSteps.UP_WITH_PIECE;
                }
                else
                {
                    roboM1Steps = RoboSteps.IDLE;
                }
            }
            else if (roboM1Steps == RoboSteps.UP_WITH_PIECE)
            {
                roboM1Z.Value = 4.0f;
                if (roboM1ZPos.Value < 6.0f)
                {
                    roboM1Steps = RoboSteps.TO_DESTINATION;
                }
            }
            else if (roboM1Steps == RoboSteps.TO_DESTINATION)
            {
                roboM1X.Value = 5.0f;
                roboM1Y.Value = 9.5f;
                if (roboM1YPos.Value > 9.45f)
                {
                    roboM1Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                roboM1Z.Value = 8.4f;
                if (roboM1ZPos.Value > 8.25f)
                {
                    roboM1Steps = RoboSteps.RELEASE_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.RELEASE_PIECE)
            {
                roboM1Grab.Value = false;
                roboM1Steps = RoboSteps.UP_WITHOUT_PIECE;
            }
            else if (roboM1Steps == RoboSteps.UP_WITHOUT_PIECE)
            {
                roboM1Z.Value = 4.0f;
                if (roboM1ZPos.Value < 6.0f)
                {
                    roboM1Steps = RoboSteps.IDLE;
                    return (true);
                }
            }
            return (false);
        }

        public bool C3fromB3toM1()
        {
            if (roboM1Steps == RoboSteps.IDLE)
            {
                roboM1Grab.Value = false;
                roboM1Steps = RoboSteps.WAITING_FOR_PIECE;
            }
            else if (roboM1Steps == RoboSteps.WAITING_FOR_PIECE)
            {
                roboM1X.Value = 9.2f;
                if (sensorB3start.Value)
                {
                    conveyorB3.Value = true;
                    pieceFound = true;
                }
                else if ((!pieceFound || sensorB3end.Value) && Math.Abs(roboM1XPos.Value - roboM1X.Value) < 0.1f)
                {
                    conveyorB3.Value = false;
                    roboM1Steps = RoboSteps.DOWN_FOR_PIECE;
                    pieceFound = false;
                }
            }
            else if (roboM1Steps == RoboSteps.DOWN_FOR_PIECE)
            {
                roboM1Z.Value = 9.0f;
                if (roboM1ZPos.Value > 8.7f)
                {
                    roboM1Steps = RoboSteps.GRAB_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.GRAB_PIECE)
            {
                roboM1Grab.Value = true;
                MemoryMap.Instance.Update();
                Thread.Sleep(300);
                if (roboM1Grabbed.Value)
                {
                    roboM1Steps = RoboSteps.UP_WITH_PIECE;
                }
                else
                {
                    roboM1Steps = RoboSteps.IDLE;
                }
            }
            else if (roboM1Steps == RoboSteps.UP_WITH_PIECE)
            {
                roboM1Z.Value = 4.0f;
                if (roboM1ZPos.Value < 6.0f)
                {
                    roboM1Steps = RoboSteps.TO_DESTINATION;
                }
            }
            else if (roboM1Steps == RoboSteps.TO_DESTINATION)
            {
                roboM1X.Value = 5.0f;
                roboM1Y.Value = 9.5f;
                if (roboM1YPos.Value > 9.45f)
                {
                    roboM1Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                roboM1Z.Value = 7.6f;
                if (roboM1ZPos.Value > 7.5f)
                {
                    roboM1Steps = RoboSteps.RELEASE_PIECE;
                }
            }
            else if (roboM1Steps == RoboSteps.RELEASE_PIECE)
            {
                roboM1Grab.Value = false;
                roboM1Steps = RoboSteps.UP_WITHOUT_PIECE;
            }
            else if (roboM1Steps == RoboSteps.UP_WITHOUT_PIECE)
            {
                roboM1Z.Value = 4.0f;
                if (roboM1ZPos.Value < 6.0f)
                {
                    roboM1Steps = RoboSteps.TAKE_PIECE_AWAY;
                    return (true);
                }
            }
            return (false);
        }

        public void Idle()
        {
            if (roboM1Steps == RoboSteps.TAKE_PIECE_AWAY)
            {
                conveyorM1.Value = true;
                if (sensorM1end.Value)
                {
                    conveyorM1.Value = false;
                    roboM1Steps = RoboSteps.IDLE;
                }
            }
            else
            {
                roboM1X.Value = 5.2f;
                roboM1Y.Value = 0.7f;
                roboM1Z.Value = 5.0f;
                roboM1Grab.Value = false;
            }
        }
    }
}

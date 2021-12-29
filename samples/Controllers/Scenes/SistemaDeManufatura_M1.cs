using EngineIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Controllers.Scenes
{
    class SistemaDeManufatura_M1
    {
        private enum RoboSteps
        {
            IDLE,
            DOWN_FOR_PIECE,
            SEARCHING_FOR_PIECE,
            GRAB_PIECE,
            UP_WITH_PIECE,
            TO_DESTINATION,
            DOWN_WITH_PIECE,
            RELEASE_PIECE,
            UP_WITHOUT_PIECE
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
        readonly MemoryBit startC1fromB1toM1;
        readonly MemoryBit startC2fromB1toM1;
        readonly MemoryBit startC2fromB2toM1;
        readonly MemoryBit startC3fromB3toM1;
        public SistemaDeManufatura_M1(MemoryFloat roboM1X, MemoryFloat roboM1XPos, MemoryFloat roboM1Y, MemoryFloat roboM1YPos, MemoryFloat roboM1Z, MemoryFloat roboM1ZPos, MemoryBit roboM1Grab, MemoryBit roboM1Grabbed, MemoryBit startC1fromB1toM1, MemoryBit startC2fromB1toM1, MemoryBit startC2fromB2toM1, MemoryBit startC3fromB3toM1)
        {
            this.roboM1X = roboM1X;
            this.roboM1XPos = roboM1XPos;
            this.roboM1Y = roboM1Y;
            this.roboM1YPos = roboM1YPos;
            this.roboM1Z = roboM1Z;
            this.roboM1ZPos = roboM1ZPos;
            this.roboM1Grab = roboM1Grab;
            this.roboM1Grabbed = roboM1Grabbed;
            this.startC1fromB1toM1 = startC1fromB1toM1;
            this.startC2fromB1toM1 = startC2fromB1toM1;
            this.startC2fromB2toM1 = startC2fromB2toM1;
            this.startC3fromB3toM1 = startC3fromB3toM1;
            roboM1Steps = RoboSteps.IDLE;
        }

        public void B1toM1()
        {
            if (roboM1Steps == RoboSteps.IDLE)
            {
                roboM1X.Value = 1.2f;
                roboM1Y.Value = 0.6f;
                roboM1Z.Value = 5.0f;
                if (startC1fromB1toM1.Value || startC2fromB1toM1.Value)
                {
                    roboM1Steps = RoboSteps.DOWN_FOR_PIECE;
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
                roboM1Steps = RoboSteps.UP_WITH_PIECE;
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
                if (roboM1YPos.Value > 9.4f)
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
                }
            }
        }

        public void B2toM1()
        {
            if (roboM1Steps == RoboSteps.IDLE)
            {
                roboM1X.Value = 5.0f;
                roboM1Y.Value = 0.6f;
                roboM1Z.Value = 5.0f;
                if (startC2fromB2toM1.Value)
                {
                    roboM1Steps = RoboSteps.DOWN_FOR_PIECE;
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
                roboM1Steps = RoboSteps.UP_WITH_PIECE;
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
                if (roboM1YPos.Value > 9.4f)
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
                }
            }
        }

        public void B3toM1()
        {
            if (roboM1Steps == RoboSteps.IDLE)
            {
                roboM1X.Value = 9.2f;
                roboM1Y.Value = 0.6f;
                roboM1Z.Value = 5.0f;
                if (startC2fromB2toM1.Value)
                {
                    roboM1Steps = RoboSteps.DOWN_FOR_PIECE;
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
                roboM1Steps = RoboSteps.UP_WITH_PIECE;
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
                if (roboM1YPos.Value > 9.4f)
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
                }
            }
        }

    }
}

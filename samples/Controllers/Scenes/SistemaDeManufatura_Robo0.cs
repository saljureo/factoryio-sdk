using EngineIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Controllers.Scenes
{
    class SistemaDeManufatura_Robo0
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
        RoboSteps robo0Steps;
        readonly MemoryFloat robo0X;
        readonly MemoryFloat robo0XPos;
        readonly MemoryFloat robo0Y;
        readonly MemoryFloat robo0YPos;
        readonly MemoryFloat robo0Z;
        readonly MemoryFloat robo0ZPos;
        readonly MemoryBit robo0Grab;
        readonly MemoryBit robo0Grabbed;
        readonly MemoryBit robo0RotatePiece;
        readonly MemoryBit startC1toB1;
        readonly MemoryBit startC2toB1;
        readonly MemoryBit startC2toB2;
        readonly MemoryBit startC3toB3;
        readonly MemoryBit startC1toE2;
        readonly MemoryBit startC2toE2;
        readonly MemoryBit startC3toE2;
        float highestYinSearch;
        float searchingForPieceYvalue;
        float pieceFoundYcoordinates;
        public SistemaDeManufatura_Robo0(MemoryFloat robo0X, MemoryFloat robo0XPos, MemoryFloat robo0Y, MemoryFloat robo0YPos, MemoryFloat robo0Z, MemoryFloat robo0ZPos, MemoryBit robo0Grab, MemoryBit robo0Grabbed, MemoryBit robo0RotatePiece, MemoryBit startC1toB1, MemoryBit startC2toB1, MemoryBit startC2toB2, MemoryBit startC3toB3, MemoryBit startC1toE2, MemoryBit startC2toE2, MemoryBit startC3toE2)
        {
            this.robo0X = robo0X;
            this.robo0XPos = robo0XPos;
            this.robo0Y = robo0Y;
            this.robo0YPos = robo0YPos;
            this.robo0Z = robo0Z;
            this.robo0ZPos = robo0ZPos;
            this.robo0Grab = robo0Grab;
            this.robo0Grabbed = robo0Grabbed;
            this.robo0RotatePiece = robo0RotatePiece;
            this.startC1toB1 = startC1toB1;
            this.startC2toB1 = startC2toB1;
            this.startC2toB2 = startC2toB2;
            this.startC3toB3 = startC3toB3;
            this.startC1toE2 = startC1toE2;
            this.startC2toE2 = startC2toE2;
            this.startC3toE2 = startC3toE2;
            robo0Steps = RoboSteps.IDLE;
        }

        public void E1toB1()
        {
            if (robo0Steps == RoboSteps.IDLE)
            {
                robo0X.Value = 1.2f;
                robo0Y.Value = 0.0f;
                robo0Z.Value = 5.0f;
                highestYinSearch = 13.0f;
                searchingForPieceYvalue = 0.0f;
                if (startC1toB1.Value || startC2toB1.Value)
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
                robo0Y.Value = searchingForPieceYvalue;
                if (Math.Abs(robo0YPos.Value - robo0Y.Value) < 0.1)
                {
                    if (robo0Grabbed.Value)
                    {
                        highestYinSearch = searchingForPieceYvalue;
                    }
                    searchingForPieceYvalue += 0.8f;
                }
                if (searchingForPieceYvalue > 5.5f)
                {
                    if (highestYinSearch == 13.0f)
                    {
                        robo0Steps = RoboSteps.IDLE;
                    }
                    else
                    {
                        robo0Steps = RoboSteps.GRAB_PIECE;
                        robo0Z.Value = 6.9f;
                        robo0X.Value = 0.9f;
                        pieceFoundYcoordinates = Math.Abs(highestYinSearch - 1.3f);
                    }
                }
            }
            else if (robo0Steps == RoboSteps.GRAB_PIECE)
            {
                if (robo0ZPos.Value < 8.0f)
                {
                    robo0Y.Value = pieceFoundYcoordinates;
                    if (Math.Abs(robo0Y.Value - robo0YPos.Value) < 0.01)
                    {
                        robo0Z.Value = 9.0f;
                        robo0Grab.Value = true;
                        MemoryMap.Instance.Update();
                        Thread.Sleep(300);
                        robo0Steps = RoboSteps.UP_WITH_PIECE;
                    }
                }
            }
            else if (robo0Steps == RoboSteps.UP_WITH_PIECE)
            {
                robo0Z.Value = 4.0f;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.TO_DESTINATION;
                }
            }
            else if (robo0Steps == RoboSteps.TO_DESTINATION)
            {
                robo0X.Value = 1.2f;
                robo0Y.Value = 9.5f;
                if (robo0YPos.Value > 9.4f)
                {
                    robo0Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                robo0Z.Value = 8.4f;
                if (robo0ZPos.Value > 8.25f)
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
                robo0RotatePiece.Value = false;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.IDLE;
                }
            }
        }

        public void E1toB2()
        {
            if (robo0Steps == RoboSteps.IDLE)
            {
                robo0X.Value = 1.2f;
                robo0Y.Value = 0.0f;
                robo0Z.Value = 5.0f;
                highestYinSearch = 13.0f;
                searchingForPieceYvalue = 0.0f;
                if (startC2toB2.Value)
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
                robo0Y.Value = searchingForPieceYvalue;
                if (Math.Abs(robo0YPos.Value - robo0Y.Value) < 0.1)
                {
                    if (robo0Grabbed.Value)
                    {
                        highestYinSearch = searchingForPieceYvalue;
                    }
                    searchingForPieceYvalue += 0.8f;
                }
                if (searchingForPieceYvalue > 5.5f)
                {
                    if (highestYinSearch == 13.0f)
                    {
                        robo0Steps = RoboSteps.IDLE;
                    }
                    else
                    {
                        robo0Steps = RoboSteps.GRAB_PIECE;
                        robo0Z.Value = 6.9f;
                        robo0X.Value = 0.9f;
                        pieceFoundYcoordinates = Math.Abs(highestYinSearch - 1.3f);
                    }
                }
            }
            else if (robo0Steps == RoboSteps.GRAB_PIECE)
            {
                if (robo0ZPos.Value < 8.0f)
                {
                    robo0Y.Value = pieceFoundYcoordinates;
                    if (Math.Abs(robo0Y.Value - robo0YPos.Value) < 0.01)
                    {
                        robo0Z.Value = 9.0f;
                        robo0Grab.Value = true;
                        MemoryMap.Instance.Update();
                        Thread.Sleep(300);
                        robo0Steps = RoboSteps.UP_WITH_PIECE;
                    }
                }
            }
            else if (robo0Steps == RoboSteps.UP_WITH_PIECE)
            {
                robo0Z.Value = 4.0f;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.TO_DESTINATION;
                }
            }
            else if (robo0Steps == RoboSteps.TO_DESTINATION)
            {
                robo0X.Value = 5.2f;
                robo0Y.Value = 9.5f;
                if (robo0YPos.Value > 9.4f)
                {
                    robo0Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                robo0Z.Value = 8.4f;
                if (robo0ZPos.Value > 8.25f)
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
                robo0RotatePiece.Value = false;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.IDLE;
                }
            }
        }

        public void E1toB3()
        {
            if (robo0Steps == RoboSteps.IDLE)
            {
                robo0X.Value = 1.2f;
                robo0Y.Value = 0.0f;
                robo0Z.Value = 5.0f;
                highestYinSearch = 13.0f;
                searchingForPieceYvalue = 0.0f;
                if (startC3toB3.Value)
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
                robo0Y.Value = searchingForPieceYvalue;
                if (Math.Abs(robo0YPos.Value - robo0Y.Value) < 0.1)
                {
                    if (robo0Grabbed.Value)
                    {
                        highestYinSearch = searchingForPieceYvalue;
                    }
                    searchingForPieceYvalue += 0.8f;
                }
                if (searchingForPieceYvalue > 5.5f)
                {
                    if (highestYinSearch == 13.0f)
                    {
                        robo0Steps = RoboSteps.IDLE;
                    }
                    else
                    {
                        robo0Steps = RoboSteps.GRAB_PIECE;
                        robo0Z.Value = 6.9f;
                        robo0X.Value = 0.9f;
                        pieceFoundYcoordinates = Math.Abs(highestYinSearch - 1.3f);
                    }
                }
            }
            else if (robo0Steps == RoboSteps.GRAB_PIECE)
            {
                if (robo0ZPos.Value < 8.0f)
                {
                    robo0Y.Value = pieceFoundYcoordinates;
                    if (Math.Abs(robo0Y.Value - robo0YPos.Value) < 0.01)
                    {
                        robo0Z.Value = 9.0f;
                        robo0Grab.Value = true;
                        MemoryMap.Instance.Update();
                        Thread.Sleep(300);
                        robo0Steps = RoboSteps.UP_WITH_PIECE;
                    }
                }
            }
            else if (robo0Steps == RoboSteps.UP_WITH_PIECE)
            {
                robo0Z.Value = 4.0f;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.TO_DESTINATION;
                }
            }
            else if (robo0Steps == RoboSteps.TO_DESTINATION)
            {
                robo0X.Value = 9.1f;
                robo0Y.Value = 9.5f;
                if (robo0YPos.Value > 9.4f)
                {
                    robo0Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                robo0Z.Value = 8.4f;
                if (robo0ZPos.Value > 8.25f)
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
                robo0RotatePiece.Value = false;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.IDLE;
                }
            }
        }

        public void E1toE2()
        {
            if (robo0Steps == RoboSteps.IDLE)
            {
                robo0X.Value = 1.2f;
                robo0Y.Value = 0.0f;
                robo0Z.Value = 5.0f;                
                highestYinSearch = 13.0f;
                searchingForPieceYvalue = 0.0f;
                if (startC1toE2.Value || startC2toE2.Value || startC3toE2.Value)
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
                robo0Y.Value = searchingForPieceYvalue;
                if (Math.Abs(robo0YPos.Value - robo0Y.Value) < 0.1)
                {
                    if (robo0Grabbed.Value)
                    {
                        highestYinSearch = searchingForPieceYvalue;
                    }
                    searchingForPieceYvalue += 0.8f;
                }
                if (searchingForPieceYvalue > 5.5f)
                {
                    if (highestYinSearch == 13.0f)
                    {
                        robo0Steps = RoboSteps.IDLE;
                    }
                    else
                    {
                        robo0Steps = RoboSteps.GRAB_PIECE;
                        robo0Z.Value = 6.9f;
                        robo0X.Value = 0.9f;
                        pieceFoundYcoordinates = Math.Abs(highestYinSearch - 1.3f);
                    }
                }
            }
            else if (robo0Steps == RoboSteps.GRAB_PIECE)
            {
                if (robo0ZPos.Value < 8.0f)
                {
                    robo0Y.Value = pieceFoundYcoordinates;
                    if (Math.Abs(robo0Y.Value - robo0YPos.Value) < 0.01)
                    {
                        robo0Z.Value = 9.0f;
                        robo0Grab.Value = true;
                        MemoryMap.Instance.Update();
                        Thread.Sleep(300);
                        robo0Steps = RoboSteps.UP_WITH_PIECE;
                    }
                }
            }
            else if (robo0Steps == RoboSteps.UP_WITH_PIECE)
            {
                robo0Z.Value = 4.0f;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.TO_DESTINATION;
                }
            }
            else if (robo0Steps == RoboSteps.TO_DESTINATION)
            {
                robo0X.Value = 9.25f;
                robo0Y.Value = 2.3f;
                robo0RotatePiece.Value = true;
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
                robo0RotatePiece.Value = false;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.IDLE;
                }
            }
        }

    }
}

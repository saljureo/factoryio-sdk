using EngineIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Controllers.Scenes.SistemaDeManufatura
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
        float highestYinSearch;
        float searchingForPieceYvalue;
        float pieceFoundYcoordinates;
        readonly MemoryBit stopbladeEndE1;
        readonly SistemaDeManufaturaSupervisor sistemaDeManufaturaSupervisor;
        public SistemaDeManufatura_Robo0(MemoryFloat robo0X, MemoryFloat robo0XPos, MemoryFloat robo0Y, MemoryFloat robo0YPos, MemoryFloat robo0Z, MemoryFloat robo0ZPos, MemoryBit robo0Grab, MemoryBit robo0Grabbed, MemoryBit robo0RotatePiece, MemoryBit stopbladeEndE1, SistemaDeManufaturaSupervisor sistemaDeManufaturaSupervisor)
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
            this.stopbladeEndE1 = stopbladeEndE1;
            this.sistemaDeManufaturaSupervisor = sistemaDeManufaturaSupervisor;
            robo0Steps = RoboSteps.IDLE;
        }

        public void Idle()
        {
            robo0X.Value = 1.2f;
            robo0Y.Value = 3.5f;
            robo0Z.Value = 5.0f;
        }
        public bool E1toB1(string color)
        {
            if (robo0Steps == RoboSteps.IDLE)
            {
                highestYinSearch = 13.0f;
                searchingForPieceYvalue = 0.0f;
                robo0Steps = RoboSteps.DOWN_FOR_PIECE;
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
                robo0Y.Value = 5.8f;
                while (robo0YPos.Value < 5.5f)
                {
                    MemoryMap.Instance.Update();
                    if (robo0Grabbed.Value)
                        highestYinSearch = robo0YPos.Value;
                }
                if (robo0YPos.Value > 5.5f)
                {
                    if (highestYinSearch == 13.0f)
                    {
                        robo0Steps = RoboSteps.IDLE;
                    }
                    else
                    {
                        robo0Steps = RoboSteps.GRAB_PIECE;
                        robo0Z.Value = 6.9f;
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
                        robo0Z.Value = 8.7f;
                        robo0X.Value = 0.84f;
                        if (robo0X.Value < 0.841)
                        {
                            robo0Grab.Value = true;
                            stopbladeEndE1.Value = false;
                            MemoryMap.Instance.Update();
                            Thread.Sleep(300);
                            robo0Steps = RoboSteps.UP_WITH_PIECE;
                        }
                    }
                }
            }
            else if (robo0Steps == RoboSteps.UP_WITH_PIECE)
            {
                robo0Z.Value = 4.0f;
                if (robo0ZPos.Value < 4.1f)
                {
                    robo0Steps = RoboSteps.TO_DESTINATION;
                }
            }
            else if (robo0Steps == RoboSteps.TO_DESTINATION)
            {
                stopbladeEndE1.Value = true;
                robo0X.Value = 0.9f;
                robo0Y.Value = 9.5f;
                if (robo0YPos.Value > 9.4f)
                {
                    robo0Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                robo0Z.Value = 8.7f;
                if (robo0ZPos.Value > 8.6f)
                {
                    robo0Steps = RoboSteps.RELEASE_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.RELEASE_PIECE)
            {
                robo0Grab.Value = false;
                MemoryMap.Instance.Update();
                Thread.Sleep(300);
                robo0Steps = RoboSteps.UP_WITHOUT_PIECE;
                if (color == "c1")
                {
                    sistemaDeManufaturaSupervisor.On("r_okc1b1");
                }
                else if (color == "c2")
                {
                    sistemaDeManufaturaSupervisor.On("r_okc2b1");
                }
                Thread.Sleep(50);
            }
            else if (robo0Steps == RoboSteps.UP_WITHOUT_PIECE)
            {
                robo0Z.Value = 4.0f;
                robo0RotatePiece.Value = false;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.IDLE;
                    return (true);
                }
            }
            return (false);
        }

        public bool E1toB2()
        {
            if (robo0Steps == RoboSteps.IDLE)
            {
                highestYinSearch = 13.0f;
                searchingForPieceYvalue = 0.0f;
                robo0Steps = RoboSteps.DOWN_FOR_PIECE;
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
                robo0Y.Value = 5.8f;
                while (robo0YPos.Value < 5.5f)
                {
                    MemoryMap.Instance.Update();
                    if (robo0Grabbed.Value)
                        highestYinSearch = robo0YPos.Value;
                }
                if (robo0YPos.Value > 5.5f)
                {
                    if (highestYinSearch == 13.0f)
                    {
                        robo0Steps = RoboSteps.IDLE;
                    }
                    else
                    {
                        robo0Steps = RoboSteps.GRAB_PIECE;
                        robo0Z.Value = 6.9f;
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
                        robo0Z.Value = 8.7f;
                        robo0X.Value = 0.84f;
                        if (robo0X.Value < 0.841f)
                        {
                            robo0Grab.Value = true;
                            stopbladeEndE1.Value = false;
                            MemoryMap.Instance.Update();
                            Thread.Sleep(300);
                            robo0Steps = RoboSteps.UP_WITH_PIECE;
                        }
                    }
                }
            }
            else if (robo0Steps == RoboSteps.UP_WITH_PIECE)
            {
                robo0Z.Value = 4.0f;
                if (robo0ZPos.Value < 4.5f)
                {
                    robo0Steps = RoboSteps.TO_DESTINATION;
                }
            }
            else if (robo0Steps == RoboSteps.TO_DESTINATION)
            {
                stopbladeEndE1.Value = true;
                robo0X.Value = 5.0f;
                robo0Y.Value = 9.5f;
                if (robo0YPos.Value > 9.4f)
                {
                    robo0Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                robo0Z.Value = 8.7f;
                if (robo0ZPos.Value > 8.6f)
                {
                    robo0Steps = RoboSteps.RELEASE_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.RELEASE_PIECE)
            {
                robo0Grab.Value = false;
                MemoryMap.Instance.Update();
                Thread.Sleep(100);
                robo0Steps = RoboSteps.UP_WITHOUT_PIECE;
                sistemaDeManufaturaSupervisor.On("r_okc2b2");
            }
            else if (robo0Steps == RoboSteps.UP_WITHOUT_PIECE)
            {
                robo0Z.Value = 4.0f;
                robo0RotatePiece.Value = false;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.IDLE;
                    return (true);
                }
            }
            return (false);
        }

        public bool E1toB3()
        {
            if (robo0Steps == RoboSteps.IDLE)
            {
                highestYinSearch = 13.0f;
                searchingForPieceYvalue = 0.0f;
                robo0Steps = RoboSteps.DOWN_FOR_PIECE;
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
                robo0Y.Value = 5.8f;
                while (robo0YPos.Value < 5.5f)
                {
                    MemoryMap.Instance.Update();
                    if (robo0Grabbed.Value)
                        highestYinSearch = robo0YPos.Value;
                }
                if (robo0YPos.Value > 5.5f)
                {
                    if (highestYinSearch == 13.0f)
                    {
                        robo0Steps = RoboSteps.IDLE;
                    }
                    else
                    {
                        robo0Steps = RoboSteps.GRAB_PIECE;
                        robo0Z.Value = 6.9f;
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
                        robo0Z.Value = 8.7f;
                        robo0X.Value = 0.84f;
                        if (robo0X.Value < 0.841f)
                        {
                            robo0Grab.Value = true;
                            stopbladeEndE1.Value = false;
                            MemoryMap.Instance.Update();
                            Thread.Sleep(300);
                            robo0Steps = RoboSteps.UP_WITH_PIECE;
                        }
                    }
                }
            }
            else if (robo0Steps == RoboSteps.UP_WITH_PIECE)
            {
                robo0Z.Value = 2.0f;
                if (robo0ZPos.Value < 2.5f)
                {
                    robo0Steps = RoboSteps.TO_DESTINATION;
                }
            }
            else if (robo0Steps == RoboSteps.TO_DESTINATION)
            {
                stopbladeEndE1.Value = true;
                robo0X.Value = 9.2f;
                robo0Y.Value = 9.5f;
                if (robo0YPos.Value > 9.4f && robo0X.Value > 9.1f)
                {
                    robo0Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                robo0Z.Value = 8.7f;
                if (robo0ZPos.Value > 8.6f)
                {
                    robo0Steps = RoboSteps.RELEASE_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.RELEASE_PIECE)
            {
                robo0Grab.Value = false;
                MemoryMap.Instance.Update();
                Thread.Sleep(100);
                robo0Steps = RoboSteps.UP_WITHOUT_PIECE;
                sistemaDeManufaturaSupervisor.On("r_okc3b3");
            }
            else if (robo0Steps == RoboSteps.UP_WITHOUT_PIECE)
            {
                robo0Z.Value = 4.0f;
                robo0RotatePiece.Value = false;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.IDLE;
                    return (true);
                }
            }
            return (false);
        }

        public bool E1toE2()
        {
            if (robo0Steps == RoboSteps.IDLE)
            {
                highestYinSearch = 13.0f;
                searchingForPieceYvalue = 0.0f;
                robo0Steps = RoboSteps.DOWN_FOR_PIECE;
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
                robo0Y.Value = 5.8f;
                while (robo0YPos.Value < 5.5f)
                {
                    MemoryMap.Instance.Update();
                    if (robo0Grabbed.Value)
                        highestYinSearch = robo0YPos.Value;
                }
                if (robo0YPos.Value > 5.5f)
                {
                    if (highestYinSearch == 13.0f)
                    {
                        robo0Steps = RoboSteps.IDLE;
                    }
                    else
                    {
                        robo0Steps = RoboSteps.GRAB_PIECE;
                        robo0Z.Value = 6.9f;
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
                        robo0Z.Value = 8.7f;
                        robo0X.Value = 0.84f;
                        if (robo0X.Value < 0.841f)
                        {
                            robo0Grab.Value = true;
                            stopbladeEndE1.Value = false;
                            MemoryMap.Instance.Update();
                            Thread.Sleep(300);
                            robo0Steps = RoboSteps.UP_WITH_PIECE;
                        }
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
                stopbladeEndE1.Value = true;
                robo0X.Value = 9.25f;
                robo0Y.Value = 2.5f;
                robo0RotatePiece.Value = true;
                if (robo0XPos.Value > 9.24f && robo0YPos.Value > 2.09f)
                {
                    robo0Steps = RoboSteps.DOWN_WITH_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.DOWN_WITH_PIECE)
            {
                robo0Z.Value = 8.7f;
                if (robo0ZPos.Value > 8.6f)
                {
                    robo0Steps = RoboSteps.RELEASE_PIECE;
                }
            }
            else if (robo0Steps == RoboSteps.RELEASE_PIECE)
            {
                robo0Grab.Value = false;
                robo0Steps = RoboSteps.UP_WITHOUT_PIECE;
                sistemaDeManufaturaSupervisor.On("r_oke2");
            }
            else if (robo0Steps == RoboSteps.UP_WITHOUT_PIECE)
            {
                robo0Z.Value = 4.0f;
                robo0RotatePiece.Value = false;
                if (robo0ZPos.Value < 6.0f)
                {
                    robo0Steps = RoboSteps.IDLE;
                    return (true);
                }
            }
            return (false);
        }

    }
}

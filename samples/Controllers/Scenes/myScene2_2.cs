//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using Controllers.Scenes;
using EngineIO;
using System;

namespace Controllers
{
    public class myScene2_2 : Controller
    {
        //Mc0
        MemoryBit mc0StartButton = MemoryMap.Instance.GetBit("Start Button 0", MemoryType.Input);
        //MemoryBit mc0FailButton = MemoryMap.Instance.GetBit("Stop Button 0", MemoryType.Input);
        MemoryBit mc0RepairButton = MemoryMap.Instance.GetBit("Reset Button 0", MemoryType.Input);
        MemoryBit mc0Busy = MemoryMap.Instance.GetBit("Machining Center 0 (Is Busy)", MemoryType.Input);//Machining Center 0 busy
        MemoryBit mc0Open = MemoryMap.Instance.GetBit("Machining Center 0 (Opened)", MemoryType.Input);//Machining Center 0 opened
        MemoryInt mc0Progress = MemoryMap.Instance.GetInt("Machining Center 0 (Progress)", MemoryType.Input);//Machining Center 0 opened
        MemoryBit mc0PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 0 (Clamped)", MemoryType.Input);//Mc0 positioner clamped sensor
        MemoryBit mc0GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Item Detected)", MemoryType.Input);//Mc0 positioner item detected sensor
        
        MemoryBit mc0Start = MemoryMap.Instance.GetBit("Machining Center 0 (Start)", MemoryType.Output);//Machining Center start
        MemoryBit mc0Reset = MemoryMap.Instance.GetBit("Machining Center 0 (Reset)", MemoryType.Output);//Machining Center reset (so it leaves piece incomplete)
        //MemoryBit mc0Fail = MemoryMap.Instance.GetBit("Machining Center 0 (Stop)", MemoryType.Output);//Machining Center start
        MemoryBit mc0RedLight = MemoryMap.Instance.GetBit("Stack Light 0 (Red)", MemoryType.Output);
        MemoryBit mc0YellowLight = MemoryMap.Instance.GetBit("Stack Light 0 (Yellow)", MemoryType.Output);
        MemoryBit mc0GreenLight = MemoryMap.Instance.GetBit("Stack Light 0 (Green)", MemoryType.Output);
        MemoryBit mc0AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 0", MemoryType.Output);
        MemoryBit mc0PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 0 (Raise)", MemoryType.Output);//mc0 positioner rise
        MemoryBit mc0PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 0 (Clamp)", MemoryType.Output);//mc0 positioner clamp
        


        //Sensors
        MemoryBit sensorEmitter = MemoryMap.Instance.GetBit("Diffuse Sensor 0", MemoryType.Input);//Diffuse Sensor 0 - Emitter
        MemoryBit sensorEntranceMc0 = MemoryMap.Instance.GetBit("Diffuse Sensor 1", MemoryType.Input);//Diffuse Sensor 1 - MC0 entrance
        MemoryBit sensorExitMc0 = MemoryMap.Instance.GetBit("Diffuse Sensor 2", MemoryType.Input);//Diffuse Sensor 2 - MC0 exit/Buffer conveyor entry/MC0 bad piece filter
        MemoryBit sensorBufferEnd = MemoryMap.Instance.GetBit("Diffuse Sensor 3", MemoryType.Input);//Diffuse Sensor 3 - Buffer end
        MemoryBit sensorMc1LoadingConveyorStart = MemoryMap.Instance.GetBit("Diffuse Sensor 3_2", MemoryType.Input);//Diffuse Sensor 3_2 - Buffer start
        MemoryBit sensorEntranceMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 5", MemoryType.Input);//Diffuse Sensor 5 - MC1 entrance
        MemoryBit sensorBadPieceFilterConveyorStartMc0 = MemoryMap.Instance.GetBit("Diffuse Sensor 6", MemoryType.Input);//Diffuse Sensor 6 - MC0 bad piece filter conveyor entrance
        MemoryBit sensorBadPieceFilterConveyorEndMc0 = MemoryMap.Instance.GetBit("Diffuse Sensor 7", MemoryType.Input);//Diffuse Sensor 7 - MC0 bad piece filter conveyor exit
        MemoryBit sensorExitMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 8", MemoryType.Input);//Diffuse Sensor 8 - MC1 exit/MC1 bad piece filter
        MemoryBit sensorFinishedPartExit = MemoryMap.Instance.GetBit("Diffuse Sensor 9", MemoryType.Input);//Diffuse Sensor 9 - Finished piece exit
        MemoryBit sensorBadPieceFilterConveyorStartMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 10", MemoryType.Input);//Diffuse Sensor 10 - MC1 bad piece conveyor entrance
        MemoryBit sensorBadPieceFilterConveyorEndMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);//Diffuse Sensor 11 - MC1 bad piece conveyor exit
        MemoryBit sensorBufferSpot2 = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);//Diffuse Sensor 13 - spot 2 on buffer

        //Mc1
        MemoryBit mc1StartButton = MemoryMap.Instance.GetBit("Start Button 1", MemoryType.Input); //MC start button
        MemoryBit mc1FailButton = MemoryMap.Instance.GetBit("Stop Button 1", MemoryType.Input); //MC fail button
        MemoryBit mc1RepairButton = MemoryMap.Instance.GetBit("Reset Button 1", MemoryType.Input); //MC repair button
        MemoryBit mc1Busy = MemoryMap.Instance.GetBit("Machining Center 1 (Is Busy)", MemoryType.Input);//Machining Center 1 busy
        MemoryBit mc1PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamped)", MemoryType.Input);//Mc1 positioner clamped sensor
        MemoryBit mc1GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 1 (Item Detected)", MemoryType.Input);//Mc1 positioner item detected sensor

        MemoryBit mc1Start = MemoryMap.Instance.GetBit("Machining Center 1 (Start)", MemoryType.Output);//Machining Center 1 start
        MemoryBit mc1RedLight = MemoryMap.Instance.GetBit("Stack Light 1 (Red)", MemoryType.Output);
        MemoryBit mc1YellowLight = MemoryMap.Instance.GetBit("Stack Light 1 (Yellow)", MemoryType.Output);
        MemoryBit mc1GreenLight = MemoryMap.Instance.GetBit("Stack Light 1 (Green)", MemoryType.Output);
        MemoryBit mc1AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 1", MemoryType.Output);
        MemoryBit mc1PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamp)", MemoryType.Output);//mc1 positioner clamp
        MemoryBit mc1PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 3 (Raise)", MemoryType.Output);//mc1 positioner rise

        //Emitter
        MemoryBit emitterStartButton = MemoryMap.Instance.GetBit("Start Button 2", MemoryType.Input); //Emitter start button
        MemoryBit emitter = MemoryMap.Instance.GetBit("Emitter 0 (Emit)", MemoryType.Output);//Emitter

        GripperArm gripperMc0 = new GripperArm(
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Position (V)", MemoryType.Input),
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Position (V)", MemoryType.Input),
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Set Point (V)", MemoryType.Output),
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Set Point (V)", MemoryType.Output),
            MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Grab)", MemoryType.Output)
        );

        GripperArm gripperMc1 = new GripperArm(
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 X Position (V)", MemoryType.Input),
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 Z Position (V)", MemoryType.Input),
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 X Set Point (V)", MemoryType.Output),
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 Z Set Point (V)", MemoryType.Output),
            MemoryMap.Instance.GetBit("Two-Axis Pick & Place 1 (Grab)", MemoryType.Output)
        );

        //conveyor belts
        MemoryBit conveyorMc1Entrance = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 0", MemoryType.Output);//Conveyor mc1 entrance
        MemoryBit conveyorMc0BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 1", MemoryType.Output);//Conveyor mc0 bad piece
        MemoryBit conveyorFinishedPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 2", MemoryType.Output);//Conveyor finished piece
        MemoryBit conveyorMc1BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 3", MemoryType.Output);//Conveyor mc1 bad piece
        MemoryBit conveyorEmitter = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 0", MemoryType.Output);//Conveyor emitter
        MemoryBit conveyorBuffer = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 1", MemoryType.Output);//Conveyor buffer

        //Buffer
        MemoryBit bufferStopblade = MemoryMap.Instance.GetBit("Stop Blade 0", MemoryType.Output);//Buffer stopblade

        bool pieceReadyAtMc0;
        bool mc0Failed;
        bool mc1Failed;

        McStatus mc0Status = McStatus.IDLE;
        McStatus mc1Status = McStatus.IDLE;

        McPositionerStatus mc0PositionerStatus = McPositionerStatus.UP;
        McPositionerStatus mc1PositionerStatus = McPositionerStatus.UP;

        BufferStatus bufferStatus = BufferStatus.EMPTY;

        LoadingMc1Step loadingMc1Step = LoadingMc1Step.IDLE;

        FTRIG ftAtEntranceMc0 = new FTRIG();
        FTRIG ftAtExitMc0 = new FTRIG();
        FTRIG ftAtExitMc1 = new FTRIG();
        FTRIG ftAtBufferEnd = new FTRIG();
        FTRIG ftAtBadPieceExitMc0 = new FTRIG();
        FTRIG ftAtBadPieceExitMc1 = new FTRIG();
        FTRIG ftAtFinishedPieceExit = new FTRIG();
        FTRIG ftAtBufferStart = new FTRIG();
        FTRIG ftAtMc1LoadingConveyorStart = new FTRIG();

        public myScene2_2()// %%%%%%%%%%%%%%%%% CONSTRUCTOR STARTS %%%%%%%%%%%%%%%%
        {
            Console.WriteLine("State mc0Status: " + mc0Status);
            Console.WriteLine("State mc1Status: " + mc1Status);
            Console.WriteLine("State bufferStatus: " + bufferStatus);
            ////mc0
            //mc0Start.Value = false;

            ////mc0 buttons
            mc0StartButton.Value = false;
            //mc0FailButton.Value = true;//unpressed is true
            mc0RepairButton.Value = false;

            ////mc0 lights
            //mc0RedLight.Value = false;
            //mc0YellowLight.Value = false;
            //mc0GreenLight.Value = true;

            //mc0 others
            pieceReadyAtMc0 = false;
            mc0PositionerClamp.Value = false;
            mc0PositionerRise.Value = true;

            ////Emitter
            emitter.Value = true;

            //mc1
            mc1Failed = false;

            ////mc1 buttons            
            //mc1FailButton.Value = true;//True is unpressed
            mc1StartButton.Value = false;
            mc1RepairButton.Value = false;

            ////mc1 lights
            //mc1RedLight.Value = false; 
            //mc1YellowLight.Value = false;
            //mc1GreenLight.Value = true;

            //Buffer
            bufferStopblade.Value = true;//True is rised


        } // %%%%%%%%%%%%%%%%% CONSTRUCTOR ENDS %%%%%%%%%%%%%%%%

        public override void Execute(int elapsedMilliseconds) // %%%%%%%%%%%%%%%%% EXECUTE STARTS %%%%%%%%%%%%%%%%
        {
            

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FALLING TRIGGERS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // Falling triggers
            ftAtEntranceMc0.CLK(sensorEntranceMc0.Value);
            ftAtExitMc0.CLK(sensorExitMc0.Value);
            ftAtExitMc1.CLK(sensorExitMc1.Value);
            ftAtBufferEnd.CLK(sensorBufferEnd.Value);
            ftAtBufferStart.CLK(sensorMc1LoadingConveyorStart.Value);
            ftAtBadPieceExitMc0.CLK(sensorBadPieceFilterConveyorEndMc0.Value);
            ftAtBadPieceExitMc1.CLK(sensorBadPieceFilterConveyorEndMc1.Value);
            ftAtFinishedPieceExit.CLK(sensorFinishedPartExit.Value);
            ftAtMc1LoadingConveyorStart.CLK(sensorMc1LoadingConveyorStart.Value);

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FALLING TRIGGERS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONVEYORS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //Emitter conveyor
            if (sensorEmitter.Value == true && sensorEntranceMc0.Value == false)
            {
                conveyorEmitter.Value = true;
            }
            else if (ftAtEntranceMc0.Q == true)
            {
                conveyorEmitter.Value = false;
                pieceReadyAtMc0 = true;
                Console.WriteLine("pieceReadyAtMc0 = " + pieceReadyAtMc0);
            }

            //Buffer conveyor
            if (sensorExitMc0.Value == true)
            {
                conveyorBuffer.Value = true;
            }
            else if (bufferStatus == BufferStatus.EMPTY && sensorBufferEnd.Value == true)
            {
                conveyorBuffer.Value = false;
            }
            else if (bufferStatus == BufferStatus.FULL && sensorBufferSpot2.Value == true)
            {
                conveyorBuffer.Value = false;
            }

            //Buffer conveyor
            if (sensorExitMc0.Value == true)
            {
                conveyorBuffer.Value = true;
            }
            else if (sensorBufferEnd.Value == true && loadingMc1Step == LoadingMc1Step.IDLE)
            {
                conveyorBuffer.Value = false;
            }

            //Mc0 bad piece conveyor
            if (sensorBadPieceFilterConveyorStartMc0.Value == true)
            {
                conveyorMc0BadPiece.Value = true;
            }
            else if (ftAtBadPieceExitMc0.Q == true)
            {
                conveyorMc0BadPiece.Value = false;
            }

            //Finished piece conveyor
            if (sensorExitMc1.Value == true)
            {
                conveyorFinishedPiece.Value = true;
            }
            else if (ftAtFinishedPieceExit.Q == true)
            {
                conveyorFinishedPiece.Value = false;
            }

            //Mc1 bad piece conveyor
            if (sensorBadPieceFilterConveyorStartMc1.Value == true)
            {
                conveyorMc1BadPiece.Value = true;
            }
            else if (ftAtBadPieceExitMc1.Q == true)
            {
                conveyorMc1BadPiece.Value = false;
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONVEYORS ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (bufferStatus == BufferStatus.EMPTY)
            {
                //type here
                if (sensorExitMc0.Value == true && mc0Failed == false)
                {
                    bufferStatus = BufferStatus.FULL;
                    Console.WriteLine("State bufferStatus: " + bufferStatus);
                }
            }
            else if (bufferStatus == BufferStatus.FULL)
            {
                //type here
                if (sensorEntranceMc1.Value == true)
                {
                    bufferStatus = BufferStatus.EMPTY;
                    Console.WriteLine("State bufferStatus: " + bufferStatus);
                }
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER ENDS   %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% MACHINE CENTER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%%% MC0 STARTS %%%%%%%%%%%%%%%%%%%

            

            if (mc0Status == McStatus.IDLE)
            {
                if (mc0StartButton.Value == true && pieceReadyAtMc0 == true)
                {
                    mc0Start.Value = true;
                }

                if (mc0Busy.Value == true)
                {
                    mc0Status = McStatus.WORKING;
                    Console.WriteLine("State mc0Status: " + mc0Status);
                    pieceReadyAtMc0 = false;
                    mc0Start.Value = false;
                    Console.WriteLine("pieceReadyAtMc0 = " + pieceReadyAtMc0);
                }
            }
            else if (mc0Status == McStatus.WORKING)
            {
                if (mc0Progress.Value == 90)
                {
                    mc0Reset.Value = true;
                }

                if (mc0Busy.Value == false && mc0Failed == false)
                {
                    mc0Reset.Value = false;
                    mc0Status = McStatus.IDLE;
                    Console.WriteLine("State mc0Status: " + mc0Status);
                    mc0Failed = true; //will fail next time
                }
                else if (mc0Busy.Value == false && mc0Failed == true)
                {
                    mc0Reset.Value = false;
                    mc0Status = McStatus.DOWN;
                    Console.WriteLine("State mc0Status: " + mc0Status);
                }
            }
            else if (mc0Status == McStatus.DOWN)
            {
                mc0AlarmSiren.Value = true;

                if (mc0RepairButton.Value == true)
                {
                    mc0Failed = false;//Next piece will not fail
                    mc0Status = McStatus.IDLE;
                    Console.WriteLine("State mc0Status: " + mc0Status);
                    mc0AlarmSiren.Value = false;
                }
            }
            // %%%%%%%%%%%% MC0 ENDS %%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%% MC1 STARTS %%%%%%%%%%%%%%%%%%%%

            if (mc1Status == McStatus.IDLE)
            {
                // %%%%%%%%% MC1 LOADING STEPS START %%%%%%%%%%%%%%
                if (loadingMc1Step == LoadingMc1Step.IDLE)
                {
                    //type here
                    if (mc1StartButton.Value == true && bufferStatus == BufferStatus.FULL)
                    {
                        loadingMc1Step = LoadingMc1Step.PIECE_TO_LOADING_CONVEYOR;
                        Console.WriteLine("Mc1 green button pressed with part available");
                        Console.WriteLine("loadingMc1Step = " + loadingMc1Step);
                    }
                }
                else if (loadingMc1Step == LoadingMc1Step.PIECE_TO_LOADING_CONVEYOR)
                {
                    bufferStopblade.Value = false;//Drop Stopblade
                    mc1Start.Value = true;
                    conveyorBuffer.Value = true;
                    conveyorMc1Entrance.Value = true;

                    if (sensorMc1LoadingConveyorStart.Value == true)
                    {
                        loadingMc1Step = LoadingMc1Step.SEPARATE_OTHER_PIECES;
                        Console.WriteLine("loadingMc1Step = " + loadingMc1Step);
                    }
                }
                else if (loadingMc1Step == LoadingMc1Step.SEPARATE_OTHER_PIECES)
                {
                    conveyorBuffer.Value = false;

                    if (ftAtMc1LoadingConveyorStart.Q == true)
                    {
                        loadingMc1Step = LoadingMc1Step.RESTORING_BUFFER_ORDER;
                        Console.WriteLine("loadingMc1Step = " + loadingMc1Step);
                    }
                }
                else if (loadingMc1Step == LoadingMc1Step.RESTORING_BUFFER_ORDER)
                {
                    bufferStopblade.Value = true;
                    conveyorBuffer.Value = true;

                    if (sensorBufferEnd.Value == true)
                    {
                        loadingMc1Step = LoadingMc1Step.IDLE;
                        Console.WriteLine("loadingMc1Step = " + loadingMc1Step);
                    }
                }

                if (mc1Busy.Value == true)
                {
                    mc1Status = McStatus.WORKING;
                    mc1Start.Value = false;
                    Console.WriteLine("State mc1Status: " + mc1Status);
                }
            }
            else if (mc1Status == McStatus.WORKING)
            {
                if (mc1Busy.Value == false && mc1Failed == false)
                {
                    mc1Status = McStatus.IDLE;
                    Console.WriteLine("State mc1Status: " + mc1Status);
                    mc1Failed = true; //will fail next time
                }
                else if (mc1Busy.Value == false && mc1Failed == true)
                {
                    mc1Status = McStatus.DOWN;
                    Console.WriteLine("State mc1Status: " + mc1Status);
                }
            }
            else if (mc1Status == McStatus.DOWN)
            {
                mc1AlarmSiren.Value = true;

                if (mc1RepairButton.Value == true)
                {
                    mc1Failed = false;
                    mc1Status = McStatus.IDLE;
                    Console.WriteLine("State mc1Status: " + mc1Status);
                    mc1AlarmSiren.Value = false;
                }
            }

            // %%%%%%%%%%%% MC1 ENDS %%%%%%%%%%%%%%%%%%%%


            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% MACHINE CENTER ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% EMITTER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (emitterStartButton.Value == true || sensorFinishedPartExit.Value == true || sensorBadPieceFilterConveyorEndMc0.Value == true || sensorBadPieceFilterConveyorEndMc1.Value == true)
            {
                emitter.Value = true;
            }
            else
            {
                emitter.Value = false;
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% EMITTER ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BAD PIECES FILTER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //mc0
            gripperMc0.stateTransition();

            if (mc0PositionerStatus == McPositionerStatus.UP)
            {
                mc0PositionerClamp.Value = false;
                mc0PositionerRise.Value = true;
                if (sensorExitMc0.Value == true && mc0Failed == true)
                {
                    mc0PositionerStatus = McPositionerStatus.DOWN;
                }
            }
            else if (mc0PositionerStatus == McPositionerStatus.DOWN)
            {
                mc0PositionerRise.Value = false;
                if (ftAtExitMc0.Q == true)
                {
                    mc0PositionerStatus = McPositionerStatus.CLAMP;
                }
            }
            else if (mc0PositionerStatus == McPositionerStatus.CLAMP)
            {
                mc0PositionerClamp.Value = true;
                if (mc0PositionerClamped.Value == true)
                {
                    gripperMc0.start();
                    if (mc0GripperItemDetected.Value == true)
                    {
                        mc0PositionerStatus = McPositionerStatus.GOING_UP;
                    }
                }
            }
            else if (mc0PositionerStatus == McPositionerStatus.GOING_UP)
            {
                mc0PositionerClamp.Value = false;
                if (sensorBadPieceFilterConveyorStartMc0.Value == true)
                {
                    mc0PositionerStatus = McPositionerStatus.UP;
                }
            }

            //mc1
            gripperMc1.stateTransition();

            if (mc1PositionerStatus == McPositionerStatus.UP)
            {
                mc1PositionerClamp.Value = false;
                mc1PositionerRise.Value = true;
                if (sensorExitMc1.Value == true && mc1Failed == true)
                {
                    mc1PositionerStatus = McPositionerStatus.DOWN;
                }
            }
            else if (mc1PositionerStatus == McPositionerStatus.DOWN)
            {
                mc1PositionerRise.Value = false;
                if (ftAtExitMc1.Q == true)
                {
                    mc1PositionerStatus = McPositionerStatus.CLAMP;
                }
            }
            else if (mc1PositionerStatus == McPositionerStatus.CLAMP)
            {
                mc1PositionerClamp.Value = true;
                if (mc1PositionerClamped.Value == true)
                {
                    gripperMc1.start();
                    if (mc1GripperItemDetected.Value == true)
                    {
                        mc1PositionerStatus = McPositionerStatus.GOING_UP;
                    }
                }
            }
            else if (mc1PositionerStatus == McPositionerStatus.GOING_UP)
            {
                mc1PositionerClamp.Value = false;
                if (sensorBadPieceFilterConveyorStartMc1.Value == true)
                {
                    mc1PositionerStatus = McPositionerStatus.UP;
                }
            }
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BAD PIECES FILTER ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        }
    }
}

public enum BufferStatus
{
    EMPTY,
    FULL,
}

public enum McPositionerStatus
{
    UP,
    DOWN,
    CLAMP,
    GOING_UP
}

public enum LoadingMc1Step
{
    IDLE,
    PIECE_TO_LOADING_CONVEYOR,
    SEPARATE_OTHER_PIECES,
    RESTORING_BUFFER_ORDER
}
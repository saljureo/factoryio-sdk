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
        MemoryBit mc0StartButton;
        MemoryBit mc0FailButton;
        MemoryBit mc0RepairButton;
        MemoryBit mc0Busy;//Machining Center 0 busy
        MemoryBit mc0Open;//Machining Center 0 opened
        MemoryInt mc0Progress;//Machining Center 0 opened
        MemoryBit mc0PositionerClamped;//Mc0 positioner clamped sensor
        MemoryBit mc0GripperItemDetected;//Mc0 positioner item detected sensor
        
        MemoryBit mc0Start;//Machining Center start
        MemoryBit mc0Reset;//Machining Center reset (so it leaves piece incomplete)
        //MemoryBit mc0Fail;//Machining Center fail
        MemoryBit mc0RedLight;
        MemoryBit mc0YellowLight;
        MemoryBit mc0GreenLight;
        MemoryBit mc0AlarmSiren;
        MemoryBit mc0PositionerRise;//mc0 positioner rise
        MemoryBit mc0PositionerClamp;//mc0 positioner clamp
        
        //Sensors
        MemoryBit sensorMc0ConveyorEntrance;//Diffuse Sensor 0 - Emitter
        MemoryBit sensorEntranceMc0;//Diffuse Sensor 1 - MC0 entrance
        MemoryBit sensorExitMc0;//Diffuse Sensor 2 - MC0 exit/Buffer conveyor entry/MC0 bad piece filter
        MemoryBit sensorBufferEnd;//Diffuse Sensor 3 - Buffer end
        MemoryBit sensorMc2loadingConveyorStart;//Diffuse Sensor 3_2 - Buffer start
        MemoryBit sensorEntranceMc2;//Diffuse Sensor 5 - MC2 entrance
        MemoryBit sensorBadPieceFilterConveyorStartMc0;//Diffuse Sensor 6 - MC0 bad piece filter conveyor entrance
        MemoryBit sensorBadPieceFilterConveyorEndMc0;//Diffuse Sensor 7 - MC0 bad piece filter conveyor exit
        MemoryBit sensorExitMc2;//Diffuse Sensor 8 - MC2 exit/MC2 bad piece filter
        MemoryBit sensorFinishedPartExit;//Diffuse Sensor 9 - Finished piece exit
        MemoryBit sensorBadPieceFilterConveyorStartMc2;//Diffuse Sensor 10 - MC2 bad piece conveyor entrance
        MemoryBit sensorBadPieceFilterConveyorEndMc2;//Diffuse Sensor 11 - MC2 bad piece conveyor exit
        MemoryBit sensorBufferSpot2;//Diffuse Sensor 13 - spot 2 on buffer
        MemoryBit sensorEmitter;

        //Mc2
        MemoryBit mc2StartButton;
        MemoryBit mc2FailButton;
        MemoryBit mc2RepairButton;
        MemoryBit mc2Busy;
        MemoryBit mc2PositionerClamped;
        MemoryBit mc2GripperItemDetected;

        MemoryBit mc2Start;
        MemoryBit mc2RedLight;
        MemoryBit mc2YellowLight;
        MemoryBit mc2GreenLight;
        MemoryBit mc2AlarmSiren;
        MemoryBit mc2PositionerClamp;
        MemoryBit mc2PositionerRise;

        //Emitter
        MemoryBit emitterStartButton; //Emitter start button
        MemoryBit emitter;//Emitter

        GripperArm gripperMc0;

        GripperArm gripperMc2;

        SupervisoryControl supervisoryControl;

        //conveyor belts
        MemoryBit conveyorMc2Entrance;
        MemoryBit conveyorMc0Entrance;
        MemoryBit conveyorMc0BadPiece;
        MemoryBit conveyorFinishedPiece;
        MemoryBit conveyorMc2BadPiece;
        MemoryBit conveyorEmitter;
        MemoryBit conveyorBuffer;

        //Buffer
        MemoryBit bufferStopblade;//Buffer stopblade

        bool mc0Failed;
        bool mc2Failed;

        McStatus mc0Status;
        McStatus mc2Status;

        McPositionerStatus mc0PositionerStatus = McPositionerStatus.UP;
        McPositionerStatus mc2PositionerStatus = McPositionerStatus.UP;

        BufferStatus bufferStatus = BufferStatus.EMPTY;

        Mc2LoadingSteps loadingMc2Step = Mc2LoadingSteps.IDLE;

        Mc0PieceReady mc0PieceReady;
        Mc0PieceReadySteps mc0PieceReadySteps;

        EventsMc0 eventsMc0;
        EventsMc2 eventsMc2;

        BreakdownM2 breakdownM2;

        private int s0Counter;
        private int r0Counter;
        private int s2Counter;
        private int r2Counter;

        FTRIG ftAtEntranceMc0;
        FTRIG ftAtExitMc0;
        FTRIG ftAtExitMc2;
        FTRIG ftAtBufferEnd;
        FTRIG ftAtBadPieceExitMc0;
        FTRIG ftAtBadPieceExitMc2;
        FTRIG ftAtFinishedPieceExit;
        FTRIG ftAtBufferStart;
        FTRIG ftAtMc2LoadingConveyorStart;
        FTRIG ftAtEmitter;
        

        public myScene2_2()// %%%%%%%%%%%%%%%%% CONSTRUCTOR STARTS %%%%%%%%%%%%%%%%
        {  
            

            //%% EXPERIMENT START

            //Mc0
            mc0StartButton = MemoryMap.Instance.GetBit("Start Button 0", MemoryType.Input);
            mc0FailButton = MemoryMap.Instance.GetBit("Stop Button 0", MemoryType.Input);
            mc0RepairButton = MemoryMap.Instance.GetBit("Reset Button 0", MemoryType.Input);
            mc0Busy = MemoryMap.Instance.GetBit("Machining Center 0 (Is Busy)", MemoryType.Input);//Machining Center 0 busy
            mc0Open = MemoryMap.Instance.GetBit("Machining Center 0 (Opened)", MemoryType.Input);//Machining Center 0 opened
            mc0Progress = MemoryMap.Instance.GetInt("Machining Center 0 (Progress)", MemoryType.Input);//Machining Center 0 opened
            mc0PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 0 (Clamped)", MemoryType.Input);//Mc0 positioner clamped sensor
            mc0GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Item Detected)", MemoryType.Input);//Mc0 positioner item detected sensor

            mc0Start = MemoryMap.Instance.GetBit("Machining Center 0 (Start)", MemoryType.Output);//Machining Center start
            mc0Reset = MemoryMap.Instance.GetBit("Machining Center 0 (Reset)", MemoryType.Output);//Machining Center reset (so it leaves piece incomplete)
                                                                                                            //MemoryBit mc0Fail = MemoryMap.Instance.GetBit("Machining Center 0 (Stop)", MemoryType.Output);//Machining Center start
            mc0RedLight = MemoryMap.Instance.GetBit("Stack Light 0 (Red)", MemoryType.Output);
            mc0YellowLight = MemoryMap.Instance.GetBit("Stack Light 0 (Yellow)", MemoryType.Output);
            mc0GreenLight = MemoryMap.Instance.GetBit("Stack Light 0 (Green)", MemoryType.Output);
            mc0AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 0", MemoryType.Output);
            mc0PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 0 (Raise)", MemoryType.Output);//mc0 positioner rise
            mc0PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 0 (Clamp)", MemoryType.Output);//mc0 positioner clamp



            //Sensors
            sensorMc0ConveyorEntrance = MemoryMap.Instance.GetBit("Diffuse Sensor 0", MemoryType.Input);//Diffuse Sensor 0 - Emitter
            sensorEntranceMc0 = MemoryMap.Instance.GetBit("Diffuse Sensor 1", MemoryType.Input);//Diffuse Sensor 1 - MC0 entrance
            sensorExitMc0 = MemoryMap.Instance.GetBit("Diffuse Sensor 2", MemoryType.Input);//Diffuse Sensor 2 - MC0 exit/Buffer conveyor entry/MC0 bad piece filter
            sensorBufferEnd = MemoryMap.Instance.GetBit("Diffuse Sensor 3", MemoryType.Input);//Diffuse Sensor 3 - Buffer end
            sensorMc2loadingConveyorStart = MemoryMap.Instance.GetBit("Diffuse Sensor 3_2", MemoryType.Input);//Diffuse Sensor 3_2 - Buffer start
            sensorEntranceMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 5", MemoryType.Input);//Diffuse Sensor 5 - MC1 entrance
            sensorBadPieceFilterConveyorStartMc0 = MemoryMap.Instance.GetBit("Diffuse Sensor 6", MemoryType.Input);//Diffuse Sensor 6 - MC0 bad piece filter conveyor entrance
            sensorBadPieceFilterConveyorEndMc0 = MemoryMap.Instance.GetBit("Diffuse Sensor 7", MemoryType.Input);//Diffuse Sensor 7 - MC0 bad piece filter conveyor exit
            sensorExitMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 8", MemoryType.Input);//Diffuse Sensor 8 - MC1 exit/MC1 bad piece filter
            sensorFinishedPartExit = MemoryMap.Instance.GetBit("Diffuse Sensor 9", MemoryType.Input);//Diffuse Sensor 9 - Finished piece exit
            sensorBadPieceFilterConveyorStartMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 10", MemoryType.Input);//Diffuse Sensor 10 - MC1 bad piece conveyor entrance
            sensorBadPieceFilterConveyorEndMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);//Diffuse Sensor 11 - MC1 bad piece conveyor exit
            sensorBufferSpot2 = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);//Diffuse Sensor 13 - spot 2 on buffer
            sensorEmitter = MemoryMap.Instance.GetBit("Diffuse Sensor 4", MemoryType.Input);//Diffuse Sensor 4 - Sensor emitter

            //Mc2
            mc2StartButton = MemoryMap.Instance.GetBit("Start Button 1", MemoryType.Input);
            mc2FailButton = MemoryMap.Instance.GetBit("Stop Button 1", MemoryType.Input);
            mc2RepairButton = MemoryMap.Instance.GetBit("Reset Button 1", MemoryType.Input); 
            mc2Busy = MemoryMap.Instance.GetBit("Machining Center 1 (Is Busy)", MemoryType.Input);
            mc2PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamped)", MemoryType.Input);//Mc2 positioner clamped sensor
            mc2GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 1 (Item Detected)", MemoryType.Input);//Mc2 positioner item detected sensor

            mc2Start = MemoryMap.Instance.GetBit("Machining Center 1 (Start)", MemoryType.Output);/
            mc2RedLight = MemoryMap.Instance.GetBit("Stack Light 1 (Red)", MemoryType.Output);
            mc2YellowLight = MemoryMap.Instance.GetBit("Stack Light 1 (Yellow)", MemoryType.Output);
            mc2GreenLight = MemoryMap.Instance.GetBit("Stack Light 1 (Green)", MemoryType.Output);
            mc2AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 1", MemoryType.Output);
            mc2PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamp)", MemoryType.Output);//mc2 positioner clamp
            mc2PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 3 (Raise)", MemoryType.Output);//mc2 positioner rise

            //Emitter
            emitterStartButton = MemoryMap.Instance.GetBit("Start Button 2", MemoryType.Input); //Emitter start button
            emitter = MemoryMap.Instance.GetBit("Emitter 0 (Emit)", MemoryType.Output);//Emitter

            gripperMc0 = new GripperArm(
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Grab)", MemoryType.Output)
            );

            gripperMc2 = new GripperArm(
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 X Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 Z Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 X Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 Z Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetBit("Two-Axis Pick & Place 1 (Grab)", MemoryType.Output)
            );

            
            //conveyor belts
            conveyorMc2Entrance = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 0", MemoryType.Output);
            conveyorMc0Entrance = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 4", MemoryType.Output);
            conveyorMc0BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 1", MemoryType.Output);
            conveyorFinishedPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 2", MemoryType.Output);
            conveyorMc2BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 3", MemoryType.Output);
            conveyorEmitter = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 5", MemoryType.Output);
            conveyorBuffer = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 1", MemoryType.Output);

            //Buffer
            bufferStopblade = MemoryMap.Instance.GetBit("Stop Blade 0", MemoryType.Output);

            mc0Status = McStatus.IDLE;
            mc2Status = McStatus.IDLE;

            McPositionerStatus mc0PositionerStatus = McPositionerStatus.UP;
            McPositionerStatus mc2PositionerStatus = McPositionerStatus.UP;

            BufferStatus bufferStatus = BufferStatus.EMPTY;

            Mc2LoadingSteps loadingMc2Step = Mc2LoadingSteps.IDLE;

            mc0PieceReady = Mc0PieceReady.NOT_READY;
            mc0PieceReadySteps = Mc0PieceReadySteps.IDLE;

            eventsMc0 = EventsMc0.i0;
            eventsMc2 = EventsMc2.i2;

            breakdownM2 = BreakdownM2.OK;

            ftAtEntranceMc0 = new FTRIG();
            ftAtExitMc0 = new FTRIG();
            ftAtExitMc2 = new FTRIG();
            ftAtBufferEnd = new FTRIG();
            ftAtBadPieceExitMc0 = new FTRIG();
            ftAtBadPieceExitMc2 = new FTRIG();
            ftAtFinishedPieceExit = new FTRIG();
            ftAtBufferStart = new FTRIG();
            ftAtMc2LoadingConveyorStart = new FTRIG();
            ftAtEmitter = new FTRIG();

            //%% EXPERIMENT END 

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
            mc0PositionerClamp.Value = false;
            mc0PositionerRise.Value = true;

            ////Emitter
            emitter.Value = true;

            //mc2
            mc2Failed = false;

            ////mc2 buttons            
            //mc2FailButton.Value = true;//True is unpressed
            mc2StartButton.Value = false;
            mc2RepairButton.Value = false;

            ////mc2 lights
            //mc2RedLight.Value = false; 
            //mc2YellowLight.Value = false;
            //mc2GreenLight.Value = true;

            s0Counter = 0;
            r0Counter = 0;
            s2Counter = 0;
            r2Counter = 0;

            //Buffer
            bufferStopblade.Value = true;//True is rised

            Console.WriteLine("State mc0Status: " + mc0Status);
            Console.WriteLine("State mc2Status: " + mc2Status);
            Console.WriteLine("State bufferStatus: " + bufferStatus);

            supervisoryControl = new SupervisoryControl();

        } // %%%%%%%%%%%%%%%%% CONSTRUCTOR ENDS %%%%%%%%%%%%%%%%

        public override void Execute(int elapsedMilliseconds) // %%%%%%%%%%%%%%%%% EXECUTE STARTS %%%%%%%%%%%%%%%%
        {
            supervisoryControl.On(mc0Status, mc2Status);
            Console.WriteLine("Programa principal mc0Status: " + mc0Status);
            Console.WriteLine("Programa principal mc2Status: " + mc2Status);
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTROLLABLE EVENTS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //s0
            if (mc0StartButton.Value == true)
            {
                if (s0Counter == 0)
                {
                    eventsMc0 = EventsMc0.s0;
                    Console.WriteLine("s0 (c)");
                    Console.WriteLine("State mc0Status: WORKING");
                    s0Counter++;
                }
            }
            else
            {
                s0Counter = 0;
            }

            //r0
            if (mc0RepairButton.Value == true)
            {
                if (r0Counter == 0)
                {
                    eventsMc0 = EventsMc0.r0;
                    Console.WriteLine("r0 (c)");
                    r0Counter++;
                }
                
            }
            else
            {
                r0Counter = 0;
            }

            //s2
            if (mc2StartButton.Value == true)
            {
                if (s2Counter == 0)
                {
                    eventsMc2 = EventsMc2.s2;
                    Console.WriteLine("s2 (c)");
                    Console.WriteLine("State mc2Status: WORKING");
                    s2Counter++;
                }
            }
            else
            {
                s2Counter = 0;
            }

            //r2
            if (mc2RepairButton.Value == true)
            {
                if (r2Counter == 0)
                {
                    eventsMc2 = EventsMc2.r2;
                    Console.WriteLine("r2 (c)");
                    r2Counter++;
                }

            }
            else
            {
                r2Counter = 0;
            }


            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTROLLABLE EVENTS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FALLING TRIGGERS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // Falling triggers
            ftAtEntranceMc0.CLK(sensorEntranceMc0.Value);
            ftAtExitMc0.CLK(sensorExitMc0.Value);
            ftAtExitMc2.CLK(sensorExitMc2.Value);
            ftAtBufferEnd.CLK(sensorBufferEnd.Value);
            ftAtBufferStart.CLK(sensorMc2loadingConveyorStart.Value);
            ftAtBadPieceExitMc0.CLK(sensorBadPieceFilterConveyorEndMc0.Value);
            ftAtBadPieceExitMc2.CLK(sensorBadPieceFilterConveyorEndMc2.Value);
            ftAtFinishedPieceExit.CLK(sensorFinishedPartExit.Value);
            ftAtMc2LoadingConveyorStart.CLK(sensorMc2loadingConveyorStart.Value);
            ftAtEmitter.CLK(sensorMc0ConveyorEntrance.Value);

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FALLING TRIGGERS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONVEYORS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //Buffer conveyor
            if (sensorExitMc0.Value == true)
            {
                conveyorBuffer.Value = true;
            }
            else if (sensorBufferEnd.Value == true)
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
            if (sensorExitMc2.Value == true)
            {
                conveyorFinishedPiece.Value = true;
            }
            else if (ftAtFinishedPieceExit.Q == true)
            {
                conveyorFinishedPiece.Value = false;
            }

            //Mc2 bad piece conveyor
            if (sensorBadPieceFilterConveyorStartMc2.Value == true)
            {
                conveyorMc2BadPiece.Value = true;
            }
            else if (ftAtBadPieceExitMc2.Q == true)
            {
                conveyorMc2BadPiece.Value = false;
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONVEYORS ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (bufferStatus == BufferStatus.EMPTY)
            {
                //type here
                if (sensorExitMc0.Value == true && mc0Failed == false)
                {
                    Console.WriteLine("f0 (uc)");
                    bufferStatus = BufferStatus.FULL;
                    Console.WriteLine("State bufferStatus: " + bufferStatus);
                }
            }
            else if (bufferStatus == BufferStatus.FULL)
            {
                //type here
                if (sensorEntranceMc2.Value == true)
                {
                    bufferStatus = BufferStatus.EMPTY;
                    Console.WriteLine("State bufferStatus: " + bufferStatus);
                }
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER ENDS   %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%




            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% MACHINE CENTER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%%% MC0 STARTS %%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%% PREPARING MC0 PIECE STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (mc0PieceReady == Mc0PieceReady.NOT_READY)
            {
                emitter.Value = true;
                if (sensorEmitter.Value == true)
                {
                    mc0PieceReady = Mc0PieceReady.READY;
                }
            }
            else if (mc0PieceReady == Mc0PieceReady.READY)
            {
                emitter.Value = false;
            }

            //%%%%%%%%%%%%%%%%%%% PREPARING MC0 PIECE ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (mc0Status == McStatus.IDLE)
            {
                if (eventsMc0 == EventsMc0.s0 && mc0PieceReady == Mc0PieceReady.READY)
                {
                    mc0PieceReadySteps = Mc0PieceReadySteps.SWITCHING_CONVEYORS;
                    eventsMc0 = EventsMc0.i0;
                }
                else if (mc0PieceReadySteps == Mc0PieceReadySteps.SWITCHING_CONVEYORS)
                {
                    conveyorEmitter.Value = true;//Turns on both conveyors
                    conveyorMc0Entrance.Value = true;//Turns on both conveyors
                    if (ftAtEmitter.Q == true)//If it exits emitter sensor
                    {
                        mc0PieceReadySteps = Mc0PieceReadySteps.REACHING_MC0ENTRANCE;
                    }
                }
                else if (mc0PieceReadySteps == Mc0PieceReadySteps.REACHING_MC0ENTRANCE)
                {
                    conveyorEmitter.Value = false;//Turns off emitter conveyor
                    
                    if (ftAtEntranceMc0.Q == true)
                    {
                        conveyorMc0Entrance.Value = false;//Turns off mc0 entrance conveyor
                        mc0Start.Value = true;//Starts mc0
                        mc0PieceReadySteps = Mc0PieceReadySteps.IDLE;
                        mc0PieceReady = Mc0PieceReady.NOT_READY;
                    }
                }

                if (mc0Busy.Value == true)
                {
                    mc0Status = McStatus.WORKING;
                    mc0Start.Value = false;
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
                    Console.WriteLine("b0 (uc)");
                    mc0Status = McStatus.DOWN;
                    Console.WriteLine("State mc0Status: " + mc0Status);
                }
            }
            else if (mc0Status == McStatus.DOWN)
            {
                mc0AlarmSiren.Value = true;

                if (eventsMc0 == EventsMc0.r0)
                {
                    mc0Failed = false;//Next piece will not fail
                    mc0Status = McStatus.IDLE;
                    Console.WriteLine("State mc0Status: " + mc0Status);
                    mc0AlarmSiren.Value = false;
                }
            }
            // %%%%%%%%%%%% MC0 ENDS %%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%% MC2 STARTS %%%%%%%%%%%%%%%%%%%%

            if (mc2Status == McStatus.IDLE)
            {
                // %%%%%%%%% MC2 LOADING STEPS START %%%%%%%%%%%%%%
                if (loadingMc2Step == Mc2LoadingSteps.IDLE)
                {
                    //type here
                    if (eventsMc2 == EventsMc2.s2 && bufferStatus == BufferStatus.FULL)
                    {
                        loadingMc2Step = Mc2LoadingSteps.PIECE_TO_LOADING_CONVEYOR;
                    }
                }
                else if (loadingMc2Step == Mc2LoadingSteps.PIECE_TO_LOADING_CONVEYOR)
                {
                    bufferStopblade.Value = false;//Drop Stopblade
                    mc2Start.Value = true;
                    conveyorBuffer.Value = true;
                    conveyorMc2Entrance.Value = true;

                    if (sensorMc2loadingConveyorStart.Value == true)
                    {
                        loadingMc2Step = Mc2LoadingSteps.SEPARATE_OTHER_PIECES;
                    }
                }
                else if (loadingMc2Step == Mc2LoadingSteps.SEPARATE_OTHER_PIECES)
                {
                    conveyorBuffer.Value = false;

                    if (ftAtMc2LoadingConveyorStart.Q == true)
                    {
                        loadingMc2Step = Mc2LoadingSteps.RESTORING_BUFFER_ORDER;
                    }
                }
                else if (loadingMc2Step == Mc2LoadingSteps.RESTORING_BUFFER_ORDER)
                {
                    bufferStopblade.Value = true;
                    conveyorBuffer.Value = true;

                    if (sensorBufferEnd.Value == true)
                    {
                        conveyorBuffer.Value = false;
                        loadingMc2Step = Mc2LoadingSteps.IDLE;
                    }
                }

                if (mc2Busy.Value == true)
                {
                    mc2Status = McStatus.WORKING;
                    mc2Start.Value = false;
                }
            }
            else if (mc2Status == McStatus.WORKING)
            {
                if (mc2Busy.Value == false && mc2Failed == false)
                {
                    eventsMc2 = EventsMc2.f2;
                    Console.WriteLine("f2 (uc)");
                    mc2Status = McStatus.IDLE;
                    Console.WriteLine("State mc2Status: " + mc2Status);
                    mc2Failed = true; //will fail next time
                }
                else if (mc2Busy.Value == false && mc2Failed == true)
                {
                    eventsMc2 = EventsMc2.b2;
                    Console.WriteLine("b2 (uc)");
                    mc2Status = McStatus.DOWN;
                    Console.WriteLine("State mc2Status: " + mc2Status);
                }
            }
            else if (mc2Status == McStatus.DOWN)
            {
                mc2AlarmSiren.Value = true;

                if (eventsMc2 == EventsMc2.r2)
                {
                    mc2Failed = false;
                    mc2Status = McStatus.IDLE;
                    Console.WriteLine("State mc2Status: " + mc2Status);
                    mc2AlarmSiren.Value = false;
                }
            }

            // %%%%%%%%%%%% MC2 ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% MACHINE CENTER ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            
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

            //mc2
            gripperMc2.stateTransition();

            if (mc2PositionerStatus == McPositionerStatus.UP)
            {
                mc2PositionerClamp.Value = false;
                mc2PositionerRise.Value = true;
                if (sensorExitMc2.Value == true && mc2Failed == true)
                {
                    mc2PositionerStatus = McPositionerStatus.DOWN;
                }
            }
            else if (mc2PositionerStatus == McPositionerStatus.DOWN)
            {
                mc2PositionerRise.Value = false;
                if (ftAtExitMc2.Q == true)
                {
                    mc2PositionerStatus = McPositionerStatus.CLAMP;
                }
            }
            else if (mc2PositionerStatus == McPositionerStatus.CLAMP)
            {
                mc2PositionerClamp.Value = true;
                if (mc2PositionerClamped.Value == true)
                {
                    gripperMc2.start();
                    if (mc2GripperItemDetected.Value == true)
                    {
                        mc2PositionerStatus = McPositionerStatus.GOING_UP;
                    }
                }
            }
            else if (mc2PositionerStatus == McPositionerStatus.GOING_UP)
            {
                mc2PositionerClamp.Value = false;
                if (sensorBadPieceFilterConveyorStartMc2.Value == true)
                {
                    mc2PositionerStatus = McPositionerStatus.UP;
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

public enum BufferSteps
{
    PIECE_ARRIVED_TO_BUFFER,
    FILTERING_BAD_PIECE,
    PIECE_REACHING_MC2,
    PIECE_ARRIVED_TO_MC2
}

public enum McPositionerStatus
{
    UP,
    DOWN,
    CLAMP,
    GOING_UP
}
public enum Mc0PieceReady
{
    NOT_READY,
    READY
}

public enum Mc0PieceReadySteps
{
    IDLE,
    SWITCHING_CONVEYORS,
    REACHING_MC0ENTRANCE,
    ENTERINGMC0
}

public enum Mc2LoadingSteps
{
    IDLE,
    PIECE_TO_LOADING_CONVEYOR,
    SEPARATE_OTHER_PIECES,
    RESTORING_BUFFER_ORDER
}

public enum EventsMc0
{
    s0,
    f0,
    b0,
    r0,
    i0
}

public enum EventsMc2
{
    s2,
    f2,
    b2,
    r2,
    i2
}

public enum BreakdownM2
{
    OK,
    KO
}




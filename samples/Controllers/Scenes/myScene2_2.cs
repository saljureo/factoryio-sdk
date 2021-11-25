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
        MemoryBit mc1StartButton;
        MemoryBit mc1FailButton;
        MemoryBit mc1RepairButton;
        MemoryBit mc1Busy;
        MemoryBit mc1Open;
        MemoryInt mc1Progress;
        MemoryBit mc1PositionerClamped;//Mc1 positioner clamped sensor
        MemoryBit mc1GripperItemDetected;//Mc1 positioner item detected sensor
        
        MemoryBit mc1Start;//Machining Center start
        MemoryBit mc1Reset;//Machining Center reset (so it leaves piece incomplete)
        //MemoryBit mc0Fail;//Machining Center fail
        MemoryBit mc1RedLight;
        MemoryBit mc1YellowLight;
        MemoryBit mc1GreenLight;
        MemoryBit mc1AlarmSiren;
        MemoryBit mc1PositionerRise;
        MemoryBit mc1PositionerClamp;
        
        //Sensors
        MemoryBit sensorMc1ConveyorEntrance;//Diffuse Sensor 0 - Emitter
        MemoryBit sensorEntranceMc1;//Diffuse Sensor 1 - MC0 entrance
        MemoryBit sensorExitMc1;//Diffuse Sensor 2 - MC0 exit/Buffer conveyor entry/MC0 bad piece filter
        MemoryBit sensorBufferEnd;//Diffuse Sensor 3 - Buffer end
        MemoryBit sensorMc2loadingConveyorStart;//Diffuse Sensor 3_2 - Buffer start
        MemoryBit sensorEntranceMc2;//Diffuse Sensor 5 - MC2 entrance
        MemoryBit sensorBadPieceFilterConveyorStartMc1;//Diffuse Sensor 6 - MC0 bad piece filter conveyor entrance
        MemoryBit sensorBadPieceFilterConveyorEndMc1;//Diffuse Sensor 7 - MC0 bad piece filter conveyor exit
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

        GripperArm gripperMc1;

        GripperArm gripperMc2;

        SupervisoryControl supervisoryControl;

        //conveyor belts
        MemoryBit conveyorMc2Entrance;
        MemoryBit conveyorMc1Entrance;
        MemoryBit conveyorMc1BadPiece;
        MemoryBit conveyorFinishedPiece;
        MemoryBit conveyorMc2BadPiece;
        MemoryBit conveyorEmitter;
        MemoryBit conveyorBuffer;

        //Buffer
        MemoryBit bufferStopblade;//Buffer stopblade

        bool mc1Failed;
        bool mc2Failed;

        McStatus mc1Status;
        McStatus mc2Status;

        McPositionerStatus mc1PositionerStatus = McPositionerStatus.UP;
        McPositionerStatus mc2PositionerStatus = McPositionerStatus.UP;

        BufferStatus bufferStatus = BufferStatus.EMPTY;

        Mc2LoadingSteps loadingMc2Step = Mc2LoadingSteps.IDLE;

        Mc1PieceReady mc1PieceReady;
        Mc1PieceReadySteps mc1PieceReadySteps;

        Events eventsMc1;
        Events eventsMc2;

        BreakdownM2 breakdownM2;

        Mc1WorkingStage mc1WorkingStage;

        private int s1Counter;
        private int r1Counter;
        private int s2Counter;
        private int r2Counter;

        FTRIG ftAtEntranceMc1;
        FTRIG ftAtEntranceMc2;
        FTRIG ftAtExitMc1;
        FTRIG ftAtExitMc2;
        FTRIG ftAtBufferEnd;
        FTRIG ftAtBadPieceExitMc1;
        FTRIG ftAtBadPieceExitMc2;
        FTRIG ftAtFinishedPieceExit;
        FTRIG ftAtBufferStart;
        FTRIG ftAtMc2LoadingConveyorStart;
        FTRIG ftAtEmitter;
        

        public myScene2_2()// %%%%%%%%%%%%%%%%% CONSTRUCTOR STARTS %%%%%%%%%%%%%%%%
        {  
            //%% EXPERIMENT START

            //Mc1
            mc1StartButton = MemoryMap.Instance.GetBit("Start Button 0", MemoryType.Input);
            mc1FailButton = MemoryMap.Instance.GetBit("Stop Button 0", MemoryType.Input);
            mc1RepairButton = MemoryMap.Instance.GetBit("Reset Button 0", MemoryType.Input);
            mc1Busy = MemoryMap.Instance.GetBit("Machining Center 0 (Is Busy)", MemoryType.Input);//Machining Center 0 busy
            mc1Open = MemoryMap.Instance.GetBit("Machining Center 0 (Opened)", MemoryType.Input);//Machining Center 0 opened
            mc1Progress = MemoryMap.Instance.GetInt("Machining Center 0 (Progress)", MemoryType.Input);//Machining Center 0 opened
            mc1PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 0 (Clamped)", MemoryType.Input);//Mc1 positioner clamped sensor
            mc1GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Item Detected)", MemoryType.Input);//Mc1 positioner item detected sensor

            mc1Start = MemoryMap.Instance.GetBit("Machining Center 0 (Start)", MemoryType.Output);//Machining Center start
            mc1Reset = MemoryMap.Instance.GetBit("Machining Center 0 (Reset)", MemoryType.Output);//Machining Center reset (so it leaves piece incomplete)
                                                                                                            //MemoryBit mc1Fail = MemoryMap.Instance.GetBit("Machining Center 0 (Stop)", MemoryType.Output);//Machining Center start
            mc1RedLight = MemoryMap.Instance.GetBit("Stack Light 0 (Red)", MemoryType.Output);
            mc1YellowLight = MemoryMap.Instance.GetBit("Stack Light 0 (Yellow)", MemoryType.Output);
            mc1GreenLight = MemoryMap.Instance.GetBit("Stack Light 0 (Green)", MemoryType.Output);
            mc1AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 0", MemoryType.Output);
            mc1PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 0 (Raise)", MemoryType.Output);
            mc1PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 0 (Clamp)", MemoryType.Output);



            //Sensors
            sensorMc1ConveyorEntrance = MemoryMap.Instance.GetBit("Diffuse Sensor 0", MemoryType.Input);//Diffuse Sensor 0 - Emitter
            sensorEntranceMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 1", MemoryType.Input);//Diffuse Sensor 1 - MC1 entrance
            sensorExitMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 2", MemoryType.Input);//Diffuse Sensor 2 - MC1 exit/Buffer conveyor entry/MC1 bad piece filter
            sensorBufferEnd = MemoryMap.Instance.GetBit("Diffuse Sensor 3", MemoryType.Input);//Diffuse Sensor 3 - Buffer end
            sensorMc2loadingConveyorStart = MemoryMap.Instance.GetBit("Diffuse Sensor 3_2", MemoryType.Input);//Diffuse Sensor 3_2 - Buffer start
            sensorEntranceMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 5", MemoryType.Input);//Diffuse Sensor 5 - MC1 entrance
            sensorBadPieceFilterConveyorStartMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 6", MemoryType.Input);//Diffuse Sensor 6 - MC1 bad piece filter conveyor entrance
            sensorBadPieceFilterConveyorEndMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 7", MemoryType.Input);//Diffuse Sensor 7 - MC1 bad piece filter conveyor exit
            sensorExitMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 8", MemoryType.Input);//Diffuse Sensor 8 - MC1 exit/MC1 bad piece filter
            sensorFinishedPartExit = MemoryMap.Instance.GetBit("Diffuse Sensor 9", MemoryType.Input);//Diffuse Sensor 9 - Finished piece exit
            sensorBadPieceFilterConveyorStartMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 10", MemoryType.Input);//Diffuse Sensor 10 - MC2 bad piece conveyor entrance
            sensorBadPieceFilterConveyorEndMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);//Diffuse Sensor 11 - MC2 bad piece conveyor exit
            sensorBufferSpot2 = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);//Diffuse Sensor 13 - spot 2 on buffer
            sensorEmitter = MemoryMap.Instance.GetBit("Diffuse Sensor 4", MemoryType.Input);//Diffuse Sensor 4 - Sensor emitter

            //Mc2
            mc2StartButton = MemoryMap.Instance.GetBit("Start Button 1", MemoryType.Input);
            mc2FailButton = MemoryMap.Instance.GetBit("Stop Button 1", MemoryType.Input);
            mc2RepairButton = MemoryMap.Instance.GetBit("Reset Button 1", MemoryType.Input); 
            mc2Busy = MemoryMap.Instance.GetBit("Machining Center 1 (Is Busy)", MemoryType.Input);
            mc2PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamped)", MemoryType.Input);//Mc2 positioner clamped sensor
            mc2GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 1 (Item Detected)", MemoryType.Input);//Mc2 positioner item detected sensor

            mc2Start = MemoryMap.Instance.GetBit("Machining Center 1 (Start)", MemoryType.Output);
            mc2RedLight = MemoryMap.Instance.GetBit("Stack Light 1 (Red)", MemoryType.Output);
            mc2YellowLight = MemoryMap.Instance.GetBit("Stack Light 1 (Yellow)", MemoryType.Output);
            mc2GreenLight = MemoryMap.Instance.GetBit("Stack Light 1 (Green)", MemoryType.Output);
            mc2AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 1", MemoryType.Output);
            mc2PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamp)", MemoryType.Output);//mc2 positioner clamp
            mc2PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 3 (Raise)", MemoryType.Output);//mc2 positioner rise

            //Emitter
            emitterStartButton = MemoryMap.Instance.GetBit("Start Button 2", MemoryType.Input); //Emitter start button
            emitter = MemoryMap.Instance.GetBit("Emitter 0 (Emit)", MemoryType.Output);//Emitter

            gripperMc1 = new GripperArm(
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
            conveyorMc1Entrance = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 4", MemoryType.Output);
            conveyorMc1BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 1", MemoryType.Output);
            conveyorFinishedPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 2", MemoryType.Output);
            conveyorMc2BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 3", MemoryType.Output);
            conveyorEmitter = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 5", MemoryType.Output);
            conveyorBuffer = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 1", MemoryType.Output);

            //Buffer
            bufferStopblade = MemoryMap.Instance.GetBit("Stop Blade 0", MemoryType.Output);

            mc1Status = McStatus.IDLE;
            mc2Status = McStatus.IDLE;

            McPositionerStatus mc1PositionerStatus = McPositionerStatus.UP;
            McPositionerStatus mc2PositionerStatus = McPositionerStatus.UP;

            BufferStatus bufferStatus = BufferStatus.EMPTY;

            Mc2LoadingSteps loadingMc2Step = Mc2LoadingSteps.IDLE;

            mc1PieceReady = Mc1PieceReady.NOT_READY;
            mc1PieceReadySteps = Mc1PieceReadySteps.IDLE;

            eventsMc1 = Events.i1;
            eventsMc2 = Events.i2;

            breakdownM2 = BreakdownM2.OK;

            mc1WorkingStage = Mc1WorkingStage.CONVEYOR;

            ftAtEntranceMc1 = new FTRIG();
            ftAtEntranceMc2 = new FTRIG();
            ftAtExitMc1 = new FTRIG();
            ftAtExitMc2 = new FTRIG();
            ftAtBufferEnd = new FTRIG();
            ftAtBadPieceExitMc1 = new FTRIG();
            ftAtBadPieceExitMc2 = new FTRIG();
            ftAtFinishedPieceExit = new FTRIG();
            ftAtBufferStart = new FTRIG();
            ftAtMc2LoadingConveyorStart = new FTRIG();
            ftAtEmitter = new FTRIG();

            //%% EXPERIMENT END 

            ////mc1
            //mc0Start.Value = false;
            mc1Busy.Value = false;

            ////mc1 buttons
            mc1StartButton.Value = false;
            //mc0FailButton.Value = true;//unpressed is true
            mc1RepairButton.Value = false;

            ////mc1 lights
            //mc0RedLight.Value = false;
            //mc0YellowLight.Value = false;
            //mc0GreenLight.Value = true;

            //mc1 others
            mc1PositionerClamp.Value = false;
            mc1PositionerRise.Value = true;

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

            s1Counter = 0;
            r1Counter = 0;
            s2Counter = 0;
            r2Counter = 0;

            //Buffer
            bufferStopblade.Value = true;//True is rised

            Console.WriteLine("State mc1Status: " + mc1Status);
            Console.WriteLine("State mc2Status: " + mc2Status);
            Console.WriteLine("State bufferStatus: " + bufferStatus);
            Console.WriteLine("State breakdownM2: " + breakdownM2);

            supervisoryControl = new SupervisoryControl();

        } // %%%%%%%%%%%%%%%%% CONSTRUCTOR ENDS %%%%%%%%%%%%%%%%

        public override void Execute(int elapsedMilliseconds) // %%%%%%%%%%%%%%%%% EXECUTE STARTS %%%%%%%%%%%%%%%%
        {
            supervisoryControl.On(mc1Status, mc2Status, bufferStatus, breakdownM2, eventsMc1, eventsMc2);
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTROLLABLE EVENTS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //s1
            if (mc1StartButton.Value == true)
            {
                if (s1Counter == 0)
                {
                    eventsMc1 = Events.s1;
                    Console.WriteLine("s1 (c)");
                    s1Counter++;
                }
            }
            else
            {
                s1Counter = 0;
            }

            //r1
            if (mc1RepairButton.Value == true)
            {
                if (r1Counter == 0)
                {
                    eventsMc1 = Events.r1;
                    Console.WriteLine("r1 (c)");
                    r1Counter++;
                }
                
            }
            else
            {
                r1Counter = 0;
            }

            //s2
            if (mc2StartButton.Value == true)
            {
                if (s2Counter == 0)
                {
                    eventsMc2 = Events.s2;
                    Console.WriteLine("s2 (c)");
                    //Console.WriteLine("State mc2Status: WORKING");
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
                    eventsMc2 = Events.r2;
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
            ftAtEntranceMc1.CLK(sensorEntranceMc1.Value);
            ftAtEntranceMc2.CLK(sensorEntranceMc2.Value);
            ftAtExitMc1.CLK(sensorExitMc1.Value);
            ftAtExitMc2.CLK(sensorExitMc2.Value);
            ftAtBufferEnd.CLK(sensorBufferEnd.Value);
            ftAtBufferStart.CLK(sensorMc2loadingConveyorStart.Value);
            ftAtBadPieceExitMc1.CLK(sensorBadPieceFilterConveyorEndMc1.Value);
            ftAtBadPieceExitMc2.CLK(sensorBadPieceFilterConveyorEndMc2.Value);
            ftAtFinishedPieceExit.CLK(sensorFinishedPartExit.Value);
            ftAtMc2LoadingConveyorStart.CLK(sensorMc2loadingConveyorStart.Value);
            ftAtEmitter.CLK(sensorMc1ConveyorEntrance.Value);

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FALLING TRIGGERS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONVEYORS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //Buffer conveyor
            if (sensorExitMc1.Value == true)
            {
                conveyorBuffer.Value = true;
            }
            else if (sensorBufferEnd.Value == true)
            {
                conveyorBuffer.Value = false;
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
                if (sensorExitMc1.Value == true && mc1Failed == false)
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

            // %%%%%%%%%%%%% MC1 STARTS %%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%% PREPARING MC1 PIECE STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (mc1PieceReady == Mc1PieceReady.NOT_READY)
            {
                emitter.Value = true;
                if (sensorEmitter.Value == true)
                {
                    mc1PieceReady = Mc1PieceReady.READY;
                }
            }
            else if (mc1PieceReady == Mc1PieceReady.READY)
            {
                emitter.Value = false;
                if (ftAtEntranceMc1.Q)
                {
                    mc1PieceReady = Mc1PieceReady.NOT_READY;
                }
            }

            //%%%%%%%%%%%%%%%%%%% PREPARING MC1 PIECE ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (mc1Status == McStatus.IDLE)
            {
                if (eventsMc1 == Events.s1 && mc1PieceReady == Mc1PieceReady.READY)
                {
                    mc1Status = McStatus.WORKING;
                    Console.WriteLine("State mc1Status: " + mc1Status);
                    mc1PieceReadySteps = Mc1PieceReadySteps.SWITCHING_CONVEYORS;
                    eventsMc1 = Events.i1;
                }
            }
            else if (mc1Status == McStatus.WORKING)
            {
                if (mc1WorkingStage == Mc1WorkingStage.CONVEYOR)
                {
                    if (mc1PieceReadySteps == Mc1PieceReadySteps.SWITCHING_CONVEYORS)
                    {
                        conveyorEmitter.Value = true;//Turns on both conveyors
                        conveyorMc1Entrance.Value = true;//Turns on both conveyors
                        if (ftAtEmitter.Q == true)//If it exits emitter sensor
                        {
                            mc1PieceReadySteps = Mc1PieceReadySteps.REACHING_MC1ENTRANCE;
                        }
                    }
                    else if (mc1PieceReadySteps == Mc1PieceReadySteps.REACHING_MC1ENTRANCE)
                    {
                        conveyorEmitter.Value = false;//Turns off emitter conveyor

                        if (ftAtEntranceMc1.Q == true)
                        {
                            conveyorMc1Entrance.Value = false;//Turns off mc1 entrance conveyor
                            mc1Start.Value = true;//Starts mc1
                            mc1PieceReadySteps = Mc1PieceReadySteps.IDLE;
                        }
                    }
                    
                }
                else if (mc1WorkingStage == Mc1WorkingStage.MACHINING_CENTER)
                {
                    if (mc1Busy.Value == true)
                    {
                        mc1Start.Value = false;
                    }

                    if (mc1Progress.Value == 90)
                    {
                        mc1Reset.Value = true;
                    }

                    if (mc1Busy.Value == false && mc1Failed == false)
                    {
                        mc1Reset.Value = false;
                        mc1Status = McStatus.IDLE;
                        Console.WriteLine("State mc1Status: " + mc1Status);
                        mc1Failed = true; //will fail next time
                    }
                    else if (mc1Busy.Value == false && mc1Failed == true)
                    {
                        mc1Reset.Value = false;
                        Console.WriteLine("b1 (uc)");
                        mc1Status = McStatus.DOWN;
                        Console.WriteLine("State mc1Status: " + mc1Status);
                    }
                }

                
            }
            else if (mc1Status == McStatus.DOWN)
            {
                mc1AlarmSiren.Value = true;

                if (eventsMc1 == Events.r1)
                {
                    mc1Failed = false;//Next piece will not fail
                    mc1Status = McStatus.IDLE;
                    Console.WriteLine("State mc1Status: " + mc1Status);
                    mc1AlarmSiren.Value = false;
                }
            }
            // %%%%%%%%%%%% MC1 ENDS %%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%% MC2 STARTS %%%%%%%%%%%%%%%%%%%%

            if (mc2Status == McStatus.IDLE)
            {
                // %%%%%%%%% MC2 LOADING STEPS START %%%%%%%%%%%%%%
                if (loadingMc2Step == Mc2LoadingSteps.IDLE)
                {
                    //type here
                    if (eventsMc2 == Events.s2 && bufferStatus == BufferStatus.FULL)
                    {
                        Console.WriteLine("State mc2Status: " + mc2Status);
                        loadingMc2Step = Mc2LoadingSteps.PIECE_TO_LOADING_CONVEYOR;
                    }
                }
                else if (loadingMc2Step == Mc2LoadingSteps.PIECE_TO_LOADING_CONVEYOR)
                {
                    bufferStopblade.Value = false;//Drop Stopblade
                    mc2Start.Value = true;
                    conveyorBuffer.Value = true;//turn on both conveyors
                    conveyorMc2Entrance.Value = true;//turn on both conveyors

                    if (sensorMc2loadingConveyorStart.Value == true)
                    {
                        loadingMc2Step = Mc2LoadingSteps.SEPARATE_OTHER_PIECES;
                    }
                }
                else if (loadingMc2Step == Mc2LoadingSteps.SEPARATE_OTHER_PIECES)
                {
                    conveyorBuffer.Value = false;//turn of buffer conveyor

                    if (ftAtMc2LoadingConveyorStart.Q == true)
                    {
                        loadingMc2Step = Mc2LoadingSteps.RESTORING_BUFFER_ORDER;
                    }
                }
                else if (loadingMc2Step == Mc2LoadingSteps.RESTORING_BUFFER_ORDER)
                {
                    bufferStopblade.Value = true;
                    conveyorBuffer.Value = true;//turn on buffer conveyor

                    if (sensorBufferEnd.Value == true || bufferStatus == BufferStatus.EMPTY)
                    {
                        conveyorBuffer.Value = false;//turn off buffer conveyor
                    }
                    if (ftAtEntranceMc2.Q == true)
                    {
                        loadingMc2Step = Mc2LoadingSteps.REACHED_MC2;
                    }
                }
                else if (loadingMc2Step == Mc2LoadingSteps.REACHED_MC2)
                {
                    conveyorMc2Entrance.Value = false;//turn off entrance conveyor
                    loadingMc2Step = Mc2LoadingSteps.IDLE;
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
                    eventsMc2 = Events.f2;
                    Console.WriteLine("f2 (uc)");
                    mc2Status = McStatus.IDLE;
                    Console.WriteLine("State mc2Status: " + mc2Status);
                    mc2Failed = true; //will fail next time
                }
                else if (mc2Busy.Value == false && mc2Failed == true)
                {
                    eventsMc2 = Events.b2;
                    Console.WriteLine("b2 (uc)");
                    mc2Status = McStatus.DOWN;
                    Console.WriteLine("State mc2Status: " + mc2Status);
                }
            }
            else if (mc2Status == McStatus.DOWN)
            {
                mc2AlarmSiren.Value = true;
                breakdownM2 = BreakdownM2.KO;
                if (eventsMc2 == Events.r2)
                {
                    mc2Failed = false;
                    mc2Status = McStatus.IDLE;
                    Console.WriteLine("State mc2Status: " + mc2Status);
                    mc2AlarmSiren.Value = false;
                    breakdownM2 = BreakdownM2.OK;
                }
            }

            // %%%%%%%%%%%% MC2 ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% MACHINE CENTER ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BAD PIECES FILTER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

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
public enum Mc1PieceReady
{
    NOT_READY,
    READY
}

public enum Mc1PieceReadySteps
{
    IDLE,
    SWITCHING_CONVEYORS,
    REACHING_MC1ENTRANCE,
    ENTERINGMC1
}

public enum Mc2LoadingSteps
{
    IDLE,
    PIECE_TO_LOADING_CONVEYOR,
    SEPARATE_OTHER_PIECES,
    RESTORING_BUFFER_ORDER,
    REACHED_MC2
}

public enum Events
{
    s1,
    f1,
    b1,
    r1,
    i1,
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

public enum Mc1WorkingStage
{
    CONVEYOR,
    MACHINING_CENTER
}




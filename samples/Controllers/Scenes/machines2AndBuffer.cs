//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using Controllers.Scenes;
using EngineIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Controllers
{
    public class machines2AndBuffer : Controller
    {
        //Mc1
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
        int timeDownMc1;
        int timeDownMc2;

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

        //Failing time (potenciometer and display)
        MemoryFloat potentiometerMc1;
        MemoryFloat potentiometerMc2;
        MemoryFloat displayMc1;
        MemoryFloat displayMc2;
        Stopwatch stopWatch;
        float tiempo;

        bool mc1Failed;
        bool mc2Failed;
        bool supervisoryApproval;

        McStatus mc1Status;
        McStatus mc2Status;

        McPositionerStatus mc1PositionerStatus = McPositionerStatus.UP;
        McPositionerStatus mc2PositionerStatus = McPositionerStatus.UP;

        BufferStatus bufferStatus = BufferStatus.EMPTY;

        Mc2andMc3LoadingSteps loadingMc2Step = Mc2andMc3LoadingSteps.IDLE;

        Mc1PieceReady mc1PieceReady;
        Mc1PieceReadySteps mc1PieceReadySteps;

        Events eventsMc;

        BreakdownMc2OrMc3 breakdownM2;

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
        
        
        
        public machines2AndBuffer()// %%%%%%%%%%%%%%%%% CONSTRUCTOR STARTS %%%%%%%%%%%%%%%%
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

            timeDownMc1 = 0;
            timeDownMc2 = 0;

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

            //Failing time (potenciometer and display)
            potentiometerMc1 = MemoryMap.Instance.GetFloat("Potentiometer 0 (V)", MemoryType.Input);
            potentiometerMc2 = MemoryMap.Instance.GetFloat("Potentiometer 1 (V)", MemoryType.Input);
            displayMc1 = MemoryMap.Instance.GetFloat("Digital Display 0", MemoryType.Output);
            displayMc2 = MemoryMap.Instance.GetFloat("Digital Display 1", MemoryType.Output);
            stopWatch = new Stopwatch();

            mc1Status = McStatus.IDLE;
            mc2Status = McStatus.IDLE;

            McPositionerStatus mc1PositionerStatus = McPositionerStatus.UP;
            McPositionerStatus mc2PositionerStatus = McPositionerStatus.UP;

            BufferStatus bufferStatus = BufferStatus.EMPTY;

            Mc2andMc3LoadingSteps loadingMc2Step = Mc2andMc3LoadingSteps.IDLE;

            mc1PieceReady = Mc1PieceReady.NOT_READY;
            mc1PieceReadySteps = Mc1PieceReadySteps.IDLE;

            eventsMc = Events.i1;

            breakdownM2 = BreakdownMc2OrMc3.OK;

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

            //Failing time and display
            //potentiometerMc1.Value = 1.0;//1 is 10 seconds, 10 is 100 seconds
            //potentiometerMc2.Value = 1.0;

            supervisoryControl = new SupervisoryControl();
            supervisoryControl = new SupervisoryControl();
            supervisoryApproval = true;

            supervisoryControl.CreateController();

        } // %%%%%%%%%%%%%%%%% CONSTRUCTOR ENDS %%%%%%%%%%%%%%%%

        public override void Execute(int elapsedMilliseconds) // %%%%%%%%%%%%%%%%% EXECUTE STARTS %%%%%%%%%%%%%%%%
        {
            //Failing time and display
            displayMc1.Value = potentiometerMc1.Value;
            displayMc2.Value = potentiometerMc2.Value;
            stopWatch.Start();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            //tiempo = ts.Seconds + "." + ts.Milliseconds;

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTROLLABLE EVENTS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //s1
            if (mc1StartButton.Value == true)
            {
                if (s1Counter == 0)
                {
                    supervisoryApproval = supervisoryControl.On2("s1");
                    if (supervisoryApproval == true)
                    {
                        eventsMc = Events.s1;
                        s1Counter++;
                    }
                    
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
                    supervisoryApproval = supervisoryControl.On2("r1");
                    if (supervisoryApproval == true)
                    {
                        eventsMc = Events.r1;
                        r1Counter++;
                    }
                    
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
                    supervisoryApproval = supervisoryControl.On2("s2");
                    if (supervisoryApproval == true)
                    {
                        eventsMc = Events.s2;
                        s2Counter++;
                    }
                    
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
                    supervisoryApproval = supervisoryControl.On2("r2");
                    if (supervisoryApproval == true)
                    {
                        eventsMc = Events.r2;
                        r2Counter++;
                    }
                    
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
                    bufferStatus = BufferStatus.FULL;
                }
            }
            else if (bufferStatus == BufferStatus.FULL)
            {
                //type here
                if (sensorEntranceMc2.Value == true)
                {
                    bufferStatus = BufferStatus.EMPTY;
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
                if (eventsMc == Events.s1 && mc1PieceReady == Mc1PieceReady.READY)
                {
                    mc1Status = McStatus.WORKING;
                    mc1WorkingStage = Mc1WorkingStage.CONVEYOR;
                    mc1PieceReadySteps = Mc1PieceReadySteps.SWITCHING_CONVEYORS;
                    eventsMc = Events.i1;
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
                            mc1WorkingStage = Mc1WorkingStage.MACHINING_CENTER1;
                        }
                    }
                    
                }
                else if (mc1WorkingStage == Mc1WorkingStage.MACHINING_CENTER1)
                {
                    if (mc1Busy.Value == true)
                    {
                        mc1Start.Value = false;
                    }

                    if (mc1Progress.Value > 90)
                    {
                        mc1Reset.Value = true;
                        mc1WorkingStage = Mc1WorkingStage.MACHINING_CENTER2;
                    }
                }
                else if (mc1WorkingStage == Mc1WorkingStage.MACHINING_CENTER2) 
                {                    
                if (mc1Busy.Value == false && mc1Failed == false)
                    {
                        mc1Reset.Value = false;
                        mc1Status = McStatus.IDLE;
                        supervisoryApproval = supervisoryControl.On2("f1");
                        mc1Failed = true; //will fail next time
                    }
                    else if (mc1Busy.Value == false && mc1Failed == true)
                    {
                        mc1Reset.Value = false;
                        mc1Status = McStatus.DOWN;
                        supervisoryApproval = supervisoryControl.On2("b1");
                    }
                }

                
            }
            else if (mc1Status == McStatus.DOWN)
            {
                mc1AlarmSiren.Value = true;

                // %%%%%%%%%%%%%%%%% DOWN LIGHTS START %%%%%%%%%%%%%%%%
                if (timeDownMc1 < 30)
                {
                    mc1GreenLight.Value = false;
                    mc1YellowLight.Value = false;
                    mc1RedLight.Value = false;
                }
                else if (timeDownMc1 < 60)
                {
                    mc1GreenLight.Value = true;
                    mc1YellowLight.Value = true;
                    mc1RedLight.Value = true;
                }
                else if (timeDownMc1 == 60)
                {
                    timeDownMc1 = 0;
                }
                timeDownMc1++;
                // %%%%%%%%%%%%%%%%% DOWN LIGHTS END %%%%%%%%%%%%%%%%

                if (eventsMc == Events.r1)
                {
                    mc1GreenLight.Value = false;
                    mc1YellowLight.Value = false;
                    mc1RedLight.Value = false;
                    mc1Failed = false;//Next piece will not fail
                    mc1Status = McStatus.IDLE;
                    mc1AlarmSiren.Value = false;
                }
            }
            // %%%%%%%%%%%% MC1 ENDS %%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%% MC2 STARTS %%%%%%%%%%%%%%%%%%%%

            if (mc2Status == McStatus.IDLE)
            {
                // %%%%%%%%% MC2 LOADING STEPS START %%%%%%%%%%%%%%
                if (loadingMc2Step == Mc2andMc3LoadingSteps.IDLE)
                {
                    //type here
                    if (eventsMc == Events.s2 && bufferStatus == BufferStatus.FULL)
                    {
                        loadingMc2Step = Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR;
                    }
                }
                else if (loadingMc2Step == Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR)
                {
                    bufferStopblade.Value = false;//Drop Stopblade
                    mc2Start.Value = true;
                    conveyorBuffer.Value = true;//turn on both conveyors
                    conveyorMc2Entrance.Value = true;//turn on both conveyors

                    if (sensorMc2loadingConveyorStart.Value == true)
                    {
                        loadingMc2Step = Mc2andMc3LoadingSteps.SEPARATE_OTHER_PIECES;
                    }
                }
                else if (loadingMc2Step == Mc2andMc3LoadingSteps.SEPARATE_OTHER_PIECES)
                {
                    conveyorBuffer.Value = false;//turn of buffer conveyor

                    if (ftAtMc2LoadingConveyorStart.Q == true)
                    {
                        loadingMc2Step = Mc2andMc3LoadingSteps.RESTORING_BUFFER_ORDER;
                    }
                }
                else if (loadingMc2Step == Mc2andMc3LoadingSteps.RESTORING_BUFFER_ORDER)
                {
                    bufferStopblade.Value = true;
                    conveyorBuffer.Value = true;//turn on buffer conveyor

                    if (sensorBufferEnd.Value == true || bufferStatus == BufferStatus.EMPTY)
                    {
                        conveyorBuffer.Value = false;//turn off buffer conveyor
                    }
                    if (ftAtEntranceMc2.Q == true)
                    {
                        loadingMc2Step = Mc2andMc3LoadingSteps.REACHED_MC;
                    }
                }
                else if (loadingMc2Step == Mc2andMc3LoadingSteps.REACHED_MC)
                {
                    conveyorMc2Entrance.Value = false;//turn off entrance conveyor
                    loadingMc2Step = Mc2andMc3LoadingSteps.IDLE;
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
                    eventsMc = Events.f2;
                    mc2Status = McStatus.IDLE;
                    supervisoryApproval = supervisoryControl.On2("f2");
                    mc2Failed = true; //will fail next time
                }
                else if (mc2Busy.Value == false && mc2Failed == true)
                {
                    eventsMc = Events.b2;
                    mc2Status = McStatus.DOWN;
                    supervisoryApproval = supervisoryControl.On2("b2");
                }
            }
            else if (mc2Status == McStatus.DOWN)
            {
                mc2AlarmSiren.Value = true;
                breakdownM2 = BreakdownMc2OrMc3.KO;

                // %%%%%%%%%%%%%%%%% DOWN LIGHTS START %%%%%%%%%%%%%%%%
                if (timeDownMc2 < 30)
                {
                    mc2GreenLight.Value = false;
                    mc2YellowLight.Value = false;
                    mc2RedLight.Value = false;
                }
                else if (timeDownMc2 < 60)
                {
                    mc2GreenLight.Value = true;
                    mc2YellowLight.Value = true;
                    mc2RedLight.Value = true;
                }
                else if (timeDownMc2 == 60)
                {
                    timeDownMc2 = 0;
                }
                timeDownMc2++;
                // %%%%%%%%%%%%%%%%% DOWN LIGHTS END %%%%%%%%%%%%%%%%

                if (eventsMc == Events.r2)
                {
                    mc2Failed = false;
                    mc2Status = McStatus.IDLE;
                    mc2AlarmSiren.Value = false;
                    breakdownM2 = BreakdownMc2OrMc3.OK;
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

public enum Mc2andMc3LoadingSteps
{
    IDLE,
    PIECE_TO_LOADING_CONVEYOR,
    SEPARATE_OTHER_PIECES,
    RESTORING_BUFFER_ORDER,
    REACHED_MC
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
    i2,
    s3,
    f3,
    b3,
    r3,
    i3
}

public enum BreakdownMc2OrMc3
{
    OK,
    KO
}

public enum Mc1WorkingStage
{
    CONVEYOR,
    MACHINING_CENTER1,
    MACHINING_CENTER2
}




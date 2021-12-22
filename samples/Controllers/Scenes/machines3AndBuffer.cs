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
    public class machines3AndBuffer : Controller
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

        //Sensors
        MemoryBit sensorMc1ConveyorEntrance;//Diffuse Sensor 0 - Emitter
        MemoryBit sensorEntranceMc1;//Diffuse Sensor 1 - MC0 entrance
        MemoryBit sensorExitMc1;//Diffuse Sensor 2 - MC0 exit/Buffer conveyor entry/MC0 bad piece filter
        MemoryBit sensorBuffer1End;//Diffuse Sensor 3 - Buffer 1 end
        MemoryBit sensorBuffer2End;//Diffuse Sensor 21 - Buffer 2 end
        MemoryBit sensorMc2loadingConveyorStart;//Diffuse Sensor 3_2
        MemoryBit sensorMc3loadingConveyorStart;//Diffuse Sensor 20
        MemoryBit sensorEntranceMc2;//Diffuse Sensor 5 - MC2 entrance
        MemoryBit sensorEntranceMc3;//Diffuse Sensor 19 - MC3 entrance
        MemoryBit sensorBadPieceFilterConveyorStartMc1;//Diffuse Sensor 6 - MC0 bad piece filter conveyor entrance
        MemoryBit sensorBadPieceFilterConveyorEndMc1;//Diffuse Sensor 7 - MC0 bad piece filter conveyor exit
        MemoryBit sensorExitMc2;//Diffuse Sensor 8 - MC2 exit/MC2 bad piece filter
        MemoryBit sensorExitMc3;//Diffuse Sensor 22 - MC3 exit/MC3 bad piece filter
        MemoryBit sensorFinishedPartExit;//Diffuse Sensor 9 - Finished piece exit
        MemoryBit sensorBadPieceFilterConveyorStartMc2;//Diffuse Sensor 10 - MC2 bad piece conveyor entrance
        MemoryBit sensorBadPieceFilterConveyorEndMc2;//Diffuse Sensor 11 - MC2 bad piece conveyor exit
        MemoryBit sensorBadPieceFilterConveyorStartMc3;//Diffuse Sensor 17 - MC3 bad piece conveyor entrance
        MemoryBit sensorBadPieceFilterConveyorEndMc3;//Diffuse Sensor 18 - MC3 bad piece conveyor exit
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

        //Mc3
        MemoryBit mc3StartButton;
        MemoryBit mc3FailButton;
        MemoryBit mc3RepairButton;
        MemoryBit mc3Busy;
        MemoryBit mc3PositionerClamped;
        MemoryBit mc3GripperItemDetected;

        MemoryBit mc3Start;
        MemoryBit mc3RedLight;
        MemoryBit mc3YellowLight;
        MemoryBit mc3GreenLight;
        MemoryBit mc3AlarmSiren;
        MemoryBit mc3PositionerClamp;
        MemoryBit mc3PositionerRise;

        //Emitter
        MemoryBit emitter;//Emitter

        GripperArm gripperMc1;
        GripperArm gripperMc2;
        GripperArm gripperMc3;

        McLightsControl mc1Lights;
        McLightsControl mc2Lights;
        McLightsControl mc3Lights;

        machines2AndBufferSupervisor supervisoryControl;

        //conveyor belts
        MemoryBit conveyorMc2Entrance;
        MemoryBit conveyorMc3Entrance;
        MemoryBit conveyorMc1Entrance;
        MemoryBit conveyorMc1BadPiece;
        MemoryBit conveyorFinishedPiece;
        MemoryBit conveyorMc2BadPiece;
        MemoryBit conveyorMc3BadPiece;
        MemoryBit conveyorEmitter;
        MemoryBit conveyorBuffer1;
        MemoryBit conveyorBuffer2;

        //Buffer 1
        MemoryBit buffer1Stopblade;//Buffer stopblade

        //Buffer 2
        MemoryBit buffer2Stopblade;//Buffer stopblade

        //Failing time (potenciometer and display)
        MemoryFloat potentiometerMc1;
        MemoryFloat potentiometerMc2;
        MemoryFloat displayMc1;
        MemoryFloat displayMc2;
        Stopwatch stopWatch;
        float tiempo;

        bool mc1Failed;
        bool mc2Failed;
        bool mc3Failed;
        bool supervisoryApproval;

        McStatus mc1Status;
        McStatus mc2Status;
        McStatus mc3Status;

        McPositionerStatus mc1PositionerStatus;
        McPositionerStatus mc2PositionerStatus;
        McPositionerStatus mc3PositionerStatus;

        BufferStatus bufferStatus1;
        BufferStatus bufferStatus2;

        Mc2andMc3LoadingSteps loadingMc2Step;
        Mc2andMc3LoadingSteps loadingMc3Step;

        Mc1PieceReady mc1PieceReady;
        Mc1PieceReadySteps mc1PieceReadySteps;

        Events eventsMc;
        Events preeventMc;

        BreakdownMc2OrMc3 breakdownM2;
        BreakdownMc2OrMc3 breakdownM3;

        Mc1WorkingStage mc1WorkingStage;

        private int s1Counter;
        private int r1Counter;
        private int s2Counter;
        private int r2Counter;
        private int s3Counter;
        private int r3Counter;


        FTRIG ftAtEntranceMc1;
        FTRIG ftAtEntranceMc2;
        FTRIG ftAtEntranceMc3;
        FTRIG ftAtExitMc1;
        FTRIG ftAtExitMc2;
        FTRIG ftAtExitMc3;
        FTRIG ftAtBufferEnd;
        FTRIG ftAtBadPieceExitMc1;
        FTRIG ftAtBadPieceExitMc2;
        FTRIG ftAtBadPieceExitMc3;
        FTRIG ftAtFinishedPieceExit;
        FTRIG ftAtBufferStart;
        FTRIG ftAtMc2LoadingConveyorStart;
        FTRIG ftAtMc3LoadingConveyorStart;
        FTRIG ftAtEmitter;



        public machines3AndBuffer()// %%%%%%%%%%%%%%%%% CONSTRUCTOR STARTS %%%%%%%%%%%%%%%%
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
            sensorBuffer1End = MemoryMap.Instance.GetBit("Diffuse Sensor 3", MemoryType.Input);//Diffuse Sensor 3 - Buffer 1 end
            sensorBuffer2End = MemoryMap.Instance.GetBit("Diffuse Sensor 21", MemoryType.Input);//Diffuse Sensor 21 - Buffer 2 end
            sensorMc2loadingConveyorStart = MemoryMap.Instance.GetBit("Diffuse Sensor 3_2", MemoryType.Input);//Diffuse Sensor 3_2
            sensorMc3loadingConveyorStart = MemoryMap.Instance.GetBit("Diffuse Sensor 20", MemoryType.Input);//Diffuse Sensor 20
            sensorEntranceMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 5", MemoryType.Input);//Diffuse Sensor 5 - MC1 entrance
            sensorEntranceMc3 = MemoryMap.Instance.GetBit("Diffuse Sensor 19", MemoryType.Input);//Diffuse Sensor 19 - MC1 entrance
            sensorBadPieceFilterConveyorStartMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 6", MemoryType.Input);//Diffuse Sensor 6 - MC1 bad piece filter conveyor entrance
            sensorBadPieceFilterConveyorEndMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 7", MemoryType.Input);//Diffuse Sensor 7 - MC1 bad piece filter conveyor exit
            sensorExitMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 8", MemoryType.Input);//Diffuse Sensor 8 - MC2 exit/MC2 bad piece filter
            sensorExitMc3 = MemoryMap.Instance.GetBit("Diffuse Sensor 22", MemoryType.Input);//Diffuse Sensor 22 - MC3 exit/MC3 bad piece filter
            sensorFinishedPartExit = MemoryMap.Instance.GetBit("Diffuse Sensor 9", MemoryType.Input);//Diffuse Sensor 9 - Finished piece exit
            sensorBadPieceFilterConveyorStartMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 10", MemoryType.Input);//Diffuse Sensor 10 - MC2 bad piece conveyor entrance
            sensorBadPieceFilterConveyorEndMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);//Diffuse Sensor 11 - MC2 bad piece conveyor exit
            sensorBadPieceFilterConveyorStartMc3 = MemoryMap.Instance.GetBit("Diffuse Sensor 17", MemoryType.Input);//Diffuse Sensor 17 - MC3 bad piece conveyor entrance
            sensorBadPieceFilterConveyorEndMc3 = MemoryMap.Instance.GetBit("Diffuse Sensor 18", MemoryType.Input);//Diffuse Sensor 18 - MC3 bad piece conveyor exit
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

            //Mc3
            mc3StartButton = MemoryMap.Instance.GetBit("Start Button 2", MemoryType.Input);
            mc3FailButton = MemoryMap.Instance.GetBit("Stop Button 2", MemoryType.Input);
            mc3RepairButton = MemoryMap.Instance.GetBit("Reset Button 2", MemoryType.Input);
            mc3Busy = MemoryMap.Instance.GetBit("Machining Center 2 (Is Busy)", MemoryType.Input);
            mc3PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 1 (Clamped)", MemoryType.Input);//Mc3 positioner clamped sensor
            mc3GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 2 (Item Detected)", MemoryType.Input);//Mc3 positioner item detected sensor

            mc3Start = MemoryMap.Instance.GetBit("Machining Center 2 (Start)", MemoryType.Output);
            mc3RedLight = MemoryMap.Instance.GetBit("Stack Light 2 (Red)", MemoryType.Output);
            mc3YellowLight = MemoryMap.Instance.GetBit("Stack Light 2 (Yellow)", MemoryType.Output);
            mc3GreenLight = MemoryMap.Instance.GetBit("Stack Light 2 (Green)", MemoryType.Output);
            mc3AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 2", MemoryType.Output);
            mc3PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 1 (Clamp)", MemoryType.Output);//mc3 positioner clamp
            mc3PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 1 (Raise)", MemoryType.Output);//mc3 positioner rise

            //Emitter
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

            gripperMc3 = new GripperArm(
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 2 X Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 2 Z Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 2 X Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 2 Z Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetBit("Two-Axis Pick & Place 2 (Grab)", MemoryType.Output)
            );

            mc1Lights = new McLightsControl(mc1RedLight, mc1YellowLight, mc1GreenLight);
            mc2Lights = new McLightsControl(mc2RedLight, mc2YellowLight, mc2GreenLight);
            mc3Lights = new McLightsControl(mc3RedLight, mc3YellowLight, mc3GreenLight);

            //conveyor belts
            conveyorMc2Entrance = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 0", MemoryType.Output);
            conveyorMc3Entrance = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 7", MemoryType.Output);
            conveyorMc1Entrance = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 4", MemoryType.Output);
            conveyorMc1BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 1", MemoryType.Output);
            conveyorFinishedPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 2", MemoryType.Output);
            conveyorMc2BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 3", MemoryType.Output);
            conveyorMc3BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 6", MemoryType.Output);
            conveyorEmitter = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 5", MemoryType.Output);
            conveyorBuffer1 = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 1", MemoryType.Output);
            conveyorBuffer2 = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 0", MemoryType.Output);

            //Buffer 1 
            buffer1Stopblade = MemoryMap.Instance.GetBit("Stop Blade 0", MemoryType.Output);

            //Buffer 2
            buffer2Stopblade = MemoryMap.Instance.GetBit("Stop Blade 2", MemoryType.Output);

            //Failing time (potenciometer and display)
            potentiometerMc1 = MemoryMap.Instance.GetFloat("Potentiometer 0 (V)", MemoryType.Input);
            potentiometerMc2 = MemoryMap.Instance.GetFloat("Potentiometer 1 (V)", MemoryType.Input);
            displayMc1 = MemoryMap.Instance.GetFloat("Digital Display 0", MemoryType.Output);
            displayMc2 = MemoryMap.Instance.GetFloat("Digital Display 1", MemoryType.Output);
            stopWatch = new Stopwatch();

            mc1Status = McStatus.IDLE;
            mc2Status = McStatus.IDLE;
            mc3Status = McStatus.IDLE;

            mc1PositionerStatus = McPositionerStatus.UP;
            mc2PositionerStatus = McPositionerStatus.UP;
            mc3PositionerStatus = McPositionerStatus.UP;


            bufferStatus1 = BufferStatus.EMPTY;
            bufferStatus2 = BufferStatus.EMPTY;

            loadingMc2Step = Mc2andMc3LoadingSteps.IDLE;
            loadingMc3Step = Mc2andMc3LoadingSteps.IDLE;

            mc1PieceReady = Mc1PieceReady.NOT_READY;
            mc1PieceReadySteps = Mc1PieceReadySteps.IDLE;

            eventsMc = Events.i1;
            preeventMc = Events.i1;

            breakdownM2 = BreakdownMc2OrMc3.OK;
            breakdownM3 = BreakdownMc2OrMc3.OK;

            mc1WorkingStage = Mc1WorkingStage.CONVEYOR;

            ftAtEntranceMc1 = new FTRIG();
            ftAtEntranceMc2 = new FTRIG();
            ftAtEntranceMc3 = new FTRIG();
            ftAtExitMc1 = new FTRIG();
            ftAtExitMc2 = new FTRIG();
            ftAtExitMc3 = new FTRIG();
            ftAtBufferEnd = new FTRIG();
            ftAtBadPieceExitMc1 = new FTRIG();
            ftAtBadPieceExitMc2 = new FTRIG();
            ftAtBadPieceExitMc3 = new FTRIG();
            ftAtFinishedPieceExit = new FTRIG();
            ftAtBufferStart = new FTRIG();
            ftAtMc2LoadingConveyorStart = new FTRIG();
            ftAtMc3LoadingConveyorStart = new FTRIG();
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

            //mc3
            mc3Failed = false;

            ////mc2 buttons            
            //mc2FailButton.Value = true;//True is unpressed
            mc2StartButton.Value = false;
            mc2RepairButton.Value = false;

            ////mc2 lights
            //mc2RedLight.Value = false; 
            //mc2YellowLight.Value = false;
            //mc2GreenLight.Value = true;

            ////mc3 buttons            
            //mc3FailButton.Value = true;//True is unpressed
            mc3StartButton.Value = false;
            mc3RepairButton.Value = false;

            s1Counter = 0;
            r1Counter = 0;
            s2Counter = 0;
            r2Counter = 0;
            s3Counter = 0;
            r3Counter = 0;

            //Buffer
            buffer1Stopblade.Value = true;//True is rised

            //Failing time and display
            //potentiometerMc1.Value = 1.0;//1 is 10 seconds, 10 is 100 seconds
            //potentiometerMc2.Value = 1.0;

            supervisoryControl = new machines2AndBufferSupervisor();
            supervisoryControl = new machines2AndBufferSupervisor();
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
                    preeventMc = Events.s1;
                    supervisoryApproval = supervisoryControl.On("s1");
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
                    preeventMc = Events.r1;
                    supervisoryApproval = supervisoryControl.On("r1");
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
                    preeventMc = Events.s2;
                    supervisoryApproval = supervisoryControl.On("s2");
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
                    preeventMc = Events.r2;
                    supervisoryApproval = supervisoryControl.On("r2");
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

            //s3
            if (mc3StartButton.Value == true)
            {
                if (s3Counter == 0)
                {
                    preeventMc = Events.s3;
                    supervisoryApproval = supervisoryControl.On("s3");
                    if (supervisoryApproval == true)
                    {
                        eventsMc = Events.s3;
                        s3Counter++;
                    }

                }
            }
            else
            {
                s3Counter = 0;
            }

            //r3
            if (mc3RepairButton.Value == true)
            {
                if (r3Counter == 0)
                {
                    preeventMc = Events.r3;
                    supervisoryApproval = supervisoryControl.On("r3");
                    if (supervisoryApproval == true)
                    {
                        eventsMc = Events.r3;
                        r3Counter++;
                    }

                }

            }
            else
            {
                r3Counter = 0;
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTROLLABLE EVENTS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FALLING TRIGGERS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // Falling triggers
            ftAtEntranceMc1.CLK(sensorEntranceMc1.Value);
            ftAtEntranceMc2.CLK(sensorEntranceMc2.Value);
            ftAtEntranceMc3.CLK(sensorEntranceMc3.Value);
            ftAtExitMc1.CLK(sensorExitMc1.Value);
            ftAtExitMc2.CLK(sensorExitMc2.Value);
            ftAtExitMc3.CLK(sensorExitMc3.Value);
            ftAtBufferEnd.CLK(sensorBuffer1End.Value);
            ftAtBufferStart.CLK(sensorMc2loadingConveyorStart.Value);
            ftAtBadPieceExitMc1.CLK(sensorBadPieceFilterConveyorEndMc1.Value);
            ftAtBadPieceExitMc2.CLK(sensorBadPieceFilterConveyorEndMc2.Value);
            ftAtBadPieceExitMc3.CLK(sensorBadPieceFilterConveyorEndMc3.Value);
            ftAtFinishedPieceExit.CLK(sensorFinishedPartExit.Value);
            ftAtMc2LoadingConveyorStart.CLK(sensorMc2loadingConveyorStart.Value);
            ftAtMc3LoadingConveyorStart.CLK(sensorMc3loadingConveyorStart.Value);
            ftAtEmitter.CLK(sensorMc1ConveyorEntrance.Value);

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FALLING TRIGGERS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONVEYORS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //Buffer 1 conveyor
            if (sensorExitMc1.Value == true)
            {
                conveyorBuffer1.Value = true;
            }
            else if (sensorBuffer1End.Value == true)
            {
                conveyorBuffer1.Value = false;
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

            //Buffer 2 conveyor
            if (sensorExitMc2.Value == true)
            {
                conveyorBuffer2.Value = true;
            }
            else if (sensorBuffer2End.Value == true)
            {
                conveyorBuffer2.Value = false;
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

            //Mc3 bad piece conveyor
            if (sensorBadPieceFilterConveyorStartMc3.Value == true)
            {
                conveyorMc3BadPiece.Value = true;
            }
            else if (ftAtBadPieceExitMc3.Q == true)
            {
                conveyorMc3BadPiece.Value = false;
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONVEYORS ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER 1 STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (bufferStatus1 == BufferStatus.EMPTY)
            {
                //type here
                if (sensorExitMc1.Value == true && mc1Failed == false)
                {
                    bufferStatus1 = BufferStatus.ONE;
                }
            }
            else if (bufferStatus1 == BufferStatus.ONE)
            {
                //type here
                if (sensorEntranceMc2.Value == true)
                {
                    bufferStatus1 = BufferStatus.EMPTY;
                }
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER 1 ENDS   %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER 2 STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (bufferStatus2 == BufferStatus.EMPTY)
            {
                //type here
                if (sensorExitMc2.Value == true && mc2Failed == false)
                {
                    bufferStatus2 = BufferStatus.ONE;
                }
            }
            else if (bufferStatus2 == BufferStatus.ONE)
            {
                //type here
                if (sensorEntranceMc3.Value == true)
                {
                    bufferStatus2 = BufferStatus.EMPTY;
                }
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER 2 ENDS   %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%




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
                        supervisoryApproval = supervisoryControl.On("f1");
                        mc1Failed = true; //will fail next time
                    }
                    else if (mc1Busy.Value == false && mc1Failed == true)
                    {
                        mc1Reset.Value = false;
                        mc1Status = McStatus.DOWN;
                        supervisoryApproval = supervisoryControl.On("b1");
                    }
                }


            }
            else if (mc1Status == McStatus.DOWN)
            {
                mc1AlarmSiren.Value = true;
                mc1Lights.failingLights();
                if (eventsMc == Events.r1)
                {
                    mc1Lights.workingLights();
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
                    if (eventsMc == Events.s2 && bufferStatus1 == BufferStatus.ONE)
                    {
                        loadingMc2Step = Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR;
                    }
                }
                else if (loadingMc2Step == Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR)
                {
                    buffer1Stopblade.Value = false;//Drop Stopblade
                    mc2Start.Value = true;
                    conveyorBuffer1.Value = true;//turn on both conveyors
                    conveyorMc2Entrance.Value = true;//turn on both conveyors

                    if (sensorMc2loadingConveyorStart.Value == true)
                    {
                        loadingMc2Step = Mc2andMc3LoadingSteps.SEPARATE_OTHER_PIECES;
                    }
                }
                else if (loadingMc2Step == Mc2andMc3LoadingSteps.SEPARATE_OTHER_PIECES)
                {
                    conveyorBuffer1.Value = false;//turn of buffer conveyor

                    if (ftAtMc2LoadingConveyorStart.Q == true)
                    {
                        loadingMc2Step = Mc2andMc3LoadingSteps.RESTORING_BUFFER_ORDER;
                    }
                }
                else if (loadingMc2Step == Mc2andMc3LoadingSteps.RESTORING_BUFFER_ORDER)
                {
                    buffer1Stopblade.Value = true;
                    conveyorBuffer1.Value = true;//turn on buffer conveyor

                    if (sensorBuffer1End.Value == true || bufferStatus1 == BufferStatus.EMPTY)
                    {
                        conveyorBuffer1.Value = false;//turn off buffer conveyor
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
                    supervisoryApproval = supervisoryControl.On("f2");
                    mc2Failed = true; //will fail next time
                }
                else if (mc2Busy.Value == false && mc2Failed == true)
                {
                    eventsMc = Events.b2;
                    mc2Status = McStatus.DOWN;
                    supervisoryApproval = supervisoryControl.On("b2");
                }
            }
            else if (mc2Status == McStatus.DOWN)
            {
                mc2AlarmSiren.Value = true;
                mc2Lights.failingLights();
                breakdownM2 = BreakdownMc2OrMc3.KO;
                if (eventsMc == Events.r2)
                {
                    mc2Lights.workingLights();
                    mc2Failed = false;
                    mc2Status = McStatus.IDLE;
                    mc2AlarmSiren.Value = false;
                    breakdownM2 = BreakdownMc2OrMc3.OK;
                }
            }

            // %%%%%%%%%%%% MC2 ENDS %%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%% MC3 STARTS %%%%%%%%%%%%%%%%%%%%

            if (mc3Status == McStatus.IDLE)
            {
                // %%%%%%%%% MC2 LOADING STEPS START %%%%%%%%%%%%%%
                if (loadingMc3Step == Mc2andMc3LoadingSteps.IDLE)
                {
                    //type here
                    if (eventsMc == Events.s3 && bufferStatus2 != BufferStatus.EMPTY)
                    {
                        loadingMc3Step = Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR;
                    }
                }
                else if (loadingMc3Step == Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR)
                {
                    buffer2Stopblade.Value = false;//Drop Stopblade
                    mc3Start.Value = true;
                    conveyorBuffer2.Value = true;//turn on both conveyors
                    conveyorMc3Entrance.Value = true;//turn on both conveyors

                    if (sensorMc3loadingConveyorStart.Value == true)
                    {
                        loadingMc3Step = Mc2andMc3LoadingSteps.SEPARATE_OTHER_PIECES;
                    }
                }
                else if (loadingMc3Step == Mc2andMc3LoadingSteps.SEPARATE_OTHER_PIECES)
                {
                    conveyorBuffer2.Value = false;//turn of buffer conveyor

                    if (ftAtMc3LoadingConveyorStart.Q == true)
                    {
                        loadingMc3Step = Mc2andMc3LoadingSteps.RESTORING_BUFFER_ORDER;
                    }
                }
                else if (loadingMc3Step == Mc2andMc3LoadingSteps.RESTORING_BUFFER_ORDER)
                {
                    buffer2Stopblade.Value = true;
                    conveyorBuffer2.Value = true;//turn on buffer conveyor

                    if (sensorBuffer2End.Value == true || bufferStatus2 == BufferStatus.EMPTY)
                    {
                        conveyorBuffer2.Value = false;//turn off buffer conveyor
                    }
                    if (ftAtEntranceMc3.Q == true)
                    {
                        loadingMc3Step = Mc2andMc3LoadingSteps.REACHED_MC;
                    }
                }
                else if (loadingMc3Step == Mc2andMc3LoadingSteps.REACHED_MC)
                {
                    conveyorMc3Entrance.Value = false;//turn off entrance conveyor
                    loadingMc3Step = Mc2andMc3LoadingSteps.IDLE;
                }

                if (mc3Busy.Value == true)
                {
                    mc3Status = McStatus.WORKING;
                    mc3Start.Value = false;
                }
            }
            else if (mc3Status == McStatus.WORKING)
            {
                if (mc3Busy.Value == false && mc3Failed == false)
                {
                    eventsMc = Events.f3;
                    mc3Status = McStatus.IDLE;
                    supervisoryApproval = supervisoryControl.On("f3");
                    mc3Failed = true; //will fail next time
                }
                else if (mc2Busy.Value == false && mc2Failed == true)
                {
                    eventsMc = Events.b3;
                    mc3Status = McStatus.DOWN;
                    supervisoryApproval = supervisoryControl.On("b3");
                }
            }
            else if (mc3Status == McStatus.DOWN)
            {
                mc3AlarmSiren.Value = true;
                mc3Lights.failingLights();
                breakdownM3 = BreakdownMc2OrMc3.KO;
                if (eventsMc == Events.r3)
                {
                    mc3Lights.workingLights();
                    mc3Failed = false;
                    mc3Status = McStatus.IDLE;
                    mc3AlarmSiren.Value = false;
                    breakdownM3 = BreakdownMc2OrMc3.OK;
                }
            }

            // %%%%%%%%%%%% MC3 ENDS %%%%%%%%%%%%%%%%%%%%

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

            //mc3
            gripperMc3.stateTransition();

            if (mc3PositionerStatus == McPositionerStatus.UP)
            {
                mc3PositionerClamp.Value = false;
                mc3PositionerRise.Value = true;
                if (sensorExitMc3.Value == true && mc2Failed == true)
                {
                    mc3PositionerStatus = McPositionerStatus.DOWN;
                }
            }
            else if (mc3PositionerStatus == McPositionerStatus.DOWN)
            {
                mc3PositionerRise.Value = false;
                if (ftAtExitMc3.Q == true)
                {
                    mc3PositionerStatus = McPositionerStatus.CLAMP;
                }
            }
            else if (mc3PositionerStatus == McPositionerStatus.CLAMP)
            {
                mc3PositionerClamp.Value = true;
                if (mc3PositionerClamped.Value == true)
                {
                    gripperMc3.start();
                    if (mc3GripperItemDetected.Value == true)
                    {
                        mc3PositionerStatus = McPositionerStatus.GOING_UP;
                    }
                }
            }
            else if (mc3PositionerStatus == McPositionerStatus.GOING_UP)
            {
                mc3PositionerClamp.Value = false;
                if (sensorBadPieceFilterConveyorStartMc3.Value == true)
                {
                    mc3PositionerStatus = McPositionerStatus.UP;
                }
            }
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BAD PIECES FILTER ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        }
    }
}
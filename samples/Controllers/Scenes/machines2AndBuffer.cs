//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using Controllers.Scenes;
using EngineIO;
using System;
using System.Diagnostics;

namespace Controllers
{
    public class machines2AndBuffer : Controller
    {
        //Mc1 inputs
        MemoryBit mc1StartButton;
        MemoryBit mc1FailButton;
        MemoryBit mc1RepairButton;
        MemoryBit mc1Busy;
        MemoryInt mc1Progress;
        MemoryBit mc1PositionerClamped;//Mc1 positioner clamped sensor
        MemoryBit mc1GripperItemDetected;//Mc1 positioner item detected sensor
        
        //Mc1 outputs
        MemoryBit mc1Start;//Machining Center start
        MemoryBit mc1Reset;//Machining Center reset (so it leaves piece incomplete)
        MemoryBit mc1RedLight;
        MemoryBit mc1YellowLight;
        MemoryBit mc1GreenLight;
        MemoryBit mc1AlarmSiren;
        MemoryBit mc1PositionerRise;
        MemoryBit mc1PositionerClamp;

        //Mc2 inputs
        MemoryBit mc2StartButton;
        MemoryBit mc2FailButton;
        MemoryBit mc2RepairButton;
        MemoryBit mc2Busy;
        MemoryBit mc2PositionerClamped;
        MemoryBit mc2GripperItemDetected;

        //Mc2 outputs
        MemoryBit mc2Start;
        MemoryBit mc2RedLight;
        MemoryBit mc2YellowLight;
        MemoryBit mc2GreenLight;
        MemoryBit mc2AlarmSiren;
        MemoryBit mc2PositionerClamp;
        MemoryBit mc2PositionerRise;

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
        MemoryBit sensorBufferSpot2;//Diffuse Sensor 13 
        MemoryBit sensorBufferSpot3;//Diffuse Sensor 14
        MemoryBit sensorEmitter;

        //Emitter
        MemoryBit emitterStartButton; //Emitter start button
        MemoryBit emitter;//Emitter

        //Buffer
        MemoryBit bufferStopblade;//Buffer stopblade

        //GripperArms
        GripperArm gripperMc1;
        GripperArm gripperMc2;

        //Light controls
        McLightsControl mc1Lights;
        McLightsControl mc2Lights;

        //SUPERVISORY CONTROL
        machines2AndBufferSupervisor supervisoryControl;
        bool supervisoryApproval;

        //conveyor belts
        MemoryBit conveyorMc2Entrance;
        MemoryBit conveyorMc1Entrance;
        MemoryBit conveyorMc1BadPiece;
        MemoryBit conveyorFinishedPiece;
        MemoryBit conveyorMc2BadPiece;
        MemoryBit conveyorEmitter;
        MemoryBit conveyorBuffer;

        //Failing time (potenciometer and display)
        MemoryFloat potentiometerMc1;
        MemoryFloat potentiometerMc2;
        MemoryFloat displayMc1;
        MemoryFloat displayMc2;
        Stopwatch stopWatch;
        float tiempo;
        bool mc1Failed;
        bool mc2Failed;
        string elapsedTime;

        //Others
        bool initialStateMessagePrinted;

        //Enums
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

        //Controllable events
        private int s1Counter;
        private int r1Counter;
        private int s2Counter;
        private int r2Counter;

        //FTRIG
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

        //RTRIG
        RTRIG rtAtExitMc1;
        RTRIG rtAtExitMc2;
        RTRIG rtAtMc2LoadingConveyorStart;

        public machines2AndBuffer()// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONSTRUCTOR STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        {
            //Mc1 inputs
            mc1StartButton = MemoryMap.Instance.GetBit("Start Button 0", MemoryType.Input);
            mc1FailButton = MemoryMap.Instance.GetBit("Stop Button 0", MemoryType.Input);
            mc1RepairButton = MemoryMap.Instance.GetBit("Reset Button 0", MemoryType.Input);
            mc1Busy = MemoryMap.Instance.GetBit("Machining Center 0 (Is Busy)", MemoryType.Input);//Machining Center 0 busy
            mc1Progress = MemoryMap.Instance.GetInt("Machining Center 0 (Progress)", MemoryType.Input);//Machining Center 0 opened
            mc1PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 0 (Clamped)", MemoryType.Input);//Mc1 positioner clamped sensor
            mc1GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Item Detected)", MemoryType.Input);//Mc1 positioner item detected sensor

            //Mc1 outputs
            mc1Start = MemoryMap.Instance.GetBit("Machining Center 0 (Start)", MemoryType.Output);//Machining Center start
            mc1Reset = MemoryMap.Instance.GetBit("Machining Center 0 (Reset)", MemoryType.Output);//Machining Center reset (so it leaves piece incomplete)
            mc1RedLight = MemoryMap.Instance.GetBit("Stack Light 0 (Red)", MemoryType.Output);
            mc1YellowLight = MemoryMap.Instance.GetBit("Stack Light 0 (Yellow)", MemoryType.Output);
            mc1GreenLight = MemoryMap.Instance.GetBit("Stack Light 0 (Green)", MemoryType.Output);
            mc1AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 0", MemoryType.Output);
            mc1PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 0 (Raise)", MemoryType.Output);
            mc1PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 0 (Clamp)", MemoryType.Output);

            //mc1 machine values
            mc1Start.Value = false;
            mc1Busy.Value = false;

            //mc1 buttons
            mc1StartButton.Value = false;
            mc1FailButton.Value = true;//unpressed is true
            mc1RepairButton.Value = false;

            //mc1 others
            mc1PositionerClamp.Value = false;
            mc1PositionerRise.Value = true;

            //Mc2 inputs
            mc2StartButton = MemoryMap.Instance.GetBit("Start Button 1", MemoryType.Input);
            mc2StartButton.Value = false;
            mc2FailButton = MemoryMap.Instance.GetBit("Stop Button 1", MemoryType.Input);
            mc2FailButton.Value = true;//True is unpressed
            mc2RepairButton = MemoryMap.Instance.GetBit("Reset Button 1", MemoryType.Input);
            mc2RepairButton.Value = false;
            mc2Busy = MemoryMap.Instance.GetBit("Machining Center 1 (Is Busy)", MemoryType.Input);
            mc2PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamped)", MemoryType.Input);//Mc2 positioner clamped sensor
            mc2GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 1 (Item Detected)", MemoryType.Input);//Mc2 positioner item detected sensor

            //Mc2 outputs
            mc2Start = MemoryMap.Instance.GetBit("Machining Center 1 (Start)", MemoryType.Output);
            mc2RedLight = MemoryMap.Instance.GetBit("Stack Light 1 (Red)", MemoryType.Output);
            mc2YellowLight = MemoryMap.Instance.GetBit("Stack Light 1 (Yellow)", MemoryType.Output);
            mc2GreenLight = MemoryMap.Instance.GetBit("Stack Light 1 (Green)", MemoryType.Output);
            mc2AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 1", MemoryType.Output);
            mc2PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamp)", MemoryType.Output);//mc2 positioner clamp
            mc2PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 3 (Raise)", MemoryType.Output);//mc2 positioner rise

            //mc2 others
            mc2Failed = false;

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
            sensorBufferSpot2 = MemoryMap.Instance.GetBit("Diffuse Sensor 13", MemoryType.Input);//Diffuse Sensor 13
            sensorBufferSpot3 = MemoryMap.Instance.GetBit("Diffuse Sensor 14", MemoryType.Input);//Diffuse Sensor 14
            sensorEmitter = MemoryMap.Instance.GetBit("Diffuse Sensor 4", MemoryType.Input);//Diffuse Sensor 4 - Sensor emitter

            //Emitter
            emitterStartButton = MemoryMap.Instance.GetBit("Start Button 2", MemoryType.Input); //Emitter start button
            emitter = MemoryMap.Instance.GetBit("Emitter 0 (Emit)", MemoryType.Output);//Emitter
            emitter.Value = true;

            //Buffer
            bufferStopblade = MemoryMap.Instance.GetBit("Stop Blade 0", MemoryType.Output);
            bufferStopblade.Value = true;//True is rised

            //GripperArms
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

            //Light controls
            mc1Lights = new McLightsControl(mc1RedLight, mc1YellowLight, mc1GreenLight);
            mc2Lights = new McLightsControl(mc2RedLight, mc2YellowLight, mc2GreenLight);

            //conveyor belts
            conveyorMc2Entrance = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 0", MemoryType.Output);
            conveyorMc1Entrance = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 4", MemoryType.Output);
            conveyorMc1BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 1", MemoryType.Output);
            conveyorFinishedPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 2", MemoryType.Output);
            conveyorMc2BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 3", MemoryType.Output);
            conveyorEmitter = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 5", MemoryType.Output);
            conveyorBuffer = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 1", MemoryType.Output);

            //Failing time (potenciometer and display)
            potentiometerMc1 = MemoryMap.Instance.GetFloat("Potentiometer 0 (V)", MemoryType.Input);
            potentiometerMc2 = MemoryMap.Instance.GetFloat("Potentiometer 1 (V)", MemoryType.Input);
            displayMc1 = MemoryMap.Instance.GetFloat("Digital Display 0", MemoryType.Output);
            displayMc2 = MemoryMap.Instance.GetFloat("Digital Display 1", MemoryType.Output);
            stopWatch = new Stopwatch();

            //Enums
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

            //FTRIG
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

            //RTRIG
            rtAtExitMc1 = new RTRIG();
            rtAtExitMc2 = new RTRIG();
            rtAtMc2LoadingConveyorStart = new RTRIG();

            // Controllable events
            s1Counter = 0;
            r1Counter = 0;
            s2Counter = 0;
            r2Counter = 0;

            //SUPERVISORY CONTROL
            supervisoryControl = new machines2AndBufferSupervisor();
            supervisoryControl = new machines2AndBufferSupervisor();
            supervisoryApproval = true;

            //Trick for printing initial state in console after start up messages
            initialStateMessagePrinted = false;

        } // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONSTRUCTOR ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public override void Execute(int elapsedMilliseconds) // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% EXECUTE STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        {


            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% TRICK FOR PRINTING INITIAL STATE AFTER START UP MESSAGES START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            if (initialStateMessagePrinted == false)
            {
                supervisoryControl.CreateController();
                initialStateMessagePrinted = true;
            }
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% TRICK FOR PRINTING INITIAL STATE AFTER START UP MESSAGES END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FAILING TIME AND DISPLAY START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            displayMc1.Value = float.Parse(String.Format("{0:0.0}", potentiometerMc1.Value));
            displayMc2.Value = float.Parse(String.Format("{0:0.0}", potentiometerMc2.Value));
            stopWatch.Start();
            TimeSpan ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            tiempo = float.Parse(String.Format("{0:0.0}", ts.Seconds + "." + ts.Milliseconds / 10));
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FAILING TIME AND DISPLAY END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTROLLABLE EVENTS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            //s1
            if (mc1StartButton.Value == true)
            {
                if (s1Counter == 0)
                {
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
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTROLLABLE EVENTS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% UNCONTROLLABLE EVENTS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            //if (mc1FailButton.Value == false || float.Parse(String.Format("{0:0.0}", (tiempo + 0.01)%displayMc1.Value)) == 0.0f)//false is button pressed
            if (mc1FailButton.Value == false || float.Parse(String.Format("{0:0.0}", (tiempo + 0.01f) % (displayMc1.Value * 100) )) == 0.0f)//false is button pressed
            {
                mc1Failed = true;
            }

            if (mc2FailButton.Value == false || float.Parse(String.Format("{0:0.0}", (tiempo + 0.01f) % (displayMc2.Value * 100) )) == 0.0f)//false is button pressed)
            {
                mc2Failed = true;
            }
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% UNCONTROLLABLE EVENTS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FALLING AND RISING TRIGGERS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
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

            rtAtExitMc1.CLK(sensorExitMc1.Value);
            rtAtExitMc2.CLK(sensorExitMc2.Value);
            rtAtMc2LoadingConveyorStart.CLK(sensorMc2loadingConveyorStart.Value);
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FALLING AND RISING TRIGGERS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%




            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONVEYORS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //Mc1 bad piece conveyor
            if (sensorBadPieceFilterConveyorStartMc1.Value == true)
            {
                conveyorMc1BadPiece.Value = true;
            }
            else if (ftAtBadPieceExitMc1.Q == true)
            {
                conveyorMc1BadPiece.Value = false;
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
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONVEYORS ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            if (bufferStatus == BufferStatus.EMPTY)
            {
                //type here
                if (rtAtExitMc1.Q == true && mc1Failed == false)
                {
                    conveyorBuffer.Value = true;
                    bufferStatus = BufferStatus.ONE;
                }
            }
            else if (bufferStatus == BufferStatus.ONE)
            {
                if (sensorBufferEnd.Value == true && loadingMc2Step == Mc2andMc3LoadingSteps.IDLE)
                {
                    conveyorBuffer.Value = false;
                }
                if (rtAtMc2LoadingConveyorStart.Q == true)
                {
                    bufferStatus = BufferStatus.EMPTY;
                }
                if (rtAtExitMc1.Q == true && mc1Failed == false)
                {
                    conveyorBuffer.Value = true;
                    bufferStatus = BufferStatus.TWO;
                }
            }
            else if (bufferStatus == BufferStatus.TWO)
            {
                if (sensorBufferSpot2.Value == true && loadingMc2Step == Mc2andMc3LoadingSteps.IDLE)
                {
                    conveyorBuffer.Value = false;
                }
                if (rtAtMc2LoadingConveyorStart.Q == true)
                {
                    bufferStatus = BufferStatus.ONE;
                }
                if (rtAtExitMc1.Q == true && mc1Failed == false)
                {
                    conveyorBuffer.Value = true;
                    bufferStatus = BufferStatus.THREE;
                }
            }
            else if (bufferStatus == BufferStatus.THREE)
            {
                if (sensorBufferSpot3.Value == true && loadingMc2Step == Mc2andMc3LoadingSteps.IDLE)
                {
                    conveyorBuffer.Value = false;
                }
                if (rtAtMc2LoadingConveyorStart.Q == true)
                {
                    bufferStatus = BufferStatus.TWO;
                }
            }
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% MACHINE CENTER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%%%%%%%%%% MC1 STARTS %%%%%%%%%%%%%%%%%%%%

            // %%%% BAD PIECES FILTER STARTS %%%%%
            gripperMc1.stateTransition();

            if (mc1PositionerStatus == McPositionerStatus.UP)
            {
                mc1PositionerClamp.Value = false;
                mc1PositionerRise.Value = true;
                if (rtAtExitMc1.Q == true && mc1Failed == true)
                {
                    conveyorBuffer.Value = true;
                    mc1PositionerStatus = McPositionerStatus.DOWN;
                }
            }
            else if (mc1PositionerStatus == McPositionerStatus.DOWN)
            {

                mc1PositionerRise.Value = false;
                if (ftAtExitMc1.Q == true)
                {
                    conveyorBuffer.Value = false;
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
            // %%%% BAD PIECES FILTER ENDS %%%%

            // %%%% PREPARING MC1 PIECE STARTS %%%%

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

            // %%%% PREPARING MC1 PIECE ENDS %%%%%


            // %%%% MC1 IDLE STARTS %%%%%
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
            // %%%% MC1 IDLE ENDS %%%%%

            // %%%% MC1 WORKING STARTS %%%%%
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
                    if (rtAtExitMc1.Q == true && mc1Failed == false)
                    {
                        mc1Reset.Value = false;
                        mc1Status = McStatus.IDLE;
                        supervisoryApproval = supervisoryControl.On("f1");
                        //mc1Failed = true; //will fail next time
                    }
                    else if (rtAtExitMc1.Q == true && mc1Failed == true)
                    {
                        mc1Reset.Value = false;
                        mc1Status = McStatus.DOWN;
                        supervisoryApproval = supervisoryControl.On("b1");
                    }
                }

                
            }
            // %%%% MC1 WORKING ENDS %%%%%

            // %%%% MC1 DOWN STARTS %%%%%
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
            // %%%% MC1 DOWN ENDS %%%%%

            // %%%%%%%%%%%%%%%%%%%% MC1 ENDS %%%%%%%%%%%%%%%%%%%%



            // %%%%%%%%%%%%%%%%%%%% MC2 STARTS %%%%%%%%%%%%%%%%%%%%

            //%%%% BAD PIECES FILTER STARTS %%%%%
            gripperMc2.stateTransition();

            if (mc2PositionerStatus == McPositionerStatus.UP)
            {
                mc2PositionerClamp.Value = false;
                mc2PositionerRise.Value = true;
                if (rtAtExitMc2.Q == true && mc2Failed == true)
                {
                    conveyorFinishedPiece.Value = true;
                    mc2PositionerStatus = McPositionerStatus.DOWN;
                }
            }
            else if (mc2PositionerStatus == McPositionerStatus.DOWN)
            {
                mc2PositionerRise.Value = false;
                if (ftAtExitMc2.Q == true)
                {
                    conveyorFinishedPiece.Value = false;
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
            //%%% BAD PIECES FILTER ENDS %%%%
            
            //%%% MC2 IDLE STARTS %%%%
            if (mc2Status == McStatus.IDLE)
            {
                if (eventsMc == Events.s2 && bufferStatus != BufferStatus.EMPTY && loadingMc2Step == Mc2andMc3LoadingSteps.IDLE )
                {
                    mc2Status = McStatus.WORKING;
                    loadingMc2Step = Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR;
                }
            }
            //%%% MC2 IDLE ENDS %%%%

            //%%% MC2 WORKING STARTS %%%%
            else if (mc2Status == McStatus.WORKING)
            {
                if (loadingMc2Step == Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR)
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
                    mc2Start.Value = false;
                }

                if (rtAtExitMc2.Q == true && mc2Failed == false)
                {
                    conveyorFinishedPiece.Value = true;
                    eventsMc = Events.f2;
                    mc2Status = McStatus.IDLE;
                    supervisoryApproval = supervisoryControl.On("f2");                    
                    //mc2Failed = true; //will fail next time
                }
                else if (rtAtExitMc2.Q == true && mc2Failed == true)
                {
                    eventsMc = Events.b2;
                    mc2Status = McStatus.DOWN;
                    supervisoryApproval = supervisoryControl.On("b2");
                }
            }
            //%%% MC2 WORKING ENDS %%%%

            //%%% MC2 DOWN STARTS %%%%
            else if (mc2Status == McStatus.DOWN)
            {
                mc2AlarmSiren.Value = true;
                breakdownM2 = BreakdownMc2OrMc3.KO;
                mc2Lights.failingLights();
                
                if (eventsMc == Events.r2)
                {
                    mc2Lights.workingLights();
                    mc2Failed = false;
                    mc2Status = McStatus.IDLE;
                    mc2AlarmSiren.Value = false;
                    breakdownM2 = BreakdownMc2OrMc3.OK;
                }
            }
            //%%% MC2 DOWN ENDS %%%%


            // %%%%%%%%%%%%%%%%%%%% MC2 ENDS %%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% MACHINE CENTER ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FINISHED PIECE STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            if (ftAtFinishedPieceExit.Q == true)
            {
                conveyorFinishedPiece.Value = false;
            }
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FINISHED PIECE ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        }
    }
}

public enum BufferStatus
{
    EMPTY,
    ONE,
    TWO,
    THREE
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

public enum McStatus
{
    IDLE,
    WORKING,
    DOWN
}




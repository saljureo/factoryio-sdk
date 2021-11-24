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
        MemoryBit sensorMc1LoadingConveyorStart;//Diffuse Sensor 3_2 - Buffer start
        MemoryBit sensorEntranceMc1;//Diffuse Sensor 5 - MC1 entrance
        MemoryBit sensorBadPieceFilterConveyorStartMc0;//Diffuse Sensor 6 - MC0 bad piece filter conveyor entrance
        MemoryBit sensorBadPieceFilterConveyorEndMc0;//Diffuse Sensor 7 - MC0 bad piece filter conveyor exit
        MemoryBit sensorExitMc1;//Diffuse Sensor 8 - MC1 exit/MC1 bad piece filter
        MemoryBit sensorFinishedPartExit;//Diffuse Sensor 9 - Finished piece exit
        MemoryBit sensorBadPieceFilterConveyorStartMc1;//Diffuse Sensor 10 - MC1 bad piece conveyor entrance
        MemoryBit sensorBadPieceFilterConveyorEndMc1;//Diffuse Sensor 11 - MC1 bad piece conveyor exit
        MemoryBit sensorBufferSpot2;//Diffuse Sensor 13 - spot 2 on buffer
        MemoryBit sensorEmitter;//Diffuse Sensor 13 - spot 2 on buffer

        //Mc1
        MemoryBit mc1StartButton; //MC start button
        MemoryBit mc1FailButton; //MC fail button
        MemoryBit mc1RepairButton; //MC repair button
        MemoryBit mc1Busy;//Machining Center 1 busy
        MemoryBit mc1PositionerClamped;//Mc1 positioner clamped sensor
        MemoryBit mc1GripperItemDetected;//Mc1 positioner item detected sensor

        MemoryBit mc1Start;//Machining Center 1 start
        MemoryBit mc1RedLight;
        MemoryBit mc1YellowLight;
        MemoryBit mc1GreenLight;
        MemoryBit mc1AlarmSiren;
        MemoryBit mc1PositionerClamp;//mc1 positioner clamp
        MemoryBit mc1PositionerRise;//mc1 positioner rise

        //Emitter
        MemoryBit emitterStartButton; //Emitter start button
        MemoryBit emitter;//Emitter

        GripperArm gripperMc0;

        GripperArm gripperMc1;

        SupervisoryControl supervisoryControl;

        //conveyor belts
        MemoryBit conveyorMc1Entrance;
        MemoryBit conveyorMc0Entrance;
        MemoryBit conveyorMc0BadPiece;
        MemoryBit conveyorFinishedPiece;
        MemoryBit conveyorMc1BadPiece;
        MemoryBit conveyorEmitter;
        MemoryBit conveyorBuffer;

        //Buffer
        MemoryBit bufferStopblade;//Buffer stopblade

        bool mc0Failed;
        bool mc1Failed;

        McStatus mc0Status;
        McStatus mc1Status;

        McPositionerStatus mc0PositionerStatus = McPositionerStatus.UP;
        McPositionerStatus mc1PositionerStatus = McPositionerStatus.UP;

        BufferStatus bufferStatus = BufferStatus.EMPTY;

        Mc1LoadingSteps loadingMc1Step = Mc1LoadingSteps.IDLE;

        Mc0PieceReady mc0PieceReady;
        Mc0PieceReadySteps mc0PieceReadySteps;

        EventsMc0 eventsMc0;
        EventsMc1 eventsMc1;

        private int s0Counter;
        private int r0Counter;
        private int s1Counter;
        private int r1Counter;

        FTRIG ftAtEntranceMc0;
        FTRIG ftAtExitMc0;
        FTRIG ftAtExitMc1;
        FTRIG ftAtBufferEnd;
        FTRIG ftAtBadPieceExitMc0;
        FTRIG ftAtBadPieceExitMc1;
        FTRIG ftAtFinishedPieceExit;
        FTRIG ftAtBufferStart;
        FTRIG ftAtMc1LoadingConveyorStart;
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
            sensorMc1LoadingConveyorStart = MemoryMap.Instance.GetBit("Diffuse Sensor 3_2", MemoryType.Input);//Diffuse Sensor 3_2 - Buffer start
            sensorEntranceMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 5", MemoryType.Input);//Diffuse Sensor 5 - MC1 entrance
            sensorBadPieceFilterConveyorStartMc0 = MemoryMap.Instance.GetBit("Diffuse Sensor 6", MemoryType.Input);//Diffuse Sensor 6 - MC0 bad piece filter conveyor entrance
            sensorBadPieceFilterConveyorEndMc0 = MemoryMap.Instance.GetBit("Diffuse Sensor 7", MemoryType.Input);//Diffuse Sensor 7 - MC0 bad piece filter conveyor exit
            sensorExitMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 8", MemoryType.Input);//Diffuse Sensor 8 - MC1 exit/MC1 bad piece filter
            sensorFinishedPartExit = MemoryMap.Instance.GetBit("Diffuse Sensor 9", MemoryType.Input);//Diffuse Sensor 9 - Finished piece exit
            sensorBadPieceFilterConveyorStartMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 10", MemoryType.Input);//Diffuse Sensor 10 - MC1 bad piece conveyor entrance
            sensorBadPieceFilterConveyorEndMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);//Diffuse Sensor 11 - MC1 bad piece conveyor exit
            sensorBufferSpot2 = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);//Diffuse Sensor 13 - spot 2 on buffer
            sensorEmitter = MemoryMap.Instance.GetBit("Diffuse Sensor 4", MemoryType.Input);//Diffuse Sensor 4 - Sensor emitter

            //Mc1
            mc1StartButton = MemoryMap.Instance.GetBit("Start Button 1", MemoryType.Input); //MC start button
            mc1FailButton = MemoryMap.Instance.GetBit("Stop Button 1", MemoryType.Input); //MC fail button
            mc1RepairButton = MemoryMap.Instance.GetBit("Reset Button 1", MemoryType.Input); //MC repair button
            mc1Busy = MemoryMap.Instance.GetBit("Machining Center 1 (Is Busy)", MemoryType.Input);//Machining Center 1 busy
            mc1PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamped)", MemoryType.Input);//Mc1 positioner clamped sensor
            mc1GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 1 (Item Detected)", MemoryType.Input);//Mc1 positioner item detected sensor

            mc1Start = MemoryMap.Instance.GetBit("Machining Center 1 (Start)", MemoryType.Output);//Machining Center 1 start
            mc1RedLight = MemoryMap.Instance.GetBit("Stack Light 1 (Red)", MemoryType.Output);
            mc1YellowLight = MemoryMap.Instance.GetBit("Stack Light 1 (Yellow)", MemoryType.Output);
            mc1GreenLight = MemoryMap.Instance.GetBit("Stack Light 1 (Green)", MemoryType.Output);
            mc1AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 1", MemoryType.Output);
            mc1PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamp)", MemoryType.Output);//mc1 positioner clamp
            mc1PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 3 (Raise)", MemoryType.Output);//mc1 positioner rise

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

            gripperMc1 = new GripperArm(
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 X Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 Z Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 X Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 1 Z Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetBit("Two-Axis Pick & Place 1 (Grab)", MemoryType.Output)
            );

            
            //conveyor belts
            conveyorMc1Entrance = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 0", MemoryType.Output);//Conveyor mc1 entrance
            conveyorMc0Entrance = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 4", MemoryType.Output);//Conveyor mc1 entrance
            conveyorMc0BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 1", MemoryType.Output);//Conveyor mc0 bad piece
            conveyorFinishedPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 2", MemoryType.Output);//Conveyor finished piece
            conveyorMc1BadPiece = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 3", MemoryType.Output);//Conveyor mc1 bad piece
            conveyorEmitter = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 5", MemoryType.Output);//Conveyor emitter
            conveyorBuffer = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 1", MemoryType.Output);//Conveyor buffer

            //Buffer
            bufferStopblade = MemoryMap.Instance.GetBit("Stop Blade 0", MemoryType.Output);//Buffer stopblade

            mc0Status = McStatus.IDLE;
            mc1Status = McStatus.IDLE;

            McPositionerStatus mc0PositionerStatus = McPositionerStatus.UP;
            McPositionerStatus mc1PositionerStatus = McPositionerStatus.UP;

            BufferStatus bufferStatus = BufferStatus.EMPTY;

            Mc1LoadingSteps loadingMc1Step = Mc1LoadingSteps.IDLE;

            mc0PieceReady = Mc0PieceReady.NOT_READY;
            mc0PieceReadySteps = Mc0PieceReadySteps.IDLE;

            eventsMc0 = EventsMc0.i0;
            eventsMc1 = EventsMc1.i1;

            ftAtEntranceMc0 = new FTRIG();
            ftAtExitMc0 = new FTRIG();
            ftAtExitMc1 = new FTRIG();
            ftAtBufferEnd = new FTRIG();
            ftAtBadPieceExitMc0 = new FTRIG();
            ftAtBadPieceExitMc1 = new FTRIG();
            ftAtFinishedPieceExit = new FTRIG();
            ftAtBufferStart = new FTRIG();
            ftAtMc1LoadingConveyorStart = new FTRIG();
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

            s0Counter = 0;
            r0Counter = 0;
            s1Counter = 0;
            r1Counter = 0;

            //Buffer
            bufferStopblade.Value = true;//True is rised

            Console.WriteLine("State mc0Status: " + mc0Status);
            Console.WriteLine("State mc1Status: " + mc1Status);
            Console.WriteLine("State bufferStatus: " + bufferStatus);

            supervisoryControl = new SupervisoryControl();

        } // %%%%%%%%%%%%%%%%% CONSTRUCTOR ENDS %%%%%%%%%%%%%%%%

        public override void Execute(int elapsedMilliseconds) // %%%%%%%%%%%%%%%%% EXECUTE STARTS %%%%%%%%%%%%%%%%
        {
            supervisoryControl.On(mc0Status, mc1Status);
            Console.WriteLine("Programa principal mc0Status: " + mc0Status);
            Console.WriteLine("Programa principal mc1Status: " + mc1Status);
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

            //s1
            if (mc1StartButton.Value == true)
            {
                if (s1Counter == 0)
                {
                    eventsMc1 = EventsMc1.s1;
                    Console.WriteLine("s1 (c)");
                    Console.WriteLine("State mc1Status: WORKING");
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
                    eventsMc1 = EventsMc1.r1;
                    Console.WriteLine("r1 (c)");
                    r1Counter++;
                }

            }
            else
            {
                r1Counter = 0;
            }


            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTROLLABLE EVENTS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


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
                    Console.WriteLine("f0 (uc)");
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

            // %%%%%%%%%%%% MC1 STARTS %%%%%%%%%%%%%%%%%%%%

            if (mc1Status == McStatus.IDLE)
            {
                // %%%%%%%%% MC1 LOADING STEPS START %%%%%%%%%%%%%%
                if (loadingMc1Step == Mc1LoadingSteps.IDLE)
                {
                    //type here
                    if (eventsMc1 == EventsMc1.s1 && bufferStatus == BufferStatus.FULL)
                    {
                        loadingMc1Step = Mc1LoadingSteps.PIECE_TO_LOADING_CONVEYOR;
                    }
                }
                else if (loadingMc1Step == Mc1LoadingSteps.PIECE_TO_LOADING_CONVEYOR)
                {
                    bufferStopblade.Value = false;//Drop Stopblade
                    mc1Start.Value = true;
                    conveyorBuffer.Value = true;
                    conveyorMc1Entrance.Value = true;

                    if (sensorMc1LoadingConveyorStart.Value == true)
                    {
                        loadingMc1Step = Mc1LoadingSteps.SEPARATE_OTHER_PIECES;
                    }
                }
                else if (loadingMc1Step == Mc1LoadingSteps.SEPARATE_OTHER_PIECES)
                {
                    conveyorBuffer.Value = false;

                    if (ftAtMc1LoadingConveyorStart.Q == true)
                    {
                        loadingMc1Step = Mc1LoadingSteps.RESTORING_BUFFER_ORDER;
                    }
                }
                else if (loadingMc1Step == Mc1LoadingSteps.RESTORING_BUFFER_ORDER)
                {
                    bufferStopblade.Value = true;
                    conveyorBuffer.Value = true;

                    if (sensorBufferEnd.Value == true)
                    {
                        conveyorBuffer.Value = false;
                        loadingMc1Step = Mc1LoadingSteps.IDLE;
                    }
                }

                if (mc1Busy.Value == true)
                {
                    mc1Status = McStatus.WORKING;
                    mc1Start.Value = false;
                }
            }
            else if (mc1Status == McStatus.WORKING)
            {
                if (mc1Busy.Value == false && mc1Failed == false)
                {
                    eventsMc1 = EventsMc1.f1;
                    Console.WriteLine("f1 (uc)");
                    mc1Status = McStatus.IDLE;
                    Console.WriteLine("State mc1Status: " + mc1Status);
                    mc1Failed = true; //will fail next time
                }
                else if (mc1Busy.Value == false && mc1Failed == true)
                {
                    eventsMc1 = EventsMc1.b1;
                    Console.WriteLine("b1 (uc)");
                    mc1Status = McStatus.DOWN;
                    Console.WriteLine("State mc1Status: " + mc1Status);
                }
            }
            else if (mc1Status == McStatus.DOWN)
            {
                mc1AlarmSiren.Value = true;

                if (eventsMc1 == EventsMc1.r1)
                {
                    mc1Failed = false;
                    mc1Status = McStatus.IDLE;
                    Console.WriteLine("State mc1Status: " + mc1Status);
                    mc1AlarmSiren.Value = false;
                }
            }

            // %%%%%%%%%%%% MC1 ENDS %%%%%%%%%%%%%%%%%%%%

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

public enum BufferSteps
{
    PIECE_ARRIVED_TO_BUFFER,
    FILTERING_BAD_PIECE,
    PIECE_REACHING_MC1,
    PIECE_ARRIVED_TO_MC1
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

public enum Mc1LoadingSteps
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

public enum EventsMc1
{
    s1,
    f1,
    b1,
    r1,
    i1
}




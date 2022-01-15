//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;
using System;
using System.Diagnostics;

namespace Controllers.Scenes.MachinesAndBuffer
{
    public class Machines3AndBuffer : Controller
    {
        //Reader
        Reader reader;
        string newState;
        string newStateName;
        bool changeStateMessagePrinted;

        //Mc1 inputs
        readonly MemoryBit mc1StartButton;
        readonly MemoryBit mc1FailButton;
        readonly MemoryBit mc1RepairButton;
        readonly MemoryBit mc1Busy;
        readonly MemoryInt mc1Progress;
        readonly MemoryBit mc1PositionerClamped;//Mc1 positioner clamped sensor
        readonly MemoryBit mc1GripperItemDetected;//Mc1 positioner item detected sensor

        //Mc1 outputs
        readonly MemoryBit mc1Start;//Machining Center start
        readonly MemoryBit mc1Reset;//Machining Center reset (so it leaves piece incomplete)
        readonly MemoryBit mc1RedLight;
        readonly MemoryBit mc1YellowLight;
        readonly MemoryBit mc1GreenLight;
        readonly MemoryBit mc1AlarmSiren;
        readonly MemoryBit mc1PositionerRise;
        readonly MemoryBit mc1PositionerClamp;

        //Mc1 others
        bool mc1Failed;

        //Mc2 inputs
        readonly MemoryBit mc2StartButton;
        readonly MemoryBit mc2FailButton;
        readonly MemoryBit mc2RepairButton;
        readonly MemoryBit mc2Busy;
        readonly MemoryInt mc2Progress;
        readonly MemoryBit mc2PositionerClamped;
        readonly MemoryBit mc2GripperItemDetected;

        //Mc2 outputs
        readonly MemoryBit mc2Start;
        readonly MemoryBit mc2Reset;//Machining Center reset (so it leaves piece incomplete)
        readonly MemoryBit mc2RedLight;
        readonly MemoryBit mc2YellowLight;
        readonly MemoryBit mc2GreenLight;
        readonly MemoryBit mc2AlarmSiren;
        readonly MemoryBit mc2PositionerClamp;
        readonly MemoryBit mc2PositionerRise;

        //Mc2 others
        bool mc2Failed;

        //Mc3 inputs
        readonly MemoryBit mc3StartButton;
        readonly MemoryBit mc3FailButton;
        readonly MemoryBit mc3RepairButton;
        readonly MemoryBit mc3Busy;
        readonly MemoryBit mc3PositionerClamped;
        readonly MemoryBit mc3GripperItemDetected;

        //Mc3 outputs
        readonly MemoryBit mc3Start;
        readonly MemoryBit mc3RedLight;
        readonly MemoryBit mc3YellowLight;
        readonly MemoryBit mc3GreenLight;
        readonly MemoryBit mc3AlarmSiren;
        readonly MemoryBit mc3PositionerClamp;
        readonly MemoryBit mc3PositionerRise;

        //Mc3 others
        bool mc3Failed;

        //Sensors
        readonly MemoryBit sensorMc1ConveyorEntrance;//Diffuse Sensor 0 - Emitter
        readonly MemoryBit sensorEntranceMc1;//Diffuse Sensor 1 - MC1 entrance
        readonly MemoryBit sensorExitMc1;//Diffuse Sensor 2 - MC1 exit/Buffer conveyor entry/MC1 bad piece filter
        readonly MemoryBit sensorBuffer1End;//Diffuse Sensor 3
        readonly MemoryBit sensorBuffer2End;//Diffuse Sensor 21
        readonly MemoryBit sensorMc2loadingConveyorStart;//Diffuse Sensor 3_2 - Buffer1 start
        readonly MemoryBit sensorMc3loadingConveyorStart;//Diffuse Sensor 20 - Buffer2 start
        readonly MemoryBit sensorEntranceMc2;//Diffuse Sensor 5 - MC2 entrance
        readonly MemoryBit sensorEntranceMc3;//Diffuse Sensor 19 - MC3 entrance
        readonly MemoryBit sensorBadPieceFilterConveyorStartMc1;//Diffuse Sensor 6 - MC0 bad piece filter conveyor entrance
        readonly MemoryBit sensorBadPieceFilterConveyorEndMc1;//Diffuse Sensor 7 - MC0 bad piece filter conveyor exit
        readonly MemoryBit sensorExitMc2;//Diffuse Sensor 8 - MC2 exit/MC2 bad piece filter
        readonly MemoryBit sensorExitMc3;//Diffuse Sensor 22 - MC3 exit/MC3 bad piece filter
        readonly MemoryBit sensorFinishedPartExit;//Diffuse Sensor 9 - Finished piece exit
        readonly MemoryBit sensorBadPieceFilterConveyorStartMc2;//Diffuse Sensor 10 - MC2 bad piece conveyor entrance
        readonly MemoryBit sensorBadPieceFilterConveyorStartMc3;//Diffuse Sensor 17 - MC3 bad piece conveyor entrance
        readonly MemoryBit sensorBadPieceFilterConveyorEndMc2;//Diffuse Sensor 11 - MC2 bad piece conveyor exit
        readonly MemoryBit sensorBadPieceFilterConveyorEndMc3;//Diffuse Sensor 18 - MC2 bad piece conveyor exit
        readonly MemoryBit sensorBuffer1Spot2;//Diffuse Sensor 13 
        readonly MemoryBit sensorBuffer1Spot3;//Diffuse Sensor 14
        readonly MemoryBit sensorBuffer2Spot2;//Diffuse Sensor 16
        readonly MemoryBit sensorBuffer2Spot3;//Diffuse Sensor 23
        readonly MemoryBit sensorEmitter;

        //Emitter
        readonly MemoryBit emitter;//Emitter

        //Buffer
        readonly MemoryBit buffer1Stopblade;//Buffer stopblade
        readonly MemoryBit buffer2Stopblade;//Buffer stopblade

        //GripperArms
        readonly GripperArm gripperMc1;
        readonly GripperArm gripperMc2;
        readonly GripperArm gripperMc3;

        //Light controls
        readonly McLightsControl mc1Lights;
        readonly McLightsControl mc2Lights;
        readonly McLightsControl mc3Lights;

        //SUPERVISORY CONTROL
        //readonly Machines3AndBufferSupervisor supervisoryControl;
        readonly Machines3AndBuffer3Supervisor supervisoryControl;
        bool supervisoryApproval;

        //conveyor belts
        readonly MemoryBit conveyorMc2Entrance;
        readonly MemoryBit conveyorMc3Entrance;
        readonly MemoryBit conveyorMc1Entrance;
        readonly MemoryBit conveyorMc1BadPiece;
        readonly MemoryBit conveyorFinishedPiece;
        readonly MemoryBit conveyorMc2BadPiece;
        readonly MemoryBit conveyorMc3BadPiece;
        readonly MemoryBit conveyorEmitter;
        readonly MemoryBit conveyorBuffer1;
        readonly MemoryBit conveyorBuffer2;

        //Failing time (potenciometer and display)
        readonly MemoryFloat potentiometerMc1;
        readonly MemoryFloat potentiometerMc2;
        readonly MemoryFloat potentiometerMc3;
        readonly MemoryFloat displayMc1;
        readonly MemoryFloat displayMc2;
        readonly MemoryFloat displayMc3;
        readonly Stopwatch failTimeMc1;
        float failTimeFloatMc1;
        readonly Stopwatch failTimeMc2;
        float failTimeFloatMc2;
        readonly Stopwatch failTimeMc3;
        float failTimeFloatMc3;
        bool timeStartBool;

        //Others
        bool initialStateMessagePrinted;

        //Enums
        McStatus mc1Status;
        McStatus mc2Status;
        McStatus mc3Status;
        McPositionerStatus mc1PositionerStatus;
        McPositionerStatus mc2PositionerStatus;
        McPositionerStatus mc3PositionerStatus;
        BufferStatus buffer1Status;
        BufferStatus buffer2Status;
        Mc2andMc3LoadingSteps loadingMc2Step;
        Mc2andMc3LoadingSteps loadingMc3Step;
        Mc1PieceReady mc1PieceReady;
        Mc1PieceReadySteps mc1PieceReadySteps;
        Events eventsMc;
        Mc1WorkingStage mc1WorkingStage;

        //Controllable events
        private int s1Counter;
        private int r1Counter;
        private int s2Counter;
        private int r2Counter;
        private int s3Counter;
        private int r3Counter;

        //FTRIG
        readonly FTRIG ftAtEntranceMc1;
        readonly FTRIG ftAtEntranceMc2;
        readonly FTRIG ftAtEntranceMc3;
        readonly FTRIG ftAtExitMc1;
        readonly FTRIG ftAtExitMc2;
        readonly FTRIG ftAtExitMc3;
        readonly FTRIG ftAtBufferEnd;
        readonly FTRIG ftAtBadPieceExitMc1;
        readonly FTRIG ftAtBadPieceExitMc2;
        readonly FTRIG ftAtBadPieceExitMc3;
        readonly FTRIG ftAtFinishedPieceExit;
        readonly FTRIG ftAtMc2LoadingConveyorStart;
        readonly FTRIG ftAtMc3LoadingConveyorStart;
        readonly FTRIG ftAtEmitter;

        //RTRIG
        readonly RTRIG rtAtExitMc1;
        readonly RTRIG rtAtExitMc2;
        readonly RTRIG rtAtExitMc3;
        readonly RTRIG rtAtMc2LoadingConveyorStart;
        readonly RTRIG rtAtMc3LoadingConveyorStart;

        private enum BufferStatus
        {
            EMPTY,
            ONE,
            TWO,
            THREE
        }
        private enum BufferSteps
        {
            PIECE_ARRIVED_TO_BUFFER,
            FILTERING_BAD_PIECE,
            PIECE_REACHING_MC2,
            PIECE_ARRIVED_TO_MC2
        }
        private enum McPositionerStatus
        {
            UP,
            DOWN,
            CLAMP,
            GOING_UP
        }
        private enum Mc1PieceReady
        {
            NOT_READY,
            READY
        }
        private enum Mc1PieceReadySteps
        {
            IDLE,
            SWITCHING_CONVEYORS,
            REACHING_MC1ENTRANCE,
            ENTERINGMC1
        }
        private enum Mc2andMc3LoadingSteps
        {
            IDLE,
            PIECE_TO_LOADING_CONVEYOR,
            SEPARATE_OTHER_PIECES,
            RESTORING_BUFFER_ORDER,
            REACHED_MC
        }
        private enum Events
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
        private enum BreakdownMc2OrMc3
        {
            OK,
            KO
        }
        private enum Mc1WorkingStage
        {
            CONVEYOR,
            MACHINING_CENTER1,
            MACHINING_CENTER2
        }
        private enum McStatus
        {
            IDLE,
            WORKING,
            DOWN
        }

        public Machines3AndBuffer()// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONSTRUCTOR STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        {
            //Reader
            reader = new Reader();
            newState = "";
            newStateName = "";
            changeStateMessagePrinted = false;

            //Mc1 inputs
            mc1StartButton = MemoryMap.Instance.GetBit("Start Mc1", MemoryType.Input);
            mc1FailButton = MemoryMap.Instance.GetBit("Force Failure Mc1", MemoryType.Input);
            mc1RepairButton = MemoryMap.Instance.GetBit("Repair Mc1", MemoryType.Input);
            mc1Busy = MemoryMap.Instance.GetBit("Machining Center 0 (Is Busy)", MemoryType.Input);
            mc1Progress = MemoryMap.Instance.GetInt("Machining Center 0 (Progress)", MemoryType.Input);
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
            mc1PositionerRise.Value = true;
            mc1PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 0 (Clamp)", MemoryType.Output);
            mc1PositionerClamp.Value = false;

            //mc1 machine values
            mc1Start.Value = false;
            mc1Busy.Value = false;

            //mc1 buttons
            mc1StartButton.Value = false;
            mc1FailButton.Value = true;//unpressed is true
            mc1RepairButton.Value = false;

            //mc1 others
            mc1Failed = false;

            //Mc2 inputs
            mc2StartButton = MemoryMap.Instance.GetBit("Start Mc2", MemoryType.Input);
            mc2StartButton.Value = false;
            mc2FailButton = MemoryMap.Instance.GetBit("Force Failure Mc2", MemoryType.Input);
            mc2FailButton.Value = true;//True is unpressed
            mc2RepairButton = MemoryMap.Instance.GetBit("Repair Mc2", MemoryType.Input);
            mc2RepairButton.Value = false;
            mc2Busy = MemoryMap.Instance.GetBit("Machining Center 1 (Is Busy)", MemoryType.Input);
            mc2Progress = MemoryMap.Instance.GetInt("Machining Center 1 (Progress)", MemoryType.Input);
            mc2PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamped)", MemoryType.Input);//Mc2 positioner clamped sensor
            mc2GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 1 (Item Detected)", MemoryType.Input);//Mc2 positioner item detected sensor

            //Mc2 outputs
            mc2Start = MemoryMap.Instance.GetBit("Machining Center 1 (Start)", MemoryType.Output);
            mc2Reset = MemoryMap.Instance.GetBit("Machining Center 1 (Reset)", MemoryType.Output);//Machining Center reset (so it leaves piece incomplete)
            mc2RedLight = MemoryMap.Instance.GetBit("Stack Light 1 (Red)", MemoryType.Output);
            mc2YellowLight = MemoryMap.Instance.GetBit("Stack Light 1 (Yellow)", MemoryType.Output);
            mc2GreenLight = MemoryMap.Instance.GetBit("Stack Light 1 (Green)", MemoryType.Output);
            mc2AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 1", MemoryType.Output);
            mc2PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 3 (Clamp)", MemoryType.Output);//mc2 positioner clamp
            mc2PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 3 (Raise)", MemoryType.Output);//mc2 positioner rise

            //mc2 others
            mc2Failed = false;

            //Mc3 inputs
            mc3StartButton = MemoryMap.Instance.GetBit("Start Mc3", MemoryType.Input);
            mc3StartButton.Value = false;
            mc3FailButton = MemoryMap.Instance.GetBit("Force Failure Mc3", MemoryType.Input);
            mc3FailButton.Value = true;//True is unpressed
            mc3RepairButton = MemoryMap.Instance.GetBit("Repair Mc3", MemoryType.Input);
            mc3RepairButton.Value = false;
            mc3Busy = MemoryMap.Instance.GetBit("Machining Center 2 (Is Busy)", MemoryType.Input);
            mc3PositionerClamped = MemoryMap.Instance.GetBit("Right Positioner 1 (Clamped)", MemoryType.Input);//Mc3 positioner clamped sensor
            mc3GripperItemDetected = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 2 (Item Detected)", MemoryType.Input);//Mc3 positioner item detected sensor

            //Mc3 outputs
            mc3Start = MemoryMap.Instance.GetBit("Machining Center 2 (Start)", MemoryType.Output);
            mc3RedLight = MemoryMap.Instance.GetBit("Stack Light 2 (Red)", MemoryType.Output);
            mc3YellowLight = MemoryMap.Instance.GetBit("Stack Light 2 (Yellow)", MemoryType.Output);
            mc3GreenLight = MemoryMap.Instance.GetBit("Stack Light 2 (Green)", MemoryType.Output);
            mc3AlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 2", MemoryType.Output);
            mc3PositionerClamp = MemoryMap.Instance.GetBit("Right Positioner 1 (Clamp)", MemoryType.Output);//mc3 positioner clamp
            mc3PositionerRise = MemoryMap.Instance.GetBit("Right Positioner 1 (Raise)", MemoryType.Output);//mc3 positioner rise

            //mc3 others
            mc3Failed = false;

            //Sensors
            sensorMc1ConveyorEntrance = MemoryMap.Instance.GetBit("Diffuse Sensor 0", MemoryType.Input);//Diffuse Sensor 0 - Emitter
            sensorEntranceMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 1", MemoryType.Input);//Diffuse Sensor 1 - MC1 entrance
            sensorExitMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 2", MemoryType.Input);//Diffuse Sensor 2 - MC1 exit/Buffer conveyor entry/MC1 bad piece filter
            sensorBuffer1End = MemoryMap.Instance.GetBit("Diffuse Sensor 3", MemoryType.Input);//Diffuse Sensor 3 - Buffer1 end
            sensorBuffer2End = MemoryMap.Instance.GetBit("Diffuse Sensor 21", MemoryType.Input);//Diffuse Sensor 21 - Buffer2 end
            sensorMc2loadingConveyorStart = MemoryMap.Instance.GetBit("Diffuse Sensor 3_2", MemoryType.Input);//Diffuse Sensor 3_2
            sensorMc3loadingConveyorStart = MemoryMap.Instance.GetBit("Diffuse Sensor 20", MemoryType.Input);//Diffuse Sensor 20
            sensorEntranceMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 5", MemoryType.Input);//Diffuse Sensor 5 - MC2 entrance
            sensorEntranceMc3 = MemoryMap.Instance.GetBit("Diffuse Sensor 19", MemoryType.Input);//Diffuse Sensor 19 - MC3 entrance
            sensorBadPieceFilterConveyorStartMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 6", MemoryType.Input);//Diffuse Sensor 6 - MC1 bad piece filter conveyor entrance
            sensorBadPieceFilterConveyorEndMc1 = MemoryMap.Instance.GetBit("Diffuse Sensor 7", MemoryType.Input);//Diffuse Sensor 7 - MC1 bad piece filter conveyor exit
            sensorExitMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 8", MemoryType.Input);//Diffuse Sensor 8 - MC2 exit/MC2 bad piece filter
            sensorExitMc3 = MemoryMap.Instance.GetBit("Diffuse Sensor 22", MemoryType.Input);//Diffuse Sensor 22 - MC3 exit/MC3 bad piece filter
            sensorFinishedPartExit = MemoryMap.Instance.GetBit("Diffuse Sensor 9", MemoryType.Input);//Diffuse Sensor 9 - Finished piece exit
            sensorBadPieceFilterConveyorStartMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 10", MemoryType.Input);//Diffuse Sensor 10 - MC2 bad piece conveyor entrance
            sensorBadPieceFilterConveyorStartMc3 = MemoryMap.Instance.GetBit("Diffuse Sensor 17", MemoryType.Input);//Diffuse Sensor 17 - MC3 bad piece conveyor entrance
            sensorBadPieceFilterConveyorEndMc2 = MemoryMap.Instance.GetBit("Diffuse Sensor 11", MemoryType.Input);//Diffuse Sensor 11 - MC2 bad piece conveyor exit
            sensorBadPieceFilterConveyorEndMc3 = MemoryMap.Instance.GetBit("Diffuse Sensor 18", MemoryType.Input);//Diffuse Sensor 18 - MC3 bad piece conveyor exit
            sensorBuffer1Spot2 = MemoryMap.Instance.GetBit("Diffuse Sensor 13", MemoryType.Input);//Diffuse Sensor 13
            sensorBuffer1Spot3 = MemoryMap.Instance.GetBit("Diffuse Sensor 14", MemoryType.Input);//Diffuse Sensor 14
            sensorBuffer2Spot2 = MemoryMap.Instance.GetBit("Diffuse Sensor 16", MemoryType.Input);//Diffuse Sensor 16
            sensorBuffer2Spot3 = MemoryMap.Instance.GetBit("Diffuse Sensor 23", MemoryType.Input);//Diffuse Sensor 23
            sensorEmitter = MemoryMap.Instance.GetBit("Diffuse Sensor 4", MemoryType.Input);//Diffuse Sensor 4 - Sensor emitter

            //Emitter
            emitter = MemoryMap.Instance.GetBit("Emitter 0 (Emit)", MemoryType.Output);//Emitter
            emitter.Value = true;

            //Buffer 1
            buffer1Stopblade = MemoryMap.Instance.GetBit("Stop Blade 0", MemoryType.Output);
            buffer1Stopblade.Value = true;//True is rised

            //Buffer 2
            buffer2Stopblade = MemoryMap.Instance.GetBit("Stop Blade 2", MemoryType.Output);
            buffer2Stopblade.Value = true;//True is rised

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

            gripperMc3 = new GripperArm(
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 2 X Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 2 Z Position (V)", MemoryType.Input),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 2 X Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 2 Z Set Point (V)", MemoryType.Output),
                MemoryMap.Instance.GetBit("Two-Axis Pick & Place 2 (Grab)", MemoryType.Output)
            );

            //Light controls
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

            //Failing time (potenciometer and display)
            potentiometerMc1 = MemoryMap.Instance.GetFloat("Mc1 Failure Time Control", MemoryType.Input);
            potentiometerMc2 = MemoryMap.Instance.GetFloat("Mc2 Failure Time Control", MemoryType.Input);
            potentiometerMc3 = MemoryMap.Instance.GetFloat("Mc3 Failure Time Control", MemoryType.Input);
            displayMc1 = MemoryMap.Instance.GetFloat("Mc1 Failure time (in seconds x 100)", MemoryType.Output);
            displayMc2 = MemoryMap.Instance.GetFloat("Mc2 Failure time (in seconds x 100)", MemoryType.Output);
            displayMc3 = MemoryMap.Instance.GetFloat("Mc3 Failure time (in seconds x 100)", MemoryType.Output);
            failTimeMc1 = new Stopwatch();
            failTimeFloatMc1 = 0.0f;
            failTimeMc2 = new Stopwatch();
            failTimeFloatMc2 = 0.0f;
            failTimeMc3 = new Stopwatch();
            failTimeFloatMc3 = 0.0f;
            timeStartBool = false;

            failTimeMc1.Start();
            failTimeMc2.Start();
            failTimeMc3.Start();


            //Enums
            mc1Status = McStatus.IDLE;
            mc2Status = McStatus.IDLE;
            mc3Status = McStatus.IDLE;
            mc1PositionerStatus = McPositionerStatus.UP;
            mc2PositionerStatus = McPositionerStatus.UP;
            mc3PositionerStatus = McPositionerStatus.UP;
            buffer1Status = BufferStatus.EMPTY;
            buffer2Status = BufferStatus.EMPTY;
            loadingMc2Step = Mc2andMc3LoadingSteps.IDLE;
            loadingMc3Step = Mc2andMc3LoadingSteps.IDLE;
            mc1PieceReady = Mc1PieceReady.NOT_READY;
            mc1PieceReadySteps = Mc1PieceReadySteps.IDLE;
            eventsMc = Events.i1;
            mc1WorkingStage = Mc1WorkingStage.CONVEYOR;

            //FTRIG
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
            ftAtMc2LoadingConveyorStart = new FTRIG();
            ftAtMc3LoadingConveyorStart = new FTRIG();
            ftAtEmitter = new FTRIG();

            //RTRIG
            rtAtExitMc1 = new RTRIG();
            rtAtExitMc2 = new RTRIG();
            rtAtExitMc3 = new RTRIG();
            rtAtMc2LoadingConveyorStart = new RTRIG();
            rtAtMc3LoadingConveyorStart = new RTRIG();

            // Controllable events
            s1Counter = 0;
            r1Counter = 0;
            s2Counter = 0;
            r2Counter = 0;
            s3Counter = 0;
            r3Counter = 0;

            //SUPERVISORY CONTROL
            //supervisoryControl = new Machines3AndBufferSupervisor();
            supervisoryControl = new Machines3AndBuffer3Supervisor();
            supervisoryApproval = true;

            //Trick for printing initial state in console after start up messages
            initialStateMessagePrinted = false;

        } // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONSTRUCTOR ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public override void Execute(int elapsedMilliseconds) // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% EXECUTE STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        {


            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% TRICK FOR PRINTING INITIAL STATE AFTER START UP MESSAGES START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            if (initialStateMessagePrinted == false)
            {
                Console.WriteLine("\nTip: Set the failure time using the potentiometers in the control panels before starting. Recomended number ranges from 1.50 to 2.50. Also recommended to place same failure time to the three machines.\n");
                supervisoryControl.CreateController();
                initialStateMessagePrinted = true;
            }
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% TRICK FOR PRINTING INITIAL STATE AFTER START UP MESSAGES END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FAILING TIME AND DISPLAY START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            


            displayMc1.Value = float.Parse(string.Format("{0:0.0}", potentiometerMc1.Value));
            displayMc2.Value = float.Parse(string.Format("{0:0.0}", potentiometerMc2.Value));
            displayMc3.Value = float.Parse(string.Format("{0:0.0}", potentiometerMc3.Value));
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FAILING TIME AND DISPLAY END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTROLLABLE EVENTS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


            // Keyboard input:
            try
            {
                if (!changeStateMessagePrinted)
                {
                    changeStateMessagePrinted = true;
                }
                newStateName = "";
                newState = Reader.ReadLine(5);
                try
                {
                    newStateName = supervisoryControl.StateName(int.Parse(newState));
                    if (newStateName != "Event number pressed does not exist")
                    {
                        if (!supervisoryControl.IsInActiveEvents(int.Parse(newState)))
                        {
                            Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                            Console.WriteLine("\nEvent " + newState + " is not in active events. Try again.\n");
                            Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                            supervisoryControl.ListOfActiveEvents();
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                    Console.WriteLine("\nSorry, please insert a number.\n");
                    Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                    supervisoryControl.ListOfActiveEvents();
                }
                changeStateMessagePrinted = false;
            }
            catch (TimeoutException)
            {
            }

            //s1
            if (mc1StartButton.Value == true || (newStateName == "s1" && supervisoryControl.IsInActiveEvents(int.Parse(newState))))
            {
                if (!timeStartBool)
                    timeStartBool = true;

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
            if (mc1RepairButton.Value == true || (newStateName == "r1" && supervisoryControl.IsInActiveEvents(int.Parse(newState))))
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
            if (mc2StartButton.Value == true || (newStateName == "s2" && supervisoryControl.IsInActiveEvents(int.Parse(newState))))
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
            if (mc2RepairButton.Value == true || (newStateName == "r2" && supervisoryControl.IsInActiveEvents(int.Parse(newState))))
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

            //s3
            if (mc3StartButton.Value == true || (newStateName == "s3" && supervisoryControl.IsInActiveEvents(int.Parse(newState))))
            {
                if (s3Counter == 0)
                {
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
            if (mc3RepairButton.Value == true || (newStateName == "r3" && supervisoryControl.IsInActiveEvents(int.Parse(newState))))
            {
                if (r3Counter == 0)
                {
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
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTROLLABLE EVENTS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% UNCONTROLLABLE EVENTS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (timeStartBool)
            {
                failTimeFloatMc1 = Convert.ToSingle(failTimeMc1.ElapsedMilliseconds / 100);
                failTimeFloatMc1 = failTimeFloatMc1 / 10;

                failTimeFloatMc2 = Convert.ToSingle(failTimeMc2.ElapsedMilliseconds / 100);
                failTimeFloatMc2 = failTimeFloatMc2 / 10;

                failTimeFloatMc3 = Convert.ToSingle(failTimeMc3.ElapsedMilliseconds / 100);
                failTimeFloatMc3 = failTimeFloatMc3 / 10;
            }

            if (mc1FailButton.Value == false || float.Parse(string.Format("{0:0.0}", (failTimeFloatMc1) % (displayMc1.Value * 100))) == 1.0f)//false is button pressed
            {
                mc1Failed = true;
            }

            if (mc2FailButton.Value == false || float.Parse(string.Format("{0:0.0}", (failTimeFloatMc2) % (displayMc2.Value * 100))) == 1.0f)//false is button pressed)
            {
                mc2Failed = true;
            }

            if (mc3FailButton.Value == false || float.Parse(string.Format("{0:0.0}", (failTimeFloatMc3) % (displayMc3.Value * 100))) == 1.0f)//false is button pressed)
            {
                mc3Failed = true;
            }
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% UNCONTROLLABLE EVENTS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FALLING AND RISING TRIGGERS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            ftAtEntranceMc1.CLK(sensorEntranceMc1.Value);
            ftAtEntranceMc2.CLK(sensorEntranceMc2.Value);
            ftAtEntranceMc3.CLK(sensorEntranceMc3.Value);

            ftAtExitMc1.CLK(sensorExitMc1.Value);
            ftAtExitMc2.CLK(sensorExitMc2.Value);
            ftAtExitMc3.CLK(sensorExitMc3.Value);
            ftAtBufferEnd.CLK(sensorBuffer1End.Value);
            ftAtBadPieceExitMc1.CLK(sensorBadPieceFilterConveyorEndMc1.Value);
            ftAtBadPieceExitMc2.CLK(sensorBadPieceFilterConveyorEndMc2.Value);
            ftAtBadPieceExitMc3.CLK(sensorBadPieceFilterConveyorEndMc3.Value);
            ftAtFinishedPieceExit.CLK(sensorFinishedPartExit.Value);
            ftAtMc2LoadingConveyorStart.CLK(sensorMc2loadingConveyorStart.Value);
            ftAtMc3LoadingConveyorStart.CLK(sensorMc3loadingConveyorStart.Value);
            ftAtEmitter.CLK(sensorMc1ConveyorEntrance.Value);

            rtAtExitMc1.CLK(sensorExitMc1.Value);
            rtAtExitMc2.CLK(sensorExitMc2.Value);
            rtAtExitMc3.CLK(sensorExitMc3.Value);
            rtAtMc2LoadingConveyorStart.CLK(sensorMc2loadingConveyorStart.Value);
            rtAtMc3LoadingConveyorStart.CLK(sensorMc3loadingConveyorStart.Value);

            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% FALLING AND RISING TRIGGERS END %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%




            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BAD PIECES CONVEYORS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

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

            //Mc3 bad piece conveyor
            if (sensorBadPieceFilterConveyorStartMc3.Value == true)
            {
                conveyorMc3BadPiece.Value = true;
            }
            else if (ftAtBadPieceExitMc3.Q == true)
            {
                conveyorMc3BadPiece.Value = false;
            }
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BAD PIECES CONVEYORS ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER 1 STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            if (buffer1Status == BufferStatus.EMPTY)
            {
                if (rtAtExitMc1.Q == true && mc1Failed == false)
                {
                    conveyorBuffer1.Value = true;
                    buffer1Status = BufferStatus.ONE;
                }
            }
            else if (buffer1Status == BufferStatus.ONE)
            {
                if (sensorBuffer1End.Value == true && loadingMc2Step == Mc2andMc3LoadingSteps.IDLE && mc1Failed == false)
                {
                    conveyorBuffer1.Value = false;
                }
                if (rtAtMc2LoadingConveyorStart.Q == true)
                {
                    buffer1Status = BufferStatus.EMPTY;
                }
                if (rtAtExitMc1.Q == true && mc1Failed == false)
                {
                    conveyorBuffer1.Value = true;
                    buffer1Status = BufferStatus.TWO;
                }
            }
            else if (buffer1Status == BufferStatus.TWO)
            {
                if (sensorBuffer1Spot2.Value == true && loadingMc2Step == Mc2andMc3LoadingSteps.IDLE && mc1Failed == false)
                {
                    conveyorBuffer1.Value = false;
                }
                if (rtAtMc2LoadingConveyorStart.Q == true)
                {
                    buffer1Status = BufferStatus.ONE;
                }
                if (rtAtExitMc1.Q == true && mc1Failed == false)
                {
                    conveyorBuffer1.Value = true;
                    buffer1Status = BufferStatus.THREE;
                }
            }
            else if (buffer1Status == BufferStatus.THREE)
            {
                if (sensorBuffer1Spot3.Value == true && loadingMc2Step == Mc2andMc3LoadingSteps.IDLE && mc1Failed == false)
                {
                    conveyorBuffer1.Value = false;
                }
                if (rtAtMc2LoadingConveyorStart.Q == true)
                {
                    buffer1Status = BufferStatus.TWO;
                }
            }
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER 1 ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER 2 STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            if (buffer2Status == BufferStatus.EMPTY)
            {
                if (rtAtExitMc2.Q == true && mc2Failed == false)
                {
                    conveyorBuffer2.Value = true;
                    buffer2Status = BufferStatus.ONE;
                }
            }
            else if (buffer2Status == BufferStatus.ONE)
            {
                if (sensorBuffer2End.Value == true && loadingMc3Step == Mc2andMc3LoadingSteps.IDLE && mc2Failed == false)
                {
                    conveyorBuffer2.Value = false;
                }
                if (rtAtMc3LoadingConveyorStart.Q == true)
                {
                    buffer2Status = BufferStatus.EMPTY;
                }
                if (rtAtExitMc2.Q == true && mc2Failed == false)
                {
                    conveyorBuffer2.Value = true;
                    buffer2Status = BufferStatus.TWO;
                }
            }
            else if (buffer2Status == BufferStatus.TWO)
            {
                if (sensorBuffer2Spot2.Value == true && loadingMc3Step == Mc2andMc3LoadingSteps.IDLE && mc2Failed == false)
                {
                    conveyorBuffer2.Value = false;
                }
                if (rtAtMc3LoadingConveyorStart.Q == true)
                {
                    buffer2Status = BufferStatus.ONE;
                }
                if (rtAtExitMc2.Q == true && mc1Failed == false)
                {
                    conveyorBuffer2.Value = true;
                    buffer2Status = BufferStatus.THREE;
                }
            }
            else if (buffer2Status == BufferStatus.THREE)
            {
                if (sensorBuffer2Spot3.Value == true && loadingMc3Step == Mc2andMc3LoadingSteps.IDLE && mc2Failed == false)
                {
                    conveyorBuffer2.Value = false;
                }
                if (rtAtMc3LoadingConveyorStart.Q == true)
                {
                    buffer2Status = BufferStatus.TWO;
                }
            }
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% BUFFER 2 ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%% MACHINE CENTER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%%%%%%%%%% MC1 STARTS %%%%%%%%%%%%%%%%%%%%

            // %%%% MC1 BAD PIECES FILTER STARTS %%%%%
            gripperMc1.StateTransition();

            if (mc1PositionerStatus == McPositionerStatus.UP)
            {
                mc1PositionerClamp.Value = false;
                mc1PositionerRise.Value = true;
                if (rtAtExitMc1.Q == true && mc1Failed == true)
                {
                    conveyorBuffer1.Value = true;
                    mc1PositionerStatus = McPositionerStatus.DOWN;
                }
            }
            else if (mc1PositionerStatus == McPositionerStatus.DOWN)
            {

                mc1PositionerRise.Value = false;
                if (ftAtExitMc1.Q == true)
                {
                    conveyorBuffer1.Value = false;
                    mc1PositionerStatus = McPositionerStatus.CLAMP;
                }
            }
            else if (mc1PositionerStatus == McPositionerStatus.CLAMP)
            {
                mc1PositionerClamp.Value = true;
                if (mc1PositionerClamped.Value == true)
                {
                    gripperMc1.Start();
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
            // %%%% MC1 BAD PIECES FILTER ENDS %%%%

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
                mc1Lights.IdleLights();
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
                mc1Lights.WorkingLights();
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

                    if (mc1Progress.Value > 80)
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
                mc1Lights.FailingLights();

                if (eventsMc == Events.r1)
                {
                    mc1Failed = false;//Next piece will not fail
                    mc1Status = McStatus.IDLE;
                    mc1AlarmSiren.Value = false;
                }
            }
            // %%%% MC1 DOWN ENDS %%%%%

            // %%%%%%%%%%%%%%%%%%%% MC1 ENDS %%%%%%%%%%%%%%%%%%%%



            // %%%%%%%%%%%%%%%%%%%% MC2 STARTS %%%%%%%%%%%%%%%%%%%%

            //%%%% MC2 BAD PIECES FILTER STARTS %%%%%
            gripperMc2.StateTransition();

            if (mc2PositionerStatus == McPositionerStatus.UP)
            {
                mc2PositionerClamp.Value = false;
                mc2PositionerRise.Value = true;
                if (rtAtExitMc2.Q == true && mc2Failed == true)
                {
                    conveyorBuffer2.Value = true;
                    mc2PositionerStatus = McPositionerStatus.DOWN;
                }
            }
            else if (mc2PositionerStatus == McPositionerStatus.DOWN)
            {
                mc2PositionerRise.Value = false;
                if (ftAtExitMc2.Q == true)
                {
                    conveyorBuffer2.Value = false;
                    mc2PositionerStatus = McPositionerStatus.CLAMP;
                }
            }
            else if (mc2PositionerStatus == McPositionerStatus.CLAMP)
            {
                mc2PositionerClamp.Value = true;
                if (mc2PositionerClamped.Value == true)
                {
                    gripperMc2.Start();
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
            //%%% MC2 BAD PIECES FILTER ENDS %%%%

            //%%% MC2 IDLE STARTS %%%%
            if (mc2Status == McStatus.IDLE)
            {
                mc2Lights.IdleLights();
                if (eventsMc == Events.s2 && buffer1Status != BufferStatus.EMPTY && loadingMc2Step == Mc2andMc3LoadingSteps.IDLE)
                {
                    mc2Status = McStatus.WORKING;
                    loadingMc2Step = Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR;
                }
            }
            //%%% MC2 IDLE ENDS %%%%

            //%%% MC2 WORKING STARTS %%%%
            else if (mc2Status == McStatus.WORKING)
            {
                mc2Lights.WorkingLights();
                if (loadingMc2Step == Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR)
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

                    if (sensorBuffer1End.Value == true || buffer1Status == BufferStatus.EMPTY)
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
                    mc2Start.Value = false;
                }

                if (mc2Progress.Value > 80)
                {
                    mc2Reset.Value = true;
                }

                if (rtAtExitMc2.Q == true && mc2Failed == false)
                {
                    mc2Reset.Value = false;
                    conveyorBuffer2.Value = true;
                    eventsMc = Events.f2;
                    mc2Status = McStatus.IDLE;
                    supervisoryApproval = supervisoryControl.On("f2");
                    //mc2Failed = true; //will fail next time
                }
                else if (rtAtExitMc2.Q == true && mc2Failed == true)
                {
                    mc2Reset.Value = false;
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
                mc2Lights.FailingLights();

                if (eventsMc == Events.r2)
                {
                    mc2Failed = false;
                    mc2Status = McStatus.IDLE;
                    mc2AlarmSiren.Value = false;
                }
            }
            //%%% MC2 DOWN ENDS %%%%


            // %%%%%%%%%%%%%%%%%%%% MC2 ENDS %%%%%%%%%%%%%%%%%%%%

            // %%%%%%%%%%%%%%%%%%%% MC3 STARTS %%%%%%%%%%%%%%%%%%%%

            //%%%% MC3 BAD PIECES FILTER STARTS %%%%%
            gripperMc3.StateTransition();

            if (mc3PositionerStatus == McPositionerStatus.UP)
            {
                mc3PositionerClamp.Value = false;
                mc3PositionerRise.Value = true;
                if (rtAtExitMc3.Q == true && mc3Failed == true)
                {
                    conveyorFinishedPiece.Value = true;
                    mc3PositionerStatus = McPositionerStatus.DOWN;
                }
            }
            else if (mc3PositionerStatus == McPositionerStatus.DOWN)
            {
                mc3PositionerRise.Value = false;
                if (ftAtExitMc3.Q == true)
                {
                    conveyorFinishedPiece.Value = false;
                    mc3PositionerStatus = McPositionerStatus.CLAMP;
                }
            }
            else if (mc3PositionerStatus == McPositionerStatus.CLAMP)
            {
                mc3PositionerClamp.Value = true;
                if (mc3PositionerClamped.Value == true)
                {
                    gripperMc3.Start();
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
            //%%% MC3 BAD PIECES FILTER ENDS %%%%

            //%%% MC3 IDLE STARTS %%%%
            if (mc3Status == McStatus.IDLE)
            {
                mc3Lights.IdleLights();
                if (eventsMc == Events.s3 && buffer2Status != BufferStatus.EMPTY && loadingMc3Step == Mc2andMc3LoadingSteps.IDLE)
                {
                    mc3Status = McStatus.WORKING;
                    loadingMc3Step = Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR;
                }
            }
            //%%% MC3 IDLE ENDS %%%%

            //%%% MC3 WORKING STARTS %%%%
            else if (mc3Status == McStatus.WORKING)
            {
                mc3Lights.WorkingLights();
                if (loadingMc3Step == Mc2andMc3LoadingSteps.PIECE_TO_LOADING_CONVEYOR)
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

                    if (sensorBuffer2End.Value == true || buffer2Status == BufferStatus.EMPTY)
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
                    mc3Start.Value = false;
                }

                if (rtAtExitMc3.Q == true && mc3Failed == false)
                {
                    conveyorFinishedPiece.Value = true;
                    eventsMc = Events.f3;
                    mc3Status = McStatus.IDLE;
                    supervisoryApproval = supervisoryControl.On("f3");
                    //mc2Failed = true; //will fail next time
                }
                else if (rtAtExitMc3.Q == true && mc3Failed == true)
                {
                    eventsMc = Events.b3;
                    mc3Status = McStatus.DOWN;
                    supervisoryApproval = supervisoryControl.On("b3");
                }
            }
            //%%% MC3 WORKING ENDS %%%%

            //%%% MC3 DOWN STARTS %%%%
            else if (mc3Status == McStatus.DOWN)
            {
                mc3AlarmSiren.Value = true;
                mc3Lights.FailingLights();

                if (eventsMc == Events.r3)
                {
                    mc3Failed = false;
                    mc3Status = McStatus.IDLE;
                    mc3AlarmSiren.Value = false;
                }
            }
            //%%% MC3 DOWN ENDS %%%%


            // %%%%%%%%%%%%%%%%%%%% MC3 ENDS %%%%%%%%%%%%%%%%%%%%

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
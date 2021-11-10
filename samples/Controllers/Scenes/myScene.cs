﻿//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;
using System;
using System.Threading;

namespace Controllers
{
    public class myScene : Controller
    {
        MemoryBit gripperStartButton = MemoryMap.Instance.GetBit("Start Button 0", MemoryType.Input);
        MemoryBit gripperFailButton = MemoryMap.Instance.GetBit("Stop Button 0", MemoryType.Input);
        MemoryBit gripperRepair = MemoryMap.Instance.GetBit("Reset Button 0", MemoryType.Input);
        MemoryBit sensorGripperConveyor = MemoryMap.Instance.GetBit("Diffuse Sensor 0", MemoryType.Input);//Diffuse Sensor 0
        MemoryBit sensorGripperConveyor2 = MemoryMap.Instance.GetBit("Diffuse Sensor 1", MemoryType.Input);//Diffuse Sensor 1
        MemoryBit sensorGripperConveyorExit = MemoryMap.Instance.GetBit("Diffuse Sensor 2", MemoryType.Input);//Diffuse Sensor where boxes are about to reach exit ramp
        MemoryBit sensorGripperConveyorStart = MemoryMap.Instance.GetBit("Diffuse Sensor 3", MemoryType.Input);//Diffuse Sensor where boxes are released from gripper
        MemoryBit sensorEmitter = MemoryMap.Instance.GetBit("Diffuse Sensor 4", MemoryType.Input);//Diffuse Sensor where boxes are emitted
        MemoryBit sensorMcEntrance = MemoryMap.Instance.GetBit("Diffuse Sensor 5", MemoryType.Input);//Diffuse Sensor where boxes are entering MC
        MemoryBit mcStartButton = MemoryMap.Instance.GetBit("Start Button 1", MemoryType.Input); //MC start button
        MemoryBit mcFailButton = MemoryMap.Instance.GetBit("Stop Button 1", MemoryType.Input); //MC fail button
        MemoryBit emitterStartButton = MemoryMap.Instance.GetBit("Start Button 2", MemoryType.Input); //Emitter start button

        MemoryFloat posX = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Position (V)", MemoryType.Input);
        MemoryFloat posZ = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Position (V)", MemoryType.Input);
        

        MemoryBit gripperRedLight = MemoryMap.Instance.GetBit("Stack Light 0 (Red)", MemoryType.Output);
        MemoryBit gripperGreenLight = MemoryMap.Instance.GetBit("Stack Light 0 (Green)", MemoryType.Output);
        MemoryBit gripperYellowLight = MemoryMap.Instance.GetBit("Stack Light 0 (Yellow)", MemoryType.Output);
        MemoryBit gripperAlarmSiren = MemoryMap.Instance.GetBit("Alarm Siren 0", MemoryType.Output);
        MemoryBit grab = MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Grab)", MemoryType.Output);
        MemoryBit conveyorGripper = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 1", MemoryType.Output);//Sensor conveyor
        MemoryBit conveyorGripper2 = MemoryMap.Instance.GetBit("Belt Conveyor (2m) 0", MemoryType.Output);//Sensor conveyor 2
        MemoryBit conveyorEmitter = MemoryMap.Instance.GetBit("Belt Conveyor (4m) 0", MemoryType.Output);//Sensor emitter
        MemoryBit emitterMc = MemoryMap.Instance.GetBit("Emitter 0 (Emit) mc", MemoryType.Output);//Machine Center start
        MemoryBit mcFail = MemoryMap.Instance.GetBit("Machining Center 0 (Stop)", MemoryType.Output);//Machine Center start
        MemoryBit emitter = MemoryMap.Instance.GetBit("Emitter 0 (Emit)", MemoryType.Output);//Emitter
        MemoryFloat setX = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Set Point (V)", MemoryType.Output);
        MemoryFloat setZ = MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Set Point (V)", MemoryType.Output);

        int timeVibration, timeWorking, timeDown;

        GripperStatus gripperStatus = GripperStatus.IDLE;
        GripperStep gripperStep = GripperStep.INITIAL;

        McStatus mcStatus = McStatus.IDLE;

        FTRIG ftAtExit = new FTRIG();
        FTRIG ftAtMcEntrance = new FTRIG();

        public myScene()// %%%%%%%%%%%%%%%%% CONSTRUCTOR STARTS %%%%%%%%%%%%%%%%
        {
            setX.Value = 0;
            setZ.Value = 0;
            grab.Value = false;
            gripperStatus = GripperStatus.IDLE;
            gripperStep = GripperStep.INITIAL;

            //Gripper buttons
            gripperStartButton.Value = false;
            gripperFailButton.Value = true;//unpressed is true
            gripperRepair.Value = false;
            
            //Gripper lights
            gripperRedLight.Value = false;
            gripperYellowLight.Value = false;
            gripperGreenLight.Value = true;

            //Gripper conveyor
            sensorGripperConveyor.Value = false;
            conveyorGripper.Value = false;

            //Emitter
            emitter.Value = false;

            //Machine Center
            emitterMc.Value = false;
            mcFailButton.Value = true;//True is unpressed

            timeVibration = 0;
            timeWorking = 0;
            timeDown = 0;
        } // %%%%%%%%%%%%%%%%% CONSTRUCTOR ENDS %%%%%%%%%%%%%%%%

        public override void Execute(int elapsedMilliseconds) // %%%%%%%%%%%%%%%%% EXECUTE STARTS %%%%%%%%%%%%%%%%
        {
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% GRIPPER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            // Gripper buttons pressed - state changes
            if (gripperStartButton.Value == true && gripperStep == GripperStep.INITIAL && gripperStatus != GripperStatus.DOWN) //Button pressed, gripper started working.
            {
                Console.WriteLine("Green button pressed");
                gripperStatus = GripperStatus.WORKING;
                gripperStep = GripperStep.INITIAL;                
            }

            if (gripperFailButton.Value == false)//FAIL Button pressed, gripper failing.
            {
                Console.WriteLine("Gripper is failing");
                gripperStatus = GripperStatus.DOWN;
                gripperStep = GripperStep.DOWN_VIBRATING;
            }

            if (gripperRepair.Value == true && gripperStatus == GripperStatus.DOWN)//REPAIR Button pressed, gripper repaired.
            {                
                Console.WriteLine("GripperRepair value is: " + gripperRepair.Value);
                setZ.Value = 0.0f;
                setX.Value = 0.0f;
                gripperStatus = GripperStatus.IDLE;
            }


            // %%%%%%%%%% gripper status: idle, working or down %%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (gripperStatus == GripperStatus.IDLE)// %%%%%%%%%% IF GRIPPER IS IDLE %%%%%%%
            {
                gripperStep = GripperStep.INITIAL;
                grab.Value = false;
                gripperGreenLight.Value = true;
                gripperYellowLight.Value = false;
                gripperRedLight.Value = false;
                gripperAlarmSiren.Value = false;
            }
            else if (gripperStatus == GripperStatus.WORKING)// %% IF GRIPPER IS WORKING %%%%
            {
                if (timeWorking < 50)
                {
                    gripperGreenLight.Value = true;
                    gripperYellowLight.Value = false;
                    gripperRedLight.Value = false;
                    gripperAlarmSiren.Value = false;
                }
                else if (timeWorking < 100)
                {
                    gripperGreenLight.Value = false;
                    gripperYellowLight.Value = false;
                    gripperRedLight.Value = false;
                    gripperAlarmSiren.Value = false;
                }
                else if (timeWorking == 100)
                {
                    timeWorking = 0;
                }
                timeWorking++;
                
                if (gripperStep == GripperStep.INITIAL) //Gripper going to initial position.
                {
                    setZ.Value = 0.0f;
                    if (posZ.Value < 0.5f)
                    {
                        setX.Value = 0.0f;
                    }                        
                    if (posX.Value < 0.01f && posZ.Value < 0.01f)
                    {
                        gripperStep = GripperStep.DOWN_LOOK_FOR_PART;
                    }
                }
                else if (gripperStep == GripperStep.DOWN_LOOK_FOR_PART)//Gripper in initial position. Z descending.
                {
                    setZ.Value = 9.4f;//Distance descended in Z until piece is reached                
                    if (posZ.Value > 9.2f)
                    {
                        gripperStep = GripperStep.GRAB;                        
                    }
                }
                else if (gripperStep == GripperStep.GRAB)
                {
                    grab.Value = true;// Z descended. Grabbing piece.
                    gripperStep = GripperStep.UP_WITH_PART;
                }
                else if (gripperStep == GripperStep.UP_WITH_PART)// Piece grabbed. Z ascending with part.
                {
                    setZ.Value = 0.0f;
                    if (posZ.Value < 0.1f)
                    {
                        gripperStep = GripperStep.X_EXTEND;// Z ascended with part.
                    }
                }
                else if (gripperStep == GripperStep.X_EXTEND)// Z ascended with part. X extending with part
                {
                    setX.Value = 7.7f;
                    if (posX.Value > 7.6f)//X extended with part
                    {
                        gripperStep = GripperStep.DOWN_WITH_PART;
                    }
                }
                else if (gripperStep == GripperStep.DOWN_WITH_PART)//X extended with part. Z descending with part.
                {
                    setZ.Value = 9.3f;
                    if (posZ.Value > 9.2f)//Z descended with part
                    {
                        gripperStep = GripperStep.RELEASE;
                    }
                }
                else if (gripperStep == GripperStep.RELEASE)//Z descended with part. Gripper releases part.
                {
                    grab.Value = false;                    
                    gripperStep = GripperStep.UP_NO_PART;//Gripper released part.                    
                }
                else if (gripperStep == GripperStep.UP_NO_PART)//Gripper released part. Z ascend w/out part.
                {
                    setZ.Value = 0.0f;
                    if (posZ.Value < 0.1f)
                    {
                        gripperStep = GripperStep.X_RETRACT;//Z ascended w/out part.
                    }
                }
                else if (gripperStep == GripperStep.X_RETRACT)//Z ascended w/out part. X retracting.
                {
                    setX.Value = 0.0f;
                    if (posX.Value < 0.1f)
                    {
                        gripperStep = GripperStep.INITIAL;//X retracting. Going for initial state.
                        gripperStatus = GripperStatus.IDLE;
                    }
                }
            }
            else if (gripperStatus == GripperStatus.DOWN) // %%%%%%%%%%%%%%%%% GRIPPER IS DOWN %%%%%%%%%%%%%%%%
            {
                if (timeDown < 30) // %%%%%%%%%%%%%%%%% DOWN LIGHTS AND ALARM START %%%%%%%%%%%%%%%%
                {
                    gripperGreenLight.Value = false;
                    gripperYellowLight.Value = false;
                    gripperRedLight.Value = false;
                    gripperAlarmSiren.Value = true;
                }
                else if (timeDown < 60)
                {
                    gripperGreenLight.Value = true;
                    gripperYellowLight.Value = true;
                    gripperRedLight.Value = true;
                    gripperAlarmSiren.Value = true;
                }
                else if (timeDown == 60)
                {
                    timeDown = 0;
                }
                timeDown++; // %%%%%%%%%%%%%%%%% DOWN LIGHTS AND ALARM END %%%%%%%%%%%%%%%%

                if (gripperStep == GripperStep.DOWN_VIBRATING) // %%%%%%%%%%%%%%%%% DOWN VIBRATION STARTS %%%%%%%%%%%%%%%%
                {
                    if (timeVibration == 1)
                    {
                        setX.Value = 10.0f;
                        setZ.Value = 10.0f;                        
                    }
                    else if (timeVibration == 6)
                    {
                        setX.Value = 0.0f;
                        setZ.Value = 0.0f;
                    }
                    else if (timeVibration == 10)
                    {
                        timeVibration = 0;
                    }
                    timeVibration++;
                }// %%%%%%%%%%%%%%%%% DOWN VIBRATION ENDS %%%%%%%%%%%%%%%%
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% GRIPPER ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% SENSOR CONVEYORS START %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%            
            ftAtExit.CLK(sensorGripperConveyorExit.Value);
            ftAtMcEntrance.CLK(sensorMcEntrance.Value);

            if (sensorGripperConveyorStart.Value == true)
            {
                conveyorGripper.Value = true;
            }
            else if (sensorGripperConveyor.Value == true)
            {
                conveyorGripper.Value = false;
            }

            if (sensorGripperConveyor2.Value == true)
            {
                conveyorGripper2.Value = true;
            }
            else if (ftAtExit.Q == true)
            {
                conveyorGripper2.Value = false;
            }

            if (sensorEmitter.Value == true)
            {
                conveyorEmitter.Value = true;
            }
            else if (ftAtMcEntrance.Q == true)
            {
                conveyorEmitter.Value = false;
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% SENSOR CONVEYORS ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%            

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% MACHINE CENTER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            if (mcStartButton.Value == true && mcStatus == McStatus.IDLE)//Machine center start button
            {
                emitterMc.Value = true;
            }
            else
            {
                emitterMc.Value = false;
            }

            if (mcFailButton.Value == false)//false is pressed
            {
                mcFail.Value = true;
            }
            else
            {
                mcFail.Value = false;
            }
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% MACHINE CENTER ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% EMITTER STARTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            if (emitterStartButton.Value == true)
            {                
                emitter.Value = true;             
            }
            else
            {
                emitter.Value = false;
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% EMITTER ENDS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        }
    }
    public enum GripperStatus
    {
        IDLE,
        WORKING,
        DOWN
    }
    public enum McStatus
    {
        IDLE,
        WORKING,
        DOWN
    }

    public enum GripperStep
    {
        INITIAL,
        DOWN_LOOK_FOR_PART,
        GRAB,
        UP_WITH_PART,
        X_EXTEND,
        DOWN_WITH_PART,
        RELEASE,
        UP_NO_PART,
        X_RETRACT,
        DOWN_VIBRATING
    }

}

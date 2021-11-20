using System;
using EngineIO;

namespace Controllers.Scenes
{
    class GripperControlBox
    {
        private readonly GripperArm gripper;
        private readonly MemoryBit gripperStartButton;
        private readonly MemoryBit gripperFailButton;
        private readonly MemoryBit gripperRepairButton;
        private readonly MemoryBit gripperRedLight;
        private readonly MemoryBit gripperYellowLight;
        private readonly MemoryBit gripperGreenLight;
        private readonly MemoryBit gripperAlarmSiren;
        private object timeWorkingGripper;

        GripperArm gripperMc0 = new GripperArm(
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Position (V)", MemoryType.Input),
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Position (V)", MemoryType.Input),
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 X Set Point (V)", MemoryType.Output),
            MemoryMap.Instance.GetFloat("Two-Axis Pick & Place 0 Z Set Point (V)", MemoryType.Output),
            MemoryMap.Instance.GetBit("Two-Axis Pick & Place 0 (Grab)", MemoryType.Output)
        );

        public GripperControlBox(MemoryBit startButtton, MemoryBit failButton, MemoryBit repairButton, MemoryBit redLight, MemoryBit yellowLight, MemoryBit greenLight, MemoryBit alarmSiren, GripperArm gripperArm)
        {
            this.gripper = gripperArm;
            this.gripperStartButton = startButtton;
            this.gripperFailButton = failButton;
            this.gripperRepairButton = repairButton;
            this.gripperRedLight = redLight;
            this.gripperYellowLight = yellowLight;
            this.gripperGreenLight = greenLight;
            this.gripperAlarmSiren = alarmSiren;
        }

        public void stateTransition()
        {
            if (gripperStartButton.Value == true)
            {
                Console.WriteLine("Green button pressed");
                gripper.start();
            }
            if (gripperFailButton.Value == false)//FAIL Button pressed, gripper failing.
            {
                Console.WriteLine("Gripper is failing");
                gripper.fail();
            }

            if (gripperRepairButton.Value == true)//REPAIR Button pressed, gripper repaired.
            {
                Console.WriteLine("GripperRepair value is: " + gripperRepairButton.Value);
                gripper.repair();
            }
        }

    }
}

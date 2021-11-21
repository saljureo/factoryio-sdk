using System;
using EngineIO;

namespace Controllers.Scenes
{
    class ControlHub
    {
        private readonly MemoryBit gripperStartButton;
        private readonly MemoryBit gripperFailButton;
        private readonly MemoryBit gripperRepairButton;
        private readonly MemoryBit gripperRedLight;
        private readonly MemoryBit gripperYellowLight;
        private readonly MemoryBit gripperGreenLight;
        private readonly MemoryBit gripperAlarmSiren;
        private readonly GripperArm gripperArm;

        public ControlHub(MemoryBit startButtton, MemoryBit failButton, MemoryBit repairButton, MemoryBit redLight,
                            MemoryBit yellowLight, MemoryBit greenLight, MemoryBit alarmSiren, GripperArm gripperArm)
        {
            this.gripperStartButton = startButtton;
            this.gripperFailButton = failButton;
            this.gripperRepairButton = repairButton;
            this.gripperRedLight = redLight;
            this.gripperYellowLight = yellowLight;
            this.gripperGreenLight = greenLight;
            this.gripperAlarmSiren = alarmSiren;
            this.gripperArm = gripperArm;
        }

        

    }
}

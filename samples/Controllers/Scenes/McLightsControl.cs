using EngineIO;

namespace Controllers.Scenes
{
    class McLightsControl
    {
        private MemoryBit mcRedLight;
        private MemoryBit mcYellowLight;
        private MemoryBit mcGreenLight;
        private int timeDownMc;

        public McLightsControl(MemoryBit mcRedLight, MemoryBit mcYellowLight, MemoryBit mcGreenLight)
        {
            this.mcRedLight = mcRedLight;
            this.mcYellowLight = mcYellowLight;
            this.mcGreenLight = mcGreenLight;
            timeDownMc = 0;
        }

        public void failingLights()
        {
            if (timeDownMc < 30)
            {
                mcGreenLight.Value = false;
                mcYellowLight.Value = false;
                mcRedLight.Value = false;
            }
            else if (timeDownMc < 60)
            {
                mcGreenLight.Value = true;
                mcYellowLight.Value = true;
                mcRedLight.Value = true;
            }
            else if (timeDownMc >= 60)
            {
                timeDownMc = 0;
            }
            timeDownMc++;
        }

        public void workingLights()
        {
            mcGreenLight.Value = false;
            mcYellowLight.Value = false;
            mcRedLight.Value = false;
        }
    }
}

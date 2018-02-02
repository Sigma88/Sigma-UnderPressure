using UnityEngine;


namespace SigmaUnderPressure
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class BallastDensityFix : MonoBehaviour
    {
        static CelestialBody planet = null;

        void Update()
        {
            if (planet != FlightGlobals.currentMainBody)
            {
                planet = FlightGlobals.currentMainBody;

                if (planet.ocean)
                {
                    PartResourceDefinition ballast = PartResourceLibrary.Instance.GetDefinition("Ballast");

                    var cfg = new ConfigNode();
                    ballast.Save(cfg);
                    cfg.RemoveValues("density");
                    cfg.AddValue("density", planet.oceanDensity * 0.001);
                    ballast.Load(cfg);
                }
            }
        }
    }

    public class ModuleWaterPump : PartModule
    {
        [KSPField(guiName = "Mode", isPersistant = true, guiActive = true, guiActiveEditor = true), UI_Toggle(scene = UI_Scene.All, affectSymCounterparts = UI_Scene.All, disabledText = "Flood Tank", enabledText = "Blow Tank")]
        public bool WaterOut;

        [KSPField(guiName = "Valves", isPersistant = true, guiActive = true, guiActiveEditor = true), UI_Toggle(scene = UI_Scene.All, affectSymCounterparts = UI_Scene.All, disabledText = "Closed", enabledText = "Open")]
        public bool PumpActive;

        void Update()
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT) return;

            // if the pump is active
            if (PumpActive)
            {
                // and it is pumping water out
                if (WaterOut)
                {
                    // stop taking water in
                    part.GetComponent<ModuleWaterPumpIN>().StopResourceConverter();
                    // start pumping water out
                    part.GetComponent<ModuleWaterPumpOUT>().StartResourceConverter();
                }
                // if it is pumping water in
                else if (vessel.checkSplashed())
                {
                    // start taking water in
                    part.GetComponent<ModuleWaterPumpIN>().StartResourceConverter();
                    // stop pumping water out
                    part.GetComponent<ModuleWaterPumpOUT>().StopResourceConverter();
                }
                else
                {
                    // stop taking water in
                    part.GetComponent<ModuleWaterPumpIN>().StopResourceConverter();
                    // stop pumping water out
                    part.GetComponent<ModuleWaterPumpOUT>().StopResourceConverter();
                }
            }
            else
            {
                // stop taking water in
                part.GetComponent<ModuleWaterPumpIN>().StopResourceConverter();
                // stop pumping water out
                part.GetComponent<ModuleWaterPumpOUT>().StopResourceConverter();
            }
        }
    }

    public class ModuleWaterPumpIN : ModuleResourceConverter
    {
        [KSPEvent(guiName = "ModuleWaterPumpIN", guiActive = false, active = false)]
        public override void StartResourceConverter()
        {
            base.StartResourceConverter();
        }

        [KSPEvent(guiName = "ModuleWaterPumpIN", guiActive = false, active = false)]
        public override void StopResourceConverter()
        {
            base.StopResourceConverter();
        }

        public override void StartResourceConverterAction(KSPActionParam param)
        {
            part.GetComponent<ModuleWaterPump>().PumpActive = true;
        }

        public override void StopResourceConverterAction(KSPActionParam param)
        {
            part.GetComponent<ModuleWaterPump>().PumpActive = false;
        }

        public override void ToggleResourceConverterAction(KSPActionParam param)
        {
            part.GetComponent<ModuleWaterPump>().PumpActive = !part.GetComponent<ModuleWaterPump>().PumpActive;
        }
    }

    public class ModuleWaterPumpOUT : ModuleResourceConverter
    {
        [KSPEvent(guiName = "ModuleWaterPumpOUT", guiActive = false, active = false)]
        public override void StartResourceConverter()
        {
            base.StartResourceConverter();
        }

        [KSPEvent(guiName = "ModuleWaterPumpOUT", guiActive = false, active = false)]
        public override void StopResourceConverter()
        {
            base.StopResourceConverter();
        }

        public override void StartResourceConverterAction(KSPActionParam param)
        {
            part.GetComponent<ModuleWaterPump>().WaterOut = false;
        }

        public override void StopResourceConverterAction(KSPActionParam param)
        {
            part.GetComponent<ModuleWaterPump>().WaterOut = true;
        }

        public override void ToggleResourceConverterAction(KSPActionParam param)
        {
            part.GetComponent<ModuleWaterPump>().WaterOut = !part.GetComponent<ModuleWaterPump>().WaterOut;
        }
    }
}

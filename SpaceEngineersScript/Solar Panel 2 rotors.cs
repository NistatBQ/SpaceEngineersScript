using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace SolarPanel
{    
    public sealed class Program : MyGridProgram
    {
        IMyCameraBlock Cam;
        //IMyGyro Gyro;
        //IMySolarPanel Panel;
        Vector3D V1, V2, Axis;
        IMyTextPanel LCD;

        Program()
        {
            Cam = GridTerminalSystem.GetBlockWithName("Camera") as IMyCameraBlock;
            //Gyro = GridTerminalSystem.GetBlockWithName("Gyroscope") as IMyGyro;
            LCD = GridTerminalSystem.GetBlockWithName("LCD") as IMyTextPanel;
        }
        public void Main(string argument)
        {
            switch (argument)
            {
                case "V1":
                    {
                        V1 = Cam.WorldMatrix.Forward;
                        LCD.WriteText("V1: " + V1, false);
                        break;
                    }
                case "V2":
                    {
                        V2 = Cam.WorldMatrix.Forward;
                        Axis = V1.Cross(V2);
                        Axis = Vector3D.Normalize(Axis);
                        LCD.WriteText("V1: " + V1, false);
                        LCD.WriteText("\nV2: " + V2, true);
                        LCD.WriteText("\nAxis: " + Axis, true);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}

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

namespace Horizont
{
    public sealed class Program : MyGridProgram
    {

        IMyShipController cockpit;        
        IMyGyro gyro;        

        Program()
        {
            cockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;
            gyro = GridTerminalSystem.GetBlockGroupWithName("Gyro") as IMyGyro;            
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        void Main()
        {
            //гироскопы включай вручную))
            Vector3D GravVector = cockpit.GetNaturalGravity();
            float Pitch = (float)GravVector.Dot(cockpit.WorldMatrix.Backward);
            float Roll = (float)GravVector.Dot(cockpit.WorldMatrix.Left);
            
            gyro.Pitch = Pitch;
            gyro.Roll = Roll;             
        }
    }
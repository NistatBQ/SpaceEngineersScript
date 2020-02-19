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

namespace Cheburashka
{
    public sealed class Program : MyGridProgram
    {
        int freq = 3;
        int i = 0, old_i = 0;
        float azimuth_KP = 4;
        float azimuth_KD = 40;

        IMyShipController kabina;
        IMyMotorStator azimuth_rotor, elevation_rotorL, elevation_rotorR;
        float target_elevation = 0;
        float target_azimuth = 0;
        float old_azimuth_angle = 0;

        float mouse_sensitivity_H = 0.005f;
        float mouse_sensitivity_V = 0.005f;

        Program()
        {
            kabina = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;
            IMyBlockGroup turret = GridTerminalSystem.GetBlockGroupWithName("Turret");
            List<IMyMotorStator> rotors = new List<IMyMotorStator>();
            turret.GetBlocksOfType<IMyMotorStator>(rotors, (b) => b.CustomName.Contains("Azimuth"));
            if (rotors.Count > 0)
                azimuth_rotor = rotors[0];
            rotors.Clear();
            turret.GetBlocksOfType<IMyMotorStator>(rotors, (b) => b.CustomName.Contains("Elevation"));

            foreach (IMyMotorStator rotor in rotors)
            {
                if (rotor.Orientation.Up == azimuth_rotor.Top.Orientation.Left)
                    elevation_rotorL = rotor;
                else
                    elevation_rotorR = rotor;
            }

            Echo("Cockpit: " + ((kabina != null) ? ("online") : ("not found")));
            Echo("Azimuth rotor: " + ((azimuth_rotor != null) ? ("found") : ("not found")));
            Echo("Left elevation rotor: " + ((elevation_rotorL != null) ? ("found") : ("not found")));
            Echo("Right elevation rotor: " + ((elevation_rotorR != null) ? ("found") : ("not found")));
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        void Main()
        {
            i++;

            if (i % freq == 0)
            {
                TurretMainCycle();
            }
        }
        void TurretMainCycle()
        {
            float hor = kabina.RotationIndicator.Y;
            azimuth_rotor.TargetVelocityRad = hor;
            int TicksPassed = i - old_i;
            target_elevation = (target_elevation + kabina.RotationIndicator.X * mouse_sensitivity_V / TicksPassed) % MathHelper.TwoPi;
            target_azimuth = (target_azimuth + kabina.RotationIndicator.Y * mouse_sensitivity_H / TicksPassed) % MathHelper.TwoPi;

            float P = (Rotate(target_azimuth - azimuth_rotor.Angle));
            float D = (Rotate(azimuth_rotor.Angle - old_azimuth_angle)) / TicksPassed;
            old_azimuth_angle = azimuth_rotor.Angle;

            azimuth_rotor.TargetVelocityRad = P * azimuth_KP - D * azimuth_KD;

            elevation_rotorL.TargetVelocityRad = (Rotate(target_elevation - elevation_rotorL.Angle)) * 5f;
            elevation_rotorR.TargetVelocityRad = (Rotate(-target_elevation - elevation_rotorR.Angle)) * 5f;
            old_i = i;
        }
        float Rotate(float Turn)
        {
            if (float.IsNaN(Turn)) Turn = 0;
            if (Turn < -Math.PI) Turn += MathHelper.TwoPi;
            else if (Turn > Math.PI) Turn -= MathHelper.TwoPi;
            return Turn;
        }
    }
}
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

namespace IngameScript
{
    public sealed class TargetPoint : MyGridProgram
    {
        IMyTimerBlock Timer;
        IMyTextPanel TP, TP2;
        IMyRemoteControl RemCon;

        int TickCount;
        int Clock;

        bool Stop;

        float GyroMult = 10;

        Vector3D TestV = new Vector3D(53608.45, -26606.04, 12064.63);

        void Main (string argument)
        {
            if (Timer == null)
                Timer = GridTerminalSystem.GetBlockWithName("Timer") as IMyTimerBlock;
            if (TP == null)
                TP = GridTerminalSystem.GetBlockWithName("LCD") as IMyTextPanel;
            if (TP2 == null)
                TP2 = GridTerminalSystem.GetBlockWithName("LCD2") as IMyTextPanel;
            if (RemCon == null)
                RemCon = GridTerminalSystem.GetBlockWithName("RemCon") as IMyRemoteControl;
            switch (argument)
            {
                case "Start":
                    {
                        Stop = false;
                        break;
                    }
                case "Stop":
                    {
                        Stop = true;
                        break;
                    }
                default:
                    break;
            }

            SetGyroOverride(true, GetNavAngles(TestV)*GyroMult,1);

            if (!Stop)
                Timer.ApplyAction("TriggerNow");
            else
                SetGyroOverride(false, new Vector3(0, 0, 0));
        }

        Vector3D GetNavAngles(Vector3D Target)
        {
            Vector3D V3Dcenter = RemCon.GetPosition();    //текущее положение
            Vector3D V3Dfow = RemCon.WorldMatrix.Forward; //вектор вперед от RemCon
            Vector3D V3Dup = RemCon.WorldMatrix.Up;
            Vector3D V3Dleft = RemCon.WorldMatrix.Left;

            Vector3D TargetNorm = Vector3D.Normalize(Target - V3Dcenter); // вектор из RemCon на цель

            double TargetPitch = Math.Acos(Vector3D.Dot(V3Dup, //тангаж на цель
                                 Vector3D.Normalize(Vector3D.Reject(TargetNorm, V3Dleft)))) - (Math.PI / 2); //Dot скалярное умножение векторов

            double TargetYaw   = Math.Acos(Vector3D.Dot(V3Dleft,//рысканье на цель
                                 Vector3D.Normalize(Vector3D.Reject(TargetNorm, V3Dup)))) - (Math.PI / 2);

            double TargetRoll  = Math.Acos(Vector3D.Dot(V3Dleft,//выравнивание по горизонту на цель
                                 Vector3D.Normalize(Vector3D.Reject(-RemCon.GetNaturalGravity(),V3Dfow)))) - (Math.PI / 2);

            Vector3D vectorResult = Target + V3Dcenter;            

            double Distance = Math.Sqrt(Vector3D.DistanceSquared(V3Dcenter, Target)); // расстояние между GPS точками

            TP.WriteText("Yaw: " + Math.Round(TargetYaw, 5) +
                    "\n Pitch: " + Math.Round(TargetPitch, 5) +
                     "\n Roll: " + Math.Round(TargetRoll, 5));

            TP2.WriteText("coordinates of Target: " + "\n" + Target + "\n" +
                          "Distance: " + "\n" + Distance);

            return new Vector3D(TargetYaw, -TargetPitch, TargetRoll);
        }

        void SetGyroOverride(bool OverrideOnOff, Vector3 settings, float Power = 1)
        {
            var Gyros = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName("Gyro", Gyros);
            for (int i = 0; i < Gyros.Count; i++)
            {
                IMyGyro Gyro = Gyros[i] as IMyGyro;
                if (Gyro!=null)
                {
                    if ((!Gyro.GyroOverride && OverrideOnOff) || (Gyro.GyroOverride && !OverrideOnOff))
                        Gyro.ApplyAction("Override");
                    Gyro.SetValue("Power",Power);
                    Gyro.SetValue("Yaw", settings.GetDim(0));
                    Gyro.SetValue("Pitch", settings.GetDim(1));
                    Gyro.SetValue("Roll", settings.GetDim(2));
                }
            }
        }
    }
}

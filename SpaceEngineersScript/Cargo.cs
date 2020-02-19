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

namespace Cargo
{
    public sealed class Program : MyGridProgram
    {
        const string PANEL_NAME = "LCDPanel1";
        const string CONTAINER_NAME = "LargeCargoContainer";
        const int PANEL_LINES = 22;
        int lineOffset = 0;
        void Main()
        {
            List<IMyTerminalBlock> work = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(PANEL_NAME, work);
            IMyTextPanel panel = (IMyTextPanel)work[0];
            GridTerminalSystem.SearchBlocksOfName(CONTAINER_NAME, work);
            IMyCargoContainer container = (IMyCargoContainer)work[0];
            
            List<MyInventoryItem> containerItems = new List<MyInventoryItem>();
            container.GetInventory(0).GetItems(containerItems);

            panel.WriteText(Convert.ToString(containerItems.Count));

            List<String> list = new List<String>();
            for (int j = containerItems.Count - 1; j >= 0; j--)
            {
                String txt = decodeItemName(containerItems[j].Type.SubtypeId, containerItems[j].Type.TypeId.ToString()) + " - ";
                String amt = amountFormatter((float)containerItems[j].Amount, containerItems[j].Type.TypeId.ToString());
                txt += amt;
                list.Add(txt);                
            }
            list.Sort();
            list.Insert(0, "------------------------------------------------------");
            list.Insert(0, container.CustomName + " Инвентарь");
            for (int o = 0; o < lineOffset; o++)
            {
                String shiftedItem = list[0];
                list.RemoveAt(0);
                list.Add(shiftedItem);
            }
            panel.WriteText(String.Join("\n", list.ToArray()), false);
            
            if (list.Count > PANEL_LINES)
            {
                lineOffset++;
                if (list.Count - lineOffset < PANEL_LINES)
                {
                    lineOffset = 0;
                }
            }
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        String amountFormatter(float amt, String typeId)
        {
            if (typeId.EndsWith("_Ore") || typeId.EndsWith("_Ingot"))
            {
                if (amt > 1000.0f)
                {
                    return "" + Math.Round((float)amt / 1000, 2).ToString("###,###,##0.00") + "K";
                }
                else
                {
                    return "" + Math.Round((float)amt, 2).ToString("###,###,##0.00");
                }
            }
            return "" + Math.Round((float)amt, 0).ToString("###,###,##0");
        }

        String decodeItemName(String name, String typeId)
        {
            if (name.Equals("Construction")) { return "Строительный Компонент"; }
            if (name.Equals("MetalGrid")) { return "Металлическая решетка"; }
            if (name.Equals("InteriorPlate")) { return "Интерьерная панель"; }
            if (name.Equals("SteelPlate")) { return "Стальная пластина"; }
            if (name.Equals("SmallTube")) { return "Маленькая стальная трубка"; }
            if (name.Equals("LargeTube")) { return "Большая стальная труба"; }
            if (name.Equals("BulletproofGlass")) { return "Бронированное стекло"; }
            if (name.Equals("Reactor")) { return "Компоненты реактора"; }
            if (name.Equals("Thrust")) { return "Компоненты ускорителя"; }
            if (name.Equals("GravityGenerator")) { return "Компонент гравитационного генератора"; }
            if (name.Equals("Medical")) { return "Медицинские компонентв"; }
            if (name.Equals("RadioCommunication")) { return "Комплектующие радио-связи"; }
            if (name.Equals("Detector")) { return "Компоненты детектора"; }
            if (name.Equals("SolarCell")) { return "Солнечная панель"; }
            if (name.Equals("PowerCell")) { return "Аккумулятор"; }
            if (name.Equals("AutomaticRifleItem")) { return "Автоматическая винтовка"; }
            if (name.Equals("AutomaticRocketLauncher")) { return "Ракетница"; }
            if (name.Equals("WelderItem")) { return "Горелка"; }
            if (name.Equals("AngleGrinderItem")) { return "Шлифовальщик"; }
            if (name.Equals("HandDrillItem")) { return "Ручная дрель"; }
            if (typeId.EndsWith("_Ore"))
            {
                if (name.Equals("Stone"))
                {
                    return name;
                }
                return name + " Ore";
            }
            if (typeId.EndsWith("_Ingot"))
            {
                if (name.Equals("Stone"))
                {
                    return "Gravel";
                }
                if (name.Equals("Magnesium"))
                {
                    return name + " Powder";
                }
                if (name.Equals("Silicon"))
                {
                    return name + " Wafer";
                }
                return name + " Ingot";
            }
            return name;
        }
    }
}
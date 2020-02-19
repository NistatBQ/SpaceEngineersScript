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

namespace Template
{
    public sealed class Program : MyGridProgram
    {
        // Inventory

// Script by rubinknoepfel 
// V3.3 -- 17.05.2018 
 
//--------------------------------------------------------------------------------------------------------------------------------------------- 
 
// Settings: 
 
static bool useCustomLanguage = false;                      // Use custom Language
static bool showEmpty = false;                                     // Show items that have zero amount
static bool sortRareItemsByRarity = true;                     // Sort rare items by ratity, otherwise they are sorted by name
static bool writeToCustomDataIsAllowed = true;         // This script is allowed to write to the Custom Data section of this programmable block
static bool selfUpdating = true;                                      // This script is updating itself regulary (even without timer block)
static bool showTextOnScreen = true;                          // Show text automatically on screens

static string offsetTop = "\n";  
static string offsetLeft = "    ";

 
// Commands:

static string lcdCommandAll = "Inventory";
static string lcdCommandOres = "Inventory Ores";
static string lcdCommandIngots = "Inventory Ingots";
static string lcdCommandMaterials = "Inventory Materials";
static string lcdCommandComponents = "Inventory Components";
static string lcdCommandTools = "Inventory Tools";
static string lcdCommandRare = "Inventory Rare";


// Labels for custom language:

static string labelOres = "Erze";
static string labelIngots = "Barren";
static string labelTools = "Werkzeuge";
static string labelComponents = "Komponenten";
static string labelTargetAmount = "Zielmenge"; 
static string labelRare = "Knappe Ressourcen";

// Ores
static string stone = "Stein";
static string ironOre = "Eisenerz"; 
static string nickelOre = "Nickelerz"; 
static string cobaltOre = "Kobalterz"; 
static string magnesiumOre = "Magnesiumerz"; 
static string siliconOre = "Siliziumerz";  
static string silverOre = "Silbererz";  
static string goldOre = "Golderz"; 
static string platinumOre = "Platinerz";   
static string uraniumOre = "Uranerz";   
static string ice = "Eis";
static string scrapMetal = "Metallabfall";
 
// Ingots
static string gravel = "Kies"; 
static string ironIngot = "Eisenbarren";  
static string nickelIngot = "Nickelbarren";  
static string cobaltIngot = "Kobaltbarren";  
static string magnesiumPowder = "Magnesiumpulver";  
static string siliconWafer = "Siliziumhalbleiter";   
static string silverIngot = "Silberbarren";   
static string goldIngot = "Goldbarren";  
static string platinumIngot = "Platinbarren";    
static string uraniumIngot = "Uranbarren";  

// Components
static string bulletproofGlass = "Panzerglas";  
static string canvas = "Canvas";   
static string computer = "Computer";    
static string constructionComponent = "Herstellungskomponente";    
static string detectorComponent = "Sensorkomponente";   
static string display = "Anzeige";     
static string explosives = "Sprengstoff"; 
static string girder = "Träger";   
static string gravityGeneratorComponents = "Komponenten für Schwerkraftgenerator";   
static string interiorPlate = "Interne Panzerung";    
static string largeSteelTube = "Stahlrohr (gross)";   
static string medicalComponents = "Medizinische Komponenten";     
static string metalGrid = "Metallgerüst"; 
static string missileContainer200mm = "Raketenbehälter (200mm)";   
static string motor = "Motor";    
static string natoAmmoContainer25x184mm = "25x184mm NATO Munitionskasten";    
static string natoMagazine5_56x45mm = "5.56x45mm NATO Magazin";   
static string powerCell = "Energiezelle";     
static string radioCommunicationComponent = "Kommunikationskomponente"; 
static string reactorComponents = "Reaktorkomponenten";   
static string smallSteelTube = "Stahlrohr (klein)";    
static string solarCell = "Solarzelle";    
static string steelPlate = "Stahlpatte";   
static string superconductorConduits = "Supraleiterkabel";     
static string thrusterComponents = "Triebwerk Komponenten";

// Tools
static string welder = "Schweissgerät";    
static string enhancedWelder = "Verbessertes Schweissgerät";      
static string proficientWelder = "Profischweissgerät";  
static string eliteWelder = "Eliteschweissgerät";     
static string grinder = "Schleifgerät";     
static string enhancedGrinder = "Verbessertes Schleifgerät";       
static string proficientGrinder = "Profischleifgerät";   
static string eliteGrinder = "Eliteschleifgerät";      
static string handDrill = "Handbohrer";     
static string enhancedHandDrill = "Verbesserter Handbohrer";       
static string proficientHandDrill = "Profihandbohrer";   
static string eliteHandDrill = "Elitehandbohrer";  
static string automaticRifle = "Sturmgewehr";     
static string preciseAutomaticRifle = "Genaues Sturmgewehr";       
static string rapidFireAutomaticRifle = "Schnellfeuersturmgewehr";   
static string eliteAutomaticRifle = "Elitesturmgewehr";  
static string hydrogenBottle = "Wasserstoffflasche";      
static string oxygenBottle = "Sauerstoffflasche";

 
//---------------------------------------------------------------------------------------------------------------------------------------------


// Item type
public enum ItemType { Ore, Ingot, Component, Tool };

// This class holds name, type, amount and target amount of an item
public class Item : IComparable<Item>
{ 
    private string code;
    private string name;
    private float amount; 
    private float targetAmount;
    private ItemType type;
    static private bool sortByRarity = false;

    public Item (string code)
    { 
        this.code = code;
        this.name = translate(code);
        this.amount = 0;
        this.targetAmount = 0;

        if (code.Contains("Ore")) 
        { 
            type = ItemType.Ore;
        } 
        else if (code.Contains("Ingot"))  
        {  
            type = ItemType.Ingot; 
        }  
        else if (code.Contains("Item") || code.Contains("Bottle"))  
        {   
            type = ItemType.Tool; 
        }  
        else 
        {   
            type = ItemType.Component; 
        } 
    } 
 
    public string Code
    { 
        get { return code; } 
    } 

    public ItemType Type
    { 
        get { return type; } 
    }

    public string Name
    {
        get { return name; }
    } 

    public float Amount
    {
        get { return amount; }
    }  
 
    public float TargetAmount 
    { 
        get { return targetAmount; } 
        set { targetAmount = value; }
    } 

    public float Rarity
    {
        get { return amount / targetAmount * 100.0f; } 
    }

    public bool IsRare
    {
        get { return amount < targetAmount; }
    } 

    static public bool SortByRarity
    {
        get { return sortByRarity; }
        set { sortByRarity = value; }
    } 

    public void Add (float amount)
    {
        this.amount += amount;
    }

    public string amountToString(bool showPercent = false)
    {
        string str = name + ": " + amount + " " + getUnit();

        if (showPercent)
        {
            str += " (" + ((int)Rarity).ToString() + "%)";
        }

        return str;
    }

    public string targetAmountToString()
    {
        return name + ": " + targetAmount + " " + getUnit();
    }

    public int CompareTo(Item item)
    {
        if (item == null)
        {
            return 1;
        }
        else if (type != item.Type)
        {
            return type.CompareTo(item.Type);
        } 
        else if (sortByRarity)
        {
            return Rarity.CompareTo(item.Rarity);
        }
        else
        {
            return name.CompareTo(item.Name);
        } 
    }
 
    private string getUnit()
    { 
        if (type == ItemType.Ore || type == ItemType.Ingot)
        {
            return "kg";
        }
        else
        {
            return "";
        }
    }

    private string translate(string code) 
    { 
        switch (code)  
        {  
            case "StoneOre": return stone;  
            case "IronOre": return ironOre;  
            case "NickelOre": return nickelOre;  
            case "CobaltOre": return cobaltOre;   
            case "MagnesiumOre": return magnesiumOre;    
            case "SiliconOre": return siliconOre;    
            case "SilverOre": return silverOre;     
            case "GoldOre": return goldOre;   
            case "PlatinumOre": return platinumOre;   
            case "UraniumOre": return uraniumOre;    
            case "IceOre": return ice;    
            case "ScrapOre": return scrapMetal;  
 
            case "StoneIngot": return gravel;    
            case "IronIngot": return ironIngot;   
            case "NickelIngot": return nickelIngot;  
            case "CobaltIngot": return cobaltIngot;  
            case "MagnesiumIngot": return magnesiumPowder;  
            case "SiliconIngot": return siliconWafer;    
            case "SilverIngot": return silverIngot;    
            case "GoldIngot": return goldIngot; 
            case "PlatinumIngot": return platinumIngot;  
            case "UraniumIngot": return uraniumIngot;  
 
            case "BulletproofGlass": return bulletproofGlass;   
            case "Canvas": return canvas;   
            case "Computer": return computer;   
            case "Construction": return constructionComponent;   
            case "Detector": return detectorComponent;  
            case "Display": return display;    
            case "Explosives": return explosives; 
            case "Girder": return girder;   
            case "GravityGenerator": return gravityGeneratorComponents;  
            case "InteriorPlate": return interiorPlate;  
            case "LargeTube": return largeSteelTube;  
            case "Medical": return medicalComponents;  
            case "MetalGrid": return metalGrid;  
            case "Missile200mm": return missileContainer200mm;  
            case "Motor": return motor;   
            case "NATO_25x184mm": return natoAmmoContainer25x184mm; 
            case "NATO_5p56x45mm": return natoMagazine5_56x45mm;  
            case "PowerCell": return powerCell; 
            case "RadioCommunication": return radioCommunicationComponent;  
            case "Reactor": return reactorComponents; 
            case "SmallTube": return smallSteelTube;  
            case "SolarCell": return solarCell;  
            case "SteelPlate": return steelPlate;  
            case "Superconductor": return superconductorConduits;  
            case "Thrust": return thrusterComponents; 
  
            case "WelderItem": return welder;   
            case "Welder2Item": return enhancedWelder;  
            case "Welder3Item": return proficientWelder;   
            case "Welder4Item": return eliteWelder;  
            case "AngleGrinderItem": return grinder;    
            case "AngleGrinder2Item": return enhancedGrinder;   
            case "AngleGrinder3Item": return proficientGrinder;    
            case "AngleGrinder4Item": return eliteGrinder;   
            case "HandDrillItem": return handDrill;     
            case "HandDrill2Item": return enhancedHandDrill;    
            case "HandDrill3Item": return proficientHandDrill;     
            case "HandDrill4Item": return eliteHandDrill;    
            case "AutomaticRifleItem": return automaticRifle;      
            case "PreciseAutomaticRifleItem": return preciseAutomaticRifle;     
            case "RapidFireAutomaticRifleItem": return rapidFireAutomaticRifle;      
            case "UltimateAutomaticRifleItem": return eliteAutomaticRifle;  
            case "HydrogenBottle": return hydrogenBottle;   
            case "OxygenBottle": return oxygenBottle;  
 
            default: return code; 
        }  
    }
}


public Program()    
{    
    // Set up self updating
    if (selfUpdating)
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update10;   
    }

    // Change language to English if custom language should not be used
    if (!useCustomLanguage)
    {
        changeLanguage();
    }
} 


public void Main() 
{
    // Get all items
    List<Item> items = getItems();

    // Show item list on screens
    showOnLCD(items);
}


// This function shows items on screens
void showOnLCD(List<Item> items)
{ 
    // Create lists divided by type
    List<string>[] linesArray = { new List<string>(), new List<string>(), new List<string>(), new List<string>() }; 

    // Create line for each item and sort them by type
    foreach (Item item in items)
    { 
        if (showEmpty || item.Amount > 0)
        {
            linesArray[(int)item.Type].Add(item.amountToString());
        }
    }

    string[] texts = { labelOres, labelIngots, labelComponents, labelTools };
 
    // Add lines to text and then clear the lines
    int i = 0;
    foreach (List<string> lines in linesArray) 
    { 
        texts[i] = offsetLeft + texts[i] + "\n" + offsetLeft + String.Join("\n" + offsetLeft, lines); 
        lines.Clear();
        ++i;
    } 

    if (sortRareItemsByRarity)
    {
        // Sort items by rarity
        Item.SortByRarity = true;
        items.Sort();
        Item.SortByRarity = false;
    }

    // Create line for each item and sort them by type
    foreach (Item item in items)
    { 
        if (item.IsRare)
        {
            linesArray[(int)item.Type].Add(item.amountToString(true));
        }
    }

    string[] labels = { labelOres, labelIngots, labelComponents, labelTools };
    string textRare = offsetLeft + labelRare; 

    // Add lines to text 
    i = 0;  
    foreach (List<string> lines in linesArray)   
    {    
        if (lines.Count > 0)
        {
            textRare += "\n\n" + offsetLeft + labels[i] + "\n" + offsetLeft + String.Join("\n" + offsetLeft, lines); 
        }

        ++i;  
    }

    // Print texts to LCDs
    string textMaterials = texts[(int)ItemType.Ore] + "\n\n" + texts[(int)ItemType.Ingot]; 
    string textAll = textMaterials + "\n\n" + texts[(int)ItemType.Component] + "\n\n" + texts[(int)ItemType.Tool];

    toLCD(offsetTop + texts[(int)ItemType.Ore], lcdCommandOres);
    toLCD(offsetTop + texts[(int)ItemType.Ingot], lcdCommandIngots);  
    toLCD(offsetTop + texts[(int)ItemType.Tool], lcdCommandTools);  
    toLCD(offsetTop + texts[(int)ItemType.Component], lcdCommandComponents);  
    toLCD(offsetTop + textMaterials, lcdCommandMaterials); 
    toLCD(offsetTop + textAll, lcdCommandAll);  
    toLCD(offsetTop + textRare, lcdCommandRare); 
}


// This function shows a text on a lcd panel with the correct public title
void toLCD(string text, string command)
{   
    // Get all lcd panels with the correct public title
    List<IMyTerminalBlock> lcds = new List<IMyTerminalBlock>();    
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, block => ((IMyTextPanel)block).GetPublicTitle() == command);        

    // Show text on screen
    foreach (IMyTextPanel lcd in lcds)  
    {  
       lcd.WriteText(text);  

        if (showTextOnScreen)
        {
            lcd.ShowPublicTextOnScreen();
        }
    }  
}


// This function collects all inventory items and combine items with same code
List<Item> getItems()
{
    // Get a list of all blocks which have an inventory
    List<IMyTerminalBlock> cargos = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType<IMyEntity>(cargos, block => block.HasInventory);   
    
    List<IMyInventoryItem> inventoryItems = new List<IMyInventoryItem>();

    // Get all inventory items
    foreach (IMyEntity cargo in cargos) 
    { 
        for (int i = 0; i < cargo.InventoryCount; ++i) 
        { 
            inventoryItems.AddRange(inventoryItems); 
        } 
    } 

    // Create empty list of all known items
    List<Item> items = createItemList();

    // Fill list and combine items with same code
    foreach (IMyInventoryItem inventoryItem in inventoryItems)
    {   
        // Get item code
        string code = getItemCode(inventoryItem);

        // Get amount
        float amount = (float)inventoryItem.Amount;
        bool found = false;

        // Search item with same code
        foreach (Item item in items)
        {
            if (item.Code == code)
            {
                // Add amount to existing item
                item.Add(amount);
                found = true;
                break;
            }
        }

        // If item not found create new item
        if (!found)
        {
            Item item = new Item(code); 
            item.Add(amount);
            items.Add(item);
        }
    } 

    // Sort item list
    Item.SortByRarity = false;
    items.Sort();

    // Get target amount of each item from Custom Data field
    getTargetAmount(items);

    return items;
} 


// This function creates the item code
string getItemCode(IMyInventoryItem item)
{
    // Get item type
    string type = item.Content.ToString().Split('_')[1]; 
 
    // Create item code
    if (type == "Ore" || type == "Ingot") 
    { 
        return item.Content.SubtypeId + type; 
    } 
    else 
    { 
        return item.Content.SubtypeId.ToString(); 
    }
}


// This function changes language to English
void changeLanguage() 
{ 
    labelOres = "Ores";  
    labelIngots = "Ingots";  
    labelTools = "Tools";  
    labelComponents = "Components";  
    labelTargetAmount = "Target Amount"; 
    labelRare = "Rare Resources";
  
    stone = "Stone";  
    ironOre = "Iron Ore";   
    nickelOre = "Nickel Ore";   
    cobaltOre = "Cobalt Ore";   
    magnesiumOre = "Magnesium Ore";   
    siliconOre = "Silicon Ore";    
    silverOre = "Silver Ore";    
    goldOre = "Gold Ore";   
    platinumOre = "Platinum Ore";     
    uraniumOre = "Uranium Ore";     
    ice = "Ice";  
    scrapMetal = "Scrap Metal";  
  
    gravel = "Gravel";   
    ironIngot = "Iron Ingot";    
    nickelIngot = "Nickel Ingot";    
    cobaltIngot = "Cobalt Ingot";    
    magnesiumPowder = "Magnesium Powder";    
    siliconWafer = "Silicon Wafer";     
    silverIngot = "Silver Ingot";     
    goldIngot = "Gold Ingot";    
    platinumIngot = "Platinum Ingot";      
    uraniumIngot = "Uranium Ingot";    
 
    bulletproofGlass = "Bulletproof Glass";     
    canvas = "Canvas";       
    computer = "Computer";      
    constructionComponent = "Construction Component";      
    detectorComponent = "Detector Component";     
    display = "Display";       
    explosives = "Explosives";   
    girder = "Girder";     
    gravityGeneratorComponents = "Gravity Generator Components";     
    interiorPlate = "Interior Plate";      
    largeSteelTube = "Large Steel Tube";     
    medicalComponents = "Medical Components";       
    metalGrid = "Metal Grid";   
    missileContainer200mm = "Missile Container 200mm";     
    motor = "Motor";      
    natoAmmoContainer25x184mm = "NATO Ammo Container 25x184mm";      
    natoMagazine5_56x45mm = "NATO Magazine 5.56x45mm";     
    powerCell = "Power Cell";       
    radioCommunicationComponent = "Radio Communication Component";   
    reactorComponents = "Reactor Components";     
    smallSteelTube = "Small Steel Tube";      
    solarCell = "Solar Cell";      
    steelPlate = "Steel Plate";     
    superconductorConduits = "Superconductor Conduits";       
    thrusterComponents = "Thruster Components";  
 
    welder = "Welder";      
    enhancedWelder = "Enhanced Welder";        
    proficientWelder = "Proficient Welder";    
    eliteWelder = "Elite Welder";       
    grinder = "Grinder";       
    enhancedGrinder = "Enhanced Grinder";         
    proficientGrinder = "Proficient Grinder";     
    eliteGrinder = "Elite Grinder";        
    handDrill = "Hand Drill";       
    enhancedHandDrill = "Enhanced Hand Drill";         
    proficientHandDrill = "Proficient Hand Drill";     
    eliteHandDrill = "Elite Hand Drill";    
    automaticRifle = "Automatic Rifle";       
    preciseAutomaticRifle = "Precise Automatic Rifle";         
    rapidFireAutomaticRifle = "Rapid Fire Automatic Rifle";     
    eliteAutomaticRifle = "Elite Automatic Rifle";    
    hydrogenBottle = "Hydrogen Bottle";        
    oxygenBottle = "Oxygen Bottle"; 
}


// This function creates a list of all known items
List<Item> createItemList() 
{ 
    List<Item> items = new List<Item>(); 

    items.Add(new Item("StoneOre"));    
    items.Add(new Item("IronOre"));   
    items.Add(new Item("NickelOre"));   
    items.Add(new Item("CobaltOre"));   
    items.Add(new Item("MagnesiumOre"));   
    items.Add(new Item("SiliconOre"));   
    items.Add(new Item("SilverOre"));   
    items.Add(new Item("GoldOre"));   
    items.Add(new Item("PlatinumOre"));   
    items.Add(new Item("UraniumOre"));   
    items.Add(new Item("IceOre"));   
    items.Add(new Item("ScrapOre"));   
 
    items.Add(new Item("StoneIngot"));   
    items.Add(new Item("IronIngot"));   
    items.Add(new Item("NickelIngot"));   
    items.Add(new Item("CobaltIngot"));   
    items.Add(new Item("MagnesiumIngot"));   
    items.Add(new Item("SiliconIngot"));   
    items.Add(new Item("SilverIngot"));   
    items.Add(new Item("GoldIngot"));   
    items.Add(new Item("PlatinumIngot"));   
    items.Add(new Item("UraniumIngot"));   
 
    items.Add(new Item("BulletproofGlass"));   
    items.Add(new Item("Computer"));   
    items.Add(new Item("Canvas"));   
    items.Add(new Item("Construction"));   
    items.Add(new Item("Detector"));   
    items.Add(new Item("Display"));   
    items.Add(new Item("Explosives"));   
    items.Add(new Item("Girder"));  
    items.Add(new Item("GravityGenerator"));   
    items.Add(new Item("InteriorPlate"));   
    items.Add(new Item("LargeTube"));   
    items.Add(new Item("Medical"));   
    items.Add(new Item("MetalGrid"));  
    items.Add(new Item("Missile200mm"));   
    items.Add(new Item("Motor"));  
    items.Add(new Item("NATO_25x184mm"));   
    items.Add(new Item("NATO_5p56x45mm"));   
    items.Add(new Item("PowerCell"));   
    items.Add(new Item("RadioCommunication"));   
    items.Add(new Item("Reactor"));   
    items.Add(new Item("SmallTube"));   
    items.Add(new Item("SolarCell"));  
    items.Add(new Item("SteelPlate"));   
    items.Add(new Item("Superconductor"));   
    items.Add(new Item("Thrust"));   
 
    items.Add(new Item("WelderItem"));   
    items.Add(new Item("Welder2Item"));   
    items.Add(new Item("Welder3Item"));   
    items.Add(new Item("Welder4Item"));  
    items.Add(new Item("AngleGrinderItem"));   
    items.Add(new Item("AngleGrinder2Item"));   
    items.Add(new Item("AngleGrinder3Item"));   
    items.Add(new Item("AngleGrinder4Item"));   
    items.Add(new Item("HandDrillItem"));   
    items.Add(new Item("HandDrill2Item"));   
    items.Add(new Item("HandDrill3Item"));  
    items.Add(new Item("HandDrill4Item"));   
    items.Add(new Item("AutomaticRifleItem"));   
    items.Add(new Item("PreciseAutomaticRifleItem"));   
    items.Add(new Item("RapidFireAutomaticRifleItem"));   
    items.Add(new Item("UltimateAutomaticRifleItem"));   
    items.Add(new Item("HydrogenBottle"));   
    items.Add(new Item("OxygenBottle"));

    return items; 
}


// This function gets the target amount from the Custom Data field
void getTargetAmount(List<Item> items)
{
    // Get lines of Custom Data field
    string[] customDataLines = Me.CustomData.Split('\n'); 

    // Set each target amount
    foreach (string line in customDataLines)
    {
        // Search item
        foreach (Item item in items)
        {
            if (line.StartsWith(item.Name) && line.Length > item.Name.Length + 1)
            {
                // Parse number
                string number = line.Substring(item.Name.Length + 1).Trim().Split()[0];
                double targetAmount = 0;

                if (Double.TryParse(number, out targetAmount))
                {
                    item.TargetAmount = (float)targetAmount;
                    break;
                }
            }
        }
    }

    // Write back to Custom Data
    if (writeToCustomDataIsAllowed)
    {
        writeTargetAmountToCustomData(items);
    }
}


// This function writes a list of the target amount of each item to the Custom Data field of this programable block
void writeTargetAmountToCustomData(List<Item> items)
{
    string[] titles = { labelOres, labelIngots, labelComponents, labelTools };
    ItemType? curType = null;
    string text = labelTargetAmount + "\n";

    // Add line for each item
    foreach (Item item in items)
    {
        // Add title for next type
        if (item.Type != curType)
        {
            curType = item.Type;
            text += "\n" + titles[(int)curType] + "\n";
        }

        text += item.targetAmountToString() + "\n";
    }

    Me.CustomData = text;
}
    }    
}

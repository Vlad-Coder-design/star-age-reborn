using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarAge3D
{
    public enum ResourceType { Stone, Uranium, Ice, Metal, Fuel, Coins, RepairKits, Boosters }
    public enum BuildingType { Empty, StoneQuarry, UraniumMine, IceWell, FuelFactory, MetalFactory, RepairKitFactory, BoosterFactory, Warehouse, Market }
    public enum GameMode { Planet, Space }
    public enum QuestType { DestroyPirates, MineStone, DeliverFuel, CollectIce }

    [Serializable]
    public class ResourceWallet
    {
        public int stone;
        public int uranium;
        public int ice;
        public int metal;
        public int fuel;
        public int coins = 300;
        public int repairKits;
        public int boosters;

        public int Get(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Stone: return stone;
                case ResourceType.Uranium: return uranium;
                case ResourceType.Ice: return ice;
                case ResourceType.Metal: return metal;
                case ResourceType.Fuel: return fuel;
                case ResourceType.Coins: return coins;
                case ResourceType.RepairKits: return repairKits;
                case ResourceType.Boosters: return boosters;
                default: return 0;
            }
        }

        public void Add(ResourceType type, int amount)
        {
            Set(type, Mathf.Max(0, Get(type) + amount));
        }

        public bool Spend(ResourceType type, int amount)
        {
            if (Get(type) < amount) return false;
            Add(type, -amount);
            return true;
        }

        public void Set(ResourceType type, int value)
        {
            switch (type)
            {
                case ResourceType.Stone: stone = value; break;
                case ResourceType.Uranium: uranium = value; break;
                case ResourceType.Ice: ice = value; break;
                case ResourceType.Metal: metal = value; break;
                case ResourceType.Fuel: fuel = value; break;
                case ResourceType.Coins: coins = value; break;
                case ResourceType.RepairKits: repairKits = value; break;
                case ResourceType.Boosters: boosters = value; break;
            }
        }
    }

    [Serializable]
    public class BuildingSave
    {
        public BuildingType type;
        public float timer;
        public int stored;
    }

    [Serializable]
    public class QuestSave
    {
        public QuestType type;
        public bool accepted;
        public bool completed;
        public int progress;
    }

    [Serializable]
    public class StarAgeSaveData
    {
        public ResourceWallet resources = new ResourceWallet();
        public List<BuildingSave> buildings = new List<BuildingSave>();
        public List<QuestSave> quests = new List<QuestSave>();
        public string shipId = "scout";
        public string weaponId = "laser";
        public int engineLevel;
        public int cargoModules;
        public int armorModules;
        public int shipHp = 100;

        public void EnsureDefaults()
        {
            if (resources == null) resources = new ResourceWallet();
            if (buildings == null) buildings = new List<BuildingSave>();
            while (buildings.Count < 9) buildings.Add(new BuildingSave());
            if (quests == null || quests.Count == 0)
            {
                quests = new List<QuestSave>
                {
                    new QuestSave { type = QuestType.DestroyPirates },
                    new QuestSave { type = QuestType.MineStone },
                    new QuestSave { type = QuestType.DeliverFuel },
                    new QuestSave { type = QuestType.CollectIce }
                };
            }
            if (string.IsNullOrEmpty(shipId)) shipId = "scout";
            if (string.IsNullOrEmpty(weaponId)) weaponId = "laser";
            if (shipHp <= 0) shipHp = ShipStats.For(shipId).hp + armorModules * 50;
        }
    }

    public struct BuildingDefinition
    {
        public string label;
        public string description;
        public ResourceType output;
        public float seconds;
        public int maxStorage;
        public int stoneCost;
        public int uraniumCost;
        public int iceCost;
        public int metalCost;

        public BuildingDefinition(string label, string description, ResourceType output, float seconds, int maxStorage, int stoneCost, int uraniumCost, int iceCost, int metalCost)
        {
            this.label = label;
            this.description = description;
            this.output = output;
            this.seconds = seconds;
            this.maxStorage = maxStorage;
            this.stoneCost = stoneCost;
            this.uraniumCost = uraniumCost;
            this.iceCost = iceCost;
            this.metalCost = metalCost;
        }
    }

    public struct ShipStats
    {
        public string id;
        public string label;
        public int hp;
        public float speed;
        public int cargo;
        public int weaponSlots;
        public int moduleSlots;
        public int cost;

        public static ShipStats For(string id)
        {
            if (id == "fighter") return new ShipStats { id = "fighter", label = "Fighter", hp = 180, speed = 10f, cargo = 30, weaponSlots = 2, moduleSlots = 2, cost = 1200 };
            if (id == "destroyer") return new ShipStats { id = "destroyer", label = "Destroyer", hp = 350, speed = 6f, cargo = 60, weaponSlots = 3, moduleSlots = 3, cost = 3500 };
            return new ShipStats { id = "scout", label = "Scout", hp = 100, speed = 8f, cargo = 20, weaponSlots = 1, moduleSlots = 1, cost = 0 };
        }
    }

    public struct WeaponStats
    {
        public string id;
        public string label;
        public int damage;
        public float fireRate;
        public int cost;

        public static WeaponStats For(string id)
        {
            if (id == "heavy") return new WeaponStats { id = "heavy", label = "Heavy Laser", damage = 25, fireRate = 1.2f, cost = 900 };
            return new WeaponStats { id = "laser", label = "Laser Cannon", damage = 10, fireRate = 0.5f, cost = 0 };
        }
    }
}

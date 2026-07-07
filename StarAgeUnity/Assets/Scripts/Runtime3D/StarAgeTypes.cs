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
        public int coins = 25000;
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
        public string currentSystem = "Fomen";
        public int engineLevel;
        public int cargoModules;
        public int armorModules;
        public int shipHp = 100;
        public int xp;
        public int level = 1;

        public void EnsureDefaults()
        {
            if (resources == null) resources = new ResourceWallet();
            if (buildings == null) buildings = new List<BuildingSave>();
            while (buildings.Count < 9) buildings.Add(new BuildingSave());
            bool emptyColony = true;
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].type != BuildingType.Empty)
                {
                    emptyColony = false;
                    break;
                }
            }

            if (emptyColony)
            {
                buildings[0].type = BuildingType.StoneQuarry;
                buildings[0].stored = 5;
                buildings[1].type = BuildingType.IceWell;
                buildings[1].stored = 5;
                resources.stone = Mathf.Max(resources.stone, 5);
                resources.ice = Mathf.Max(resources.ice, 5);
                resources.coins = Mathf.Max(resources.coins, 25000);
            }
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
            if (string.IsNullOrEmpty(currentSystem)) currentSystem = "Fomen";
            if (shipHp <= 0) shipHp = ShipStats.For(shipId).hp + armorModules * 50;
            if (level <= 0) level = 1;
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
        public int coinCost;

        public BuildingDefinition(string label, string description, ResourceType output, float seconds, int maxStorage, int stoneCost, int uraniumCost, int iceCost, int metalCost, int coinCost = 0)
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
            this.coinCost = coinCost;
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
            if (id == "fighter") return new ShipStats { id = "fighter", label = "Fighter", hp = 200, speed = 20f, cargo = 250, weaponSlots = 2, moduleSlots = 2, cost = 5000 };
            if (id == "destroyer") return new ShipStats { id = "destroyer", label = "Destroyer", hp = 400, speed = 14f, cargo = 300, weaponSlots = 3, moduleSlots = 3, cost = 15000 };
            return new ShipStats { id = "scout", label = "Scout", hp = 100, speed = 26f, cargo = 200, weaponSlots = 1, moduleSlots = 1, cost = 0 };
        }
    }

    public struct WeaponStats
    {
        public string id;
        public string label;
        public int damage;
        public int range;
        public float fireRate;
        public int cost;
        public string color;

        public static WeaponStats For(string id)
        {
            if (id == "laser2" || id == "heavy") return new WeaponStats { id = "laser2", label = "Improved Laser", damage = 15, range = 24, fireRate = 0.83f, cost = 2000, color = "#ff9d2e" };
            return new WeaponStats { id = "laser", label = "Basic Laser", damage = 10, range = 20, fireRate = 1f, cost = 0, color = "#ff4d4d" };
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace StarAge3D
{
    public class BuildingManager : MonoBehaviour
    {
        public readonly List<BuildingSlot> Slots = new List<BuildingSlot>();
        GameObject root;

        public void Init()
        {
            root = new GameObject("Lava Colony");
            BuildPlanet();
            BuildSlots();
            SetActive(false);
        }

        void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.Mode != GameMode.Planet) return;
            foreach (BuildingSlot slot in Slots)
            {
                if (slot.Building != null) slot.Building.Tick(Time.deltaTime);
            }
        }

        public void SetActive(bool active)
        {
            if (root != null) root.SetActive(active);
        }

        public static BuildingDefinition Definition(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.StoneQuarry: return new BuildingDefinition("Stone Quarry", "Produces 1 stone every 15 minutes. Max 5.", ResourceType.Stone, 15f, 5, 0, 0, 0, 0);
                case BuildingType.UraniumMine: return new BuildingDefinition("Uranium Mine", "Produces 1 uranium every 20 minutes. Max 5.", ResourceType.Uranium, 20f, 5, 4, 0, 0, 0);
                case BuildingType.IceWell: return new BuildingDefinition("Ice Mine", "Produces 1 ice every 5 minutes. Max 5.", ResourceType.Ice, 5f, 5, 2, 0, 0, 0);
                case BuildingType.FuelFactory: return new BuildingDefinition("Fuel Factory", "Converts 2 uranium + 1 ice into 1 fuel.", ResourceType.Fuel, 30f, 4, 6, 2, 2, 0);
                case BuildingType.MetalFactory: return new BuildingDefinition("Metal Factory", "Converts 2 stone into 1 metal.", ResourceType.Metal, 30f, 5, 8, 0, 0, 0);
                case BuildingType.RepairKitFactory: return new BuildingDefinition("Repair Kit Factory", "Converts 1 metal + 1 uranium into repair kits.", ResourceType.RepairKits, 30f, 4, 10, 2, 0, 2);
                case BuildingType.BoosterFactory: return new BuildingDefinition("Booster Factory", "Converts 1 fuel + 1 ice into speed boosters.", ResourceType.Boosters, 30f, 4, 10, 0, 3, 0);
                case BuildingType.Warehouse: return new BuildingDefinition("Warehouse", "Inventory capacity: 300 slots.", ResourceType.Stone, 1f, 0, 5, 0, 0, 0);
                case BuildingType.Market: return new BuildingDefinition("Market", "Sell resources and items for coins.", ResourceType.Coins, 1f, 0, 5, 0, 0, 0);
                default: return new BuildingDefinition("Empty Slot", "Choose a building to construct.", ResourceType.Stone, 1f, 0, 0, 0, 0, 0);
            }
        }

        public bool Build(int index, BuildingType type)
        {
            if (index < 0 || index >= Slots.Count) return false;
            BuildingSlot slot = Slots[index];
            if (slot.Building != null && slot.Building.Type != BuildingType.Empty) return false;

            BuildingDefinition def = Definition(type);
            ResourceWallet wallet = GameManager.Instance.Resources.Wallet;
            if (wallet.stone < def.stoneCost || wallet.uranium < def.uraniumCost || wallet.ice < def.iceCost || wallet.metal < def.metalCost) return false;
            wallet.stone -= def.stoneCost;
            wallet.uranium -= def.uraniumCost;
            wallet.ice -= def.iceCost;
            wallet.metal -= def.metalCost;

            StarAgeSaveData data = GameManager.Instance.Save.Data;
            data.buildings[index].type = type;
            data.buildings[index].timer = 0f;
            data.buildings[index].stored = 0;
            SpawnBuilding(slot, data.buildings[index]);
            GameManager.Instance.SaveGame();
            return true;
        }

        public void WriteSave()
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                if (Slots[i].Building != null) GameManager.Instance.Save.Data.buildings[i] = Slots[i].Building.Save;
            }
        }

        void BuildPlanet()
        {
            var surface = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            surface.name = "Rocky Lava Planet Surface";
            surface.transform.SetParent(root.transform);
            surface.transform.localScale = new Vector3(9.8f, 0.42f, 9.8f);
            surface.GetComponent<Renderer>().material = Mat(new Color(0.46f, 0.22f, 0.14f));

            var rim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rim.name = "Dark Basalt Rim";
            rim.transform.SetParent(root.transform);
            rim.transform.position = new Vector3(0f, 0.36f, 0f);
            rim.transform.localScale = new Vector3(10.25f, 0.08f, 10.25f);
            rim.GetComponent<Renderer>().material = Mat(new Color(0.18f, 0.11f, 0.09f));

            for (int i = 0; i < 24; i++)
            {
                var crack = GameObject.CreatePrimitive(PrimitiveType.Cube);
                crack.name = "Lava Crack";
                crack.transform.SetParent(root.transform);
                crack.transform.position = new Vector3(Random.Range(-7.5f, 7.5f), 0.52f, Random.Range(-7.5f, 7.5f));
                crack.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 180f), 0f);
                crack.transform.localScale = new Vector3(Random.Range(0.05f, 0.13f), 0.025f, Random.Range(1.0f, 3.1f));
                crack.GetComponent<Renderer>().material = Mat(new Color(1f, 0.38f, 0.08f), true);
            }

            for (int i = 0; i < 10; i++)
            {
                var crater = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                crater.name = "Crater";
                crater.transform.SetParent(root.transform);
                crater.transform.position = new Vector3(Random.Range(-7.3f, 7.3f), 0.55f, Random.Range(-7.3f, 7.3f));
                crater.transform.localScale = new Vector3(Random.Range(0.5f, 1.2f), 0.05f, Random.Range(0.5f, 1.2f));
                crater.GetComponent<Renderer>().material = Mat(new Color(0.08f, 0.06f, 0.055f));
            }

            for (int i = 0; i < 90; i++)
            {
                var star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                star.name = "Planet View Star";
                star.transform.SetParent(root.transform);
                star.transform.position = new Vector3(Random.Range(-34f, 34f), Random.Range(8f, 22f), Random.Range(-24f, 18f));
                star.transform.localScale = Vector3.one * Random.Range(0.025f, 0.075f);
                star.GetComponent<Renderer>().material = Mat(new Color(0.8f, 0.9f, 1f), true);
            }
        }

        void BuildSlots()
        {
            Slots.Clear();
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            for (int i = 0; i < 9; i++)
            {
                int x = i % 3 - 1;
                int z = i / 3 - 1;
                var baseObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                baseObject.name = $"Building Slot {i + 1}";
                baseObject.transform.SetParent(root.transform);
                baseObject.transform.position = new Vector3(x * 3.25f, 0.78f, z * 3.25f);
                baseObject.transform.localScale = new Vector3(0.92f, 0.1f, 0.92f);
                baseObject.GetComponent<Renderer>().material = Mat(new Color(0.08f, 0.38f, 0.52f), true);
                var slot = baseObject.AddComponent<BuildingSlot>();
                slot.Init(i);
                Slots.Add(slot);
                SpawnBuilding(slot, data.buildings[i]);
            }
        }

        void SpawnBuilding(BuildingSlot slot, BuildingSave save)
        {
            if (slot.Building != null) Destroy(slot.Building.gameObject);
            var visual = CreateBuildingVisual(save.type);
            visual.transform.SetParent(root.transform);
            visual.transform.position = slot.transform.position + Vector3.up * 0.65f;
            var building = visual.AddComponent<Building>();
            building.Init(save.type, save);
            slot.SetBuilding(building);
        }

        GameObject CreateBuildingVisual(BuildingType type)
        {
            var go = new GameObject(type.ToString());
            if (type == BuildingType.Empty)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.SetParent(go.transform);
                cube.transform.localPosition = Vector3.up * 0.04f;
                cube.transform.localScale = new Vector3(1.15f, 0.08f, 1.15f);
                cube.GetComponent<Renderer>().material = Mat(new Color(0.05f, 0.18f, 0.24f));
                return go;
            }

            PrimitiveType primitive = type == BuildingType.Warehouse ? PrimitiveType.Cube : PrimitiveType.Cylinder;
            var body = GameObject.CreatePrimitive(primitive);
            body.transform.SetParent(go.transform);
            body.transform.localScale = new Vector3(0.82f, 0.85f, 0.82f);
            body.GetComponent<Renderer>().material = Mat(ColorFor(type));

            var cap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cap.transform.SetParent(go.transform);
            cap.transform.localPosition = Vector3.up * 0.92f;
            cap.transform.localScale = new Vector3(0.42f, 0.2f, 0.42f);
            cap.GetComponent<Renderer>().material = Mat(ColorFor(type) * 1.2f, true);

            var antenna = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            antenna.transform.SetParent(go.transform);
            antenna.transform.localPosition = new Vector3(0.22f, 1.22f, 0.12f);
            antenna.transform.localScale = new Vector3(0.035f, 0.34f, 0.035f);
            antenna.GetComponent<Renderer>().material = Mat(new Color(0.72f, 0.78f, 0.82f));
            return go;
        }

        Color ColorFor(BuildingType type)
        {
            if (type == BuildingType.StoneQuarry) return new Color(0.48f, 0.48f, 0.44f);
            if (type == BuildingType.UraniumMine) return new Color(0.4f, 1f, 0.35f);
            if (type == BuildingType.IceWell) return new Color(0.45f, 0.85f, 1f);
            if (type == BuildingType.FuelFactory) return new Color(1f, 0.58f, 0.15f);
            if (type == BuildingType.MetalFactory) return new Color(0.65f, 0.7f, 0.76f);
            if (type == BuildingType.RepairKitFactory) return new Color(1f, 0.2f, 0.2f);
            if (type == BuildingType.BoosterFactory) return new Color(0.75f, 0.35f, 1f);
            if (type == BuildingType.Market) return new Color(0.95f, 0.85f, 0.35f);
            return new Color(0.35f, 0.45f, 0.55f);
        }

        Material Mat(Color color)
        {
            return RuntimeMaterial.Create(color);
        }

        Material Mat(Color color, bool emissive)
        {
            return RuntimeMaterial.Create(color, emissive);
        }
    }
}

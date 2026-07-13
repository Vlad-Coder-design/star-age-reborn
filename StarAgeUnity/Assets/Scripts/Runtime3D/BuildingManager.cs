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
                case BuildingType.StoneQuarry: return new BuildingDefinition("Stone Quarry", "Mines 1 stone every 15 min. Holds 5.", ResourceType.Stone, 15f, 5, 0, 0, 0, 0, 100);
                case BuildingType.UraniumMine: return new BuildingDefinition("Uranium Quarry", "Mines 1 uranium every 20 min. Holds 5. Military planets only.", ResourceType.Uranium, 20f, 5, 0, 0, 0, 0, 250);
                case BuildingType.IceWell: return new BuildingDefinition("Ice Quarry", "Extracts 1 ice every 5 min. Holds 5.", ResourceType.Ice, 5f, 5, 0, 0, 0, 0, 100);
                case BuildingType.FuelFactory: return new BuildingDefinition("Fuel Plant", "2 uranium + 1 ice -> 1 fuel every 30 min. Holds 4.", ResourceType.Fuel, 30f, 4, 0, 0, 0, 0, 500);
                case BuildingType.MetalFactory: return new BuildingDefinition("Metal Plant", "2 stone -> 1 metal every 30 min. Holds 5.", ResourceType.Metal, 30f, 5, 0, 0, 0, 0, 400);
                case BuildingType.RepairKitFactory: return new BuildingDefinition("Repair Kit Workshop", "1 metal + 1 uranium -> 1 repair kit (+20 HP in space) every 30 min.", ResourceType.RepairKits, 30f, 3, 0, 0, 0, 0, 800);
                case BuildingType.BoosterFactory: return new BuildingDefinition("Accelerator Workshop", "1 fuel + 1 ice -> 1 accelerator (+speed 30 s) every 30 min.", ResourceType.Boosters, 30f, 3, 0, 0, 0, 0, 800);
                case BuildingType.Warehouse: return new BuildingDefinition("Warehouse", "Stores collected resources. +300 capacity.", ResourceType.Stone, 1f, 0, 0, 0, 0, 0, 300);
                case BuildingType.Market: return new BuildingDefinition("Auction", "Sell goods for credits directly from the colony.", ResourceType.Coins, 1f, 0, 0, 0, 0, 0, 500);
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
            if (wallet.coins < def.coinCost) return false;
            if (wallet.stone < def.stoneCost || wallet.uranium < def.uraniumCost || wallet.ice < def.iceCost || wallet.metal < def.metalCost) return false;
            wallet.coins -= def.coinCost;
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

            for (int i = 0; i < 9; i++)
            {
                Vector2 p = RandomDisc(4.05f);
                float size = Random.Range(0.7f, 1.45f);
                AddPlanetDisc("Cooling Lava Edge", p, 0.532f, new Vector3(size * 1.35f, 0.015f, size * 0.82f), new Color(0.12f, 0.055f, 0.035f), false, Random.Range(0f, 180f));
                AddPlanetDisc("Molten Lava Pool", p, 0.558f, new Vector3(size * 1.1f, 0.018f, size * 0.65f), new Color(1f, 0.24f, 0.04f), true, Random.Range(0f, 180f));
            }

            for (int i = 0; i < 16; i++)
            {
                Vector2 p = RandomDisc(4.45f);
                float angle = Random.Range(0f, 180f);
                AddPlanetBox("Branching Lava Fissure", p, 0.565f, new Vector3(Random.Range(0.05f, 0.11f), 0.025f, Random.Range(0.55f, 1.25f)), new Color(1f, 0.38f, 0.08f), true, angle);

                if (Random.value > 0.35f)
                {
                    Vector2 branch = Vector2.ClampMagnitude(p + Random.insideUnitCircle * 0.35f, 4.55f);
                    AddPlanetBox("Small Lava Branch", branch, 0.57f, new Vector3(Random.Range(0.035f, 0.07f), 0.02f, Random.Range(0.28f, 0.65f)), new Color(1f, 0.52f, 0.11f), true, angle + Random.Range(35f, 70f));
                }
            }

            for (int i = 0; i < 14; i++)
            {
                Vector2 p = RandomDisc(4.35f);
                float size = Random.Range(0.45f, 1.05f);
                AddPlanetDisc("Raised Crater Rim", p, 0.565f, new Vector3(size * 1.28f, 0.035f, size * 1.05f), new Color(0.24f, 0.15f, 0.12f), false, Random.Range(0f, 180f));
                AddPlanetDisc("Shadowed Crater Bowl", p, 0.59f, new Vector3(size, 0.022f, size * 0.78f), new Color(0.055f, 0.045f, 0.045f), false, Random.Range(0f, 180f));
            }

            for (int i = 0; i < 26; i++)
            {
                Vector2 p = RandomDisc(4.5f);
                float size = Random.Range(0.16f, 0.42f);
                AddRock("Basalt Boulder", p, new Vector3(size * Random.Range(1.0f, 1.7f), size * Random.Range(0.55f, 1.0f), size * Random.Range(0.8f, 1.35f)), new Color(0.1f, 0.085f, 0.08f), Random.Range(0f, 180f));
            }

            for (int i = 0; i < 18; i++)
            {
                Vector2 p = RandomDisc(4.35f);
                AddPlanetDisc("Rust Mineral Patch", p, 0.575f, new Vector3(Random.Range(0.28f, 0.62f), 0.018f, Random.Range(0.18f, 0.42f)), new Color(0.64f, 0.31f, 0.16f), false, Random.Range(0f, 180f));
            }

            for (int i = 0; i < 12; i++)
            {
                Vector2 p = RandomDisc(4.25f);
                float height = Random.Range(0.18f, 0.42f);
                AddPlanetBox("Uranium Crystal", p, 0.74f, new Vector3(0.08f, height, 0.08f), new Color(0.35f, 1f, 0.42f), true, Random.Range(0f, 180f));
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

        void AddPlanetDisc(string name, Vector2 position, float height, Vector3 scale, Color color, bool emissive, float rotationY)
        {
            var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.name = name;
            disc.transform.SetParent(root.transform);
            disc.transform.position = new Vector3(position.x, height, position.y);
            disc.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
            disc.transform.localScale = scale;
            disc.GetComponent<Renderer>().material = Mat(color, emissive);
        }

        void AddPlanetBox(string name, Vector2 position, float height, Vector3 scale, Color color, bool emissive, float rotationY)
        {
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.SetParent(root.transform);
            box.transform.position = new Vector3(position.x, height, position.y);
            box.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
            box.transform.localScale = scale;
            box.GetComponent<Renderer>().material = Mat(color, emissive);
        }

        void AddRock(string name, Vector2 position, Vector3 scale, Color color, float rotationY)
        {
            var rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = name;
            rock.transform.SetParent(root.transform);
            rock.transform.position = new Vector3(position.x, 0.66f, position.y);
            rock.transform.rotation = Quaternion.Euler(Random.Range(-10f, 10f), rotationY, Random.Range(-10f, 10f));
            rock.transform.localScale = scale;
            rock.GetComponent<Renderer>().material = Mat(color);
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
                AddSlotBeacon(baseObject.transform);
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

            var foundation = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            foundation.transform.SetParent(go.transform);
            foundation.transform.localPosition = new Vector3(0f, -0.48f, 0f);
            foundation.transform.localScale = new Vector3(0.96f, 0.08f, 0.96f);
            foundation.GetComponent<Renderer>().material = Mat(new Color(0.05f, 0.09f, 0.11f));

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

            AddBuildingGlow(go.transform, ColorFor(type));
            return go;
        }

        void AddSlotBeacon(Transform parent)
        {
            Color ringColor = new Color(0.18f, 0.95f, 1f, 0.8f);
            for (int i = 0; i < 20; i += 2)
            {
                float startAngle = i / 20f * Mathf.PI * 2f;
                float endAngle = (i + 1f) / 20f * Mathf.PI * 2f;
                Vector3 start = new Vector3(Mathf.Cos(startAngle) * 0.92f, 0.7f, Mathf.Sin(startAngle) * 0.92f);
                Vector3 end = new Vector3(Mathf.Cos(endAngle) * 0.92f, 0.7f, Mathf.Sin(endAngle) * 0.92f);
                var segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segment.name = "Slot Hologram Segment";
                segment.transform.SetParent(parent);
                segment.transform.localPosition = (start + end) * 0.5f;
                Vector3 delta = end - start;
                segment.transform.localRotation = Quaternion.LookRotation(delta.normalized, Vector3.up);
                segment.transform.localScale = new Vector3(0.035f, 0.025f, delta.magnitude);
                segment.GetComponent<Renderer>().material = Mat(ringColor, true);
            }

            for (int i = 0; i < 4; i++)
            {
                var tick = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tick.name = "Slot Hologram Tick";
                tick.transform.SetParent(parent);
                float angle = i * 90f;
                Vector3 offset = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0.75f, 1.12f);
                tick.transform.localPosition = offset;
                tick.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
                tick.transform.localScale = new Vector3(0.08f, 0.04f, 0.32f);
                tick.GetComponent<Renderer>().material = Mat(new Color(0.3f, 1f, 0.9f), true);
            }
        }

        void AddBuildingGlow(Transform parent, Color color)
        {
            var strip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            strip.name = "Building Status Light";
            strip.transform.SetParent(parent);
            strip.transform.localPosition = new Vector3(0f, 0.08f, 0.43f);
            strip.transform.localScale = new Vector3(0.52f, 0.05f, 0.05f);
            strip.GetComponent<Renderer>().material = Mat(new Color(color.r, color.g, color.b, 0.88f), true);

            var light = new GameObject("Building Local Glow").AddComponent<Light>();
            light.transform.SetParent(parent);
            light.transform.localPosition = new Vector3(0f, 0.65f, 0.3f);
            light.type = LightType.Point;
            light.color = color;
            light.intensity = 0.75f;
            light.range = 2.4f;
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

        Vector2 RandomDisc(float radius)
        {
            Vector2 point = Random.insideUnitCircle * radius;
            return point;
        }
    }
}

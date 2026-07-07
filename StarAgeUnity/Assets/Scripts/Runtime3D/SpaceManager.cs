using System.Collections.Generic;
using UnityEngine;

namespace StarAge3D
{
    public class SpaceManager : MonoBehaviour
    {
        public ShipController PlayerShip { get; private set; }

        GameObject root;
        readonly List<GameObject> spawned = new List<GameObject>();
        readonly List<Transform> mapPlanets = new List<Transform>();
        readonly List<Transform> mapEnemies = new List<Transform>();
        readonly List<Transform> mapResources = new List<Transform>();

        public IReadOnlyList<Transform> MapPlanets => mapPlanets;
        public IReadOnlyList<Transform> MapEnemies => mapEnemies;
        public IReadOnlyList<Transform> MapResources => mapResources;

        public void Init()
        {
            root = new GameObject("Space Mode");
            BuildSpace();
            SetActive(false);
        }

        void LateUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.Mode != GameMode.Space || PlayerShip == null) return;
            Camera cam = GameManager.Instance.MainCamera;
            Vector3 target = PlayerShip.transform.position + new Vector3(0f, 62f, 30f);
            cam.transform.position = Vector3.Lerp(cam.transform.position, target, 0.08f);
            cam.transform.LookAt(PlayerShip.transform.position);
        }

        public void SetActive(bool active)
        {
            if (root == null) return;
            root.SetActive(active);
            if (active && PlayerShip == null) RebuildPlayerShip();
        }

        public int CargoCapacity()
        {
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            return ShipStats.For(data.shipId).cargo + data.cargoModules * 20;
        }

        public int CargoUsed()
        {
            ResourceWallet wallet = GameManager.Instance.Resources.Wallet;
            return wallet.stone + wallet.ice + wallet.uranium + wallet.metal + wallet.fuel;
        }

        public bool AddCargo(ResourceType type, int amount)
        {
            if (CargoUsed() + amount > CargoCapacity()) return false;
            GameManager.Instance.Resources.Add(type, amount);
            if (type == ResourceType.Stone) GameManager.Instance.Quests.AddProgress(QuestType.MineStone, amount);
            if (type == ResourceType.Ice) GameManager.Instance.Quests.AddProgress(QuestType.CollectIce, amount);
            return true;
        }

        public void RebuildPlayerShip()
        {
            if (PlayerShip != null) Destroy(PlayerShip.gameObject);
            var ship = CreateShipModel("Player Ship", new Color(0.15f, 0.65f, 1f), false);
            ship.transform.SetParent(root.transform);
            ship.transform.position = new Vector3(0f, 0f, 36f);
            PlayerShip = ship.AddComponent<ShipController>();
            PlayerShip.Init(false);
            AddNameplate(ship.transform, "You", PlayerShip, new Color(0.45f, 1f, 1f));
        }

        public void SpawnProjectile(ShipController owner, Vector3 origin, Vector3 direction, int damage, Color color)
        {
            var bolt = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bolt.name = owner.IsEnemy ? "Pirate Laser" : "Player Laser";
            bolt.transform.position = origin;
            bolt.transform.localScale = Vector3.one * 0.18f;
            bolt.GetComponent<Renderer>().material = Mat(color);
            var collider = bolt.GetComponent<SphereCollider>();
            collider.isTrigger = true;
            var rb = bolt.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            var projectile = bolt.AddComponent<Projectile>();
            projectile.Init(owner, direction, damage);
            spawned.Add(bolt);
        }

        public void SpawnShipExplosion(Vector3 position, bool pirate)
        {
            var explosion = new GameObject(pirate ? "Pirate Ship Explosion" : "Player Ship Explosion");
            explosion.transform.SetParent(root.transform);
            explosion.transform.position = position;
            explosion.AddComponent<ExplosionEffect>().Init(pirate);
            spawned.Add(explosion);
        }

        public void PirateDestroyed(EnemyAI pirate)
        {
            SpawnLootBurst(pirate.transform.position);
            ResourceWallet wallet = GameManager.Instance.Resources.Wallet;
            wallet.coins += Random.Range(15, 45);
            wallet.uranium += Random.Range(0, 2);
            wallet.metal += Random.Range(0, 2);
            wallet.fuel += Random.Range(0, 2);
            GameManager.Instance.Quests.AddProgress(QuestType.DestroyPirates, 1);
            GameManager.Instance.SaveGame();
        }

        void BuildSpace()
        {
            BuildNebulaBackdrop();
            BuildStarfield();
            BuildSolarSystem();
            RebuildPlayerShip();
            for (int i = 0; i < 18; i++) SpawnMiningObject(i < 12);
            for (int i = 0; i < 5; i++) SpawnPirate();
            for (int i = 0; i < 4; i++) SpawnNpcMiner();
        }

        void BuildNebulaBackdrop()
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Deep Space Nebula Floor";
            floor.transform.SetParent(root.transform);
            floor.transform.position = new Vector3(0f, -14f, 0f);
            floor.transform.localScale = new Vector3(180f, 1f, 180f);
            floor.GetComponent<Renderer>().material = RuntimeMaterial.Create(new Color(0.006f, 0.014f, 0.035f));

            Color[] colors =
            {
                new Color(0.008f, 0.055f, 0.095f),
                new Color(0.055f, 0.018f, 0.085f),
                new Color(0.035f, 0.055f, 0.11f),
                new Color(0.07f, 0.028f, 0.03f)
            };

            for (int i = 0; i < 12; i++)
            {
                var cloud = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                cloud.name = "Soft Nebula Patch";
                cloud.transform.SetParent(root.transform);
                cloud.transform.position = new Vector3(Random.Range(-150f, 150f), -13.6f, Random.Range(-150f, 150f));
                cloud.transform.localScale = new Vector3(Random.Range(16f, 34f), 0.04f, Random.Range(12f, 28f));
                cloud.GetComponent<Renderer>().material = RuntimeMaterial.Create(colors[Random.Range(0, colors.Length)], true);
            }
        }

        void BuildSolarSystem()
        {
            var sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sun.name = "Orion Star";
            sun.transform.SetParent(root.transform);
            sun.transform.position = Vector3.zero;
            sun.transform.localScale = Vector3.one * 7.5f;
            sun.GetComponent<Renderer>().material = RuntimeMaterial.Create(new Color(1f, 0.86f, 0.18f), true);

            var sunHalo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sunHalo.name = "Orion Star Glow";
            sunHalo.transform.SetParent(root.transform);
            sunHalo.transform.position = Vector3.zero;
            sunHalo.transform.localScale = Vector3.one * 11f;
            sunHalo.GetComponent<Renderer>().material = RuntimeMaterial.Create(new Color(1f, 0.45f, 0.08f), true);

            var light = new GameObject("Orion Star Light").AddComponent<Light>();
            light.transform.SetParent(root.transform);
            light.transform.position = new Vector3(0f, 8f, 0f);
            light.type = LightType.Point;
            light.color = new Color(1f, 0.72f, 0.28f);
            light.intensity = 5f;
            light.range = 95f;

            AddOrbitRing(24f, new Color(0.12f, 0.42f, 0.82f));
            AddOrbitRing(42f, new Color(0.12f, 0.42f, 0.82f));
            AddOrbitRing(62f, new Color(0.12f, 0.42f, 0.82f));

            AddSystemPlanet("Aurelia", new Vector3(20f, 0f, 13f), 2.8f, new Color(0.22f, 0.56f, 1f), true);
            AddSystemPlanet("Vulcan Reach", new Vector3(-35f, 0f, -22f), 3.5f, new Color(0.85f, 0.3f, 0.1f), true);
            AddSystemPlanet("Ionus", new Vector3(53f, 0f, -31f), 2.4f, new Color(0.4f, 0.9f, 0.55f), false);
        }

        void AddOrbitRing(float radius, Color color)
        {
            var ringObject = new GameObject("System Orbit Ring");
            ringObject.transform.SetParent(root.transform);
            var line = ringObject.AddComponent<LineRenderer>();
            line.loop = true;
            line.positionCount = 96;
            line.useWorldSpace = false;
            line.widthMultiplier = 0.08f;
            line.material = RuntimeMaterial.Create(color);
            for (int i = 0; i < line.positionCount; i++)
            {
                float angle = i / (float)line.positionCount * Mathf.PI * 2f;
                line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, -0.08f, Mathf.Sin(angle) * radius));
            }
        }

        void AddSystemPlanet(string label, Vector3 position, float radius, Color color, bool atmosphere)
        {
            var planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planet.name = label;
            planet.transform.SetParent(root.transform);
            planet.transform.position = position;
            planet.transform.localScale = Vector3.one * radius;
            planet.GetComponent<Renderer>().material = RuntimeMaterial.Create(color);
            mapPlanets.Add(planet.transform);

            if (atmosphere)
            {
                var glow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                glow.name = label + " Atmosphere";
                glow.transform.SetParent(root.transform);
                glow.transform.position = position;
                glow.transform.localScale = Vector3.one * radius * 1.28f;
                glow.GetComponent<Renderer>().material = RuntimeMaterial.Create(color * 1.25f, true);
            }

            AddSpaceLabel(planet.transform, label, new Color(0.7f, 0.9f, 1f), radius * 0.9f + 2f);
        }

        void BuildStarfield()
        {
            for (int i = 0; i < 420; i++)
            {
                var star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                star.name = "Star";
                star.transform.SetParent(root.transform);
                star.transform.position = new Vector3(Random.Range(-180f, 180f), Random.Range(-8f, 70f), Random.Range(-180f, 180f));
                star.transform.localScale = Vector3.one * Random.Range(0.035f, 0.16f);
                Color tint = Random.value > 0.82f ? new Color(0.55f, 0.82f, 1f) : new Color(0.86f, 0.91f, 1f);
                star.GetComponent<Renderer>().material = Mat(tint);
            }
        }

        void SpawnMiningObject(bool asteroid)
        {
            var rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = asteroid ? "Asteroid" : "Comet";
            rock.transform.SetParent(root.transform);
            rock.transform.position = new Vector3(Random.Range(-42f, 42f), Random.Range(-1f, 4f), Random.Range(-42f, 42f));
            float scale = Random.Range(0.9f, 1.8f);
            rock.transform.localScale = asteroid ? new Vector3(scale * 1.25f, scale * 0.82f, scale) : Vector3.one * scale;
            rock.GetComponent<Renderer>().material = Mat(asteroid ? new Color(0.42f, 0.38f, 0.34f) : new Color(0.45f, 0.9f, 1f));
            rock.AddComponent<MiningObject>().Init(asteroid ? ResourceType.Stone : ResourceType.Ice);
            spawned.Add(rock);
            mapResources.Add(rock.transform);
        }

        void SpawnPirate()
        {
            var ship = CreateShipModel("Pirate Ship", new Color(1f, 0.12f, 0.08f), true);
            ship.transform.SetParent(root.transform);
            ship.transform.position = new Vector3(Random.Range(-34f, 34f), 0f, Random.Range(18f, 48f));
            var controller = ship.AddComponent<ShipController>();
            controller.Init(true);
            AddNameplate(ship.transform, "Pirate", controller, new Color(1f, 0.32f, 0.26f));
            ship.AddComponent<EnemyAI>().Init(controller);
            spawned.Add(ship);
            mapEnemies.Add(ship.transform);
        }

        void SpawnNpcMiner()
        {
            var ship = CreateShipModel("Mining NPC", new Color(0.95f, 0.8f, 0.2f), false);
            ship.transform.SetParent(root.transform);
            ship.transform.position = new Vector3(Random.Range(-35f, 35f), 0f, Random.Range(-40f, 35f));
            ship.AddComponent<NpcMiner>();
            AddSpaceLabel(ship.transform, "Miner", new Color(1f, 0.86f, 0.28f), 2.4f);
            spawned.Add(ship);
        }

        void SpawnLootBurst(Vector3 position)
        {
            for (int i = 0; i < 5; i++)
            {
                var loot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                loot.name = "Floating Loot Crystal";
                loot.transform.SetParent(root.transform);
                loot.transform.position = position + Random.insideUnitSphere * 1.5f;
                loot.transform.localScale = Vector3.one * Random.Range(0.32f, 0.55f);
                loot.GetComponent<Renderer>().material = RuntimeMaterial.Create(i % 2 == 0 ? new Color(1f, 0.86f, 0.25f) : new Color(0.25f, 1f, 0.75f), true);
                loot.AddComponent<SpaceLootVisual>();
                spawned.Add(loot);
            }
        }

        public GameObject CreateShipModel(string name, Color color, bool pirate)
        {
            var rootShip = new GameObject(name);
            rootShip.transform.localScale = pirate ? Vector3.one * 0.9f : Vector3.one;

            Color hull = pirate ? new Color(0.72f, 0.08f, 0.08f) : new Color(0.56f, 0.68f, 0.78f);
            Color darkHull = pirate ? new Color(0.18f, 0.04f, 0.04f) : new Color(0.09f, 0.13f, 0.18f);
            Color trim = pirate ? new Color(1f, 0.24f, 0.08f) : new Color(0.1f, 0.78f, 1f);

            AddBox(rootShip.transform, "Main Armor Hull", new Vector3(0f, 0f, -0.18f), new Vector3(0.78f, 0.28f, 1.75f), hull, Quaternion.identity);
            AddBox(rootShip.transform, "Lower Keel", new Vector3(0f, -0.16f, -0.32f), new Vector3(0.48f, 0.16f, 1.35f), darkHull, Quaternion.identity);

            var nose = AddBox(rootShip.transform, "Angular Nose", new Vector3(0f, 0f, 1.02f), new Vector3(0.54f, 0.22f, 1.25f), hull * 1.15f, Quaternion.Euler(0f, 0f, 0f));
            nose.transform.localScale = new Vector3(0.48f, 0.2f, 1.15f);

            AddBox(rootShip.transform, "Nose Tip", new Vector3(0f, -0.01f, 1.78f), new Vector3(0.24f, 0.16f, 0.46f), darkHull, Quaternion.identity);

            var cockpit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cockpit.name = "Glass Cockpit";
            cockpit.transform.SetParent(rootShip.transform);
            cockpit.transform.localPosition = new Vector3(0f, 0.22f, 0.45f);
            cockpit.transform.localScale = new Vector3(0.36f, 0.13f, 0.52f);
            cockpit.GetComponent<Renderer>().material = RuntimeMaterial.Create(new Color(0.25f, 0.9f, 1f), true);

            AddBox(rootShip.transform, "Cockpit Frame", new Vector3(0f, 0.255f, 0.12f), new Vector3(0.42f, 0.035f, 0.08f), darkHull, Quaternion.identity);
            AddBox(rootShip.transform, "Center Glow Spine", new Vector3(0f, 0.16f, -0.45f), new Vector3(0.07f, 0.035f, 1.45f), trim, Quaternion.identity, true);

            for (int side = -1; side <= 1; side += 2)
            {
                AddBox(rootShip.transform, "Swept Main Wing", new Vector3(side * 0.82f, -0.05f, -0.25f), new Vector3(1.25f, 0.08f, 0.48f), hull * 0.86f, Quaternion.Euler(0f, side * -18f, side * 7f));
                AddBox(rootShip.transform, "Outer Wing Blade", new Vector3(side * 1.42f, -0.07f, -0.42f), new Vector3(0.9f, 0.055f, 0.22f), darkHull, Quaternion.Euler(0f, side * -24f, side * 4f));
                AddBox(rootShip.transform, "Wing Glow Strip", new Vector3(side * 1.18f, 0.005f, -0.23f), new Vector3(0.72f, 0.018f, 0.045f), trim, Quaternion.Euler(0f, side * -18f, 0f), true);

                AddBox(rootShip.transform, "Side Armor Intake", new Vector3(side * 0.56f, 0.02f, -0.9f), new Vector3(0.28f, 0.22f, 0.58f), darkHull, Quaternion.identity);
                AddBox(rootShip.transform, "Rear Engine Block", new Vector3(side * 0.42f, -0.04f, -1.34f), new Vector3(0.34f, 0.28f, 0.44f), darkHull, Quaternion.identity);

                var engine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                engine.name = "Round Thruster";
                engine.transform.SetParent(rootShip.transform);
                engine.transform.localPosition = new Vector3(side * 0.42f, -0.04f, -1.62f);
                engine.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                engine.transform.localScale = new Vector3(0.2f, 0.18f, 0.2f);
                engine.GetComponent<Renderer>().material = RuntimeMaterial.Create(trim, true);

                AddBox(rootShip.transform, "Vertical Tail Fin", new Vector3(side * 0.44f, 0.36f, -1.06f), new Vector3(0.14f, 0.62f, 0.34f), hull * 0.72f, Quaternion.Euler(0f, 0f, side * -8f));
                AddBox(rootShip.transform, "Laser Cannon", new Vector3(side * 0.42f, -0.12f, 1.35f), new Vector3(0.07f, 0.07f, 0.74f), new Color(0.02f, 0.025f, 0.03f), Quaternion.identity);
                AddBox(rootShip.transform, "Laser Muzzle Glow", new Vector3(side * 0.42f, -0.12f, 1.75f), new Vector3(0.09f, 0.09f, 0.08f), trim, Quaternion.identity, true);
            }

            AddBox(rootShip.transform, "Top Tail Fin", new Vector3(0f, 0.48f, -1.0f), new Vector3(0.16f, 0.7f, 0.36f), hull * 0.68f, Quaternion.identity);
            AddBox(rootShip.transform, "Port Hull Panel", new Vector3(-0.22f, 0.17f, -0.1f), new Vector3(0.2f, 0.025f, 0.86f), darkHull, Quaternion.Euler(0f, -6f, 0f));
            AddBox(rootShip.transform, "Starboard Hull Panel", new Vector3(0.22f, 0.17f, -0.1f), new Vector3(0.2f, 0.025f, 0.86f), darkHull, Quaternion.Euler(0f, 6f, 0f));
            AddBox(rootShip.transform, "Rear Reactor Glow", new Vector3(0f, 0.03f, -1.72f), new Vector3(0.22f, 0.18f, 0.08f), trim, Quaternion.identity, true);

            var collider = rootShip.AddComponent<SphereCollider>();
            collider.radius = 1.05f;
            collider.isTrigger = true;
            return rootShip;
        }

        void AddNameplate(Transform parent, string label, ShipController controller, Color color)
        {
            var text = AddSpaceLabel(parent, label, color, 2.6f);
            text.AddComponent<ShipNameplate>().Init(label, controller, color);
        }

        GameObject AddSpaceLabel(Transform parent, string label, Color color, float height)
        {
            var labelObject = new GameObject(label + " Label");
            labelObject.transform.SetParent(parent);
            labelObject.transform.localPosition = new Vector3(0f, height, 0f);
            var mesh = labelObject.AddComponent<TextMesh>();
            mesh.text = label;
            mesh.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            mesh.fontSize = 38;
            mesh.characterSize = 0.08f;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.color = color;
            labelObject.AddComponent<SpaceBillboard>();
            return labelObject;
        }

        GameObject AddBox(Transform parent, string name, Vector3 localPosition, Vector3 scale, Color color, Quaternion rotation, bool emissive = false)
        {
            var part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            part.transform.SetParent(parent);
            part.transform.localPosition = localPosition;
            part.transform.localRotation = rotation;
            part.transform.localScale = scale;
            part.GetComponent<Renderer>().material = RuntimeMaterial.Create(color, emissive);
            return part;
        }

        Material Mat(Color color)
        {
            return RuntimeMaterial.Create(color, color.maxColorComponent > 0.9f);
        }
    }
}

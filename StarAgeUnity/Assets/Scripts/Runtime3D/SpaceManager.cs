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
            Vector3 target = PlayerShip.transform.position + new Vector3(0f, 42f, 24f);
            cam.transform.position = Vector3.Lerp(cam.transform.position, target, 0.12f);
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
            var bolt = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bolt.name = owner.IsEnemy ? "Pirate Laser" : "Player Laser";
            bolt.transform.position = origin;
            bolt.transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
            bolt.transform.localScale = new Vector3(0.13f, 0.48f, 0.13f);
            bolt.GetComponent<Renderer>().material = Mat(color);
            var light = bolt.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.range = 4.5f;
            light.intensity = 2.2f;
            var trail = bolt.AddComponent<TrailRenderer>();
            trail.time = 0.18f;
            trail.startWidth = 0.22f;
            trail.endWidth = 0.02f;
            trail.material = RuntimeMaterial.Create(new Color(color.r, color.g, color.b, 0.62f), true);
            var collider = bolt.GetComponent<SphereCollider>();
            if (collider != null) collider.isTrigger = true;
            var capsule = bolt.GetComponent<CapsuleCollider>();
            if (capsule != null) capsule.isTrigger = true;
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
            GameManager.Instance.AddXp(25);
            GameManager.Instance.SaveGame();
        }

        void BuildSpace()
        {
            BuildNebulaBackdrop();
            BuildStarfield();
            BuildWarpLanes();
            BuildSolarSystem();
            RebuildPlayerShip();
            SpawnMiningObjectAt(new Vector3(-6f, 0f, 33f), true, 2.1f);
            SpawnMiningObjectAt(new Vector3(11f, 0f, 29f), false, 1.5f);
            for (int i = 0; i < 30; i++) SpawnMiningObject(i < 20);
            for (int i = 0; i < 8; i++) SpawnPirate();
            for (int i = 0; i < 8; i++) SpawnNpcMiner();
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

            for (int i = 0; i < 18; i++)
            {
                var veil = GameObject.CreatePrimitive(PrimitiveType.Cube);
                veil.name = "Volumetric Nebula Veil";
                veil.transform.SetParent(root.transform);
                veil.transform.position = new Vector3(Random.Range(-135f, 135f), Random.Range(7f, 36f), Random.Range(-135f, 135f));
                veil.transform.rotation = Quaternion.Euler(Random.Range(-8f, 8f), Random.Range(0f, 360f), Random.Range(-8f, 8f));
                veil.transform.localScale = new Vector3(Random.Range(12f, 28f), 0.035f, Random.Range(3f, 9f));
                Color tint = colors[Random.Range(0, colors.Length)];
                veil.GetComponent<Renderer>().material = RuntimeMaterial.Create(new Color(tint.r, tint.g, tint.b, 0.28f), true);
            }
        }

        void BuildSolarSystem()
        {
            var sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sun.name = "Fomen Star";
            sun.transform.SetParent(root.transform);
            sun.transform.position = new Vector3(0f, 0f, 150f);
            sun.transform.localScale = Vector3.one * 6f;
            sun.GetComponent<Renderer>().material = RuntimeMaterial.Create(new Color(1f, 0.86f, 0.18f), true);

            var sunHalo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sunHalo.name = "Fomen Star Glow";
            sunHalo.transform.SetParent(root.transform);
            sunHalo.transform.position = sun.transform.position;
            sunHalo.transform.localScale = Vector3.one * 9f;
            sunHalo.GetComponent<Renderer>().material = RuntimeMaterial.Create(new Color(1f, 0.45f, 0.08f), true);

            var light = new GameObject("Fomen Star Light").AddComponent<Light>();
            light.transform.SetParent(root.transform);
            light.transform.position = new Vector3(0f, 12f, 75f);
            light.type = LightType.Point;
            light.color = new Color(1f, 0.72f, 0.28f);
            light.intensity = 5f;
            light.range = 95f;

            AddOrbitRing(118f, new Color(0.2f, 0.55f, 1f, 0.38f));
            AddOrbitRing(142f, new Color(0.7f, 0.35f, 1f, 0.28f));
            AddOrbitRing(168f, new Color(0.25f, 0.95f, 1f, 0.24f));
            AddSystemPlanet("Novara", new Vector3(-28f, 0f, 118f), 3.5f, new Color(0.85f, 0.32f, 0.12f), true);
            AddSystemPlanet("Veles", new Vector3(34f, 0f, 134f), 3.2f, new Color(0.45f, 0.85f, 0.44f), true);
            AddSystemPlanet("Kryos", new Vector3(64f, 0f, 164f), 2.7f, new Color(0.42f, 0.84f, 1f), false);
        }

        void BuildWarpLanes()
        {
            AddWarpLane(new Vector3(-72f, 1f, -42f), new Vector3(92f, 1.5f, 58f), new Color(0.1f, 0.72f, 1f, 0.42f));
            AddWarpLane(new Vector3(-88f, 2f, 72f), new Vector3(76f, 1f, -62f), new Color(0.72f, 0.3f, 1f, 0.28f));
            AddWarpLane(new Vector3(-36f, 0.6f, 104f), new Vector3(64f, 0.8f, 148f), new Color(0.35f, 1f, 0.72f, 0.34f));
        }

        void AddWarpLane(Vector3 a, Vector3 b, Color color)
        {
            var lane = new GameObject("Luminous Warp Lane");
            lane.transform.SetParent(root.transform);
            var line = lane.AddComponent<LineRenderer>();
            line.positionCount = 18;
            line.useWorldSpace = true;
            line.widthMultiplier = 0.18f;
            line.material = RuntimeMaterial.Create(color, true);
            Vector3 side = Vector3.Cross((b - a).normalized, Vector3.up).normalized;
            for (int i = 0; i < line.positionCount; i++)
            {
                float t = i / (float)(line.positionCount - 1);
                Vector3 p = Vector3.Lerp(a, b, t);
                p += side * Mathf.Sin(t * Mathf.PI * 4f) * 1.2f;
                p.y += Mathf.Sin(t * Mathf.PI) * 3.5f;
                line.SetPosition(i, p);
            }
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

            AddPlanetRing(label, position, radius, color);

            AddSpaceLabel(planet.transform, label, new Color(0.7f, 0.9f, 1f), radius * 0.9f + 2f);
        }

        void AddPlanetRing(string label, Vector3 position, float radius, Color color)
        {
            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = label + " Orbital Disc";
            ring.transform.SetParent(root.transform);
            ring.transform.position = position;
            ring.transform.rotation = Quaternion.Euler(8f, 0f, 18f);
            ring.transform.localScale = new Vector3(radius * 1.55f, 0.012f, radius * 1.55f);
            ring.GetComponent<Renderer>().material = RuntimeMaterial.Create(new Color(color.r, color.g, color.b, 0.18f), true);
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

            for (int i = 0; i < 36; i++)
            {
                var haze = GameObject.CreatePrimitive(PrimitiveType.Cube);
                haze.name = "Soft Star Smear";
                haze.transform.SetParent(root.transform);
                haze.transform.position = new Vector3(Random.Range(-160f, 160f), Random.Range(10f, 64f), Random.Range(-160f, 160f));
                float scale = Random.Range(0.35f, 0.9f);
                haze.transform.localScale = new Vector3(scale * Random.Range(1.4f, 2.8f), 0.015f, scale);
                haze.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 180f), 0f);
                haze.GetComponent<Renderer>().material = RuntimeMaterial.Create(new Color(0.58f, 0.8f, 1f, 0.34f), true);
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

        void SpawnMiningObjectAt(Vector3 position, bool asteroid, float scale)
        {
            var rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = asteroid ? "Featured Asteroid" : "Featured Comet";
            rock.transform.SetParent(root.transform);
            rock.transform.position = position;
            rock.transform.localScale = asteroid ? new Vector3(scale * 1.25f, scale * 0.82f, scale) : Vector3.one * scale;
            rock.transform.rotation = Quaternion.Euler(Random.Range(0f, 40f), Random.Range(0f, 360f), Random.Range(0f, 40f));
            rock.GetComponent<Renderer>().material = Mat(asteroid ? new Color(0.48f, 0.52f, 0.58f) : new Color(0.45f, 0.9f, 1f));
            rock.AddComponent<MiningObject>().Init(asteroid ? ResourceType.Stone : ResourceType.Ice);
            spawned.Add(rock);
            mapResources.Add(rock.transform);
        }

        void SpawnPirate()
        {
            var ship = CreateShipModel("Pirate Ship", new Color(1f, 0.12f, 0.08f), true);
            ship.transform.SetParent(root.transform);
            ship.transform.position = new Vector3(Random.Range(-42f, 42f), 0f, Random.Range(10f, 58f));
            var controller = ship.AddComponent<ShipController>();
            controller.Init(true);
            AddNameplate(ship.transform, PirateName(), controller, new Color(1f, 0.32f, 0.26f));
            ship.AddComponent<EnemyAI>().Init(controller);
            spawned.Add(ship);
            mapEnemies.Add(ship.transform);
        }

        void SpawnNpcMiner()
        {
            var ship = CreateShipModel("Mining NPC", new Color(0.95f, 0.8f, 0.2f), false);
            ship.transform.SetParent(root.transform);
            ship.transform.position = new Vector3(Random.Range(-48f, 48f), 0f, Random.Range(-48f, 42f));
            ship.AddComponent<NpcMiner>();
            AddSpaceLabel(ship.transform, MinerName(), new Color(1f, 0.86f, 0.28f), 2.4f);
            spawned.Add(ship);
        }

        string PirateName()
        {
            string[] names = { "Dart Predator", "UltraDark", "Stargalax", "Red Fang", "Void Reaper", "Kessler", "Black Comet", "Iron Wolf" };
            return names[Random.Range(0, names.Length)];
        }

        string MinerName()
        {
            string[] names = { "Ore Hauler", "Dusty", "Prospector", "Rock Biter", "Deep Core" };
            return names[Random.Range(0, names.Length)];
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
                AddBox(rootShip.transform, "Forward Canard Wing", new Vector3(side * 0.58f, -0.03f, 0.82f), new Vector3(0.68f, 0.055f, 0.26f), hull * 0.92f, Quaternion.Euler(0f, side * -28f, side * 5f));
                AddBox(rootShip.transform, "Outer Wing Blade", new Vector3(side * 1.42f, -0.07f, -0.42f), new Vector3(0.9f, 0.055f, 0.22f), darkHull, Quaternion.Euler(0f, side * -24f, side * 4f));
                AddBox(rootShip.transform, "Wing Glow Strip", new Vector3(side * 1.18f, 0.005f, -0.23f), new Vector3(0.72f, 0.018f, 0.045f), trim, Quaternion.Euler(0f, side * -18f, 0f), true);
                AddBox(rootShip.transform, "Canard Glow Strip", new Vector3(side * 0.58f, 0.015f, 0.86f), new Vector3(0.42f, 0.014f, 0.035f), trim, Quaternion.Euler(0f, side * -28f, 0f), true);

                AddBox(rootShip.transform, "Side Armor Intake", new Vector3(side * 0.56f, 0.02f, -0.9f), new Vector3(0.28f, 0.22f, 0.58f), darkHull, Quaternion.identity);
                AddBox(rootShip.transform, "Rear Engine Block", new Vector3(side * 0.42f, -0.04f, -1.34f), new Vector3(0.34f, 0.28f, 0.44f), darkHull, Quaternion.identity);

                var engine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                engine.name = "Round Thruster";
                engine.transform.SetParent(rootShip.transform);
                engine.transform.localPosition = new Vector3(side * 0.42f, -0.04f, -1.62f);
                engine.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                engine.transform.localScale = new Vector3(0.2f, 0.18f, 0.2f);
                engine.GetComponent<Renderer>().material = RuntimeMaterial.Create(trim, true);
                AddEngineFlame(rootShip.transform, side, trim);

                AddBox(rootShip.transform, "Vertical Tail Fin", new Vector3(side * 0.44f, 0.36f, -1.06f), new Vector3(0.14f, 0.62f, 0.34f), hull * 0.72f, Quaternion.Euler(0f, 0f, side * -8f));
                AddBox(rootShip.transform, "Laser Cannon", new Vector3(side * 0.42f, -0.12f, 1.35f), new Vector3(0.07f, 0.07f, 0.74f), new Color(0.02f, 0.025f, 0.03f), Quaternion.identity);
                AddBox(rootShip.transform, "Laser Muzzle Glow", new Vector3(side * 0.42f, -0.12f, 1.75f), new Vector3(0.09f, 0.09f, 0.08f), trim, Quaternion.identity, true);
            }

            AddBox(rootShip.transform, "Top Tail Fin", new Vector3(0f, 0.48f, -1.0f), new Vector3(0.16f, 0.7f, 0.36f), hull * 0.68f, Quaternion.identity);
            AddBox(rootShip.transform, "Port Hull Panel", new Vector3(-0.22f, 0.17f, -0.1f), new Vector3(0.2f, 0.025f, 0.86f), darkHull, Quaternion.Euler(0f, -6f, 0f));
            AddBox(rootShip.transform, "Starboard Hull Panel", new Vector3(0.22f, 0.17f, -0.1f), new Vector3(0.2f, 0.025f, 0.86f), darkHull, Quaternion.Euler(0f, 6f, 0f));
            AddBox(rootShip.transform, "Rear Reactor Glow", new Vector3(0f, 0.03f, -1.72f), new Vector3(0.22f, 0.18f, 0.08f), trim, Quaternion.identity, true);
            AddBox(rootShip.transform, "Ventral Sensor Blade", new Vector3(0f, -0.38f, 0.12f), new Vector3(0.08f, 0.5f, 0.42f), darkHull, Quaternion.Euler(0f, 0f, 0f));
            AddShipShieldOutline(rootShip.transform, trim);

            var collider = rootShip.AddComponent<SphereCollider>();
            collider.radius = 1.05f;
            collider.isTrigger = true;
            return rootShip;
        }

        void AddEngineFlame(Transform parent, int side, Color trim)
        {
            var flame = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            flame.name = "Engine Plasma Flame";
            flame.transform.SetParent(parent);
            flame.transform.localPosition = new Vector3(side * 0.42f, -0.04f, -1.98f);
            flame.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            flame.transform.localScale = new Vector3(0.24f, 0.72f, 0.24f);
            flame.GetComponent<Renderer>().material = RuntimeMaterial.Create(new Color(trim.r, trim.g, trim.b, 0.68f), true);
            var light = flame.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 3.8f;
            light.intensity = 1.4f;
            light.color = trim;
        }

        void AddShipShieldOutline(Transform parent, Color trim)
        {
            var ring = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ring.name = "Subtle Shield Ring";
            ring.transform.SetParent(parent);
            ring.transform.localPosition = new Vector3(0f, 0.02f, -0.12f);
            ring.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            ring.transform.localScale = new Vector3(1.75f, 1.75f, 0.035f);
            ring.GetComponent<Renderer>().material = RuntimeMaterial.Create(new Color(trim.r, trim.g, trim.b, 0.16f), true);
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

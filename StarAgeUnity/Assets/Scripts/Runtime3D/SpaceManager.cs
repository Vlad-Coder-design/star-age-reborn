using System.Collections.Generic;
using UnityEngine;

namespace StarAge3D
{
    public class SpaceManager : MonoBehaviour
    {
        public ShipController PlayerShip { get; private set; }

        GameObject root;
        readonly List<GameObject> spawned = new List<GameObject>();

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
            Vector3 target = PlayerShip.transform.position + new Vector3(0f, 12f, -16f);
            cam.transform.position = Vector3.Lerp(cam.transform.position, target, 0.08f);
            cam.transform.LookAt(PlayerShip.transform.position + Vector3.forward * 4f);
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
            ship.transform.position = new Vector3(0f, 0f, -8f);
            PlayerShip = ship.AddComponent<ShipController>();
            PlayerShip.Init(false);
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
            BuildStarfield();
            RebuildPlayerShip();
            for (int i = 0; i < 18; i++) SpawnMiningObject(i < 12);
            for (int i = 0; i < 5; i++) SpawnPirate();
            for (int i = 0; i < 4; i++) SpawnNpcMiner();
        }

        void BuildStarfield()
        {
            for (int i = 0; i < 220; i++)
            {
                var star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                star.name = "Star";
                star.transform.SetParent(root.transform);
                star.transform.position = new Vector3(Random.Range(-90f, 90f), Random.Range(-20f, 35f), Random.Range(-90f, 90f));
                star.transform.localScale = Vector3.one * Random.Range(0.04f, 0.12f);
                star.GetComponent<Renderer>().material = Mat(Color.white);
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
        }

        void SpawnPirate()
        {
            var ship = CreateShipModel("Pirate Ship", new Color(1f, 0.12f, 0.08f), true);
            ship.transform.SetParent(root.transform);
            ship.transform.position = new Vector3(Random.Range(-34f, 34f), 0f, Random.Range(18f, 48f));
            var controller = ship.AddComponent<ShipController>();
            controller.Init(true);
            ship.AddComponent<EnemyAI>().Init(controller);
            spawned.Add(ship);
        }

        void SpawnNpcMiner()
        {
            var ship = CreateShipModel("Mining NPC", new Color(0.95f, 0.8f, 0.2f), false);
            ship.transform.SetParent(root.transform);
            ship.transform.position = new Vector3(Random.Range(-35f, 35f), 0f, Random.Range(-40f, 35f));
            ship.AddComponent<NpcMiner>();
            spawned.Add(ship);
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

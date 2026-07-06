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
            rootShip.transform.localScale = pirate ? Vector3.one * 0.85f : Vector3.one;

            var body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.transform.SetParent(rootShip.transform);
            body.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            body.transform.localScale = new Vector3(0.5f, 1.25f, 0.5f);
            body.GetComponent<Renderer>().material = Mat(color);

            var nose = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            nose.transform.SetParent(rootShip.transform);
            nose.transform.localPosition = new Vector3(0f, 0f, 1.25f);
            nose.transform.localScale = new Vector3(0.48f, 0.32f, 0.65f);
            nose.GetComponent<Renderer>().material = Mat(color * 1.1f);

            var cockpit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cockpit.transform.SetParent(rootShip.transform);
            cockpit.transform.localPosition = new Vector3(0f, 0.24f, 0.28f);
            cockpit.transform.localScale = new Vector3(0.45f, 0.18f, 0.55f);
            cockpit.GetComponent<Renderer>().material = Mat(new Color(0.45f, 0.9f, 1f));

            for (int side = -1; side <= 1; side += 2)
            {
                var wing = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wing.transform.SetParent(rootShip.transform);
                wing.transform.localPosition = new Vector3(side * 0.75f, -0.05f, -0.1f);
                wing.transform.localRotation = Quaternion.Euler(0f, 0f, side * 10f);
                wing.transform.localScale = new Vector3(0.9f, 0.08f, 0.32f);
                wing.GetComponent<Renderer>().material = Mat(color * 0.8f);

                var engine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                engine.transform.SetParent(rootShip.transform);
                engine.transform.localPosition = new Vector3(side * 0.28f, -0.05f, -0.95f);
                engine.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                engine.transform.localScale = new Vector3(0.16f, 0.22f, 0.16f);
                engine.GetComponent<Renderer>().material = Mat(new Color(0.15f, 0.16f, 0.18f));
            }

            var collider = rootShip.AddComponent<SphereCollider>();
            collider.radius = 0.85f;
            collider.isTrigger = true;
            return rootShip;
        }

        Material Mat(Color color)
        {
            return RuntimeMaterial.Create(color, color.maxColorComponent > 0.9f);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace StarAgeReborn.Unity
{
    public class StarAgeUnityGame : MonoBehaviour
    {
        const string SaveFileName = "star-age-reborn-unity.json";

        readonly Dictionary<string, ShipClassDef> shipClasses = new Dictionary<string, ShipClassDef>
        {
            { "scout", new ShipClassDef("Scout", 100, 200f, 2, 200, 0) },
            { "fighter", new ShipClassDef("Fighter", 200, 150f, 3, 250, 5000) },
            { "destroyer", new ShipClassDef("Destroyer", 400, 100f, 4, 300, 15000) }
        };

        readonly Dictionary<string, WeaponDef> weapons = new Dictionary<string, WeaponDef>
        {
            { "basicLaser", new WeaponDef("Basic Laser", 10, 150f, 1f, 0, new Color(1f, 0.35f, 0.35f)) },
            { "improvedLaser", new WeaponDef("Improved Laser", 15, 180f, 1.2f, 2000, new Color(1f, 0.55f, 0.55f)) }
        };

        readonly Dictionary<string, EngineDef> engines = new Dictionary<string, EngineDef>
        {
            { "basicEngine", new EngineDef("Basic Engine", 1f, 0) },
            { "improvedEngine", new EngineDef("Improved Engine", 1.25f, 1500) }
        };

        Camera cam;
        ShipController playerShip;
        Canvas canvas;
        Text creditsText;
        Text shipText;
        Text cargoText;
        Text contextText;
        GameObject colonyPanel;
        GameObject shipyardPanel;

        readonly List<PlanetView> planets = new List<PlanetView>();
        readonly List<ResourceObjectView> resourceObjects = new List<ResourceObjectView>();
        readonly List<ShipController> pirates = new List<ShipController>();
        readonly List<ProjectileView> projectiles = new List<ProjectileView>();
        readonly List<ColonyData> colonies = new List<ColonyData>();

        Sprite circleSprite;
        Sprite triangleSprite;
        Sprite squareSprite;
        Sprite playerSprite;
        Sprite fighterSprite;
        Sprite destroyerSprite;
        Sprite pirateSprite;

        SaveData save;
        float pirateSpawnTimer;
        float autosaveTimer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Bootstrap()
        {
            if (FindFirstObjectByType<StarAgeUnityGame>() != null) return;
            var root = new GameObject("Star Age Reborn Unity");
            root.AddComponent<StarAgeUnityGame>();
            DontDestroyOnLoad(root);
        }

        void Awake()
        {
            save = LoadSave();
            colonies.AddRange(save.colonies ?? new List<ColonyData>());
            NormalizeColonies();
            BuildSprites();
            LoadArt();
            BuildCamera();
            BuildWorld();
            BuildPlayer();
            BuildUi();
            SyncUi();
        }

        void Update()
        {
            HandleInput();
            UpdatePirates();
            UpdateProjectiles();
            UpdateColonies();

            autosaveTimer += Time.deltaTime;
            if (autosaveTimer >= 5f)
            {
                autosaveTimer = 0f;
                SaveGame();
            }
        }

        void OnApplicationQuit()
        {
            SaveGame();
        }

        void HandleInput()
        {
            if (Input.GetMouseButtonDown(0) && !PointerOverUi())
            {
                Vector3 point = cam.ScreenToWorldPoint(Input.mousePosition);
                point.z = 0f;
                playerShip.SetDestination(point);
            }

            if (playerShip != null)
            {
                Vector3 target = playerShip.transform.position + new Vector3(0f, 0f, -10f);
                cam.transform.position = Vector3.Lerp(cam.transform.position, target, 0.08f);
            }
        }

        bool PointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        void BuildCamera()
        {
            cam = Camera.main;
            if (cam == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cam = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            cam.orthographic = true;
            cam.orthographicSize = 260f;
            cam.backgroundColor = new Color(0.01f, 0.012f, 0.04f);
            cam.transform.position = new Vector3(0f, 0f, -10f);
        }

        void BuildWorld()
        {
            CreateStarfield();

            string[] planetNames = { "Myth-Sec", "Astra-12", "Cinder-IV", "Nyx-Prime" };
            Color[] colors =
            {
                new Color(1f, 0.75f, 0.25f),
                new Color(0.25f, 0.65f, 1f),
                new Color(0.45f, 0.95f, 0.45f),
                new Color(0.9f, 0.35f, 0.35f)
            };

            for (int i = 0; i < planetNames.Length; i++)
            {
                float orbit = 190f + i * 115f;
                float angle = i / (float)planetNames.Length * Mathf.PI * 2f + 0.3f;
                var planet = CreateCircle($"Planet {planetNames[i]}", new Vector3(Mathf.Cos(angle) * orbit, Mathf.Sin(angle) * orbit), 38f + i * 4f, colors[i]);
                var view = planet.AddComponent<PlanetView>();
                view.Init(this, $"planet-{i}", planetNames[i], i == 0 ? "scientific" : "mining");
                planets.Add(view);
            }

            for (int i = 0; i < 18; i++)
            {
                float orbit = Random.Range(230f, 650f);
                float angle = Random.Range(0f, Mathf.PI * 2f);
                CreateResourceObject(i < 11 ? "asteroid" : "comet", new Vector3(Mathf.Cos(angle) * orbit, Mathf.Sin(angle) * orbit));
            }

            if (colonies.Count == 0)
            {
                colonies.Add(new ColonyData("planet-0", "Myth-Sec", "scientific", true));
            }
        }

        void BuildPlayer()
        {
            var shipDef = shipClasses[save.shipClass];
            var player = new GameObject("Player Ship");
            player.transform.position = new Vector3(0f, -150f, 0f);
            var renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteForShip(save.shipClass);
            renderer.color = Color.white;
            renderer.sortingOrder = 5;
            player.AddComponent<CircleCollider2D>().radius = 18f;
            playerShip = player.AddComponent<ShipController>();
            playerShip.Init(this, false, save.shipClass, save.weaponId, save.engineId, save.hp <= 0 ? shipDef.hp : save.hp);
        }

        void BuildUi()
        {
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            canvas = new GameObject("Star Age UI").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvas.gameObject.AddComponent<GraphicRaycaster>();

            creditsText = MakeText("Credits", new Vector2(18f, -18f), TextAnchor.UpperLeft, 18);
            shipText = MakeText("Ship", new Vector2(18f, -48f), TextAnchor.UpperLeft, 16);
            cargoText = MakeText("Cargo", new Vector2(18f, -148f), TextAnchor.UpperLeft, 16);
            contextText = MakeText("Context", new Vector2(-18f, -18f), TextAnchor.UpperRight, 16);

            MakeButton("Save", new Vector2(-110f, 34f), SaveGame);
            MakeButton("Shipyard", new Vector2(-110f, 76f), ToggleShipyard);
            MakeButton("Colony", new Vector2(-110f, 118f), OpenHomeColony);

            colonyPanel = MakePanel("Colony Panel", new Vector2(0f, 0f), new Vector2(520f, 310f));
            colonyPanel.SetActive(false);

            shipyardPanel = MakePanel("Shipyard Panel", new Vector2(0f, 0f), new Vector2(560f, 420f));
            shipyardPanel.SetActive(false);
        }

        void SyncUi()
        {
            if (creditsText == null) return;

            var ship = shipClasses[save.shipClass];
            var weapon = weapons[save.weaponId];
            var engine = engines[save.engineId];
            int cargoUsed = save.cargo.stone + save.cargo.ice + save.cargo.uranium + save.cargo.fuel;

            creditsText.text = $"Credits: {save.credits:n0}";
            shipText.text =
                $"{ship.label}\nHP: {playerShip.Hp}/{ship.hp}\nSpeed: {Mathf.RoundToInt(ship.speed * engine.speedMultiplier)} ({engine.speedMultiplier:0.##}x)\nWeapon: {weapon.label}\nDamage: {weapon.damage}  Range: {weapon.range}";
            cargoText.text = $"Cargo: {cargoUsed}/{ship.cargo}\nStone: {save.cargo.stone}  Ice: {save.cargo.ice}\nUranium: {save.cargo.uranium}  Fuel: {save.cargo.fuel}";
            contextText.text = "Click space to move. Click asteroids to mine. Click pirates to attack.";
        }

        void ToggleShipyard()
        {
            shipyardPanel.SetActive(!shipyardPanel.activeSelf);
            if (shipyardPanel.activeSelf) RenderShipyard();
        }

        void RenderShipyard()
        {
            ClearChildren(shipyardPanel.transform);
            AddPanelTitle(shipyardPanel, "Shipyard");

            int y = -44;
            foreach (var pair in shipClasses)
            {
                string shipId = pair.Key;
                ShipClassDef def = pair.Value;
                string label = shipId == save.shipClass ? $"{def.label} - Current" : $"{def.label} - {def.cost:n0} credits";
                MakePanelButton(shipyardPanel, label, new Vector2(0f, y), () => PurchaseShip(shipId));
                y -= 48;
            }

            MakePanelButton(shipyardPanel, $"Improved Laser - {weapons["improvedLaser"].cost:n0}", new Vector2(0f, y - 12), () => PurchaseEquipment("weapon", "improvedLaser"));
            MakePanelButton(shipyardPanel, $"Improved Engine - {engines["improvedEngine"].cost:n0}", new Vector2(0f, y - 60), () => PurchaseEquipment("engine", "improvedEngine"));
            MakePanelButton(shipyardPanel, "Close", new Vector2(0f, -180f), () => shipyardPanel.SetActive(false));
        }

        void OpenColony(ColonyData colony)
        {
            if (colony == null) return;
            colony.WithDefaults();

            if (colonyPanel == null)
            {
                colonyPanel = MakePanel("Colony Panel", new Vector2(0f, 0f), new Vector2(520f, 310f));
            }

            colonyPanel.SetActive(true);
            ClearChildren(colonyPanel.transform);
            AddPanelTitle(colonyPanel, $"{colony.planetName} Colony");
            AddPanelText(colonyPanel, $"Storage\nStone: {colony.storage.stone}\nIce: {colony.storage.ice}\nUranium: {colony.storage.uranium}\nFuel: {colony.storage.fuel}", new Vector2(-150f, -70f));
            MakePanelButton(colonyPanel, "Collect Production", new Vector2(120f, -42f), () =>
            {
                CollectColony(colony);
                OpenColony(colony);
            });
            MakePanelButton(colonyPanel, "Sell Storage", new Vector2(120f, -92f), () =>
            {
                SellColonyStorage(colony);
                OpenColony(colony);
            });
            MakePanelButton(colonyPanel, "Close", new Vector2(120f, -142f), () => colonyPanel.SetActive(false));
        }

        public void OpenPlanet(string planetId, string planetName, string planetType)
        {
            ColonyData colony = colonies.Find(c => c.planetId == planetId);
            if (colony == null)
            {
                colony = new ColonyData(planetId, planetName, planetType, false);
                colonies.Add(colony);
                SaveGame();
            }

            OpenColony(colony);
        }

        void OpenHomeColony()
        {
            if (colonies.Count == 0)
            {
                colonies.Add(new ColonyData("planet-0", "Myth-Sec", "scientific", true));
            }

            OpenColony(colonies[0]);
        }

        void PurchaseShip(string shipId)
        {
            if (shipId == save.shipClass) return;
            var def = shipClasses[shipId];
            if (save.credits < def.cost) return;

            save.credits -= def.cost;
            save.shipClass = shipId;
            save.weaponId = "basicLaser";
            save.engineId = "basicEngine";
            save.hp = def.hp;
            playerShip.ApplyLoadout(shipId, save.weaponId, save.engineId, def.hp);
            playerShip.GetComponent<SpriteRenderer>().sprite = SpriteForShip(shipId);
            RenderShipyard();
            SyncUi();
            SaveGame();
        }

        void PurchaseEquipment(string type, string id)
        {
            if (type == "weapon")
            {
                if (save.weaponId == id || save.credits < weapons[id].cost) return;
                save.credits -= weapons[id].cost;
                save.weaponId = id;
            }
            else
            {
                if (save.engineId == id || save.credits < engines[id].cost) return;
                save.credits -= engines[id].cost;
                save.engineId = id;
            }

            playerShip.ApplyLoadout(save.shipClass, save.weaponId, save.engineId, playerShip.Hp);
            RenderShipyard();
            SyncUi();
            SaveGame();
        }

        public void AttackOrMine(GameObject target)
        {
            var pirate = target.GetComponent<ShipController>();
            if (pirate != null && pirate.IsNpc)
            {
                playerShip.Target = pirate;
                return;
            }

            var resource = target.GetComponent<ResourceObjectView>();
            if (resource != null)
            {
                int amount = resource.Harvest();
                AddCargo(resource.ResourceType == "comet" ? "ice" : "stone", amount);
                SyncUi();
            }
        }

        public void FireProjectile(ShipController attacker, ShipController target)
        {
            var weapon = weapons[attacker.WeaponId];
            var shot = CreateCircle("Laser Shot", attacker.transform.position, 4f, weapon.color);
            shot.GetComponent<SpriteRenderer>().sortingOrder = 9;
            var projectile = shot.AddComponent<ProjectileView>();
            projectile.Init(target, weapon.damage, attacker.IsNpc ? Color.red : weapon.color);
            projectiles.Add(projectile);
        }

        public float EffectiveSpeed(string shipId, string engineId)
        {
            return shipClasses[shipId].speed * engines[engineId].speedMultiplier;
        }

        public WeaponDef Weapon(string id)
        {
            return weapons[id];
        }

        public ShipClassDef ShipClass(string id)
        {
            return shipClasses[id];
        }

        void UpdatePirates()
        {
            pirateSpawnTimer += Time.deltaTime;
            if (pirateSpawnTimer >= 18f && pirates.Count < 3)
            {
                pirateSpawnTimer = 0f;
                SpawnPirate();
            }

            pirates.RemoveAll(p => p == null || p.Hp <= 0);
        }

        void SpawnPirate()
        {
            Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(330f, 520f);
            var pirate = new GameObject("Pirate");
            pirate.transform.position = playerShip.transform.position + new Vector3(offset.x, offset.y, 0f);
            var renderer = pirate.AddComponent<SpriteRenderer>();
            renderer.sprite = pirateSprite != null ? pirateSprite : triangleSprite;
            renderer.color = pirateSprite != null ? Color.white : new Color(1f, 0.2f, 0.2f);
            renderer.sortingOrder = 5;
            pirate.AddComponent<CircleCollider2D>().radius = 18f;
            var controller = pirate.AddComponent<ShipController>();
            controller.Init(this, true, "scout", "basicLaser", "basicEngine", 100);
            controller.Target = playerShip;
            pirates.Add(controller);
        }

        void UpdateProjectiles()
        {
            projectiles.RemoveAll(p => p == null);
        }

        void UpdateColonies()
        {
            foreach (var colony in colonies)
            {
                if (colony == null) continue;
                colony.WithDefaults();

                if (Time.realtimeSinceStartup - colony.lastProductionTime >= 10f)
                {
                    colony.lastProductionTime = Time.realtimeSinceStartup;
                    colony.storage.stone += 1;
                    colony.storage.ice += 1;
                }
            }
        }

        void CollectColony(ColonyData colony)
        {
            AddCargo("stone", colony.storage.stone);
            AddCargo("ice", colony.storage.ice);
            AddCargo("uranium", colony.storage.uranium);
            AddCargo("fuel", colony.storage.fuel);
            colony.storage = new ResourceStore();
            SyncUi();
            SaveGame();
        }

        void SellColonyStorage(ColonyData colony)
        {
            save.credits += colony.storage.stone * 3 + colony.storage.ice * 2 + colony.storage.uranium * 5 + colony.storage.fuel * 20;
            colony.storage = new ResourceStore();
            SyncUi();
            SaveGame();
        }

        void AddCargo(string resource, int amount)
        {
            int used = save.cargo.stone + save.cargo.ice + save.cargo.uranium + save.cargo.fuel;
            int capacity = shipClasses[save.shipClass].cargo;
            int added = Mathf.Min(amount, Mathf.Max(capacity - used, 0));
            if (added <= 0) return;

            if (resource == "stone") save.cargo.stone += added;
            if (resource == "ice") save.cargo.ice += added;
            if (resource == "uranium") save.cargo.uranium += added;
            if (resource == "fuel") save.cargo.fuel += added;
        }

        void CreateResourceObject(string type, Vector3 position)
        {
            var color = type == "asteroid" ? new Color(0.55f, 0.52f, 0.48f) : new Color(0.7f, 0.95f, 1f);
            var obj = CreateCircle(type, position, type == "asteroid" ? 16f : 13f, color);
            var view = obj.AddComponent<ResourceObjectView>();
            view.Init(this, type);
            resourceObjects.Add(view);
        }

        GameObject CreateCircle(string name, Vector3 position, float scale, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.position = position;
            obj.transform.localScale = Vector3.one * scale;
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = circleSprite;
            renderer.color = color;
            renderer.sortingOrder = 1;
            obj.AddComponent<CircleCollider2D>().radius = 0.5f;
            return obj;
        }

        void CreateStarfield()
        {
            for (int i = 0; i < 180; i++)
            {
                var star = CreateCircle("Star", new Vector3(Random.Range(-900f, 900f), Random.Range(-650f, 650f), 1f), Random.Range(1f, 3f), Color.white);
                star.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Random.Range(0.25f, 0.85f));
            }
        }

        void BuildSprites()
        {
            circleSprite = MakeCircleSprite();
            triangleSprite = MakeTriangleSprite();
            squareSprite = MakeSquareSprite();
        }

        void LoadArt()
        {
            playerSprite = LoadSpriteFromTexture("Sprites/Ships/scout");
            fighterSprite = LoadSpriteFromTexture("Sprites/Ships/fighter");
            destroyerSprite = LoadSpriteFromTexture("Sprites/Ships/destroyer");
            pirateSprite = LoadSpriteFromTexture("Sprites/Ships/pirate");
        }

        Sprite LoadSpriteFromTexture(string resourcePath)
        {
            var texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null) return null;
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), Mathf.Max(texture.width, texture.height) / 60f);
        }

        Sprite SpriteForShip(string shipId)
        {
            if (shipId == "fighter" && fighterSprite != null) return fighterSprite;
            if (shipId == "destroyer" && destroyerSprite != null) return destroyerSprite;
            if (playerSprite != null) return playerSprite;
            return triangleSprite;
        }

        Sprite MakeCircleSprite()
        {
            var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            var center = new Vector2(31.5f, 31.5f);
            for (int y = 0; y < 64; y++)
            for (int x = 0; x < 64; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                texture.SetPixel(x, y, distance <= 30f ? Color.white : Color.clear);
            }
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64f);
        }

        Sprite MakeSquareSprite()
        {
            var texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            for (int y = 0; y < 16; y++)
            for (int x = 0; x < 16; x++)
                texture.SetPixel(x, y, Color.white);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16f);
        }

        Sprite MakeTriangleSprite()
        {
            var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            for (int y = 0; y < 64; y++)
            for (int x = 0; x < 64; x++)
            {
                bool inside = y > 8 && y < 58 && Mathf.Abs(x - 32) < (58 - y) * 0.45f;
                texture.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64f);
        }

        Text MakeText(string name, Vector2 anchoredPosition, TextAnchor anchor, int size)
        {
            var text = new GameObject(name).AddComponent<Text>();
            text.transform.SetParent(canvas.transform, false);
            text.font = UiFont();
            text.fontSize = size;
            text.color = new Color(0.82f, 0.9f, 1f);
            text.alignment = anchor;
            text.rectTransform.anchorMin = anchor.ToString().Contains("Right") ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            text.rectTransform.anchorMax = text.rectTransform.anchorMin;
            text.rectTransform.pivot = anchor.ToString().Contains("Right") ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            text.rectTransform.anchoredPosition = anchoredPosition;
            text.rectTransform.sizeDelta = new Vector2(430f, 160f);
            return text;
        }

        void MakeButton(string label, Vector2 anchoredPosition, UnityEngine.Events.UnityAction action)
        {
            var button = MakeButtonObject(label, canvas.transform, new Vector2(170f, 34f));
            button.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            button.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
            button.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
            button.GetComponent<Button>().onClick.AddListener(action);
        }

        GameObject MakePanel(string name, Vector2 anchoredPosition, Vector2 size)
        {
            var panel = new GameObject(name).AddComponent<Image>();
            panel.transform.SetParent(canvas.transform, false);
            panel.color = new Color(0.03f, 0.07f, 0.15f, 0.94f);
            panel.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            panel.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            panel.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            panel.rectTransform.anchoredPosition = anchoredPosition;
            panel.rectTransform.sizeDelta = size;
            return panel.gameObject;
        }

        void AddPanelTitle(GameObject panel, string title)
        {
            var text = AddPanelText(panel, title, new Vector2(0f, 126f));
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleCenter;
        }

        Text AddPanelText(GameObject panel, string value, Vector2 anchoredPosition)
        {
            var text = new GameObject("Text").AddComponent<Text>();
            text.transform.SetParent(panel.transform, false);
            text.font = UiFont();
            text.fontSize = 17;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleLeft;
            text.rectTransform.anchoredPosition = anchoredPosition;
            text.rectTransform.sizeDelta = new Vector2(360f, 120f);
            return text;
        }

        void MakePanelButton(GameObject panel, string label, Vector2 anchoredPosition, UnityEngine.Events.UnityAction action)
        {
            var button = MakeButtonObject(label, panel.transform, new Vector2(280f, 36f));
            button.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
            button.GetComponent<Button>().onClick.AddListener(action);
        }

        GameObject MakeButtonObject(string label, Transform parent, Vector2 size)
        {
            var root = new GameObject(label, typeof(Image), typeof(Button));
            root.transform.SetParent(parent, false);
            var image = root.GetComponent<Image>();
            image.color = new Color(0.12f, 0.26f, 0.42f, 0.95f);
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = size;

            var text = new GameObject("Label").AddComponent<Text>();
            text.transform.SetParent(root.transform, false);
            text.font = UiFont();
            text.fontSize = 15;
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = Vector2.zero;
            text.rectTransform.offsetMax = Vector2.zero;
            text.text = label;
            return root;
        }

        Font UiFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        SaveData LoadSave()
        {
            string path = Path.Combine(Application.persistentDataPath, SaveFileName);
            if (!File.Exists(path)) return new SaveData();
            try
            {
                var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
                return data == null ? new SaveData() : data.WithDefaults();
            }
            catch
            {
                return new SaveData();
            }
        }

        void NormalizeColonies()
        {
            colonies.RemoveAll(c => c == null);
            foreach (var colony in colonies)
            {
                colony.WithDefaults();
            }
        }

        void SaveGame()
        {
            save.hp = playerShip != null ? playerShip.Hp : save.hp;
            save.colonies = colonies;
            string path = Path.Combine(Application.persistentDataPath, SaveFileName);
            File.WriteAllText(path, JsonUtility.ToJson(save, true));
        }
    }

    public class ShipController : MonoBehaviour
    {
        StarAgeUnityGame game;
        Vector3 destination;
        float cooldown;

        public bool IsNpc { get; private set; }
        public string ShipId { get; private set; }
        public string WeaponId { get; private set; }
        public string EngineId { get; private set; }
        public int Hp { get; private set; }
        public ShipController Target { get; set; }

        public void Init(StarAgeUnityGame owner, bool npc, string shipId, string weaponId, string engineId, int hp)
        {
            game = owner;
            IsNpc = npc;
            ApplyLoadout(shipId, weaponId, engineId, hp);
            destination = transform.position;
        }

        public void ApplyLoadout(string shipId, string weaponId, string engineId, int hp)
        {
            ShipId = shipId;
            WeaponId = weaponId;
            EngineId = engineId;
            Hp = Mathf.Clamp(hp, 1, game.ShipClass(shipId).hp);
        }

        public void SetDestination(Vector3 value)
        {
            destination = value;
        }

        void Update()
        {
            Move();
            Attack();
        }

        void Move()
        {
            if (Vector3.Distance(transform.position, destination) < 3f) return;
            float speed = game.EffectiveSpeed(ShipId, EngineId);
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            Vector3 dir = destination - transform.position;
            if (dir.sqrMagnitude > 0.1f) transform.up = dir.normalized;
        }

        void Attack()
        {
            if (Target == null || Target.Hp <= 0) return;
            float distance = Vector3.Distance(transform.position, Target.transform.position);
            WeaponDef weapon = game.Weapon(WeaponId);

            if (IsNpc && distance > weapon.range * 0.85f)
            {
                SetDestination(Target.transform.position);
                return;
            }

            if (distance > weapon.range)
            {
                if (!IsNpc) SetDestination(Target.transform.position);
                return;
            }

            cooldown -= Time.deltaTime;
            if (cooldown <= 0f)
            {
                cooldown = 1f / weapon.fireRate;
                game.FireProjectile(this, Target);
            }
        }

        public void TakeDamage(int amount)
        {
            Hp = Mathf.Max(0, Hp - amount);
            if (Hp == 0) Destroy(gameObject);
        }

        void OnMouseDown()
        {
            if (IsNpc) game.AttackOrMine(gameObject);
        }
    }

    public class ProjectileView : MonoBehaviour
    {
        ShipController target;
        int damage;
        float ttl = 1.5f;

        public void Init(ShipController targetShip, int amount, Color color)
        {
            target = targetShip;
            damage = amount;
            GetComponent<SpriteRenderer>().color = color;
        }

        void Update()
        {
            ttl -= Time.deltaTime;
            if (ttl <= 0f || target == null)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 700f * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.transform.position) < 10f)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    public class ResourceObjectView : MonoBehaviour
    {
        StarAgeUnityGame game;
        int hp;
        public string ResourceType { get; private set; }

        public void Init(StarAgeUnityGame owner, string type)
        {
            game = owner;
            ResourceType = type;
            hp = type == "asteroid" ? 50 : 40;
        }

        public int Harvest()
        {
            hp -= 50;
            if (hp <= 0)
            {
                Destroy(gameObject);
                return Random.Range(2, 5);
            }
            return 0;
        }

        void OnMouseDown()
        {
            game.AttackOrMine(gameObject);
        }
    }

    public class PlanetView : MonoBehaviour
    {
        StarAgeUnityGame game;
        string planetId;
        string planetName;
        string planetType;

        public void Init(StarAgeUnityGame owner, string id, string name, string type)
        {
            game = owner;
            planetId = id;
            planetName = name;
            planetType = type;
        }

        void OnMouseDown()
        {
            game.OpenPlanet(planetId, planetName, planetType);
        }
    }

    [Serializable]
    public class SaveData
    {
        public int credits = 1000;
        public string shipClass = "scout";
        public string weaponId = "basicLaser";
        public string engineId = "basicEngine";
        public int hp = 100;
        public ResourceStore cargo = new ResourceStore();
        public List<ColonyData> colonies = new List<ColonyData>();

        public SaveData WithDefaults()
        {
            if (string.IsNullOrEmpty(shipClass)) shipClass = "scout";
            if (string.IsNullOrEmpty(weaponId)) weaponId = "basicLaser";
            if (string.IsNullOrEmpty(engineId)) engineId = "basicEngine";
            if (cargo == null) cargo = new ResourceStore();
            if (colonies == null) colonies = new List<ColonyData>();
            colonies.RemoveAll(c => c == null);
            foreach (var colony in colonies)
            {
                colony.WithDefaults();
            }
            return this;
        }
    }

    [Serializable]
    public class ColonyData
    {
        public string planetId;
        public string planetName;
        public string planetType;
        public ResourceStore storage = new ResourceStore();
        public float lastProductionTime;

        public ColonyData()
        {
        }

        public ColonyData(string id, string name, string type, bool home)
        {
            planetId = id;
            planetName = name;
            planetType = type;
            if (home)
            {
                storage.stone = 5;
                storage.ice = 5;
            }
            lastProductionTime = Time.realtimeSinceStartup;
        }

        public ColonyData WithDefaults()
        {
            if (string.IsNullOrEmpty(planetId)) planetId = "planet-0";
            if (string.IsNullOrEmpty(planetName)) planetName = "Myth-Sec";
            if (string.IsNullOrEmpty(planetType)) planetType = "scientific";
            if (storage == null) storage = new ResourceStore();
            if (lastProductionTime <= 0f) lastProductionTime = Time.realtimeSinceStartup;
            return this;
        }
    }

    [Serializable]
    public class ResourceStore
    {
        public int stone;
        public int ice;
        public int uranium;
        public int fuel;
    }

    public readonly struct ShipClassDef
    {
        public readonly string label;
        public readonly int hp;
        public readonly float speed;
        public readonly int weaponSlots;
        public readonly int cargo;
        public readonly int cost;

        public ShipClassDef(string label, int hp, float speed, int weaponSlots, int cargo, int cost)
        {
            this.label = label;
            this.hp = hp;
            this.speed = speed;
            this.weaponSlots = weaponSlots;
            this.cargo = cargo;
            this.cost = cost;
        }
    }

    public readonly struct WeaponDef
    {
        public readonly string label;
        public readonly int damage;
        public readonly float range;
        public readonly float fireRate;
        public readonly int cost;
        public readonly Color color;

        public WeaponDef(string label, int damage, float range, float fireRate, int cost, Color color)
        {
            this.label = label;
            this.damage = damage;
            this.range = range;
            this.fireRate = fireRate;
            this.cost = cost;
            this.color = color;
        }
    }

    public readonly struct EngineDef
    {
        public readonly string label;
        public readonly float speedMultiplier;
        public readonly int cost;

        public EngineDef(string label, float speedMultiplier, int cost)
        {
            this.label = label;
            this.speedMultiplier = speedMultiplier;
            this.cost = cost;
        }
    }
}

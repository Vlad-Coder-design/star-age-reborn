using UnityEngine;
using UnityEngine.EventSystems;

namespace StarAge3D
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public SaveManager Save { get; private set; }
        public ResourceManager Resources { get; private set; }
        public InventoryManager Inventory { get; private set; }
        public BuildingManager Buildings { get; private set; }
        public CraftingManager Crafting { get; private set; }
        public MarketManager Market { get; private set; }
        public QuestManager Quests { get; private set; }
        public ShipyardManager Shipyard { get; private set; }
        public SpaceManager Space { get; private set; }
        public UIManager UI { get; private set; }

        public GameMode Mode { get; private set; }
        public Camera MainCamera { get; private set; }

        float saveTimer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Bootstrap()
        {
            if (FindFirstObjectByType<GameManager>() != null) return;
            var root = new GameObject("Star Age 3D Game");
            root.AddComponent<GameManager>();
            DontDestroyOnLoad(root);
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Save = gameObject.AddComponent<SaveManager>();
            Resources = gameObject.AddComponent<ResourceManager>();
            Inventory = gameObject.AddComponent<InventoryManager>();
            Buildings = gameObject.AddComponent<BuildingManager>();
            Crafting = gameObject.AddComponent<CraftingManager>();
            Market = gameObject.AddComponent<MarketManager>();
            Quests = gameObject.AddComponent<QuestManager>();
            Shipyard = gameObject.AddComponent<ShipyardManager>();
            Space = gameObject.AddComponent<SpaceManager>();
            UI = gameObject.AddComponent<UIManager>();
        }

        void Start()
        {
            EnsureEventSystem();
            BuildLighting();
            Save.Load();
            Resources.Init(Save.Data.resources);
            Inventory.Init(300);
            Quests.Init(Save.Data.quests);
            Crafting.Init();
            Market.Init();
            BuildCamera();
            Buildings.Init();
            Space.Init();
            UI.Init();
            EnterPlanetMode();
        }

        void Update()
        {
            saveTimer += Time.deltaTime;
            if (saveTimer >= 5f)
            {
                saveTimer = 0f;
                SaveGame();
            }
        }

        void OnApplicationQuit()
        {
            SaveGame();
        }

        public void SaveGame()
        {
            if (Save.Data == null) return;
            Save.Data.resources = Resources.Wallet;
            Buildings.WriteSave();
            Quests.WriteSave();
            if (Space.PlayerShip != null)
            {
                Save.Data.shipHp = Space.PlayerShip.Hp;
            }
            Save.Save();
            UI.Refresh();
        }

        public void EnterPlanetMode()
        {
            Mode = GameMode.Planet;
            Space.SetActive(false);
            Buildings.SetActive(true);
            UI.ShowPlanetHud();
            MainCamera.clearFlags = CameraClearFlags.SolidColor;
            MainCamera.backgroundColor = new Color(0.018f, 0.026f, 0.055f);
            MainCamera.transform.position = new Vector3(0f, 12.5f, -12f);
            MainCamera.transform.LookAt(new Vector3(0f, 0.55f, 0f));
        }

        public void EnterSpaceMode()
        {
            Mode = GameMode.Space;
            Buildings.SetActive(false);
            Space.SetActive(true);
            UI.ShowSpaceHud();
        }

        void BuildCamera()
        {
            MainCamera = Camera.main;
            if (MainCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                MainCamera = cameraObject.AddComponent<Camera>();
            }

            MainCamera.nearClipPlane = 0.1f;
            MainCamera.farClipPlane = 1200f;
            MainCamera.fieldOfView = 55f;
        }

        void BuildLighting()
        {
            RenderSettings.ambientLight = new Color(0.18f, 0.18f, 0.24f);
            foreach (Light sceneLight in FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (sceneLight.name == "Star Key Light" || sceneLight.name == "Blue Fill Light") continue;
                sceneLight.enabled = false;
            }

            if (GameObject.Find("Star Key Light") != null) return;
            var keyObject = new GameObject("Star Key Light");
            var key = keyObject.AddComponent<Light>();
            key.type = LightType.Directional;
            key.intensity = 2.15f;
            key.color = new Color(1f, 0.9f, 0.72f);
            keyObject.transform.rotation = Quaternion.Euler(48f, -28f, 0f);

            var fillObject = new GameObject("Blue Fill Light");
            var fill = fillObject.AddComponent<Light>();
            fill.type = LightType.Point;
            fill.intensity = 2.4f;
            fill.range = 34f;
            fill.color = new Color(0.22f, 0.48f, 1f);
            fillObject.transform.position = new Vector3(-7f, 7f, -7f);
        }

        void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }
    }
}

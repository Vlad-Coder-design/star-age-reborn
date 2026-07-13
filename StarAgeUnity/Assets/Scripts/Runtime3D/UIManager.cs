using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarAge3D
{
    public class UIManager : MonoBehaviour
    {
        Canvas canvas;
        Text resourceText;
        Text statusText;
        Text spaceText;
        Text hpBarText;
        Text cargoBarText;
        RectTransform hpBarFill;
        RectTransform cargoBarFill;
        GameObject minimapPanel;
        RawImage minimapImage;
        Texture2D minimapTexture;
        GameObject planetPanel;
        GameObject detailPanel;
        GameObject craftingPanel;
        GameObject marketPanel;
        GameObject questPanel;
        GameObject shipyardPanel;
        GameObject spacePanel;
        GameObject spaceStatsPanel;
        GameObject rightRailPanel;
        GameObject galaxyPanel;
        float refreshTimer;

        readonly BuildingType[] buildable =
        {
            BuildingType.StoneQuarry,
            BuildingType.UraniumMine,
            BuildingType.IceWell,
            BuildingType.FuelFactory,
            BuildingType.MetalFactory,
            BuildingType.RepairKitFactory,
            BuildingType.BoosterFactory,
            BuildingType.Warehouse,
            BuildingType.Market
        };

        public void Init()
        {
            canvas = new GameObject("Star Age 3D UI").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
            canvas.gameObject.AddComponent<GraphicRaycaster>();

            var resourcePanel = MakePanel("Resource HUD", new Vector2(16f, -14f), new Vector2(330f, 88f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            MakeStatBar(resourcePanel.transform, "Hull Bar", new Vector2(14f, -10f), new Color(0.23f, 0.9f, 0.38f), out hpBarFill, out hpBarText);
            MakeStatBar(resourcePanel.transform, "XP Bar", new Vector2(14f, -36f), new Color(1f, 0.62f, 0.16f), out cargoBarFill, out cargoBarText);
            resourceText = MakeText("Resources", resourcePanel.transform, new Vector2(14f, -64f), new Vector2(302f, 20f), TextAnchor.UpperLeft, 11);
            var statusPanel = MakePanel("Status HUD", new Vector2(-16f, -14f), new Vector2(154f, 48f), new Vector2(1f, 1f), new Vector2(1f, 1f));
            statusText = MakeText("Status", statusPanel.transform, new Vector2(-12f, -8f), new Vector2(130f, 34f), TextAnchor.UpperRight, 13);
            MakeMinimap();

            planetPanel = MakePanel("Planet Controls", new Vector2(0f, 18f), new Vector2(760f, 54f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            MakeButton("Colony", planetPanel.transform, new Vector2(-292f, 27f), new Vector2(112f, 34f), HidePopups);
            MakeButton("Auction", planetPanel.transform, new Vector2(-164f, 27f), new Vector2(120f, 34f), ToggleMarket);
            MakeButton("Shipyard", planetPanel.transform, new Vector2(-28f, 27f), new Vector2(126f, 34f), ToggleShipyard);
            MakeButton("Tasks", planetPanel.transform, new Vector2(108f, 27f), new Vector2(108f, 34f), ToggleQuests);
            MakeButton("Take off", planetPanel.transform, new Vector2(272f, 27f), new Vector2(146f, 34f), GameManager.Instance.EnterSpaceMode);

            spacePanel = MakePanel("Space Controls", new Vector2(0f, 18f), new Vector2(420f, 30f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            spaceText = MakeText("Space HUD", spacePanel.transform, new Vector2(0f, -6f), new Vector2(404f, 20f), TextAnchor.MiddleCenter, 11);
            spaceText.text = "WASD / RMB to fly - LMB or Space fires - click asteroids or pirates";
            spaceStatsPanel = MakePanel("Mission Tracker", new Vector2(16f, 52f), new Vector2(330f, 32f), new Vector2(0f, 0f), new Vector2(0f, 0f));
            MakeText("Mission Toast", spaceStatsPanel.transform, new Vector2(12f, -8f), new Vector2(306f, 18f), TextAnchor.UpperLeft, 11)
                .text = "While you were away your colonies produced resources";

            rightRailPanel = MakePanel("Space Right Rail", new Vector2(-14f, -34f), new Vector2(74f, 324f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
            MakeButton("Galaxy", rightRailPanel.transform, new Vector2(0f, 116f), new Vector2(64f, 38f), ToggleGalaxyMap);
            MakeButton("Home", rightRailPanel.transform, new Vector2(0f, 62f), new Vector2(64f, 38f), GameManager.Instance.EnterPlanetMode);
            MakeButton("Kit", rightRailPanel.transform, new Vector2(0f, 8f), new Vector2(64f, 38f), TryRepairShip);
            MakeButton("Boost", rightRailPanel.transform, new Vector2(0f, -46f), new Vector2(64f, 38f), TryBoostShip);

            detailPanel = MakePanel("Building Details", Vector2.zero, new Vector2(540f, 430f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            craftingPanel = MakePanel("Crafting", Vector2.zero, new Vector2(520f, 360f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            marketPanel = MakePanel("Market", Vector2.zero, new Vector2(520f, 430f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            questPanel = MakePanel("Quest Board", Vector2.zero, new Vector2(700f, 520f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            shipyardPanel = MakePanel("Shipyard", Vector2.zero, new Vector2(560f, 460f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            galaxyPanel = MakePanel("Galaxy Map", Vector2.zero, new Vector2(1160f, 650f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            HidePopups();
            Refresh();
        }

        void Update()
        {
            refreshTimer += Time.deltaTime;
            if (refreshTimer > 0.25f)
            {
                refreshTimer = 0f;
                Refresh();
            }
        }

        public void ShowPlanetHud()
        {
            planetPanel.SetActive(true);
            spacePanel.SetActive(false);
            if (spaceStatsPanel != null) spaceStatsPanel.SetActive(false);
            if (rightRailPanel != null) rightRailPanel.SetActive(false);
            if (minimapPanel != null) minimapPanel.SetActive(false);
            HidePopups();
            Refresh();
        }

        public void ShowSpaceHud()
        {
            planetPanel.SetActive(false);
            spacePanel.SetActive(true);
            if (spaceStatsPanel != null) spaceStatsPanel.SetActive(true);
            if (rightRailPanel != null) rightRailPanel.SetActive(true);
            if (minimapPanel != null) minimapPanel.SetActive(true);
            HidePopups();
            Refresh();
        }

        public void Refresh()
        {
            if (resourceText == null || GameManager.Instance == null || GameManager.Instance.Resources == null) return;
            ResourceWallet wallet = GameManager.Instance.Resources.Wallet;
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            ShipStats ship = ShipStats.For(data.shipId);
            WeaponStats weapon = WeaponStats.For(data.weaponId);
            RefreshTopBars(data, ship);
            int cargo = GameManager.Instance.Space != null ? GameManager.Instance.Space.CargoUsed() : 0;
            int cargoMax = GameManager.Instance.Space != null ? GameManager.Instance.Space.CargoCapacity() : ship.cargo;
            resourceText.text = $"Cargo {cargo}/{cargoMax}   Stone {wallet.stone}  Ice {wallet.ice}  U {wallet.uranium}";
            string systemName = string.IsNullOrEmpty(data.currentSystem) ? "Fomen" : data.currentSystem;
            statusText.text = $"{wallet.coins:n0} credits\nNovara";
            if (GameManager.Instance.Mode == GameMode.Space) statusText.text = $"{wallet.coins:n0} credits\n{systemName} System";
            if (minimapPanel != null) minimapPanel.SetActive(GameManager.Instance.Mode == GameMode.Space);
            if (GameManager.Instance.Mode == GameMode.Space) DrawMinimap();

            if (spaceText != null && spaceText.gameObject.activeSelf)
                spaceText.text = "WASD / RMB to fly - LMB or Space fires - Galaxy opens the route map";
        }

        void MakeMinimap()
        {
            minimapPanel = MakePanel("Reference Minimap", new Vector2(-16f, -116f), new Vector2(166f, 166f), new Vector2(1f, 1f), new Vector2(1f, 1f));
            minimapTexture = new Texture2D(148, 148, TextureFormat.RGBA32, false);
            minimapTexture.filterMode = FilterMode.Point;
            var imageObject = new GameObject("Minimap Image", typeof(RawImage));
            imageObject.transform.SetParent(minimapPanel.transform, false);
            minimapImage = imageObject.GetComponent<RawImage>();
            minimapImage.texture = minimapTexture;
            RectTransform rect = minimapImage.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(148f, 148f);
        }

        void DrawMinimap()
        {
            if (minimapTexture == null || GameManager.Instance.Space == null) return;
            int size = minimapTexture.width;
            Color bg = new Color(0.015f, 0.035f, 0.075f, 0.92f);
            Color[] pixels = minimapTexture.GetPixels();
            for (int i = 0; i < pixels.Length; i++) pixels[i] = bg;
            minimapTexture.SetPixels(pixels);

            int center = size / 2;
            DrawCircle(center, center, 24, new Color(0.2f, 0.42f, 0.9f, 0.55f));
            DrawCircle(center, center, 42, new Color(0.2f, 0.42f, 0.9f, 0.45f));
            DrawCircle(center, center, 62, new Color(0.2f, 0.42f, 0.9f, 0.35f));
            DrawDot(center, center, 4, new Color(1f, 0.86f, 0.2f));

            DrawTransforms(GameManager.Instance.Space.MapPlanets, new Color(0.35f, 0.72f, 1f), 3);
            DrawTransforms(GameManager.Instance.Space.MapResources, new Color(0.75f, 0.78f, 0.72f), 1);
            DrawTransforms(GameManager.Instance.Space.MapEnemies, new Color(1f, 0.25f, 0.25f), 2);

            ShipController player = GameManager.Instance.Space.PlayerShip;
            if (player != null)
            {
                Vector2Int p = WorldToMap(player.transform.position);
                DrawDot(p.x, p.y, 3, new Color(0.35f, 1f, 0.55f));
            }

            minimapTexture.Apply(false);
        }

        void DrawTransforms(IReadOnlyList<Transform> transforms, Color color, int radius)
        {
            foreach (Transform item in transforms)
            {
                if (item == null || !item.gameObject.activeInHierarchy) continue;
                Vector2Int p = WorldToMap(item.position);
                DrawDot(p.x, p.y, radius, color);
            }
        }

        Vector2Int WorldToMap(Vector3 world)
        {
            int size = minimapTexture.width;
            float scale = size / 150f;
            int x = Mathf.RoundToInt(size * 0.5f + world.x * scale);
            int y = Mathf.RoundToInt(size * 0.5f + world.z * scale);
            return new Vector2Int(x, y);
        }

        void DrawCircle(int cx, int cy, int radius, Color color)
        {
            for (int i = 0; i < 96; i++)
            {
                float a = i / 96f * Mathf.PI * 2f;
                int x = Mathf.RoundToInt(cx + Mathf.Cos(a) * radius);
                int y = Mathf.RoundToInt(cy + Mathf.Sin(a) * radius);
                SetPixel(x, y, color);
            }
        }

        void DrawDot(int cx, int cy, int radius, Color color)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radius * radius) SetPixel(cx + x, cy + y, color);
                }
            }
        }

        void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= minimapTexture.width || y >= minimapTexture.height) return;
            minimapTexture.SetPixel(x, y, color);
        }

        public void OpenBuildingDetails(BuildingSlot slot)
        {
            HidePopups();
            detailPanel.SetActive(true);
            Clear(detailPanel.transform);
            AddTitle(detailPanel, $"Slot {slot.Index + 1}");
            BuildingType type = slot.Building != null ? slot.Building.Type : BuildingType.Empty;
            BuildingDefinition def = BuildingManager.Definition(type);
            AddText(detailPanel, $"{def.label}\n{def.description}\nStored: {(slot.Building != null ? slot.Building.Save.stored : 0)}", new Vector2(0f, 125f), new Vector2(460f, 100f));

            if (type == BuildingType.Empty)
            {
                int y = 55;
                for (int i = 0; i < buildable.Length; i++)
                {
                    BuildingType buildType = buildable[i];
                    BuildingDefinition buildDef = BuildingManager.Definition(buildType);
                    string label = $"{buildDef.label} - {buildDef.coinCost} credits";
                    MakeButton(label, detailPanel.transform, new Vector2(0f, y), new Vector2(450f, 30f), () =>
                    {
                        GameManager.Instance.Buildings.Build(slot.Index, buildType);
                        OpenBuildingDetails(slot);
                    });
                    y -= 34;
                }
            }
            else
            {
                MakeButton("Collect", detailPanel.transform, new Vector2(0f, 20f), new Vector2(180f, 34f), () =>
                {
                    slot.Building.Collect();
                    OpenBuildingDetails(slot);
                });
            }

            MakeButton("Close", detailPanel.transform, new Vector2(0f, -180f), new Vector2(180f, 34f), HidePopups);
        }

        void ToggleCrafting()
        {
            bool open = !craftingPanel.activeSelf;
            HidePopups();
            craftingPanel.SetActive(open);
            if (!open) return;
            Clear(craftingPanel.transform);
            AddTitle(craftingPanel, "Colony Crafting");
            AddText(craftingPanel, "Factories automate these Starkus recipes over time. Use this panel for instant MVP crafting.", new Vector2(0f, 110f), new Vector2(440f, 60f));
            MakeButton("Craft Metal: 2 stone -> 1 metal", craftingPanel.transform, new Vector2(0f, 45f), new Vector2(360f, 34f), () => { GameManager.Instance.Crafting.CraftMetal(); ToggleCrafting(); ToggleCrafting(); });
            MakeButton("Craft Fuel: 2 uranium + 1 ice -> 1 fuel", craftingPanel.transform, new Vector2(0f, 5f), new Vector2(360f, 34f), () => { GameManager.Instance.Crafting.CraftFuel(); ToggleCrafting(); ToggleCrafting(); });
            MakeButton("Craft Repair Kit: 1 metal + 1 uranium", craftingPanel.transform, new Vector2(0f, -35f), new Vector2(360f, 34f), () => { GameManager.Instance.Crafting.CraftRepairKit(); ToggleCrafting(); ToggleCrafting(); });
            MakeButton("Craft Booster: 1 fuel + 1 ice", craftingPanel.transform, new Vector2(0f, -75f), new Vector2(360f, 34f), () => { GameManager.Instance.Crafting.CraftBooster(); ToggleCrafting(); ToggleCrafting(); });
            MakeButton("Close", craftingPanel.transform, new Vector2(0f, -135f), new Vector2(180f, 34f), HidePopups);
        }

        void ToggleMarket()
        {
            bool open = !marketPanel.activeSelf;
            HidePopups();
            marketPanel.SetActive(open);
            if (!open) return;
            Clear(marketPanel.transform);
            AddTitle(marketPanel, "Auction - sell from cargo hold");
            AddText(marketPanel, "Sell prices: Ice 2, Stone 3, Uranium 5, Metal 15, Fuel 20, Repair Kit 50, Booster 50.", new Vector2(0f, 135f), new Vector2(450f, 70f));
            ResourceType[] types = { ResourceType.Ice, ResourceType.Stone, ResourceType.Uranium, ResourceType.Metal, ResourceType.Fuel, ResourceType.RepairKits, ResourceType.Boosters };
            int y = 70;
            foreach (ResourceType type in types)
            {
                ResourceType captured = type;
                MakeButton($"Sell 1 {captured} for {GameManager.Instance.Market.Price(captured)}", marketPanel.transform, new Vector2(0f, y), new Vector2(320f, 30f), () => { GameManager.Instance.Market.Sell(captured, 1); ToggleMarket(); ToggleMarket(); });
                y -= 34;
            }
            MakeButton("Sell All", marketPanel.transform, new Vector2(-95f, -180f), new Vector2(180f, 34f), () => { GameManager.Instance.Market.SellAll(); ToggleMarket(); ToggleMarket(); });
            MakeButton("Close", marketPanel.transform, new Vector2(105f, -180f), new Vector2(180f, 34f), HidePopups);
        }

        void ToggleQuests()
        {
            bool open = !questPanel.activeSelf;
            HidePopups();
            questPanel.SetActive(open);
            if (!open) return;
            Clear(questPanel.transform);
            AddTitle(questPanel, "Active missions / Mission board");
            int y = 160;
            foreach (QuestSave quest in GameManager.Instance.Quests.Quests)
            {
                QuestSave captured = quest;
                string state = quest.completed ? "Completed" : quest.accepted ? $"{quest.progress}/{GameManager.Instance.Quests.Target(quest.type)}" : "Not accepted";
                AddText(questPanel, $"{GameManager.Instance.Quests.Title(quest.type)}\n{GameManager.Instance.Quests.Description(quest.type)}\nProgress: {state}", new Vector2(-90f, y), new Vector2(430f, 78f));
                MakeButton(quest.accepted ? "Complete" : "Accept", questPanel.transform, new Vector2(240f, y - 8f), new Vector2(140f, 32f), () =>
                {
                    if (!captured.accepted) GameManager.Instance.Quests.Accept(captured);
                    else GameManager.Instance.Quests.Complete(captured);
                    ToggleQuests();
                    ToggleQuests();
                });
                y -= 102;
            }
            MakeButton("Close", questPanel.transform, new Vector2(0f, -235f), new Vector2(180f, 34f), HidePopups);
        }

        void ToggleShipyard()
        {
            bool open = !shipyardPanel.activeSelf;
            HidePopups();
            shipyardPanel.SetActive(open);
            if (!open) return;
            Clear(shipyardPanel.transform);
            AddTitle(shipyardPanel, "Shipyard");
            MakeButton("Scout - HP 100 / Speed 26 / Cargo 200 - Current", shipyardPanel.transform, new Vector2(0f, 125f), new Vector2(420f, 34f), () => { GameManager.Instance.Shipyard.BuyShip("scout"); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Fighter - HP 200 / Speed 20 / Cargo 250 - 5000 credits", shipyardPanel.transform, new Vector2(0f, 85f), new Vector2(420f, 34f), () => { GameManager.Instance.Shipyard.BuyShip("fighter"); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Destroyer - HP 400 / Speed 14 / Cargo 300 - 15000 credits", shipyardPanel.transform, new Vector2(0f, 45f), new Vector2(420f, 34f), () => { GameManager.Instance.Shipyard.BuyShip("destroyer"); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Basic Laser - Damage 10 / Range 20 / 1.0s", shipyardPanel.transform, new Vector2(0f, 5f), new Vector2(420f, 34f), () => { GameManager.Instance.Shipyard.BuyWeapon("laser"); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Improved Laser - Damage 15 / Range 24 / 2000 credits", shipyardPanel.transform, new Vector2(0f, -35f), new Vector2(420f, 34f), () => { GameManager.Instance.Shipyard.BuyWeapon("laser2"); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Improved Engine - Speed x1.25 - 1500 credits", shipyardPanel.transform, new Vector2(0f, -75f), new Vector2(420f, 34f), () => { GameManager.Instance.Shipyard.BuyEngine(); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Close", shipyardPanel.transform, new Vector2(0f, -185f), new Vector2(180f, 34f), HidePopups);
        }

        void ToggleGalaxyMap()
        {
            bool open = !galaxyPanel.activeSelf;
            HidePopups();
            galaxyPanel.SetActive(open);
            if (!open) return;
            Clear(galaxyPanel.transform);
            AddGalaxyBackdrop();
            AddTitle(galaxyPanel, "Galaxy Navigation");
            AddText(galaxyPanel, "Route chart: travel lanes, unlock levels, faction zones, threat, and economy at a glance.", new Vector2(-520f, 266f), new Vector2(760f, 34f));

            Vector2 fomen = new Vector2(-42f, -122f);
            Vector2 bellum = new Vector2(-270f, -8f);
            Vector2 tais = new Vector2(236f, -42f);
            Vector2 sparta = new Vector2(-108f, 122f);
            Vector2 pioner = new Vector2(390f, 126f);
            Vector2 orion = new Vector2(-438f, 94f);

            AddGalaxyConnection(fomen, bellum, true);
            AddGalaxyConnection(fomen, tais, false);
            AddGalaxyConnection(bellum, sparta, false);
            AddGalaxyConnection(sparta, orion, false);
            AddGalaxyConnection(tais, pioner, false);
            AddGalaxyConnection(orion, pioner, false);
            AddGalaxyRouteLabel("trade lane", fomen, bellum, true);
            AddGalaxyRouteLabel("survey permit L2", fomen, tais, false);
            AddGalaxyRouteLabel("war front L3", bellum, sparta, false);
            AddGalaxyRouteLabel("deep gate L5", sparta, orion, false);
            AddGalaxyRouteLabel("pioneer jump L4", tais, pioner, false);

            StarAgeSaveData data = GameManager.Instance.Save.Data;
            AddGalaxySystem("Fomen", "home colony", "Military", "Safe", "Ore + ice", fomen, new Color(0.32f, 1f, 0.58f), 1, data.currentSystem == "Fomen", true);
            AddGalaxySystem("Bellum", "open trade", "Trade", "Low", "Market + contracts", bellum, new Color(0.38f, 0.95f, 1f), 1, data.currentSystem == "Bellum", true);
            AddGalaxySystem("Tais", "frontier lock", "Mining", "Medium", "Uranium belt", tais, new Color(1f, 0.78f, 0.28f), 2, data.currentSystem == "Tais", false);
            AddGalaxySystem("Sparta", "pirate warzone", "Combat", "High", "Bounties", sparta, new Color(1f, 0.48f, 0.22f), 3, data.currentSystem == "Sparta", false);
            AddGalaxySystem("Pioner", "colonist gate", "Expansion", "High", "Rare cargo", pioner, new Color(1f, 0.26f, 0.22f), 4, data.currentSystem == "Pioner", false);
            AddGalaxySystem("Orion", "capital route", "Core", "Extreme", "Elite ships", orion, new Color(0.9f, 0.32f, 1f), 5, data.currentSystem == "Orion", false);

            AddGalaxyInfoPanel(data);
            AddGalaxyLegend();
            MakeButton("Close", galaxyPanel.transform, new Vector2(398f, -286f), new Vector2(180f, 36f), HidePopups);
        }

        void AddGalaxyBackdrop()
        {
            AddGalaxyPanelBlock("Core Sector", new Vector2(-300f, 48f), new Vector2(450f, 360f), new Color(0.02f, 0.08f, 0.16f, 0.42f));
            AddGalaxyPanelBlock("Frontier Sector", new Vector2(220f, 40f), new Vector2(430f, 350f), new Color(0.12f, 0.06f, 0.02f, 0.38f));
            AddGalaxyPanelBlock("Outer Lock Zone", new Vector2(0f, 124f), new Vector2(1000f, 236f), new Color(0.08f, 0.02f, 0.08f, 0.22f));
            AddGalaxyGridLine(new Vector2(-520f, -210f), new Vector2(520f, -210f), new Color(0.22f, 0.45f, 0.85f, 0.16f));
            AddGalaxyGridLine(new Vector2(-520f, 210f), new Vector2(520f, 210f), new Color(0.22f, 0.45f, 0.85f, 0.16f));
            AddGalaxyGridLine(new Vector2(-520f, -210f), new Vector2(-520f, 210f), new Color(0.22f, 0.45f, 0.85f, 0.16f));
            AddGalaxyGridLine(new Vector2(520f, -210f), new Vector2(520f, 210f), new Color(0.22f, 0.45f, 0.85f, 0.16f));
        }

        void AddGalaxyPanelBlock(string label, Vector2 position, Vector2 size, Color color)
        {
            var block = new GameObject(label, typeof(Image));
            block.transform.SetParent(galaxyPanel.transform, false);
            Image image = block.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            RectTransform rect = block.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            block.transform.SetAsFirstSibling();
            Text text = MakeText(label + " Label", block.transform, new Vector2(12f, -10f), new Vector2(size.x - 24f, 24f), TextAnchor.UpperLeft, 12);
            text.color = new Color(0.62f, 0.78f, 1f, 0.72f);
            text.text = label.ToUpperInvariant();
        }

        void AddGalaxySystem(string title, string subtitle, string faction, string threat, string economy, Vector2 position, Color color, int requiredLevel, bool current, bool reachable)
        {
            bool unlocked = GameManager.Instance.Save.Data.level >= requiredLevel;
            string lockText = unlocked ? (reachable ? "JUMP READY" : "SCOUTED") : "LOCKED L" + requiredLevel;
            string markerText = title + "\n" + lockText;
            Vector2 markerSize = current ? new Vector2(116f, 82f) : new Vector2(106f, 74f);
            GameObject marker = MakeButton(markerText, galaxyPanel.transform, position, markerSize, () =>
            {
                if (GameManager.Instance.TravelToSystem(title, requiredLevel)) HidePopups();
            });

            if (marker != null)
            {
                Image image = marker.GetComponent<Image>();
                if (image != null) image.color = unlocked ? color : new Color(0.22f, 0.24f, 0.32f, 0.98f);
                Button button = marker.GetComponent<Button>();
                if (button != null) button.interactable = unlocked;
                Text label = marker.GetComponentInChildren<Text>();
                if (label != null) label.fontSize = current ? 14 : 12;
            }

            AddGalaxyHalo(position, current ? 84f : 64f, current ? new Color(0.35f, 1f, 0.55f, 0.24f) : new Color(color.r, color.g, color.b, 0.12f));
            AddGalaxyOrbit(position, current ? 94f : 74f, unlocked ? color : new Color(0.36f, 0.38f, 0.46f, 0.5f));
            AddText(galaxyPanel, $"{subtitle}\n{faction} | Threat: {threat}\n{economy}", position + new Vector2(-58f, -72f), new Vector2(150f, 58f));
        }

        void AddGalaxyConnection(Vector2 a, Vector2 b, bool reachable)
        {
            Vector2 delta = b - a;
            float length = delta.magnitude;
            Vector2 direction = delta.normalized;
            int segments = Mathf.Max(6, Mathf.RoundToInt(length / 28f));
            for (int i = 0; i < segments; i++)
            {
                if (i % 2 == 1) continue;
                Vector2 start = a + direction * (length * i / segments);
                Vector2 end = a + direction * (length * (i + 0.72f) / segments);
                AddGalaxyLineSegment(start, end, reachable);
            }
        }

        void AddGalaxyLineSegment(Vector2 a, Vector2 b, bool reachable)
        {
            var line = new GameObject("Galaxy Route", typeof(Image));
            line.transform.SetParent(galaxyPanel.transform, false);
            Image image = line.GetComponent<Image>();
            image.color = reachable ? new Color(0.35f, 1f, 0.55f, 0.72f) : new Color(0.2f, 0.48f, 0.95f, 0.32f);
            image.raycastTarget = false;
            RectTransform rect = line.GetComponent<RectTransform>();
            Vector2 delta = b - a;
            rect.anchoredPosition = a + delta * 0.5f;
            rect.sizeDelta = new Vector2(delta.magnitude, reachable ? 4f : 3f);
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        }

        void AddGalaxyGridLine(Vector2 a, Vector2 b, Color color)
        {
            var line = new GameObject("Galaxy Grid", typeof(Image));
            line.transform.SetParent(galaxyPanel.transform, false);
            Image image = line.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            RectTransform rect = line.GetComponent<RectTransform>();
            Vector2 delta = b - a;
            rect.anchoredPosition = a + delta * 0.5f;
            rect.sizeDelta = new Vector2(delta.magnitude, 2f);
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        }

        void AddGalaxyRouteLabel(string label, Vector2 a, Vector2 b, bool reachable)
        {
            Vector2 position = (a + b) * 0.5f + new Vector2(0f, 18f);
            Text text = MakeText("Route Label", galaxyPanel.transform, position, new Vector2(160f, 22f), TextAnchor.UpperLeft, 11);
            text.color = reachable ? new Color(0.42f, 1f, 0.64f, 0.92f) : new Color(0.58f, 0.72f, 1f, 0.56f);
            text.text = label.ToUpperInvariant();
        }

        void AddGalaxyHalo(Vector2 position, float size, Color color)
        {
            var halo = new GameObject("Galaxy Halo", typeof(Image));
            halo.transform.SetParent(galaxyPanel.transform, false);
            Image image = halo.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            RectTransform rect = halo.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(size, size);
        }

        void AddGalaxyOrbit(Vector2 position, float size, Color color)
        {
            for (int i = 0; i < 28; i += 2)
            {
                float a = i / 28f * Mathf.PI * 2f;
                float b = (i + 1f) / 28f * Mathf.PI * 2f;
                Vector2 start = position + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * size * 0.5f;
                Vector2 end = position + new Vector2(Mathf.Cos(b), Mathf.Sin(b)) * size * 0.5f;
                AddGalaxyGridLine(start, end, new Color(color.r, color.g, color.b, 0.34f));
            }
        }

        void AddGalaxyInfoPanel(StarAgeSaveData data)
        {
            AddGalaxyPanelBlock("Navigation Intel", new Vector2(402f, -108f), new Vector2(270f, 246f), new Color(0.018f, 0.04f, 0.085f, 0.78f));
            AddText(galaxyPanel,
                $"CURRENT: {data.currentSystem}\nLEVEL: {data.level}   XP: {data.xp}\n\nNEXT UNLOCKS\nL2 Tais: uranium belt\nL3 Sparta: pirate warzone\nL4 Pioner: expansion gate\nL5 Orion: capital route\n\nAVAILABLE NOW\nFomen <-> Bellum",
                new Vector2(282f, -24f),
                new Vector2(245f, 192f));
        }

        void AddGalaxyLegend()
        {
            AddText(galaxyPanel, "Legend: bright lane = available jump | blue dashed = locked route | rings = system orbit | dark card = locked node.", new Vector2(-520f, -286f), new Vector2(660f, 34f));
        }

        void TryRepairShip()
        {
            ShipController player = GameManager.Instance.Space.PlayerShip;
            if (player != null) player.TryUseRepairKit();
        }

        void TryBoostShip()
        {
            ShipController player = GameManager.Instance.Space.PlayerShip;
            if (player != null) player.TryUseBooster();
        }

        void HidePopups()
        {
            if (detailPanel != null) detailPanel.SetActive(false);
            if (craftingPanel != null) craftingPanel.SetActive(false);
            if (marketPanel != null) marketPanel.SetActive(false);
            if (questPanel != null) questPanel.SetActive(false);
            if (shipyardPanel != null) shipyardPanel.SetActive(false);
            if (galaxyPanel != null) galaxyPanel.SetActive(false);
        }

        void RefreshTopBars(StarAgeSaveData data, ShipStats ship)
        {
            ShipController player = GameManager.Instance.Space.PlayerShip;
            int hp = player != null ? player.Hp : data.shipHp;
            int maxHp = player != null ? player.MaxHp : ship.hp + data.armorModules * 50;
            SetBar(hpBarFill, hp, maxHp, 216f);
            int nextXp = NextXpThreshold(data.level);
            int prevXp = PreviousXpThreshold(data.level);
            int levelProgress = Mathf.Max(0, data.xp - prevXp);
            int levelRange = Mathf.Max(1, nextXp - prevXp);
            SetBar(cargoBarFill, levelProgress, levelRange, 216f);
            if (hpBarText != null) hpBarText.text = $"{hp}/{maxHp}";
            if (cargoBarText != null) cargoBarText.text = $"Lv {data.level} - {data.xp} XP";
        }

        int PreviousXpThreshold(int level)
        {
            int[] thresholds = { 0, 100, 300, 700, 1500, 3000, 6000 };
            int index = Mathf.Clamp(level - 1, 0, thresholds.Length - 1);
            return thresholds[index];
        }

        int NextXpThreshold(int level)
        {
            int[] thresholds = { 0, 100, 300, 700, 1500, 3000, 6000 };
            int index = Mathf.Clamp(level, 1, thresholds.Length - 1);
            return thresholds[index];
        }

        void SetBar(RectTransform fill, int value, int max, float width)
        {
            if (fill == null) return;
            float pct = max > 0 ? Mathf.Clamp01(value / (float)max) : 0f;
            fill.sizeDelta = new Vector2(width * pct, fill.sizeDelta.y);
        }

        GameObject MakePanel(string name, Vector2 position, Vector2 size, Vector2 anchor, Vector2 pivot)
        {
            var panel = new GameObject(name, typeof(Image));
            panel.transform.SetParent(canvas.transform, false);
            Image image = panel.GetComponent<Image>();
            image.color = new Color(0.006f, 0.018f, 0.04f, 0.88f);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            AddPanelChrome(panel.transform, size);
            return panel;
        }

        void AddPanelChrome(Transform parent, Vector2 size)
        {
            AddUiLine(parent, "Top Neon Edge", new Vector2(0f, size.y * 0.5f - 2f), new Vector2(size.x - 8f, 2f), new Color(0.18f, 0.82f, 1f, 0.72f));
            AddUiLine(parent, "Bottom Shadow Edge", new Vector2(0f, -size.y * 0.5f + 2f), new Vector2(size.x - 8f, 2f), new Color(0.04f, 0.11f, 0.22f, 0.92f));
            AddUiLine(parent, "Left Neon Edge", new Vector2(-size.x * 0.5f + 2f, 0f), new Vector2(2f, size.y - 8f), new Color(0.08f, 0.45f, 0.8f, 0.64f));
            AddUiLine(parent, "Right Neon Edge", new Vector2(size.x * 0.5f - 2f, 0f), new Vector2(2f, size.y - 8f), new Color(0.06f, 0.24f, 0.44f, 0.76f));
        }

        void AddUiLine(Transform parent, string name, Vector2 position, Vector2 size, Color color)
        {
            var line = new GameObject(name, typeof(Image));
            line.transform.SetParent(parent, false);
            Image image = line.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            RectTransform rect = line.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        Text MakeText(string name, Transform parent, Vector2 position, Vector2 size, TextAnchor anchor, int fontSize)
        {
            var text = new GameObject(name).AddComponent<Text>();
            text.transform.SetParent(parent, false);
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = anchor;
            RectTransform rect = text.rectTransform;
            rect.anchorMin = anchor.ToString().Contains("Right") ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = anchor.ToString().Contains("Right") ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return text;
        }

        void AddTitle(GameObject panel, string title)
        {
            Text text = MakeText("Title", panel.transform, new Vector2(0f, panel.GetComponent<RectTransform>().sizeDelta.y * 0.5f - 34f), new Vector2(460f, 36f), TextAnchor.UpperLeft, 22);
            text.alignment = TextAnchor.MiddleCenter;
            text.text = title;
        }

        void AddText(GameObject panel, string value, Vector2 position, Vector2 size)
        {
            Text text = MakeText("Text", panel.transform, position, size, TextAnchor.UpperLeft, 15);
            text.text = value;
        }

        GameObject MakeButton(string label, Transform parent, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(label, typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.035f, 0.18f, 0.34f, 0.98f);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(action);
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.035f, 0.18f, 0.34f, 0.98f);
            colors.highlightedColor = new Color(0.08f, 0.42f, 0.72f, 1f);
            colors.pressedColor = new Color(0.03f, 0.7f, 0.92f, 1f);
            colors.disabledColor = new Color(0.08f, 0.09f, 0.13f, 0.82f);
            button.colors = colors;
            AddButtonAccent(buttonObject.transform, size);

            Text labelText = MakeText("Label", buttonObject.transform, Vector2.zero, size, TextAnchor.MiddleCenter, 14);
            labelText.text = label;
            labelText.rectTransform.anchorMin = Vector2.zero;
            labelText.rectTransform.anchorMax = Vector2.one;
            labelText.rectTransform.offsetMin = Vector2.zero;
            labelText.rectTransform.offsetMax = Vector2.zero;
            return buttonObject;
        }

        void AddButtonAccent(Transform parent, Vector2 size)
        {
            AddUiLine(parent, "Button Scanline", new Vector2(0f, size.y * 0.5f - 3f), new Vector2(size.x - 10f, 2f), new Color(0.25f, 0.95f, 1f, 0.62f));
            AddUiLine(parent, "Button Hot Corner L", new Vector2(-size.x * 0.5f + 7f, 0f), new Vector2(3f, size.y - 10f), new Color(0.2f, 0.95f, 1f, 0.36f));
        }

        void MakeStatBar(Transform parent, string name, Vector2 position, Color color, out RectTransform fillRect, out Text labelText)
        {
            var bar = new GameObject(name, typeof(Image));
            bar.transform.SetParent(parent, false);
            Image background = bar.GetComponent<Image>();
            background.color = new Color(0.02f, 0.05f, 0.11f, 0.9f);
            RectTransform barRect = bar.GetComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0f, 1f);
            barRect.anchorMax = new Vector2(0f, 1f);
            barRect.pivot = new Vector2(0f, 1f);
            barRect.anchoredPosition = position;
            barRect.sizeDelta = new Vector2(216f, 18f);

            var fill = new GameObject(name + " Fill", typeof(Image));
            fill.transform.SetParent(bar.transform, false);
            Image fillImage = fill.GetComponent<Image>();
            fillImage.color = color;
            fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(0f, 1f);
            fillRect.pivot = new Vector2(0f, 0.5f);
            fillRect.anchoredPosition = Vector2.zero;
            fillRect.sizeDelta = new Vector2(216f, 0f);

            labelText = MakeText(name + " Label", bar.transform, Vector2.zero, new Vector2(216f, 18f), TextAnchor.MiddleCenter, 11);
            labelText.rectTransform.anchorMin = Vector2.zero;
            labelText.rectTransform.anchorMax = Vector2.one;
            labelText.rectTransform.offsetMin = Vector2.zero;
            labelText.rectTransform.offsetMax = Vector2.zero;
            AddUiLine(bar.transform, "Bar Spark", new Vector2(0f, 8f), new Vector2(216f, 1.5f), new Color(color.r, color.g, color.b, 0.58f));
        }

        void Clear(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                if (IsPanelChrome(child.name)) continue;
                Destroy(child.gameObject);
            }
        }

        bool IsPanelChrome(string objectName)
        {
            return objectName == "Top Neon Edge"
                || objectName == "Bottom Shadow Edge"
                || objectName == "Left Neon Edge"
                || objectName == "Right Neon Edge";
        }
    }
}

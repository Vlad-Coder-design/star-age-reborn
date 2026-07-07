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
            MakeStatBar(resourcePanel.transform, "Cargo Bar", new Vector2(14f, -36f), new Color(1f, 0.62f, 0.16f), out cargoBarFill, out cargoBarText);
            resourceText = MakeText("Resources", resourcePanel.transform, new Vector2(14f, -64f), new Vector2(302f, 20f), TextAnchor.UpperLeft, 11);
            var statusPanel = MakePanel("Status HUD", new Vector2(-178f, -14f), new Vector2(390f, 48f), new Vector2(1f, 1f), new Vector2(1f, 1f));
            statusText = MakeText("Status", statusPanel.transform, new Vector2(-14f, -9f), new Vector2(362f, 32f), TextAnchor.UpperRight, 13);
            MakeMinimap();

            planetPanel = MakePanel("Planet Controls", new Vector2(0f, 18f), new Vector2(760f, 54f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            MakeButton("Crafting", planetPanel.transform, new Vector2(-285f, 27f), new Vector2(128f, 34f), ToggleCrafting);
            MakeButton("Market", planetPanel.transform, new Vector2(-145f, 27f), new Vector2(118f, 34f), ToggleMarket);
            MakeButton("Quest Board", planetPanel.transform, new Vector2(0f, 27f), new Vector2(138f, 34f), ToggleQuests);
            MakeButton("Shipyard", planetPanel.transform, new Vector2(145f, 27f), new Vector2(126f, 34f), ToggleShipyard);
            MakeButton("Fly To Space", planetPanel.transform, new Vector2(292f, 27f), new Vector2(146f, 34f), GameManager.Instance.EnterSpaceMode);

            spacePanel = MakePanel("Space Controls", new Vector2(0f, 12f), new Vector2(760f, 46f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            MakeButton("Return Planet", spacePanel.transform, new Vector2(-270f, 23f), new Vector2(150f, 30f), GameManager.Instance.EnterPlanetMode);
            MakeButton("Galaxy", spacePanel.transform, new Vector2(-110f, 23f), new Vector2(120f, 30f), ToggleGalaxyMap);
            MakeButton("Save", spacePanel.transform, new Vector2(24f, 23f), new Vector2(90f, 30f), GameManager.Instance.SaveGame);
            MakeButton("Market", spacePanel.transform, new Vector2(144f, 23f), new Vector2(110f, 30f), ToggleMarket);
            MakeButton("Quests", spacePanel.transform, new Vector2(270f, 23f), new Vector2(110f, 30f), ToggleQuests);
            spaceStatsPanel = MakePanel("Space Status HUD", new Vector2(16f, -142f), new Vector2(320f, 106f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            spaceText = MakeText("Space HUD", spaceStatsPanel.transform, new Vector2(14f, -10f), new Vector2(292f, 86f), TextAnchor.UpperLeft, 13);

            rightRailPanel = MakePanel("Space Right Rail", new Vector2(-14f, -34f), new Vector2(58f, 324f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
            MakeButton("Map", rightRailPanel.transform, new Vector2(0f, 116f), new Vector2(44f, 36f), ToggleGalaxyMap);
            MakeButton("Home", rightRailPanel.transform, new Vector2(0f, 62f), new Vector2(44f, 36f), GameManager.Instance.EnterPlanetMode);
            MakeButton("Fix", rightRailPanel.transform, new Vector2(0f, 8f), new Vector2(44f, 36f), TryRepairShip);
            MakeButton("Go", rightRailPanel.transform, new Vector2(0f, -46f), new Vector2(44f, 36f), TryBoostShip);

            detailPanel = MakePanel("Building Details", Vector2.zero, new Vector2(540f, 430f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            craftingPanel = MakePanel("Crafting", Vector2.zero, new Vector2(520f, 360f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            marketPanel = MakePanel("Market", Vector2.zero, new Vector2(520f, 430f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            questPanel = MakePanel("Quest Board", Vector2.zero, new Vector2(700f, 520f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            shipyardPanel = MakePanel("Shipyard", Vector2.zero, new Vector2(560f, 460f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            galaxyPanel = MakePanel("Galaxy Map", Vector2.zero, new Vector2(760f, 520f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

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
            resourceText.text = $"Stone {wallet.stone}   U {wallet.uranium}   Ice {wallet.ice}   Metal {wallet.metal}   Fuel {wallet.fuel}   Coins {wallet.coins}";
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            ShipStats ship = ShipStats.For(data.shipId);
            WeaponStats weapon = WeaponStats.For(data.weaponId);
            RefreshTopBars(data, ship);
            statusText.text = $"{GameManager.Instance.Mode}   {ship.label}   {weapon.label}";
            if (GameManager.Instance.Mode == GameMode.Space) statusText.text = $"Orion System   {ship.label}   {weapon.label}";
            if (minimapPanel != null) minimapPanel.SetActive(GameManager.Instance.Mode == GameMode.Space);
            if (GameManager.Instance.Mode == GameMode.Space) DrawMinimap();

            if (spaceText != null && spaceText.gameObject.activeSelf)
            {
                ShipController player = GameManager.Instance.Space.PlayerShip;
                int hp = player != null ? player.Hp : data.shipHp;
                int maxHp = player != null ? player.MaxHp : ship.hp + data.armorModules * 50;
                spaceText.text = $"Ship: {ship.label}\nHP: {hp}/{maxHp}\nCargo: {GameManager.Instance.Space.CargoUsed()}/{GameManager.Instance.Space.CargoCapacity()}\nLMB/Space fire  RMB fly";
            }
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
                    string label = $"{buildDef.label}  Cost S{buildDef.stoneCost} U{buildDef.uraniumCost} I{buildDef.iceCost} M{buildDef.metalCost}";
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
            AddTitle(craftingPanel, "Crafting");
            AddText(craftingPanel, "Craft advanced items instantly for this MVP. Factories automate the same recipes over time.", new Vector2(0f, 110f), new Vector2(440f, 60f));
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
            AddTitle(marketPanel, "Market / Auction");
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
            AddTitle(questPanel, "Quest Board");
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
            AddTitle(shipyardPanel, "Shop / Shipyard");
            MakeButton("Buy Scout - Free", shipyardPanel.transform, new Vector2(0f, 125f), new Vector2(360f, 34f), () => { GameManager.Instance.Shipyard.BuyShip("scout"); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Buy Fighter - 1200 coins", shipyardPanel.transform, new Vector2(0f, 85f), new Vector2(360f, 34f), () => { GameManager.Instance.Shipyard.BuyShip("fighter"); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Buy Destroyer - 3500 coins", shipyardPanel.transform, new Vector2(0f, 45f), new Vector2(360f, 34f), () => { GameManager.Instance.Shipyard.BuyShip("destroyer"); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Buy Heavy Laser - 900 coins", shipyardPanel.transform, new Vector2(0f, 5f), new Vector2(360f, 34f), () => { GameManager.Instance.Shipyard.BuyWeapon("heavy"); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Engine Upgrade +2 speed - 650 coins", shipyardPanel.transform, new Vector2(0f, -35f), new Vector2(360f, 34f), () => { GameManager.Instance.Shipyard.BuyEngine(); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Cargo Module +20 cargo - 700 coins", shipyardPanel.transform, new Vector2(0f, -75f), new Vector2(360f, 34f), () => { GameManager.Instance.Shipyard.BuyCargoModule(); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Armor Module +50 HP - 800 coins", shipyardPanel.transform, new Vector2(0f, -115f), new Vector2(360f, 34f), () => { GameManager.Instance.Shipyard.BuyArmorModule(); ToggleShipyard(); ToggleShipyard(); });
            MakeButton("Close", shipyardPanel.transform, new Vector2(0f, -185f), new Vector2(180f, 34f), HidePopups);
        }

        void ToggleGalaxyMap()
        {
            bool open = !galaxyPanel.activeSelf;
            HidePopups();
            galaxyPanel.SetActive(open);
            if (!open) return;
            Clear(galaxyPanel.transform);
            AddTitle(galaxyPanel, "Galaxy Map");
            AddText(galaxyPanel, "Route network prototype. Orion is playable now; nearby systems are visual targets like the Starkus map.", new Vector2(-310f, 188f), new Vector2(620f, 48f));

            Vector2 orion = new Vector2(0f, 58f);
            Vector2 aurora = new Vector2(-230f, 120f);
            Vector2 asgard = new Vector2(220f, 132f);
            Vector2 sparta = new Vector2(-210f, -92f);
            Vector2 frontier = new Vector2(240f, -118f);
            AddGalaxyConnection(orion, aurora);
            AddGalaxyConnection(orion, asgard);
            AddGalaxyConnection(orion, sparta);
            AddGalaxyConnection(orion, frontier);
            AddGalaxySystem("Orion", "Home star", orion, new Color(1f, 0.84f, 0.25f), true);
            AddGalaxySystem("Aurora", "Ice fields", aurora, new Color(0.32f, 0.82f, 1f), false);
            AddGalaxySystem("Asgard", "Trade hub", asgard, new Color(0.62f, 1f, 0.62f), false);
            AddGalaxySystem("Sparta", "Pirate danger", sparta, new Color(1f, 0.22f, 0.18f), false);
            AddGalaxySystem("Frontier", "Asteroid belt", frontier, new Color(0.82f, 0.68f, 1f), false);
            AddText(galaxyPanel, "Tip: use the space view to mine asteroids, fight pirates, collect loot, and return home to upgrade the colony.", new Vector2(-310f, -204f), new Vector2(620f, 44f));
            MakeButton("Close", galaxyPanel.transform, new Vector2(0f, -236f), new Vector2(180f, 34f), HidePopups);
        }

        void AddGalaxySystem(string title, string subtitle, Vector2 position, Color color, bool active)
        {
            var marker = new GameObject(title + " Marker", typeof(Image));
            marker.transform.SetParent(galaxyPanel.transform, false);
            marker.GetComponent<Image>().color = color;
            RectTransform rect = marker.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = active ? new Vector2(24f, 24f) : new Vector2(18f, 18f);
            AddText(galaxyPanel, $"{title}\n{subtitle}", position + new Vector2(18f, 16f), new Vector2(160f, 48f));
        }

        void AddGalaxyConnection(Vector2 a, Vector2 b)
        {
            var line = new GameObject("Galaxy Route", typeof(Image));
            line.transform.SetParent(galaxyPanel.transform, false);
            line.GetComponent<Image>().color = new Color(0.18f, 0.55f, 0.95f, 0.38f);
            RectTransform rect = line.GetComponent<RectTransform>();
            Vector2 delta = b - a;
            rect.anchoredPosition = a + delta * 0.5f;
            rect.sizeDelta = new Vector2(delta.magnitude, 3f);
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
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
            int cargo = GameManager.Instance.Space != null ? GameManager.Instance.Space.CargoUsed() : 0;
            int cargoMax = GameManager.Instance.Space != null ? GameManager.Instance.Space.CargoCapacity() : ship.cargo + data.cargoModules * 20;
            SetBar(hpBarFill, hp, maxHp, 216f);
            SetBar(cargoBarFill, cargo, cargoMax, 216f);
            if (hpBarText != null) hpBarText.text = $"{hp}/{maxHp}";
            if (cargoBarText != null) cargoBarText.text = $"Cargo {cargo}/{cargoMax}";
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
            image.color = new Color(0.012f, 0.028f, 0.055f, 0.84f);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return panel;
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

        void MakeButton(string label, Transform parent, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(label, typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.08f, 0.24f, 0.44f, 0.96f);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(action);

            Text labelText = MakeText("Label", buttonObject.transform, Vector2.zero, size, TextAnchor.MiddleCenter, 14);
            labelText.text = label;
            labelText.rectTransform.anchorMin = Vector2.zero;
            labelText.rectTransform.anchorMax = Vector2.one;
            labelText.rectTransform.offsetMin = Vector2.zero;
            labelText.rectTransform.offsetMax = Vector2.zero;
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
        }

        void Clear(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}

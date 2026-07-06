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
        GameObject planetPanel;
        GameObject detailPanel;
        GameObject craftingPanel;
        GameObject marketPanel;
        GameObject questPanel;
        GameObject shipyardPanel;
        GameObject spacePanel;
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

            var resourcePanel = MakePanel("Resource HUD", new Vector2(16f, -16f), new Vector2(320f, 116f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            resourceText = MakeText("Resources", resourcePanel.transform, new Vector2(14f, -10f), new Vector2(292f, 96f), TextAnchor.UpperLeft, 14);
            var statusPanel = MakePanel("Status HUD", new Vector2(-16f, -16f), new Vector2(360f, 92f), new Vector2(1f, 1f), new Vector2(1f, 1f));
            statusText = MakeText("Status", statusPanel.transform, new Vector2(-14f, -10f), new Vector2(332f, 72f), TextAnchor.UpperRight, 13);

            planetPanel = MakePanel("Planet Controls", new Vector2(0f, 18f), new Vector2(760f, 54f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            MakeButton("Crafting", planetPanel.transform, new Vector2(-285f, 27f), new Vector2(128f, 34f), ToggleCrafting);
            MakeButton("Market", planetPanel.transform, new Vector2(-145f, 27f), new Vector2(118f, 34f), ToggleMarket);
            MakeButton("Quest Board", planetPanel.transform, new Vector2(0f, 27f), new Vector2(138f, 34f), ToggleQuests);
            MakeButton("Shipyard", planetPanel.transform, new Vector2(145f, 27f), new Vector2(126f, 34f), ToggleShipyard);
            MakeButton("Fly To Space", planetPanel.transform, new Vector2(292f, 27f), new Vector2(146f, 34f), GameManager.Instance.EnterSpaceMode);

            spacePanel = MakePanel("Space Controls", new Vector2(0f, 18f), new Vector2(360f, 54f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            MakeButton("Return Planet", spacePanel.transform, new Vector2(-68f, 27f), new Vector2(150f, 34f), GameManager.Instance.EnterPlanetMode);
            MakeButton("Save", spacePanel.transform, new Vector2(92f, 27f), new Vector2(90f, 34f), GameManager.Instance.SaveGame);
            spaceText = MakeText("Space HUD", canvas.transform, new Vector2(-16f, -118f), new Vector2(340f, 112f), TextAnchor.UpperRight, 13);

            detailPanel = MakePanel("Building Details", Vector2.zero, new Vector2(540f, 430f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            craftingPanel = MakePanel("Crafting", Vector2.zero, new Vector2(520f, 360f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            marketPanel = MakePanel("Market", Vector2.zero, new Vector2(520f, 430f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            questPanel = MakePanel("Quest Board", Vector2.zero, new Vector2(700f, 520f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            shipyardPanel = MakePanel("Shipyard", Vector2.zero, new Vector2(560f, 460f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

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
            spaceText.gameObject.SetActive(false);
            HidePopups();
            Refresh();
        }

        public void ShowSpaceHud()
        {
            planetPanel.SetActive(false);
            spacePanel.SetActive(true);
            spaceText.gameObject.SetActive(true);
            HidePopups();
            Refresh();
        }

        public void Refresh()
        {
            if (resourceText == null || GameManager.Instance == null || GameManager.Instance.Resources == null) return;
            resourceText.text = GameManager.Instance.Resources.Summary();
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            ShipStats ship = ShipStats.For(data.shipId);
            WeaponStats weapon = WeaponStats.For(data.weaponId);
            statusText.text = $"{GameManager.Instance.Mode} View\n{ship.label} / {weapon.label}\nWASD + mouse, LMB fire";

            if (spaceText != null && spaceText.gameObject.activeSelf)
            {
                ShipController player = GameManager.Instance.Space.PlayerShip;
                int hp = player != null ? player.Hp : data.shipHp;
                int maxHp = player != null ? player.MaxHp : ship.hp + data.armorModules * 50;
                spaceText.text = $"HP: {hp}/{maxHp}\nCargo: {GameManager.Instance.Space.CargoUsed()}/{GameManager.Instance.Space.CargoCapacity()}\nCoins: {GameManager.Instance.Resources.Wallet.coins}\nActive quests update automatically.";
            }
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

        void HidePopups()
        {
            if (detailPanel != null) detailPanel.SetActive(false);
            if (craftingPanel != null) craftingPanel.SetActive(false);
            if (marketPanel != null) marketPanel.SetActive(false);
            if (questPanel != null) questPanel.SetActive(false);
            if (shipyardPanel != null) shipyardPanel.SetActive(false);
        }

        GameObject MakePanel(string name, Vector2 position, Vector2 size, Vector2 anchor, Vector2 pivot)
        {
            var panel = new GameObject(name, typeof(Image));
            panel.transform.SetParent(canvas.transform, false);
            Image image = panel.GetComponent<Image>();
            image.color = new Color(0.015f, 0.025f, 0.045f, 0.78f);
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
            image.color = new Color(0.08f, 0.22f, 0.38f, 0.94f);
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

        void Clear(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}

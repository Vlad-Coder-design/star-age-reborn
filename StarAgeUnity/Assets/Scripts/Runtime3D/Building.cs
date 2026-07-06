using UnityEngine;

namespace StarAge3D
{
    public class Building : MonoBehaviour
    {
        public BuildingType Type { get; private set; }
        public BuildingSave Save { get; private set; }

        public void Init(BuildingType type, BuildingSave save)
        {
            Type = type;
            Save = save;
            Save.type = type;
        }

        public void Tick(float deltaTime)
        {
            if (Type == BuildingType.Empty || Type == BuildingType.Warehouse || Type == BuildingType.Market) return;
            BuildingDefinition def = BuildingManager.Definition(Type);
            if (Save.stored >= def.maxStorage) return;

            Save.timer += deltaTime;
            while (Save.timer >= def.seconds && Save.stored < def.maxStorage)
            {
                Save.timer -= def.seconds;
                if (!TryConsumeInputs(Type)) break;
                Save.stored++;
            }
        }

        bool TryConsumeInputs(BuildingType type)
        {
            ResourceManager resources = GameManager.Instance.Resources;
            if (type == BuildingType.FuelFactory)
            {
                if (resources.Wallet.uranium < 2 || resources.Wallet.ice < 1) return false;
                resources.Wallet.uranium -= 2;
                resources.Wallet.ice -= 1;
            }
            else if (type == BuildingType.MetalFactory)
            {
                if (resources.Wallet.stone < 2) return false;
                resources.Wallet.stone -= 2;
            }
            else if (type == BuildingType.RepairKitFactory)
            {
                if (resources.Wallet.metal < 1 || resources.Wallet.uranium < 1) return false;
                resources.Wallet.metal -= 1;
                resources.Wallet.uranium -= 1;
            }
            else if (type == BuildingType.BoosterFactory)
            {
                if (resources.Wallet.fuel < 1 || resources.Wallet.ice < 1) return false;
                resources.Wallet.fuel -= 1;
                resources.Wallet.ice -= 1;
            }

            return true;
        }

        public void Collect()
        {
            if (Save.stored <= 0) return;
            BuildingDefinition def = BuildingManager.Definition(Type);
            if (!GameManager.Instance.Inventory.HasRoom(GameManager.Instance.Resources.Wallet, Save.stored)) return;
            GameManager.Instance.Resources.Add(def.output, Save.stored);
            if (def.output == ResourceType.Stone) GameManager.Instance.Quests.AddProgress(QuestType.MineStone, Save.stored);
            if (def.output == ResourceType.Ice) GameManager.Instance.Quests.AddProgress(QuestType.CollectIce, Save.stored);
            Save.stored = 0;
            GameManager.Instance.SaveGame();
        }
    }
}

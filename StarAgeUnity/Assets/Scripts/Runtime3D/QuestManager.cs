using System.Collections.Generic;
using UnityEngine;

namespace StarAge3D
{
    public class QuestManager : MonoBehaviour
    {
        public List<QuestSave> Quests { get; private set; }

        public void Init(List<QuestSave> quests)
        {
            Quests = quests ?? new List<QuestSave>();
            if (Quests.Count == 0)
            {
                Quests.Add(new QuestSave { type = QuestType.DestroyPirates });
                Quests.Add(new QuestSave { type = QuestType.MineStone });
                Quests.Add(new QuestSave { type = QuestType.DeliverFuel });
                Quests.Add(new QuestSave { type = QuestType.CollectIce });
            }
        }

        public void WriteSave()
        {
            GameManager.Instance.Save.Data.quests = Quests;
        }

        public string Title(QuestType type)
        {
            switch (type)
            {
                case QuestType.DestroyPirates: return "Pirate bounty x3";
                case QuestType.MineStone: return "Stone shipment x9";
                case QuestType.DeliverFuel: return "Fuel delivery x5";
                case QuestType.CollectIce: return "Ice harvest x7";
                default: return type.ToString();
            }
        }

        public string Description(QuestType type)
        {
            switch (type)
            {
                case QuestType.DestroyPirates: return "Local traders are under attack. Destroy 3 pirate ships in any system. Reward: 560 credits.";
                case QuestType.MineStone: return "A construction guild needs 9 stone. Mine asteroids and deliver the cargo. Reward: 148 credits.";
                case QuestType.DeliverFuel: return "A frontier convoy needs 5 fuel. Load or craft fuel and deliver it from Novara. Reward: 260 credits.";
                case QuestType.CollectIce: return "Hydroponics farms request 7 ice. Break comets and deliver the cargo. Reward: 100 credits.";
                default: return "";
            }
        }

        public int Target(QuestType type)
        {
            if (type == QuestType.DestroyPirates) return 3;
            if (type == QuestType.MineStone) return 9;
            if (type == QuestType.DeliverFuel) return 5;
            if (type == QuestType.CollectIce) return 7;
            return 1;
        }

        public void Accept(QuestSave quest)
        {
            quest.accepted = true;
            GameManager.Instance.SaveGame();
        }

        public void AddProgress(QuestType type, int amount)
        {
            foreach (QuestSave quest in Quests)
            {
                if (quest.type == type && quest.accepted && !quest.completed)
                {
                    quest.progress = Mathf.Min(Target(type), quest.progress + amount);
                }
            }
            GameManager.Instance.UI.Refresh();
        }

        public bool Complete(QuestSave quest)
        {
            if (!quest.accepted || quest.completed || quest.progress < Target(quest.type)) return false;

            ResourceWallet wallet = GameManager.Instance.Resources.Wallet;
            if (quest.type == QuestType.DeliverFuel && wallet.fuel < 5) return false;
            if (quest.type == QuestType.DeliverFuel) wallet.fuel -= 5;

            if (quest.type == QuestType.DestroyPirates)
            {
                wallet.coins += 560;
            }
            else if (quest.type == QuestType.MineStone)
            {
                wallet.coins += 148;
            }
            else if (quest.type == QuestType.DeliverFuel)
            {
                wallet.coins += 260;
            }
            else if (quest.type == QuestType.CollectIce)
            {
                wallet.coins += 100;
            }

            quest.completed = true;
            GameManager.Instance.SaveGame();
            return true;
        }
    }
}

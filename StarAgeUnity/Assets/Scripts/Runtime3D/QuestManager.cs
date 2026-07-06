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
                case QuestType.DestroyPirates: return "Destroy 3 pirates";
                case QuestType.MineStone: return "Mine 10 stone from asteroids";
                case QuestType.DeliverFuel: return "Deliver 5 fuel";
                case QuestType.CollectIce: return "Collect 8 ice from comets";
                default: return type.ToString();
            }
        }

        public string Description(QuestType type)
        {
            switch (type)
            {
                case QuestType.DestroyPirates: return "Clear pirate raiders from nearby space. Reward: 100 coins and 2 uranium.";
                case QuestType.MineStone: return "Mine asteroid stone for colony expansion. Reward: 80 coins.";
                case QuestType.DeliverFuel: return "Deliver 5 fuel to a frontier convoy. Reward: 150 coins.";
                case QuestType.CollectIce: return "Collect comet ice for life support. Reward: 90 coins.";
                default: return "";
            }
        }

        public int Target(QuestType type)
        {
            if (type == QuestType.DestroyPirates) return 3;
            if (type == QuestType.MineStone) return 10;
            if (type == QuestType.DeliverFuel) return 5;
            if (type == QuestType.CollectIce) return 8;
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
                wallet.coins += 100;
                wallet.uranium += 2;
            }
            else if (quest.type == QuestType.MineStone)
            {
                wallet.coins += 80;
            }
            else if (quest.type == QuestType.DeliverFuel)
            {
                wallet.coins += 150;
            }
            else if (quest.type == QuestType.CollectIce)
            {
                wallet.coins += 90;
            }

            quest.completed = true;
            GameManager.Instance.SaveGame();
            return true;
        }
    }
}

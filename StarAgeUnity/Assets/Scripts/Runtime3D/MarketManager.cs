using UnityEngine;

namespace StarAge3D
{
    public class MarketManager : MonoBehaviour
    {
        public void Init()
        {
        }

        public int Price(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Ice: return 2;
                case ResourceType.Stone: return 3;
                case ResourceType.Uranium: return 5;
                case ResourceType.Metal: return 15;
                case ResourceType.Fuel: return 20;
                case ResourceType.RepairKits: return 50;
                case ResourceType.Boosters: return 50;
                default: return 0;
            }
        }

        public bool Sell(ResourceType type, int amount)
        {
            if (amount <= 0) return false;
            ResourceWallet wallet = GameManager.Instance.Resources.Wallet;
            if (wallet.Get(type) < amount) return false;
            wallet.Add(type, -amount);
            wallet.coins += Price(type) * amount;
            GameManager.Instance.SaveGame();
            return true;
        }

        public void SellAll()
        {
            Sell(ResourceType.Ice, GameManager.Instance.Resources.Wallet.ice);
            Sell(ResourceType.Stone, GameManager.Instance.Resources.Wallet.stone);
            Sell(ResourceType.Uranium, GameManager.Instance.Resources.Wallet.uranium);
            Sell(ResourceType.Metal, GameManager.Instance.Resources.Wallet.metal);
            Sell(ResourceType.Fuel, GameManager.Instance.Resources.Wallet.fuel);
            Sell(ResourceType.RepairKits, GameManager.Instance.Resources.Wallet.repairKits);
            Sell(ResourceType.Boosters, GameManager.Instance.Resources.Wallet.boosters);
        }
    }
}

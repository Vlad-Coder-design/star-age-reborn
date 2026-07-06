using UnityEngine;

namespace StarAge3D
{
    public class CraftingManager : MonoBehaviour
    {
        public void Init()
        {
        }

        public bool CraftMetal()
        {
            ResourceWallet wallet = GameManager.Instance.Resources.Wallet;
            if (wallet.stone < 2) return false;
            wallet.stone -= 2;
            wallet.metal += 1;
            GameManager.Instance.SaveGame();
            return true;
        }

        public bool CraftFuel()
        {
            ResourceWallet wallet = GameManager.Instance.Resources.Wallet;
            if (wallet.uranium < 2 || wallet.ice < 1) return false;
            wallet.uranium -= 2;
            wallet.ice -= 1;
            wallet.fuel += 1;
            GameManager.Instance.SaveGame();
            return true;
        }

        public bool CraftRepairKit()
        {
            ResourceWallet wallet = GameManager.Instance.Resources.Wallet;
            if (wallet.metal < 1 || wallet.uranium < 1) return false;
            wallet.metal -= 1;
            wallet.uranium -= 1;
            wallet.repairKits += 1;
            GameManager.Instance.SaveGame();
            return true;
        }

        public bool CraftBooster()
        {
            ResourceWallet wallet = GameManager.Instance.Resources.Wallet;
            if (wallet.fuel < 1 || wallet.ice < 1) return false;
            wallet.fuel -= 1;
            wallet.ice -= 1;
            wallet.boosters += 1;
            GameManager.Instance.SaveGame();
            return true;
        }
    }
}

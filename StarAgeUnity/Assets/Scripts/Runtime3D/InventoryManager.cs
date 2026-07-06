using UnityEngine;

namespace StarAge3D
{
    public class InventoryManager : MonoBehaviour
    {
        public int Capacity { get; private set; } = 300;

        public void Init(int capacity)
        {
            Capacity = Mathf.Max(20, capacity);
        }

        public int Used(ResourceWallet wallet)
        {
            return wallet.stone + wallet.uranium + wallet.ice + wallet.metal + wallet.fuel + wallet.repairKits + wallet.boosters;
        }

        public bool HasRoom(ResourceWallet wallet, int amount)
        {
            return Used(wallet) + amount <= Capacity;
        }
    }
}

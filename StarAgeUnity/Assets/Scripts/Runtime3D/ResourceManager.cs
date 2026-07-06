using System.Text;
using UnityEngine;

namespace StarAge3D
{
    public class ResourceManager : MonoBehaviour
    {
        public ResourceWallet Wallet { get; private set; }

        public void Init(ResourceWallet wallet)
        {
            Wallet = wallet ?? new ResourceWallet();
        }

        public bool Spend(ResourceType type, int amount)
        {
            bool ok = Wallet.Spend(type, amount);
            GameManager.Instance.UI.Refresh();
            return ok;
        }

        public void Add(ResourceType type, int amount)
        {
            Wallet.Add(type, amount);
            GameManager.Instance.UI.Refresh();
        }

        public string Summary()
        {
            var b = new StringBuilder();
            b.AppendLine($"Coins: {Wallet.coins}");
            b.AppendLine($"Stone: {Wallet.stone}   Uranium: {Wallet.uranium}   Ice: {Wallet.ice}");
            b.AppendLine($"Metal: {Wallet.metal}   Fuel: {Wallet.fuel}");
            b.AppendLine($"Repair Kits: {Wallet.repairKits}   Boosters: {Wallet.boosters}");
            return b.ToString();
        }
    }
}

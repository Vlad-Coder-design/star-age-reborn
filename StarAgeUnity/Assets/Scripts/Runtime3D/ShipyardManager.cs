using UnityEngine;

namespace StarAge3D
{
    public class ShipyardManager : MonoBehaviour
    {
        public bool BuyShip(string shipId)
        {
            ShipStats stats = ShipStats.For(shipId);
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            if (data.shipId == shipId) return true;
            if (GameManager.Instance.Resources.Wallet.coins < stats.cost) return false;
            GameManager.Instance.Resources.Wallet.coins -= stats.cost;
            data.shipId = shipId;
            data.shipHp = stats.hp + data.armorModules * 50;
            GameManager.Instance.Space.RebuildPlayerShip();
            GameManager.Instance.SaveGame();
            return true;
        }

        public bool BuyWeapon(string weaponId)
        {
            WeaponStats weapon = WeaponStats.For(weaponId);
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            if (data.weaponId == weaponId) return true;
            if (GameManager.Instance.Resources.Wallet.coins < weapon.cost) return false;
            GameManager.Instance.Resources.Wallet.coins -= weapon.cost;
            data.weaponId = weaponId;
            GameManager.Instance.SaveGame();
            return true;
        }

        public bool BuyEngine()
        {
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            if (GameManager.Instance.Resources.Wallet.coins < 650) return false;
            GameManager.Instance.Resources.Wallet.coins -= 650;
            data.engineLevel += 1;
            GameManager.Instance.SaveGame();
            return true;
        }

        public bool BuyCargoModule()
        {
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            if (GameManager.Instance.Resources.Wallet.coins < 700) return false;
            GameManager.Instance.Resources.Wallet.coins -= 700;
            data.cargoModules += 1;
            GameManager.Instance.SaveGame();
            return true;
        }

        public bool BuyArmorModule()
        {
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            if (GameManager.Instance.Resources.Wallet.coins < 800) return false;
            GameManager.Instance.Resources.Wallet.coins -= 800;
            data.armorModules += 1;
            data.shipHp += 50;
            GameManager.Instance.SaveGame();
            return true;
        }
    }
}

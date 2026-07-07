using UnityEngine;

namespace StarAge3D
{
    public class EnemyAI : MonoBehaviour
    {
        ShipController ship;

        public void Init(ShipController controller)
        {
            ship = controller;
        }

        void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.Mode != GameMode.Space) return;
            ShipController player = GameManager.Instance.Space.PlayerShip;
            if (player == null || ship == null) return;

            Vector3 toPlayer = player.transform.position - transform.position;
            float distance = toPlayer.magnitude;
            if (ship.Hp < 24)
            {
                ship.EnemyMove(-toPlayer, 6.4f);
                return;
            }

            if (distance < 30f && distance > 12f) ship.EnemyMove(toPlayer, 5.8f);
            if (distance < 20f) ship.EnemyShootAt(player);
        }
    }
}

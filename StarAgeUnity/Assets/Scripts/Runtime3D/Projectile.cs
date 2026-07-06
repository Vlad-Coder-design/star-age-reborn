using UnityEngine;

namespace StarAge3D
{
    public class Projectile : MonoBehaviour
    {
        ShipController owner;
        Vector3 direction;
        int damage;
        float life = 2.2f;

        public void Init(ShipController ownerShip, Vector3 dir, int amount)
        {
            owner = ownerShip;
            direction = dir.normalized;
            damage = amount;
        }

        void Update()
        {
            transform.position += direction * 28f * Time.deltaTime;
            life -= Time.deltaTime;
            if (life <= 0f) Destroy(gameObject);
        }

        void OnTriggerEnter(Collider other)
        {
            ShipController ship = other.GetComponent<ShipController>();
            if (ship != null && ship != owner && ship.IsEnemy != owner.IsEnemy)
            {
                ship.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            MiningObject mining = other.GetComponent<MiningObject>();
            if (!owner.IsEnemy && mining != null)
            {
                mining.Hit(damage);
                Destroy(gameObject);
            }
        }
    }
}

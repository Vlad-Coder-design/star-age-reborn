using UnityEngine;

namespace StarAge3D
{
    public class ShipController : MonoBehaviour
    {
        public bool IsEnemy { get; private set; }
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }

        float fireTimer;
        float boosterTimer;
        Vector3 velocity;

        public void Init(bool enemy)
        {
            IsEnemy = enemy;
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            ShipStats stats = enemy ? ShipStats.For("fighter") : ShipStats.For(data.shipId);
            MaxHp = stats.hp + (!enemy ? data.armorModules * 50 : 0);
            Hp = enemy ? 80 : Mathf.Clamp(data.shipHp, 1, MaxHp);
        }

        void Update()
        {
            if (!IsEnemy) UpdatePlayer();
        }

        void UpdatePlayer()
        {
            StarAgeSaveData data = GameManager.Instance.Save.Data;
            ShipStats stats = ShipStats.For(data.shipId);
            float speed = stats.speed + data.engineLevel * 2f + (boosterTimer > 0f ? 4f : 0f);
            boosterTimer = Mathf.Max(0f, boosterTimer - Time.deltaTime);

            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            input = Vector3.ClampMagnitude(input, 1f);
            velocity = Vector3.Lerp(velocity, input * speed, 8f * Time.deltaTime);
            transform.position += velocity * Time.deltaTime;

            AimAtMouse();
            fireTimer -= Time.deltaTime;
            if (Input.GetMouseButton(0)) TryShoot();
            if (Input.GetKeyDown(KeyCode.Space)) UseBooster();
            if (Input.GetKeyDown(KeyCode.R)) UseRepairKit();
        }

        public void EnemyMove(Vector3 direction, float speed)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f)
            {
                transform.position += direction.normalized * speed * Time.deltaTime;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction.normalized, Vector3.up), 8f * Time.deltaTime);
            }
        }

        public void EnemyShootAt(ShipController target)
        {
            if (target == null) return;
            fireTimer -= Time.deltaTime;
            if (fireTimer > 0f) return;
            fireTimer = 0.9f;
            Vector3 direction = (target.transform.position - transform.position).normalized;
            GameManager.Instance.Space.SpawnProjectile(this, transform.position + direction * 1.1f, direction, 8, Color.red);
        }

        void AimAtMouse()
        {
            Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float enter;
            if (!plane.Raycast(ray, out enter)) return;
            Vector3 point = ray.GetPoint(enter);
            Vector3 direction = point - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction.normalized, Vector3.up), 12f * Time.deltaTime);
            }
        }

        void TryShoot()
        {
            WeaponStats weapon = WeaponStats.For(GameManager.Instance.Save.Data.weaponId);
            if (fireTimer > 0f) return;
            fireTimer = weapon.fireRate;
            GameManager.Instance.Space.SpawnProjectile(this, transform.position + transform.forward * 1.2f, transform.forward, weapon.damage, new Color(0.4f, 0.9f, 1f));
        }

        void UseBooster()
        {
            if (boosterTimer > 0f) return;
            if (!GameManager.Instance.Resources.Spend(ResourceType.Boosters, 1)) return;
            boosterTimer = 30f;
        }

        void UseRepairKit()
        {
            if (Hp >= MaxHp) return;
            if (!GameManager.Instance.Resources.Spend(ResourceType.RepairKits, 1)) return;
            Hp = Mathf.Min(MaxHp, Hp + 20);
            GameManager.Instance.Save.Data.shipHp = Hp;
            GameManager.Instance.SaveGame();
        }

        public void TakeDamage(int amount)
        {
            Hp = Mathf.Max(0, Hp - amount);
            if (!IsEnemy) GameManager.Instance.Save.Data.shipHp = Hp;
            if (Hp <= 0)
            {
                GameManager.Instance.Space.SpawnShipExplosion(transform.position, IsEnemy);
                if (IsEnemy)
                {
                    EnemyAI ai = GetComponent<EnemyAI>();
                    if (ai != null) GameManager.Instance.Space.PirateDestroyed(ai);
                    Destroy(gameObject);
                }
                else
                {
                    Hp = MaxHp;
                    transform.position = new Vector3(0f, 0f, -8f);
                    GameManager.Instance.Save.Data.shipHp = Hp;
                }
            }
            GameManager.Instance.UI.Refresh();
        }
    }
}

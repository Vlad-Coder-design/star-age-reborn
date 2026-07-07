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
        Vector3 moveTarget;
        bool hasMoveTarget;

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
            float engineMultiplier = data.engineLevel > 0 ? 1.25f : 1f;
            float speed = stats.speed * engineMultiplier * (boosterTimer > 0f ? 1.35f : 1f);
            boosterTimer = Mathf.Max(0f, boosterTimer - Time.deltaTime);

            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            input = Vector3.ClampMagnitude(input, 1f);

            if (input.sqrMagnitude > 0.01f)
            {
                hasMoveTarget = false;
                velocity = Vector3.Lerp(velocity, input * speed, 8f * Time.deltaTime);
            }
            else if (hasMoveTarget)
            {
                Vector3 toTarget = moveTarget - transform.position;
                toTarget.y = 0f;
                float distance = toTarget.magnitude;
                if (distance < 0.45f)
                {
                    hasMoveTarget = false;
                    velocity = Vector3.Lerp(velocity, Vector3.zero, 7f * Time.deltaTime);
                }
                else
                {
                    float approachSpeed = Mathf.Min(speed, Mathf.Max(1.4f, distance * 1.8f));
                    velocity = Vector3.Lerp(velocity, toTarget.normalized * approachSpeed, 4.8f * Time.deltaTime);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toTarget.normalized, Vector3.up), 8f * Time.deltaTime);
                }
            }
            else
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, 4f * Time.deltaTime);
            }

            transform.position += velocity * Time.deltaTime;

            AimAtMouse();
            fireTimer -= Time.deltaTime;
            if (Input.GetMouseButtonDown(1)) SetMoveTargetFromMouse();
            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) TryShoot();
            if (Input.GetKeyDown(KeyCode.LeftShift)) TryUseBooster();
            if (Input.GetKeyDown(KeyCode.R)) TryUseRepairKit();
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

        void SetMoveTargetFromMouse()
        {
            Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float enter;
            if (!plane.Raycast(ray, out enter)) return;
            moveTarget = ray.GetPoint(enter);
            moveTarget.y = 0f;
            hasMoveTarget = true;
        }

        void TryShoot()
        {
            WeaponStats weapon = WeaponStats.For(GameManager.Instance.Save.Data.weaponId);
            if (fireTimer > 0f) return;
            fireTimer = weapon.fireRate;
            GameManager.Instance.Space.SpawnProjectile(this, transform.position + transform.forward * 1.2f, transform.forward, weapon.damage, new Color(0.4f, 0.9f, 1f));
        }

        public bool TryUseBooster()
        {
            if (boosterTimer > 0f) return false;
            if (!GameManager.Instance.Resources.Spend(ResourceType.Boosters, 1)) return false;
            boosterTimer = 30f;
            GameManager.Instance.UI.Refresh();
            return true;
        }

        public bool TryUseRepairKit()
        {
            if (Hp >= MaxHp) return false;
            if (!GameManager.Instance.Resources.Spend(ResourceType.RepairKits, 1)) return false;
            Hp = Mathf.Min(MaxHp, Hp + 20);
            GameManager.Instance.Save.Data.shipHp = Hp;
            GameManager.Instance.SaveGame();
            return true;
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
                    transform.position = new Vector3(0f, 0f, 36f);
                    GameManager.Instance.Save.Data.shipHp = Hp;
                }
            }
            GameManager.Instance.UI.Refresh();
        }
    }
}

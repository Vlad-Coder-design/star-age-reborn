using UnityEngine;

namespace StarAge3D
{
    public class MiningObject : MonoBehaviour
    {
        ResourceType resource;
        int hp;

        public void Init(ResourceType output)
        {
            resource = output;
            hp = output == ResourceType.Stone ? 30 : 24;
        }

        public void Hit(int damage)
        {
            hp -= damage;
            if (hp > 0) return;

            int amount = Random.Range(2, 5);
            GameManager.Instance.Space.AddCargo(resource, amount);
            GameManager.Instance.AddXp(8);
            Destroy(gameObject);
        }
    }
}

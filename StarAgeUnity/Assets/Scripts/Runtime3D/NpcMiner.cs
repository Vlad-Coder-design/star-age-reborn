using UnityEngine;

namespace StarAge3D
{
    public class NpcMiner : MonoBehaviour
    {
        Vector3 target;

        void Start()
        {
            PickTarget();
        }

        void Update()
        {
            Vector3 direction = target - transform.position;
            if (direction.magnitude < 1f) PickTarget();
            direction.y = 0f;
            transform.position += direction.normalized * 2.2f * Time.deltaTime;
            if (direction.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }

        void PickTarget()
        {
            target = new Vector3(Random.Range(-38f, 38f), 0f, Random.Range(-38f, 38f));
        }
    }
}

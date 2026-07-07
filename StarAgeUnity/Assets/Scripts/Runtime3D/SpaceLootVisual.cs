using UnityEngine;

namespace StarAge3D
{
    public class SpaceLootVisual : MonoBehaviour
    {
        float life = 18f;
        float bobSeed;
        Vector3 drift;
        Vector3 startScale;

        void Start()
        {
            bobSeed = Random.Range(0f, 100f);
            drift = Random.insideUnitSphere * 0.45f;
            drift.y = 0f;
            startScale = transform.localScale;
        }

        void Update()
        {
            life -= Time.deltaTime;
            transform.position += drift * Time.deltaTime;
            transform.position += Vector3.up * Mathf.Sin(Time.time * 4f + bobSeed) * 0.006f;
            transform.Rotate(0f, 115f * Time.deltaTime, 65f * Time.deltaTime);
            if (life < 3f) transform.localScale = startScale * Mathf.Clamp01(life / 3f);
            if (life <= 0f) Destroy(gameObject);
        }
    }
}

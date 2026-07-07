using UnityEngine;

namespace StarAge3D
{
    public class ExplosionEffect : MonoBehaviour
    {
        float life;
        float maxLife;
        Transform shockwave;
        Light flash;
        readonly Vector3[] debrisVelocity = new Vector3[14];
        readonly Transform[] debris = new Transform[14];

        public void Init(bool pirate)
        {
            life = 1.55f;
            maxLife = life;

            Color core = pirate ? new Color(1f, 0.22f, 0.04f) : new Color(0.1f, 0.8f, 1f);
            Color hot = new Color(1f, 0.75f, 0.18f);
            Color smoke = new Color(0.12f, 0.11f, 0.12f);

            flash = gameObject.AddComponent<Light>();
            flash.type = LightType.Point;
            flash.range = 10f;
            flash.intensity = 7f;
            flash.color = core;

            AddBurst("Core Fireball", core, 0.35f, Vector3.zero);
            AddBurst("Hot Plasma Bloom", hot, 0.52f, new Vector3(0f, 0.05f, 0f));
            AddBurst("Dark Smoke Bloom", smoke, 0.78f, new Vector3(0f, 0.12f, 0f));

            shockwave = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
            shockwave.name = "Explosion Shockwave";
            shockwave.SetParent(transform);
            shockwave.localPosition = Vector3.zero;
            shockwave.localScale = new Vector3(0.3f, 0.025f, 0.3f);
            shockwave.GetComponent<Renderer>().material = RuntimeMaterial.Create(core, true);

            for (int i = 0; i < debris.Length; i++)
            {
                var shard = GameObject.CreatePrimitive(i % 3 == 0 ? PrimitiveType.Cylinder : PrimitiveType.Cube);
                shard.name = "Ship Explosion Debris";
                shard.transform.SetParent(transform);
                shard.transform.localPosition = Random.insideUnitSphere * 0.2f;
                shard.transform.localScale = new Vector3(Random.Range(0.07f, 0.18f), Random.Range(0.035f, 0.11f), Random.Range(0.12f, 0.32f));
                shard.transform.rotation = Random.rotation;
                shard.GetComponent<Renderer>().material = RuntimeMaterial.Create(i % 4 == 0 ? core : new Color(0.08f, 0.09f, 0.1f), i % 4 == 0);
                debris[i] = shard.transform;
                debrisVelocity[i] = Random.onUnitSphere * Random.Range(3.2f, 7.5f);
                debrisVelocity[i].y = Mathf.Abs(debrisVelocity[i].y) * 0.25f + Random.Range(-0.2f, 0.8f);
            }
        }

        void Update()
        {
            life -= Time.deltaTime;
            float t = Mathf.Clamp01(1f - life / maxLife);
            float fade = 1f - t;

            if (flash != null)
            {
                flash.intensity = 7f * fade * fade;
                flash.range = Mathf.Lerp(10f, 3f, t);
            }

            if (shockwave != null)
            {
                float radius = Mathf.Lerp(0.3f, 5.4f, t);
                shockwave.localScale = new Vector3(radius, 0.018f, radius);
            }

            for (int i = 0; i < debris.Length; i++)
            {
                if (debris[i] == null) continue;
                debris[i].position += debrisVelocity[i] * Time.deltaTime;
                debris[i].Rotate(180f * Time.deltaTime, 240f * Time.deltaTime, 120f * Time.deltaTime);
                debris[i].localScale *= 1f - 0.9f * Time.deltaTime;
            }

            if (life <= 0f) Destroy(gameObject);
        }

        void AddBurst(string name, Color color, float scale, Vector3 localPosition)
        {
            var burst = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            burst.name = name;
            burst.transform.SetParent(transform);
            burst.transform.localPosition = localPosition;
            burst.transform.localScale = Vector3.one * scale;
            burst.GetComponent<Renderer>().material = RuntimeMaterial.Create(color, true);
        }
    }
}

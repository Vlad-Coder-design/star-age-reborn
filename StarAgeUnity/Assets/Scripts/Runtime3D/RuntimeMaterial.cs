using UnityEngine;

namespace StarAge3D
{
    static class RuntimeMaterial
    {
        public static Material Create(Color color, bool emissive = false)
        {
            Shader shader = Shader.Find("Unlit/Color");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Sprites/Default");

            Material mat;
            if (shader != null)
            {
                mat = new Material(shader);
            }
            else
            {
                Material fallback = Resources.GetBuiltinResource<Material>("Default-Material.mat");
                mat = fallback != null ? new Material(fallback) : new Material(Shader.Find("Hidden/InternalErrorShader"));
            }

            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            if (emissive && mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", color * 0.25f);
            }

            return mat;
        }
    }
}

using UnityEngine;

namespace StarAge3D
{
    public class ShipNameplate : MonoBehaviour
    {
        TextMesh label;
        ShipController ship;
        string displayName;
        Color labelColor;

        public void Init(string value, ShipController controller, Color color)
        {
            displayName = value;
            ship = controller;
            labelColor = color;
            label = GetComponent<TextMesh>();
        }

        void LateUpdate()
        {
            if (label == null) label = GetComponent<TextMesh>();
            if (ship == null) ship = GetComponentInParent<ShipController>();
            if (label == null || ship == null || GameManager.Instance == null) return;

            label.text = $"{displayName}\nHP {ship.Hp}/{ship.MaxHp}";
            label.color = labelColor;
            Camera cam = GameManager.Instance.MainCamera;
            if (cam != null)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position, Vector3.up);
            }
        }
    }
}

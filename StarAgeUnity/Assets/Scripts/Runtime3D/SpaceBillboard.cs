using UnityEngine;

namespace StarAge3D
{
    public class SpaceBillboard : MonoBehaviour
    {
        void LateUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.MainCamera == null) return;
            Camera cam = GameManager.Instance.MainCamera;
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position, Vector3.up);
        }
    }
}

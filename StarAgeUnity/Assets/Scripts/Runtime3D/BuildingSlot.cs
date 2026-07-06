using UnityEngine;

namespace StarAge3D
{
    public class BuildingSlot : MonoBehaviour
    {
        public int Index { get; private set; }
        public Building Building { get; private set; }

        public void Init(int index)
        {
            Index = index;
        }

        public void SetBuilding(Building building)
        {
            Building = building;
        }

        void OnMouseDown()
        {
            GameManager.Instance.UI.OpenBuildingDetails(this);
        }
    }
}

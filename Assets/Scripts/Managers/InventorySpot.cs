using UnityEngine;
using UnityEngine.UI;

public class InventorySpot : MonoBehaviour
{
    [SerializeField] private int position;
    private void Start()
    {
        InventoryManager.Instance.AssignIInventorySpot(this.GetComponent<Image>(),position);
    }
}

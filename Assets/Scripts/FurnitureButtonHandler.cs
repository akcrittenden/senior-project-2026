using UnityEngine;

public class FurnitureButtonHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject furniturePrefab;

    public GameObject GetFurniturePrefab()
    {
        return furniturePrefab;
    }
}

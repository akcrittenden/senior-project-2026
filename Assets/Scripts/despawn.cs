using UnityEngine;

public class despawn : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Despawn"))
        {
            Destroy(gameObject);
        }
        // if this object collides with the despawn plane, destroy it


    }
}

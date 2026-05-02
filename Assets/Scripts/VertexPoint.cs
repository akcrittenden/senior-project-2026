using UnityEngine;

public class VertexPoint
{
    public GameObject gameObject { get; private set; }
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }
    public int index { get; set; }

    public VertexPoint(GameObject pointObject, int pointIndex = 0)
    {
        gameObject = pointObject;
        position = pointObject.transform.position;
        rotation = pointObject.transform.rotation;
        index = pointIndex;
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        position = newPosition;
        if (gameObject != null)
        {
            gameObject.transform.position = newPosition;
        }
    }

    public void Destroy()
    {
        if (gameObject != null)
        {
            Object.Destroy(gameObject);
        }
    }
}
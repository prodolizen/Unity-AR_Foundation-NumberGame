using UnityEngine;

public class ARDestroyOnCollision : MonoBehaviour
{
    private ARObjectSpawner objectSpawner;

    void Start()
    {
        objectSpawner = FindObjectOfType<ARObjectSpawner>();
        if (objectSpawner == null)
        {
            Debug.LogError("ARObjectSpawner script not found in the scene.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            objectSpawner.HandleObjectDestroyed(gameObject);
            Destroy(gameObject);
        }
    }
}

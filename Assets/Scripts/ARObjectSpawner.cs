using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Assign your prefabs here in the inspector
    public float spawnDelay = 5f; // The delay between spawning objects

    private List<GameObject> remainingObjectsToSpawn;
    private ARRaycastManager arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private Camera arCamera;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public GameObject spawnedObject { get; private set; }
    private bool spawned = false;

    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
        arCamera = Camera.main;
        remainingObjectsToSpawn = new List<GameObject>(objectsToSpawn);
    }

    void Update()
    {
        if (!spawned && remainingObjectsToSpawn.Count > 0)
        {
            // Search for the first detected plane
            foreach (var plane in arPlaneManager.trackables)
            {
                if (arRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
                {
                    Pose hitPose = hits[0].pose;
                    StartCoroutine(SpawnWithDelay(hitPose));
                    break;
                }
            }
        }
        else if (spawned)
        {
            HandleObjectInteraction();
        }
    }

    IEnumerator SpawnWithDelay(Pose hitPose)
    {
        spawned = true;
        yield return new WaitForSeconds(spawnDelay);

        if (remainingObjectsToSpawn.Count > 0)
        {
            GameObject nextPrefab = remainingObjectsToSpawn[0];
            spawnedObject = SpawnObject(hitPose.position, hitPose.rotation, nextPrefab);
            remainingObjectsToSpawn.RemoveAt(0);
        }
        else
        {
            spawned = false;
        }
    }

    void HandleObjectInteraction()
    {
        // Check if the camera intersects with the sphere collider of the prefab
        SphereCollider sphereCollider = spawnedObject.GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            Vector3 cameraPosition = arCamera.transform.position;
            Vector3 objectCenter = sphereCollider.bounds.center;
            float radius = sphereCollider.radius * Mathf.Max(spawnedObject.transform.lossyScale.x, spawnedObject.transform.lossyScale.y, spawnedObject.transform.lossyScale.z);
            float distance = Vector3.Distance(cameraPosition, objectCenter);

            if (distance < radius)
            {
                // Destroy the prefab and allow a new one to spawn
                Destroy(spawnedObject);
                spawned = false;
            }
        }
    }

    GameObject SpawnObject(Vector3 position, Quaternion rotation, GameObject prefab)
    {
        return Instantiate(prefab, position, rotation);
    }

    public void HandleObjectDestroyed(GameObject obj)
    {
        // Do something when an object is destroyed, e.g., update the score or
        // spawn a new object
    }
}

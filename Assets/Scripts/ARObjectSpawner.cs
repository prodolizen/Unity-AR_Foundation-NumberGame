using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARObjectSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn;

    private ARRaycastManager arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private Camera arCamera;
    private UIManager uiManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private List<GameObject> remainingObjectsToSpawn;

    public GameObject spawnedObject { get; private set; }
    private bool spawned = false;

    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
        arCamera = Camera.main;

        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager script not found in the scene.");
        }

        remainingObjectsToSpawn = new List<GameObject>(objectsToSpawn);
        uiManager.OnCorrectAnswerSelected += HandleCorrectAnswerSelected;
    }

    void OnDestroy()
    {
        uiManager.OnCorrectAnswerSelected -= HandleCorrectAnswerSelected;
    }

    void Update()
    {
        if (!spawned)
        {
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
    }

    IEnumerator SpawnWithDelay(Pose hitPose)
    {
        if (remainingObjectsToSpawn.Count > 0)
        {
            spawned = true;
            yield return new WaitForSeconds(5);
            GameObject nextPrefab = remainingObjectsToSpawn[0];
            spawnedObject = SpawnObject(hitPose.position, hitPose.rotation, nextPrefab);
            remainingObjectsToSpawn.RemoveAt(0);
            uiManager.ShowUIPanel(spawnedObject);
        }
    }

    private void HandleCorrectAnswerSelected()
    {
        spawned = false;
    }

    GameObject SpawnObject(Vector3 position, Quaternion rotation, GameObject prefab)
    {
        return Instantiate(prefab, position, rotation);
    }

    public void SetSpawned(bool value)
    {
        spawned = value;
    }

}

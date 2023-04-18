using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceTrackedImages : MonoBehaviour
{
    //ref to ar tracked image manager comp - detects images
    private ARTrackedImageManager _trackedImagesManager;

    //list of prefabs to instantiate same as 2D images in ref image lib
    public GameObject[] ArPrefabs;

    //keep dictionary array of created prefabs - keyed array
    private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();

    void Awake()
    {
        //ref to tracked image manager comp
        _trackedImagesManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        //attach event handler when the tracked image changes
        _trackedImagesManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        //remove event handler
        _trackedImagesManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    //event handler
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        //loop through all new tracked images that have been detected 
        foreach (var trackedImage in eventArgs.added)
        {
            //get name of ref image
            var imageName = trackedImage.referenceImage.name;

            //loop through array of prefabs
            foreach (var currentPrefab in ArPrefabs)
            {
                //check if prefab matches tracked image name and that it hasnt already been created (ignore case to ignore capitilisation errors in names)
                if (string.Compare(currentPrefab.name, imageName, System.StringComparison.OrdinalIgnoreCase) == 0 && !_instantiatedPrefabs.ContainsKey(imageName))
                {
                    //create new prefab and parent to ARTrackedImage
                    var newPrefab = Instantiate(currentPrefab, trackedImage.transform);
                    //add to array
                    _instantiatedPrefabs[imageName] = newPrefab;
                }

            }
        }

        //set created prefabs to active / not active depending if corresponding image is being tracked
        foreach (var trackedImage in eventArgs.updated)
        {
            _instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            //destroy prefab
            Destroy(_instantiatedPrefabs[trackedImage.referenceImage.name]);
            //remove from array
            _instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
        }
    }
}


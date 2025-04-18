using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageTracker : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;

    [System.Serializable]
    public struct TrackedPrefab
    {
        public string imageName;
        public GameObject prefab;
    }

    public List<TrackedPrefab> trackedPrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            SpawnPrefab(trackedImage);
        }

        foreach (var trackedImage in args.updated)
        {
            UpdatePrefab(trackedImage);
        }

        foreach (var trackedImage in args.removed)
        {
            RemovePrefab(trackedImage);
        }
    }

    void SpawnPrefab(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        if (!spawnedPrefabs.ContainsKey(name))
        {
            GameObject prefab = trackedPrefabs.Find(x => x.imageName == name).prefab;
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab, trackedImage.transform.position, trackedImage.transform.rotation);
                go.transform.SetParent(trackedImage.transform);
                spawnedPrefabs[name] = go;
            }
        }
    }

    void UpdatePrefab(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        if (spawnedPrefabs.ContainsKey(name))
        {
            GameObject go = spawnedPrefabs[name];
            go.transform.position = trackedImage.transform.position;
            go.transform.rotation = trackedImage.transform.rotation;
            go.SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }
    }

    void RemovePrefab(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        if (spawnedPrefabs.ContainsKey(name))
        {
            Destroy(spawnedPrefabs[name]);
            spawnedPrefabs.Remove(name);
        }
    }
}
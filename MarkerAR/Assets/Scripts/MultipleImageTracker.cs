using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MultipleImageTracker : MonoBehaviour
{
    ARTrackedImageManager imageManager;
    // Start is called before the first frame update
    void Start()
    {
        imageManager = GetComponent<ARTrackedImageManager>();

        imageManager.trackedImagesChanged += OnTrackedImage;
        //StartCoroutine(TurnOnImageTracker());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTrackedImage(ARTrackedImagesChangedEventArgs args)
    {
        foreach(ARTrackedImage trackedImage in args.added)
        {
            string imageName = trackedImage.referenceImage.name;

            GameObject imagePrefab = Resources.Load<GameObject>(imageName);
            if (imagePrefab != null)
            {
                if (trackedImage.transform.childCount < 1)
                {
                    GameObject go = Instantiate(imagePrefab, trackedImage.transform.position, trackedImage.transform.rotation);
                    go.transform.SetParent(trackedImage.transform);
                }
            }

            Vector2 myPos = new Vector2(GPS_Manager.Instance.latitude, GPS_Manager.Instance.longitude);

            StartCoroutine(DB_Manager.instance.LoadData(myPos, trackedImage.transform));
        }

        foreach(ARTrackedImage trackedImage in args.updated)
        {
            if(trackedImage.transform.childCount > 0)
            {
                trackedImage.transform.GetChild(0).position = trackedImage.transform.position;
                trackedImage.transform.GetChild(0).rotation = trackedImage.transform.rotation;
            }
        }
    }

    IEnumerator TurnOnImageTracker()
    {
        imageManager.enabled = false;

        while(!GPS_Manager.Instance.receiveGPS)
        {
            yield return null;
        }
        imageManager.enabled = true;

        imageManager.trackedImagesChanged += OnTrackedImage;
    }
}

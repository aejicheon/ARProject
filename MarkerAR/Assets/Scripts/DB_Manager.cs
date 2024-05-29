using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System;
using System.Threading.Tasks;

public class DB_Manager : MonoBehaviour
{
    public static DB_Manager instance;
    public string databaseURL = "https://myar-project-9bb8b-default-rtdb.asia-southeast1.firebasedatabase.app/";

    Vector2 currentPos;
    bool isSearch = false;
    string objectName = "";
    string currentKey = "";

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(databaseURL);

        //SaveData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SaveData()
    {
        ImageGPSData data1 = new ImageGPSData("Cat", 37.88622f, 127.7356f, false);
        ImageGPSData data2 = new ImageGPSData("SCar", 37.88622f, 127.7356f, false);

        string jsonCat = JsonUtility.ToJson(data1);
        string jsonSCar = JsonUtility.ToJson(data2);

        DatabaseReference refData = FirebaseDatabase.DefaultInstance.RootReference;

        refData.Child("Markers").Child("Data1").SetRawJsonValueAsync(jsonCat);
        refData.Child("Markers").Child("Data2").SetRawJsonValueAsync(jsonSCar);

        Debug.Log("Data Saved!");
    }


    public IEnumerator LoadData(Vector2 myPos, Transform trackedImage)
    {
        currentPos = myPos;
        DatabaseReference refData = FirebaseDatabase.DefaultInstance.GetReference("Markers");

        isSearch = true;
        refData.GetValueAsync().ContinueWith(LoadFunc);

        while(isSearch)
        {
            yield return null;
        }

        GameObject imagePrefab = Resources.Load<GameObject>(objectName);

        if(imagePrefab != null )
        {
            if(trackedImage.transform.childCount < 1)
            {
                GameObject go = Instantiate(imagePrefab, trackedImage.transform.position, trackedImage.transform.rotation);

                go.transform.SetParent(trackedImage.transform);
            }
        }
    }
    void LoadFunc(Task<DataSnapshot> task)
    {
        if(task.IsFaulted)
        {
            Debug.LogError("[MarkerAR] Failed to Load Data.");
        }
        else if(task.IsCanceled)
        {
            Debug.Log("[MarkerAR] Canceled to Load Data.");
        }
        else if(task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot data in snapshot.Children)
            {
                string myData = data.GetRawJsonValue();

                ImageGPSData myClassData = JsonUtility.FromJson<ImageGPSData>(myData);

                if(!myClassData.isCaptured)
                {
                    Vector2 dataPos = new Vector2(myClassData.latitude, myClassData.longitude);

                    float distance = Vector2.Distance(currentPos, dataPos);

                    if(distance < 0.001f)
                    {
                        objectName = myClassData.name;
                        currentKey = data.Key;
                    }
                }
            }
        }
        isSearch = false;
    }
    public void UpdateCaptured()
    {
        string dataPath = "Markers/" + currentKey + "/isCaptured";

        DatabaseReference refData = FirebaseDatabase.DefaultInstance.GetReference(dataPath);

        if(refData != null)
        {
            refData.SetValueAsync(true);
        }
    }
}

public class ImageGPSData
{
    public string name;
    public float latitude = 0;
    public float longitude = 0;
    public bool isCaptured = false;

    public ImageGPSData(string objName, float lat, float lon, bool captured)
    {
        name = objName;
        latitude = lat;
        longitude = lon;
        isCaptured = captured;
    }
}

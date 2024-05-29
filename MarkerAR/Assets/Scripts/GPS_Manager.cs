using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Android;

public class GPS_Manager : MonoBehaviour
{
    public static GPS_Manager Instance;
    public TMP_Text latitude_text;
    public TMP_Text longitude_text;

    public float latitude = 0;
    public float longitude = 0;
    public float maxWaitTime = 10.0f;

    public bool receiveGPS = false;
    public float resendTime = 1.0f;

    float waitTime = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GPS_On());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator GPS_On()
    {
        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

            while(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }
        }

        if(!Input.location.isEnabledByUser)
        {
            latitude_text.text = "GPS off";
            longitude_text.text = "GPS off";
            yield break;
        }

        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing && waitTime < maxWaitTime)
        {
            yield return new WaitForSeconds(1.0f);
            waitTime++;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            latitude_text.text = "위치 정보 수신 실패!";
            longitude_text.text = "위치 정보 수신 실패!";
        }

        if(waitTime >= maxWaitTime)
        {
            latitude_text.text = "응답 대기 시간 초과!";
            longitude_text.text = "응답 대기 시간 초과!";
        }

        receiveGPS = true;

        while(receiveGPS)
        {
            yield return new WaitForSeconds(resendTime);

            LocationInfo li = Input.location.lastData;
            latitude = li.latitude;
            longitude = li.longitude;

            latitude_text.text = "Latitude: " + latitude.ToString();
            longitude_text.text = "Longitude: " + longitude.ToString();

        }
    }
}

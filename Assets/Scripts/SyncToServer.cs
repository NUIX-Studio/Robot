using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Networking;


public class SyncToServer : MonoBehaviour
{
    RobotData storedData;

    [SerializeField]
    public string serverUrl;

    // Start is called before the first frame update
    void Start()
    {
        if (storedData == null)
        {
            storedData = new RobotData();
        }
        SyncData();
    }

    // Update is called once per frame
    void Update()
    {
        storedData.position_x = this.transform.position.x;
        storedData.position_y = this.transform.position.y;
        storedData.position_z = this.transform.position.z;
    }


    private void UpdateData(RobotData newRobotData)
    {
        Vector3 storedPosition = new(newRobotData.position_x, newRobotData.position_y, newRobotData.position_z);
        Quaternion storedRotation = new(newRobotData.rotation_x, newRobotData.rotation_y, newRobotData.rotation_z, newRobotData.rotation_w);

        this.transform.position = storedPosition;
        this.transform.rotation = storedRotation;
    }


    public void SyncData()
    {
        InvokeRepeating(nameof(PushDataToServer), 0f, 0.02f);
        //InvokeRepeating(nameof(PullDataFromServer), 0.5f, 1f);
    }

    public void PushDataToServer()
    {
        StartCoroutine(PushDataToServerCoroutine());
    }
    IEnumerator PushDataToServerCoroutine()
    {
        // We send the items stored parames, but not retrieve them to reduce computation costs
        string json = JsonUtility.ToJson(storedData);

        var req = new UnityWebRequest(serverUrl + "send", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("Received: " + req.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error While Sending: " + req.error);
        }
    }

    public void PullDataFromServer()
    {
        StartCoroutine(PullDataFromServerCoroutine());
    }

    IEnumerator PullDataFromServerCoroutine()
    {
        var req = new UnityWebRequest(serverUrl + "receive", "GET");

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("Received: " + req.downloadHandler.text);
            var text = req.downloadHandler.text;
            UpdateData(JsonUtility.FromJson<RobotData>(text));
        }
        else
        {
            Debug.Log("Error While Sending: " + req.error);
        }

    }
}

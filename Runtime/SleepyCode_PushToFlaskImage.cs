using System;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.XR;

public class SleepyCode_PushToFlaskImage : MonoBehaviour
{

    public string m_urlToPushAt = "http://localhost:80/image/save/{0}x{1}";
    public string m_buildUrl = "";
    public Texture2D m_textureToPush = null;

    [TextArea(3, 10)]
    public string m_callbackText = "";
    [TextArea(3, 10)]
    public string m_errorText = "";
    public bool m_useAwake = true;

    public int m_width = 0;
    public int m_height = 0;

    private void OnEnable()
    {
        if (m_useAwake)
        {
            PushImageToFlask();
        }
    }


    public bool m_inverseHorizontal = true;
    public bool m_inverseVertical = true;

    [ContextMenu("Push Image to Flask")]
    public void PushImageToFlask()
    {
        m_width = m_textureToPush.width;
        m_height = m_textureToPush.height;
        m_buildUrl = string.Format(m_urlToPushAt, m_width, m_height);

        Color32[] pixels = m_textureToPush.GetPixels32();
        byte[] bytes = new byte[pixels.Length * 4 +8];

        BitConverter.GetBytes(m_width).CopyTo(bytes, 0);
        BitConverter.GetBytes(m_height).CopyTo(bytes, 4);
        // buffer copy bytes to pixels to bytes

        for (int i = 0; i < pixels.Length; i++)
        {
            int x = i % m_width;
            int y = i / m_width;
            int targetX = m_inverseHorizontal ? (m_width - 1 - x) : x;
            int targetY = m_inverseVertical ? (m_height - 1 - y) : y;
            int targetIndex = (targetY * m_width + targetX) * 4;

            bytes[8 + targetIndex] = pixels[i].r;
            bytes[8 + targetIndex + 1] = pixels[i].g;
            bytes[8 + targetIndex + 2] = pixels[i].b;
            bytes[8 + targetIndex + 3] = pixels[i].a;
        }


        //for (int i = 0; i < pixels.Length; i++)
        //{
        //    bytes[i * 4] = pixels[i].r;
        //    bytes[i * 4 + 1] = pixels[i].g;
        //    bytes[i * 4 + 2] = pixels[i].b;
        //    bytes[i * 4 + 3] = pixels[i].a;
        //}
        StartCoroutine(SleepyCode_PushToFlaskImageCoroutine(m_urlToPushAt, bytes));
    }

    private IEnumerator SleepyCode_PushToFlaskImageCoroutine(string url, byte[] bytes)
    {

        // use unity webrequest with coroutine

        using (var www = new UnityEngine.Networking.UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bytes);
            www.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/octet-stream");
            // Send the request and wait for a response
            yield return www.SendWebRequest();
            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                m_errorText = www.error;
                m_callbackText = "Error uploading image: " + www.error;
            }
            else
            {
                m_callbackText = "Image uploaded successfully!";
                m_errorText = "";
            }
        }


    }
}

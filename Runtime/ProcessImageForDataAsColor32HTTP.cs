using System;
using System.Collections;
using UnityEngine;

namespace Eloi.TextureToServer {

    /// <summary>
    /// I am a class that request to a server to process an image and send by text result
    /// </summary>
    [System.Serializable]   
    public class ProcessImageForDataAsColor32HTTP
{
    public long m_timeStampSent = 0;
    public long m_timeStampReceived = 0;
    public long m_proceedTimeInMilliseconds = 0;
    public Texture2D m_image = null;
    public string m_requestUrl = "http://localhost:80";
    public bool m_sentRequest = false;
    public bool m_errorHappened = false;
    public bool m_callbackReceived = false;
    public string m_callbackText = "";
    public string m_errorText = "";

    public Action<ProcessImageForDataAsColor32HTTP> m_onCallbackReceived = null;

    public ProcessImageForDataAsColor32HTTP(Texture2D image, string urlToContact)
    {
        m_image = image;
        m_requestUrl = urlToContact;
    }
    public IEnumerator SendRequest() {


            m_timeStampSent = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int m_width = m_image.width;
            int  m_height = m_image.height;
            Color32[] pixels = m_image.GetPixels32();
            byte[] bytes = new byte[pixels.Length * 4 + 8];

            BitConverter.GetBytes(m_width).CopyTo(bytes, 0);
            BitConverter.GetBytes(m_height).CopyTo(bytes, 4);
            // buffer copy bytes to pixels to bytes

            for (int i = 0; i < pixels.Length; i++)
            {
                int x = i % m_width;
                int y = i / m_width;
                int targetX = x; //(m_width - 1 - x) ;
                int targetY =(m_height - 1 - y) ;
                int targetIndex = (targetY * m_width + targetX) * 4;

                bytes[8 + targetIndex] = pixels[i].r;
                bytes[8 + targetIndex + 1] = pixels[i].g;
                bytes[8 + targetIndex + 2] = pixels[i].b;
                bytes[8 + targetIndex + 3] = pixels[i].a;
            }

            m_sentRequest =true;
            using (var www = new UnityEngine.Networking.UnityWebRequest(m_requestUrl, "POST"))
            {
                www.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bytes);
                www.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/octet-stream");
                // Send the request and wait for a response
                yield return www.SendWebRequest();
                if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    m_errorText = www.error;
                    m_callbackText = "";
                    m_errorHappened = true;
               

                }
                else
                {
                    m_callbackText = www.downloadHandler.text;
                    m_errorText = "";
                }
                m_callbackReceived = true;
            m_timeStampReceived = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            m_proceedTimeInMilliseconds = m_timeStampReceived - m_timeStampSent;
        
            m_onCallbackReceived?.Invoke(this);

        }
    }


}

}
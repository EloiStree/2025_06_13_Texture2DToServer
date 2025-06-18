using System;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TextureToServer {

    public interface I_ImageToFlaskSettableIP
    {
        void SetServerUrl(string serverUrl);
    }
    public abstract class AbstractImageToFlaskDataUrlMono : MonoBehaviour, I_ImageToFlaskSettableIP
    {
        public string m_serverUrl = "http://localhost:80";
        public string m_urlRelativePath = "/image/";
        public UnityEvent<ProcessImageForDataAsColor32HTTP> m_onRequestCompleted;
        public UnityEvent<string> m_onValideCallbackReceived;



        public void SetServerUrl(string serverUrl)
        {
            m_serverUrl = serverUrl;
        }
        public void SetUrlRelativePath(string urlRelativePath)
        {
            m_urlRelativePath = urlRelativePath;
        }
        public void SetServerUrlAndRelativePath(string serverUrl, string urlRelativePath)
        {
            m_serverUrl = serverUrl;
            m_urlRelativePath = urlRelativePath;
        }
        private void Reset()
        {
            m_serverUrl = "http://localhost:80";
            m_urlRelativePath = GetRelativePathAtReset();
        }

        public abstract string GetRelativePathAtReset();

        public void BuildUrl(out string url)
        {
            url = m_serverUrl + m_urlRelativePath;
        }
        public void BuildRequest(Texture2D image, out ProcessImageForDataAsColor32HTTP request)
        {
            BuildUrl(out string url);
            request = new ProcessImageForDataAsColor32HTTP(image, url);
        }
        

        public void BuildAndRunRequest(Texture2D image)
        {
            BuildRequest(image, out ProcessImageForDataAsColor32HTTP request);
            request.m_onCallbackReceived = (result) => BroadcastValideRequest(result);
            StartCoroutine(request.SendRequest());
        }

       

        private void BroadcastValideRequest(ProcessImageForDataAsColor32HTTP request)
        {
            
                bool isValid = request != null && !string.IsNullOrEmpty(request.m_callbackText);

                if (!isValid) return;

                m_onRequestCompleted?.Invoke(request);
                if (!string.IsNullOrEmpty(request.m_callbackText))
                {
                    m_onValideCallbackReceived?.Invoke(request.m_callbackText);
                }
            
        }

    }

}
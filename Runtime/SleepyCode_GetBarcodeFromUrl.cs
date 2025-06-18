using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class SleepyCode_GetBarcodeFromUrl : MonoBehaviour
{
    [Header("URL to fetch barcode from")]
    public string m_urlToFetch = "http://localhost:80/image/get/barcode";

    [Header("Response Text")]
    [TextArea(3, 10)]
    public string m_callbackText = "";

    [Header("Error Text")]
    [TextArea(3, 10)]
    public string m_errorText = "";

    [Header("Settings")]
    public bool m_useAwake = true;

    public List<TypeValue> m_typeValueFound = new List<TypeValue>();

    [System.Serializable]
    public class TypeValue {

        public string m_key;
        public string m_value;
    }

    public UnityEvent<string, string> m_onTypeValueFound;

    private void Awake()
    {
        if (m_useAwake)
        {
            FetchInfo();
        }
    }

    [ContextMenu("Fetch Info")]
    public void FetchInfo()
    {
        StartCoroutine(Coroutine_FetchInfo());
    }

    private IEnumerator Coroutine_FetchInfo()
    {
        m_typeValueFound.Clear();
        using (UnityWebRequest www = UnityWebRequest.Get(m_urlToFetch))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                m_errorText = www.error;
                m_callbackText = "";
            }
            else
            {
                m_callbackText = www.downloadHandler.text;
                m_errorText = "";
                string[] lines = m_callbackText.Split("\n");
                foreach (string line in lines)
                {
                    int index = line.IndexOf(":");
                    if (index >= 0) // Ensure ':' exists in the line
                    {
                        string leftPart = line[..index]; // Simplified Substring
                        string rightPart = line[(index + 1)..]; // Simplified Substring

                        m_typeValueFound.Add(new TypeValue()
                        {
                            m_key = leftPart,
                            m_value = rightPart
                        });
                        m_onTypeValueFound.Invoke(leftPart, rightPart);
                    }
                }
            }
        }
    }
}

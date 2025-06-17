
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PushAndRecalledFlaskImageToResult {


    public void TurnColor32ToBytesByMemoryCopy(Color32[] colors, out byte[] bytesResult)
    {
        if (colors == null || colors.Length == 0)
        {
            bytesResult = null;
            return;
        }
        int colorCount = colors.Length;
        bytesResult = new byte[colorCount * 4];
        System.Buffer.BlockCopy(colors, 0, bytesResult, 0, bytesResult.Length);
    }

    public IEnumerator PushIn(Texture2D texture)
    {
        if (texture == null)
        {
            yield break;
        }
        m_pushedUrl = m_pushedUrl.Trim();
        if (string.IsNullOrEmpty(m_pushedUrl))
        {
            yield break;
        }
        TurnColor32ToBytesByMemoryCopy(m_textureAsColor, out byte[] bytesToPush);
        if (bytesToPush == null || bytesToPush.Length == 0)
        {
            yield break;
        }
        using (UnityWebRequest request = UnityWebRequest.Put(m_pushedUrl, bytesToPush))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/octet-stream");
            request.uploadHandler.contentType = "application/octet-stream";
            request.timeout = 10;
            m_isPushed = true;
            yield return request.SendWebRequest();
            m_isRecalled = true;
            if (request.result == UnityWebRequest.Result.Success)
            {
                m_hadError = false;
                m_errorMessage = "";
                m_callbackText = request.downloadHandler.text; 
            }
            else
            {
                m_hadError = true;
                m_errorMessage = $"Error pushing texture: {request.error}";
                m_callbackText = m_errorMessage;
            }
        }
    }

    public string m_pushedUrl = "";
    public Texture2D m_textureToPush;
    Color32[] m_textureAsColor;
    public int m_widthSize = 0;
    public int m_heightSize = 0;
    public int m_pixelsCount = 0;
    public string m_callbackText = "";
    public bool m_isPushed = false;
    public bool m_isRecalled = false;
    public bool m_hadError = false;
    public string m_errorMessage = "";
    public string m_recallResult = "";
}

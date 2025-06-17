using UnityEngine;

public class TexturePushMono_PushToFlaskHTTP : MonoBehaviour
{

    public string m_urlToPushAt = "http://127.0.0.1:80/image/get_barcode";
    public PushAndRecalledFlaskImageToResult m_lastPushed;

    public void PushIn(Texture2D textureToPush)
    {
        if (textureToPush == null)
        {
            return;
        }
        m_lastPushed = new PushAndRecalledFlaskImageToResult();
        StartCoroutine(m_lastPushed.PushIn(textureToPush));
    }

}

using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TextureToServer
{
    public class ImageFlaskMono_PushTextureForDebug : MonoBehaviour
    {
        public Texture2D m_textureToPush;
        public UnityEvent<Texture2D> m_onTexturePushed;

        [ContextMenu("Push Inspector Texture")]
        public void PushInspectorTexture()
        {

            if (m_textureToPush != null)
            {
                m_onTexturePushed?.Invoke(m_textureToPush);
            }

        }

    }
}
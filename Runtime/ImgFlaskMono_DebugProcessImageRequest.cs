using UnityEngine;

namespace Eloi.TextureToServer {
    public class ImgFlaskMono_DebugProcessImageRequest : MonoBehaviour { 
    
        public ProcessImageForDataAsColor32HTTP m_processImageRequest;
        public void PushIn(ProcessImageForDataAsColor32HTTP request)
        {
            m_processImageRequest = request;
        }
    }
}
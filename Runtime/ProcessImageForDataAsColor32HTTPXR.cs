using UnityEngine;

namespace Eloi.TextureToServer {
    [System.Serializable]
public class ProcessImageForDataAsColor32HTTPXR : ProcessImageForDataAsColor32HTTP
{

    public ProcessImageForDataAsColor32HTTPXR(Texture2D image, string urlToContact) : base(image, urlToContact)
    {
        // This class is just a placeholder for XR-specific processing if needed in the future
    }

        public ProcessImageForDataAsColor32HTTPXR(Texture2D image, string urlToContact, Transform root, Transform center) : base(image, urlToContact)
        { 
        
            SetCameraXR(root, center);
        }


    public Pose m_cameraRootXR = Pose.identity;
    public Pose m_cameraCenterXR = Pose.identity;


    public void SetCameraXR(Transform root, Transform center)
    {
        m_cameraRootXR = new Pose(root.position, root.rotation);
        m_cameraCenterXR = new Pose(center.position, center.rotation);
    }
}

}
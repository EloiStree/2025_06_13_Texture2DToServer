using System;
using UnityEditor;
using UnityEngine;

namespace Eloi.TextureUtility {


    [System.Serializable]
    public struct STRUCT_Dimesion_Pixel
    {
        public int m_width;
        public int m_height;
    }

    [System.Serializable]
    public struct STRUCT_Index1D_Pixel
    {
        public int m_index;
    }

    [System.Serializable]
    public struct STRUCT_Index2D_Pixel_LRDT
    {
        public int m_indexPixelLeftRightX;
        public int m_indexPixelDownTopY;
    }

    [System.Serializable]
    public struct STRUCT_Index2D_Pixel_LRTD
    {
        public int m_indexPixelLeftRightX;
        public int m_indexPixelTopDownY;
    }

    [System.Serializable]
    public struct STRUCT_Index2D_Percent_LRDT
    {

        public int m_indexPercentLeftRightX;
        public int m_indexPercentTopDownY;
    }
    [System.Serializable]
    public struct STRUCT_Index2D_Percent_LRTD
    {
        public int m_indexPercentLeftRightX;
        public int m_indexPercentTopDownY;
    }


    /// <summary>
    /// I am a class that store the width and height of a Color32 as to make computation I dont really need Texture2D information.
    /// </summary>
    [Serializable]
    public class Color2D32
    {
        public int m_width;
        public int m_height;
        public Color32[] m_color32;
    }
    /// <summary>
    /// If you plan to use color to be exported by UDP, Websocket or memory you will need it to be Bufferable.
    /// I am a class that store the color as bytes in a arry starting by it width and height in ushort.
    /// To win 1/4 of bandwidth, I store the color as RGB instead of RGBA.
    /// </summary>
    [Serializable]
    public class ColorBytesRGB2D
    {
        public ushort m_width;
        public ushort m_height;
        public byte[] m_rgbBytes;
        public ColorBytesRGB2D(ushort width, ushort height)
        {
            m_width = width;
            m_height = height;
            m_rgbBytes = new byte[width * height * 3 + 4];
            BitConverter.GetBytes(m_width).CopyTo(m_rgbBytes, 0);
            BitConverter.GetBytes(m_height).CopyTo(m_rgbBytes, 2);
        }
        public void FlushToZeroColor()
        {
            for (int i = 4; i < m_rgbBytes.Length; i++)
            {
                m_rgbBytes[i] = 0;
            }
        }

        public void SetColor32(Color32[] color32)
        {
            if (color32.Length != m_width * m_height)
            {
                throw new ArgumentException("Color32 array length does not match width and height.");
            }
            for (int i = 0; i < color32.Length; i++)
            {
                m_rgbBytes[i * 3 + 4] = color32[i].r;
                m_rgbBytes[i * 3 + 5] = color32[i].g;
                m_rgbBytes[i * 3 + 6] = color32[i].b;
            }
        }
    }


public class ImgFlaskMono_QuestWebcamToUdpSizeColor32 : MonoBehaviour
{

    public Color32[] m_color32ToInWebcam;
    public Color32[] m_color32ToFitInUdp;

    public Texture2D m_producedTexture2D;

    public bool m_mipmap = false;
    public bool m_linear = false;
    public bool m_useUdp = true;


    [Header("For info")]
    public int m_widthExpectedQuest3 = 1280;
    public int m_heightExpectedQuest3 = 930;
    public int m_widthToFitInUdp = 128;
    public int m_heightToFitInUdp = 93;


    [Header("For info, not used in this script")]

    public int m_sourcePixelCounts;
    public int m_fitBuildPixelCounts;
    public int m_rgbaSourceInBytes;
    public int m_rgbaToFitInUdpInBytes;
    public int m_rgbToFitInUdpInBytes;


    void Update()
    {
        if (m_useUdp) {
            ConvertWebcamToColor32();
        }
    }

    public void SetColor2D32(int height, int width , Color32 webcamTexture)
    {
        if (m_color32ToInWebcam == null || m_color32ToInWebcam.Length != height * width)
        {
            m_color32ToInWebcam = new Color32[height * width];
        }
        for (int i = 0; i < m_color32ToInWebcam.Length; i++)
        {
            m_color32ToInWebcam[i] = webcamTexture;
        }
        m_rgbaSourceInBytes = m_color32ToInWebcam.Length + 4; // +4 for the header
    }

    private void ConvertWebcamToColor32()
    {
     
        if (m_color32ToFitInUdp == null || m_color32ToFitInUdp.Length != m_widthToFitInUdp * m_heightToFitInUdp)
        {
            m_color32ToFitInUdp = new Color32[m_widthToFitInUdp * m_heightToFitInUdp];
        }
        // Get the pixels from the webcam texture
        m_rgbaSourceInBytes = m_color32ToInWebcam.Length +4;
        // make average of color32ToInWebcam in fitinudp color array by merging pixels
        int widthWebcam = m_webcamTextureToUse.width;
        int heightWebcam = m_webcamTextureToUse.height;
        int widthToFit = m_widthToFitInUdp;
        int heightToFit = m_heightToFitInUdp;
        for (int y = 0; y < heightToFit; y++)
        {
            for (int x = 0; x < widthToFit; x++)
            {
                int srcX = x * widthWebcam / widthToFit;
                int srcY = y * heightWebcam / heightToFit;
                int indexSrc = srcY * widthWebcam + srcX;
                Color32 pixelColor = color32ToInWebcam[indexSrc];
                m_color32ToFitInUdp[y * widthToFit + x] = pixelColor;
            }
        }
        if (m_producedTexture2D == null || m_producedTexture2D.width != widthToFit || m_producedTexture2D.height != heightToFit)
        {
            m_producedTexture2D = new Texture2D(widthToFit, heightToFit, TextureFormat.RGBA32, m_mipmap,m_linear);
            m_producedTexture2D.filterMode = FilterMode.Point;
            m_producedTexture2D.wrapMode = TextureWrapMode.Clamp;
            m_producedTexture2D.Apply();
            m_rgbaToFitInUdpInBytes = m_producedTexture2D.width * m_producedTexture2D.height * 4; // RGBA32 has 4 bytes per pixel
            m_rgbToFitInUdpInBytes = m_producedTexture2D.width * m_producedTexture2D.height * 3; // RGB has 3 bytes per pixel

            m_fitBuildPixelCounts = m_producedTexture2D.width * m_producedTexture2D.height;
            m_sourcePixelCounts = m_webcamTextureToUse.width * m_webcamTextureToUse.height;
        }
        m_producedTexture2D.SetPixels32(m_color32ToFitInUdp);
        m_producedTexture2D.Apply();
    }
}

}
namespace Eloi.TextureUtility {
    public static class  Index1D2DUtility
    {

        public static void ParseUnityTexture(
            in STRUCT_Index1D_Pixel index, in STRUCT_Dimesion_Pixel dimension,
            out STRUCT_Index2D_Pixel_LRDT result)
        {
            result.m_indexPixelLeftRightX = index.m_index;
            result.m_indexPixelDownTopY = index.m_index;
        }

        public static void ParseOneDimToTwoDimLRTD(in int index, in int width, out int leftRight, out int topDown)
        {
            leftRight = index % width;
            topDown = index / width;
        }
        public static void ParseOneDimToTwoDimLRDT(in int index, in int width, in int height, out int leftRight, out int downTop)
        {
            leftRight = width - 1 - (index % width);
            downTop = (height - 1) - (index / width);
        }

        public static void ParseOneDimToTwoDimRLTD(in int index, in int width, out int rightLeft, out int topDown)
        {
            rightLeft = width-1-(index % width);
            topDown = index / width;
        }
        public static void ParseOneDimToTwoDimRLDT(in int index, in int width, in int height, out int rightLeft, out int downTop)
        {
            rightLeft =  index % width;
            downTop = (height - 1) - (index / width);
        }

        public static void ParseTwoDimToOneDimLRTD(in int leftRight, in int topDown, in int width, out int index)
        {

            index = topDown * width + leftRight;
        }


        public static void ParseTwoDimToOneDimLRDT(in int leftRight, in int topDown, in int width, in int height, out int index)
        {
            index = ((height - 1) - topDown) * width + leftRight;
        }

        public static void ParseTwoDimToOneDimRLTD(in int leftRight, in int topDown, in int width, out int index)
        {

            index = topDown * width + (width-1- leftRight);
        }


        public static void ParseTwoDimToOneDimRLDT(in int leftRight, in int topDown, in int width, in int height, out int index)
        {
            index = ((height - 1) - topDown) * (width - 1 -  leftRight);
        }


    }

}
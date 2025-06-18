namespace Eloi.TextureToServer {
    public class ImgFlaskMono_FetchBarcodeAndQrCode : AbstractImageToFlaskDataUrlMono
    {
        public override string GetRelativePathAtReset()
        {
            return "/image/post/get_scancode";
        }
    }
}
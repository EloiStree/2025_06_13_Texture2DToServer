namespace Eloi.TextureToServer {
    public class ImgFlaskMono_SaveImageWithDateNow : AbstractImageToFlaskDataUrlMono
    {
        public override string GetRelativePathAtReset()
        {
            return "/image/save/png/date";
        }
    }
}
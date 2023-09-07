using UserManager.Common.Models.AvatarImage;

namespace UserManager.BLL.Helpers
{
    public class ImageExtensionHelper
    {
        public static string GetImageExtention(ImageExtensions imageExtension)
        {
            return imageExtension switch
            {
                ImageExtensions.Png => ".png",
                ImageExtensions.Jpg => ".jpg",
                _ => throw new Exception("Incorrect picture type"),
            };
        }
    }
}

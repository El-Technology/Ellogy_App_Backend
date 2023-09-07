namespace UserManager.Common.Models.AvatarImage
{
    public class UploadAvatar
    {
        public Guid UserId { get; set; }
        public string Base64Avatar { get; set; }
        public ImageExtensions ImageExtension { get; set; }
    }
}

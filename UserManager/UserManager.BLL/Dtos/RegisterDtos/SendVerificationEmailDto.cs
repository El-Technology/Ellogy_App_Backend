namespace UserManager.BLL.Dtos.RegisterDtos
{
    public class SendVerificationEmailDto
    {
        public string RedirectLink { get; set; }
        public string UserEmail { get; set; }
    }
}

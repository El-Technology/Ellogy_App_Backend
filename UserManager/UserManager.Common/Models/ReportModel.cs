namespace UserManager.Common.Models
{
    public class ReportModel
    {
        public string ReceiverEmail { get; set; }
        public string UserEmail { get; set; }
        public string UserText { get; set; }
        public List<string> Files { get; set; }
    }
}

namespace FIXIT.BLL
{
    public class MailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public string AdminEmail { get; set; }
    }
}
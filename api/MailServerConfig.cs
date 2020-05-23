namespace api
{
    public class MailServerConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public MailServerConfig()
        {
            Host = "mail";
            Port = 1025;
        }
    }
}
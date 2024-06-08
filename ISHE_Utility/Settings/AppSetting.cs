namespace ISHE_Utility.Settings
{
    public class AppSetting
    {
        public string SecretKey { get; set; } = null!;
        public string RefreshTokenSecret { get; set; } = null!;
        public int ContractDeposited { get; set; }

        public string Bucket { get; set; } = null!;
        public string Folder { get; set; } = null!;
        public string FolderContract {  get; set; } = null!;
        public ZaloPaySetting ZaloPay { get; set; } = null!;


        public string NameApp { get; set; } = null!;
        public string EMailAddress { get; set; } = null!;
        public bool UseSSL { get; set; }
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public bool UseStartTls { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;


    }
}

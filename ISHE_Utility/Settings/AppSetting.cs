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
    }
}

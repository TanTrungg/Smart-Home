namespace ISHE_Utility.Helpers.ZaloPay
{
    public class ZaloPayViewModel
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; } = null!;
        public int SubReturnCode { get; set; }
        public string SubReturnMessage { get; set; } = null!;
        public string OrderUrl { get; set; } = null!;
        public string ZPTransToken { get; set; } = null!;
        public string OrderToken { get; set; } = null!;
        public string QRCode { get; set; } = null!;
    }

    public class ZaloPayErrorViewModel
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; } = null!;
        public int SubReturnCode { get; set; }
        public string? SubReturnMessage { get; set; }
        public string? ZPTransToken { get; set; }
        public string? OrderUrl { get; set; }
        public string? OrderToken { get; set; }
    }
}

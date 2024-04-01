namespace ISHE_Utility.Helpers.Utils
{
    public static class GetValid
    {
        public static int GetValidOrDefault(this int value, int defaultValue)
        {
            return value == 0 ? defaultValue : value;
        }
    }
}

namespace DistributedAsync.Redis
{
    static class ConstantValues
    {
        public const string PerfixChannelName = "async__channel__";
        public const string PerfixKeyName = "async__key__";
        public static string NormalizeChannelName(this string channelName) => PerfixChannelName + channelName.Trim().ToLower();
        public static string NormalizeKeyName(this string keyName) => PerfixKeyName + keyName.Trim().ToLower();
    }
}

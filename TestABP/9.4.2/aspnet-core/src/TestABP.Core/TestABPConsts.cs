using TestABP.Debugging;

namespace TestABP
{
    public class TestABPConsts
    {
        public const string LocalizationSourceName = "TestABP";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "b33731116d4e4985a87a0dba5a87c9ba";
    }
}

namespace Assignment.API.Utils
{
    public class ConvertUtils
    {
        public static string StatusMapping(string status)
        {
            if (string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                return "A";
            }
            else if (string.Equals(status, "Failed", StringComparison.OrdinalIgnoreCase) || string.Equals(status, "Rejected", StringComparison.OrdinalIgnoreCase))
            {
                return "R";
            }
            else if (string.Equals(status, "Finished", StringComparison.OrdinalIgnoreCase) || string.Equals(status, "Done", StringComparison.OrdinalIgnoreCase))
            {
                return "D";
            }
            else
            {
                return "";
            }
        }

    }
}

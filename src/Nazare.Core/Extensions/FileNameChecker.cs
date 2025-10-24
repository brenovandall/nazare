using System.Text.RegularExpressions;

namespace Nazare.Core.Extensions
{
    public static partial class FileNameChecker
    {
        public static bool CheckAndThrow(string filename)
        {
            const string rules = "Name file rules:\n[R__] = Rerun every time.\n[V**__] = Run in order.";

            if (!(filename.StartsWith("R__") || filename.StartsWith("V")))
                throw new Exception(rules);

            if (Path.GetExtension(filename) == ".sql")
                throw new Exception("Only sql files are accepted.");

            if (filename.StartsWith("V") && !StartsWithVThenVersionNumberAndFileName().IsMatch(filename))
                throw new Exception(rules);

            return true;
        }

        [GeneratedRegex(@"^V\d+__.+$")]
        private static partial Regex StartsWithVThenVersionNumberAndFileName();
    }
}

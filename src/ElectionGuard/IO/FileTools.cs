namespace ElectionGuard.SDK.IO
{
    public static class FileTools
    {
        public static File CreateNewFile(string prefix)
        {
            var template = $"{prefix}-XXXXXX";
            return FileApi.CreateNewFile(template);
        }

        public static void SeekFileToBeginning(File file)
        {
            FileApi.SeekFileToBeginning(file);
        }

        public static void CloseFile(File file)
        {
            FileApi.CloseFile(file);
        }
    }
}
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.IO
{
    internal struct FileApi
    {
        [DllImport("electionguard", EntryPoint = "File_new")]
        internal static extern File CreateNewFile(string template);

        [DllImport("electionguard", EntryPoint = "File_seek")]
        internal static extern void SeekFileToBeginning(File file);

        [DllImport("electionguard", EntryPoint = "File_close")]
        internal static extern void CloseFile(File file);
    }
}
namespace FolderSync.IO.FileCompare;

internal static class FileDateComparer
{
	public static bool CompareFiles(FileInfo fileInfo1, FileInfo fileInfo2)
	{
		return fileInfo1.LastWriteTimeUtc == fileInfo2.LastWriteTimeUtc;
	}
}

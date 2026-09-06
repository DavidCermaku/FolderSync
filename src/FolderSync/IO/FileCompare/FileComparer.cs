namespace FolderSync.IO.FileCompare;

internal static class FileComparer
{
	public static bool CompareFiles(FileInfo fileInfo1, FileInfo fileInfo2)
	{
		if (FileDateComparer.CompareFiles(fileInfo1, fileInfo2)) return true;

		if (!MD5Comparer.CompareFiles(fileInfo1, fileInfo2)) return true;

		return false;
	}
}

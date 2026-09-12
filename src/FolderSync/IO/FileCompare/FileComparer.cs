namespace FolderSync.IO.FileCompare;

internal static class FileComparer
{
	public static bool CompareFiles(FileInfo fileInfo1, FileInfo fileInfo2)
	{
		if (fileInfo1.Length != fileInfo2.Length) return false;

		if (FileDateComparer.CompareFiles(fileInfo1, fileInfo2)) return true;

		return MD5Comparer.CompareFiles(fileInfo1, fileInfo2);
	}
}

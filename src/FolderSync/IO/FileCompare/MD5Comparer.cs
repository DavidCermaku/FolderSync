using System.Security.Cryptography;

namespace FolderSync.IO.FileCompare;

internal static class MD5Comparer
{
	public static bool CompareFiles(FileInfo fileInfo1, FileInfo fileInfo2)
	{
		using var fileStream01 = fileInfo1.OpenRead();
		using var fileStream02 = fileInfo2.OpenRead();
		using var md5Creator = MD5.Create();

		var fileStream01Hash = md5Creator.ComputeHash(fileStream01);
		var fileStream02Hash = md5Creator.ComputeHash(fileStream02);

		return fileStream01Hash.AsSpan().SequenceEqual(fileStream02Hash);
	}
}

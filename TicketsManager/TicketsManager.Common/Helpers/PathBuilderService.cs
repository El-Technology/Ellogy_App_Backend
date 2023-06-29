namespace TicketsManager.Common.Helpers;

public static class PathBuilderHelper
{
	public static string BuildPath(IEnumerable<string> directories)
	{
		return string.Join(Path.DirectorySeparatorChar, directories);
	}
}



namespace ESS.FW.Bpm.Engine.Variable.Type
{

	/// <summary>
	/// 
	/// 
	/// </summary>
	public interface IFileValueType : IValueType
	{

	  /// <summary>
	  /// Identifies the file's name as specified on value creation.
	  /// </summary>

	  /// <summary>
	  /// Identifies the file's mime type as specified on value creation.
	  /// </summary>

	  /// <summary>
	  /// Identifies the file's encoding as specified on value creation.
	  /// </summary>

	}

	public static class FileValueTypeFields
	{
	  public const string ValueInfoFileName = "filename";
	  public const string ValueInfoFileMimeType = "mimeType";
	  public const string ValueInfoFileEncoding = "encoding";
	}

}
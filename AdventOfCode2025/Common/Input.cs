using System.Text;

namespace AdventOfCode2025.Common;

public class Input
{
	private readonly string _fileName;
	public string Text { get; }
	public byte[] Bytes { get; }
	public string[] Lines { get; }
	public Memory<string> Memory { get; }
	public IEnumerable<string> Enumerable { get; }

	public Input(string fileName, string text, string[] lines, IEnumerable<string> enumerable)
	{
		_fileName = fileName;

		Text = text;
		Lines = lines;
		Enumerable = enumerable;
		Memory = new Memory<string>(lines);
		Bytes = Encoding.UTF8.GetBytes(Text);
	}

	public override string ToString()
	{
		return _fileName;
	}
}
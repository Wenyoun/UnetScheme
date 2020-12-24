using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class SyncAttributeCopy
{
	public const string PathEditorProto = "Assets/Scripts/Game/Editor/SyncAttribute";
	public const string PathClientProto = "Assets/Scripts/Game/Client/Attribute/Sync";
	public const string PathServerProto = "Assets/Scripts/Game/Server/Attribute/Sync";

	[MenuItem("Proto/CopySyncAttribute")]
	private static void CopyProto()
	{
		List<string> files = new List<string>();

		LoadFiles(files, PathClientProto);
		LoadFiles(files, PathServerProto);

		for (int i = 0; i < files.Count; ++i)
		{
			if (File.Exists(files[i]))
			{
				File.Delete(files[i]);
			}
		}

		files.Clear();
		LoadFiles(files, PathEditorProto);
		for (int i = 0; i < files.Count; ++i)
		{
			string filename = files[i];
			string name = Path.GetFileName(filename);
			CopyFile(filename, PathClientProto + "/" + name, "Zyq.Game.Client");
			CopyFile(filename, PathServerProto + "/" + name, "Zyq.Game.Server");
		}

		AssetDatabase.Refresh();
	}

	private static void LoadFiles(List<string> root, string path)
	{
		string[] dirs = Directory.GetFileSystemEntries(path);
		foreach (string dir in dirs)
		{
			if (dir.IndexOf(".meta") == -1)
			{
				if (Directory.Exists(dir))
				{
					LoadFiles(root, dir);
				}
				else if (File.Exists(dir))
				{
					if (dir.EndsWith(".cs"))
					{
						root.Add(dir.Replace("\\", "/"));
					}
				}
			}
		}
	}

	private static void CopyFile(string srcfile, string dstfile, string nsapce)
	{
		using (StreamReader reader = new StreamReader(File.OpenRead(srcfile), Encoding.UTF8))
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("//警告，警告，自动生成，不要修改，不要修改\n");
			builder.Append(string.Format("namespace {0}\n", nsapce));
			builder.Append("{\n");
			string line = reader.ReadLine();
			while (line != null)
			{
				builder.Append("    ").Append(line).Append("\n");
				line = reader.ReadLine();
			}

			builder.Append("}");
			byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
			using (FileWriter writer = new FileWriter(dstfile))
			{
				writer.Write(data);
			}
		}
	}
}
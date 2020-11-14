using System;
using System.IO;

public class FileWriter : IDisposable
{
    private BinaryWriter mWriter;

    public FileWriter(string filename)
    {
        if (File.Exists(filename))
        {
            File.Delete(filename);
        }

        mWriter = new BinaryWriter(new FileStream(filename, FileMode.CreateNew, FileAccess.Write));
    }

    public void Write(byte value)
    {
        mWriter.Write(value);
    }

    public void Write(short value)
    {
        mWriter.Write(value);
    }

    public void Write(int value)
    {
        mWriter.Write(value);
    }

    public void Write(byte[] value)
    {
        mWriter.Write(value);
    }

    public void Write(byte[] value, int index, int count)
    {
        mWriter.Write(value, index, count);
    }

    public void Write(string value)
    {
        mWriter.Write(value);
    }

    public void Dispose()
    {
        mWriter.Flush();
        mWriter.Close();
    }
}
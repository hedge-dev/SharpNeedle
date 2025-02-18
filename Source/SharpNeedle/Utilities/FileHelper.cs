namespace SharpNeedle.Utilities;

public class FileHelper
{
    public static FileMode FileAccessToMode(FileAccess access)
    {
        switch (access)
        {
            case FileAccess.Read:
                return FileMode.Open;

            case FileAccess.Write:
            case FileAccess.ReadWrite:
                return FileMode.OpenOrCreate;
        }

        return FileMode.OpenOrCreate;
    }
}
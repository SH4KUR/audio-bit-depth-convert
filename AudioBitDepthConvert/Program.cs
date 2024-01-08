namespace AudioBitDepthConvert;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.Write("Enter the path of the directory: ");
        var directoryPath = Console.ReadLine();

        if (Directory.Exists(directoryPath))
        {
            ConvertFiles(directoryPath);

            Console.WriteLine("Converting completed successfully.");
        }
        else
        {
            Console.WriteLine("Directory not found.");
        }
    }
    
    private static void ConvertFiles(string directoryPath) 
    {
        var audioFiles = Directory.GetFiles(directoryPath, "*.wav", SearchOption.AllDirectories);

        foreach (var file in audioFiles)
        {
            ConvertBitDepth(file);
        }
    }

    private static void ConvertBitDepth(string filePath)
    {
        Console.WriteLine(filePath);

        // code
    }
}
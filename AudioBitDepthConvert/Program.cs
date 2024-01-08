using NAudio.Wave;

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
        using (var reader = new WaveFileReader(filePath))
        {
            if (reader.WaveFormat.BitsPerSample != 32)
            {
                return;
            }

            Console.WriteLine($"> {filePath}");
            
            var targetWaveFormat = new WaveFormat(reader.WaveFormat.SampleRate, 24, reader.WaveFormat.Channels);

            using (var conversionStream = new WaveFormatConversionStream(targetWaveFormat, reader))
            {
                using (var writer = new WaveFileWriter("24_" + filePath, conversionStream.WaveFormat))
                {
                    var buffer = new byte[8192];
                    int bytesRead;

                    while ((bytesRead = conversionStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }
}
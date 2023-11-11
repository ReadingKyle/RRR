using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroReadingRacing.StateManagement
{
    public static class NewTrack
    {
        public static void GenerateTrack()
        {
            // Specify the path for the text document
            string filePath = "content/example1.txt";

            // Example 1: Writing to a text document using StreamWriter
            WriteToFile(filePath, "tiles.png\r\n32, 32\r\n10, 10\r\n10,10,10,10,10,10,10,10,10,10,10,10,6,13,13,13,13,7,10,10,10,6,9,10,10,10,10,11,10,10,10,11,10,10,10,10,10,11,10,10,10,11,10,10,10,10,10,8,7,10,10,12,10,10,10,10,10,10,11,10,10,11,10,10,10,10,10,10,11,10,10,8,13,13,7,10,10,10,11,10,10,10,10,10,8,13,13,13,9,10,10,10,10,10,10,10,10,10,10,10");
        }

        static void WriteToFile(string filePath, string content)
        {
            // Write to the file (creates a new file or overwrites existing content)
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(content);
            }
        }
    }
}

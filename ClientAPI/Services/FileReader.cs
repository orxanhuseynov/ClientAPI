using ClientAPI.Models;

namespace ClientAPI.Services
{
    public class FileReader
    {
        public List<Data> GetData(string filePath)
        {
            var result = new List<Data>();

            if (File.Exists(filePath))
            {
                // This code section reads file line by line  
                using StreamReader file = new StreamReader(filePath);

                while (file.ReadLine() is { } ln)
                {
                    result.Add(new Data { Word = ln });
                }

                file.Close();
            }

            return result;
        }
    }
}

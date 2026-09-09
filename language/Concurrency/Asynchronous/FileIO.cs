namespace Asyncrhonous
{
    class FileWriter
    {
        public async Task Write(string fileName, string content)
        {
            using (StreamWriter streamWriter = new StreamWriter(fileName))
            {
                await streamWriter.WriteAsync(content);
            }
        }             
    }

    class FileReader
    {
        public async Task<string> Read(string fileName)
        {

            using (StreamReader streamReader = new StreamReader(fileName))
            {
                string content = await streamReader.ReadToEndAsync();
                return content;
            }
            
        }
    }

    internal class FileIO
    {
        public static async Task Demo()
        {
            //File.Create("demo.txt").Close();
            string fileName = "demo.txt";
            FileWriter fileWriter = new FileWriter();
            FileReader fileReader = new FileReader();

            // blocks writer task without bloking the current thread
            await fileWriter.Write(fileName, "Lorem ipsum dolor sit amet, consectetur adipiscing elit");

            // blocks reader task without bloking the current thread
            string content = await fileReader.Read(fileName); 
           
            Console.WriteLine("File content:\n" + content);
        }
    }
}

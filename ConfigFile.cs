using System.Text;

namespace VSFTConfig
{
    public class ConfigFile
    {
        public List<Section> Sections = new List<Section>();

        private string _configPath;
        private FileStream _configStream;

        public ConfigFile(string inputPath) {
            //check if the config path is valid
            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException($"Config {inputPath} does not exist.");
            }
            //open and save stream
            _configPath = inputPath;
            _configStream = File.OpenRead(_configPath);
            Sections = DetectSections();
        }


        //detect all sections and make them into a list
        private List<Section> DetectSections()
        {
            //config lines
            List<string> lines = GetConfigLines();

            //section detection lists
            List<(string, int)> sectionStarts = new List<(string, int)>();
            List<Section> sections = new List<Section>();

            int lineCounter = 0;
            foreach (string line in lines)
            {
                //empty line
                if (line.ToCharArray().Length == 0)
                {
                    lineCounter++;
                    continue;
                }
                //line has section start token
                if (line.ToCharArray()[0] == '>')
                {
                    //polish
                    string unpolished = line.Split('>')[1];
                    string name = unpolished.Replace(";","");

                    //add to starts
                    sectionStarts.Add((name,lineCounter));
                }
                lineCounter++;
            }

            //detect line endings
            lineCounter = 0;
            foreach ((string,int) start in sectionStarts)
            {
                try
                {
                    //find the first character of the next section
                    int endl = sectionStarts[lineCounter + 1].Item2;

                    sections.Add(new Section(start.Item1,start.Item2,endl,this));

                }catch(ArgumentOutOfRangeException) //exception thrown when the section is the last section
                {
                    //get the last line
                    int endl = lines.Count;

                    sections.Add(new Section(start.Item1, start.Item2, endl, this));
                }
                lineCounter++;
            }
            return sections;
        }

        //return all file lines
        public List<string> GetConfigLines()
        {
            //read bytes
            byte[] byteContents = new byte[(int)_configStream.Length];
            _configStream.Read(byteContents, 0, byteContents.Length);

            //utf-8
            string fullText = Encoding.UTF8.GetString(byteContents);

            //return lines using ; as line-break
            return new List<string>(fullText.Split(";"));
        }

        //clean-up
        public void Dispose()
        {
            if (IsStreamDisposed())
                return;
            _configStream.Close();
            _configStream.Dispose();
        }

        //check-up
        private bool IsStreamDisposed()
        {
            try
            {
                _configStream.Flush();
                return true;
            }catch(ObjectDisposedException)
            {
                return false;
            }
        }
    }
}
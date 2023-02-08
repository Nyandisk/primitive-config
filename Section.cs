using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSFTConfig
{
    public class Section
    {
        //public properties for common things
        public string SectionName { get; set; }
        public ConfigFile ParentConfig { get; set; }


        //private line variables used for scanning the config
        private int _endLine;
        private int _startLine;

        public Section(string name, int startLine, int endLine, ConfigFile config)
        {
            SectionName = name;
            ParentConfig = config;
            _startLine = startLine;
            _endLine = endLine;
        }
        //get all key value pairs in the section
        public List<Pair> GetSectionPairs()
        {
            List<Pair> pairs = new List<Pair>();
            
            int currentLine = 0;
            foreach(string line in ParentConfig.GetConfigLines())
            {
                //if we are not past the starting line, we don't care. continue.
                if (currentLine <= _startLine)
                {
                    currentLine++;
                    continue;
                }

                //if we are at the end line, we have read everything. break.
                if (currentLine == _endLine)
                {
                    currentLine++;
                    break;
                }

                //if the line is empty, we don't care. continue.
                if (line.ToCharArray().Length == 0 || string.IsNullOrWhiteSpace(line)) {
                    currentLine++;
                    continue;
                }

                //get pair
                Pair pair = new(line.Split('=')[0], line.Split('=')[1]);
                pairs.Add(pair);
                currentLine++;
            }
            return pairs;
        }

        //get all the keys in a section
        public List<string> GetKeys()
        {
            List<string> keys = new List<string>();
            List<Pair> pairs = GetSectionPairs();

            foreach (Pair p in pairs)
            {
                keys.Add(p.Key);
            }
            return keys;
        }

        //get value of a key (may be null)
        public string? GetValue(string key)
        {
            List<Pair> pairs = GetSectionPairs();
            foreach (Pair p in pairs)
            {
                if (p.Key.Equals(key))
                    return p.Value;
            }
            return null;
        }
    }
}

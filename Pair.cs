using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSFTConfig
{
    public class Pair
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Pair(string key, string value) {
            Key = key;  
            Value = value;
        }   
    }
}

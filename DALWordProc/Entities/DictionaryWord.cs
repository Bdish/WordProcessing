using System;
using System.Collections.Generic;
using System.Text;

namespace DALWordProc.Entities
{
    public class DictionaryWord
    {
        public int Id { get; set; }

        public string Word { get; set; }

        public int Frequency { get; set; }
    }
}

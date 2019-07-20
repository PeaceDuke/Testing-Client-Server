using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messagess
{
    [Serializable]
    public class TestMessage
    {
        public string FileName { get; }
        public string FileText { get; }

        public TestMessage(string fileName, string fileText)
        {
            FileName = fileName;
            FileText = fileText;
        }
    }
}

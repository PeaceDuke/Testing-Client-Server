using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messagess
{
    [Serializable]
    public class ResponseMessage
    {
        public string FileName { get;}
        public bool Response { get; set; }

        public ResponseMessage(string fileName, bool response)
        {
            FileName = fileName;
            Response = response;
        }
    }
}

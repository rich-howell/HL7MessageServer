using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7TCPListener
{
    public class AppConfig
    {
        public int Port { get; set; } = 4040;
        public string FolderPath { get; set; } = "";        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor2EV_EJ2
{
    internal class Cliente
    {
        public string NombreUsuario { get; set; }
        public string Ip { get; set; }
        public StreamWriter Sw { get; set; }
        public Cliente(string nombreUsuario,string ip,StreamWriter sw) { 
            NombreUsuario = nombreUsuario;
            Ip = ip;
            Sw = sw;
        }


    }
}

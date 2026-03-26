using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Servidor2EV_EJ2
{
    internal class Chatroom
    {
        private List<Cliente> clientes = new List<Cliente>();
        static readonly object l = new object();
        public int Port { get; set; } = 31416;
        public bool ServerRunning { get; set; } = true;
        public void InitServer()
        {

            IPEndPoint ie = new IPEndPoint(IPAddress.Any, Port);
            using (Socket s = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp))
            {
                s.Bind(ie);
                s.Listen(10);
                Console.WriteLine($"Chat server iniciado. " +
                $"Escuchando en {ie.Address}:{ie.Port}");
                Console.WriteLine("Esperando conexiones... (Ctrl+C para salir)");
                while (ServerRunning)
                {
                    Socket client = s.Accept();
                    Thread hilo = new Thread(() => ClientDispatcher(client));
                    hilo.Start();

                }
            }
        }
        private void ClientDispatcher(Socket sClient)
        {
            using (sClient)
            {
                IPEndPoint ieClient = (IPEndPoint)sClient.RemoteEndPoint;
                Console.WriteLine($"Cliente conectado:{ieClient.Address} " +
                    $"en puerto {ieClient.Port}");
                Encoding codificacion = Console.OutputEncoding;
                using (NetworkStream ns = new NetworkStream(sClient))
                using (StreamReader sr = new StreamReader(ns, codificacion))
                using (StreamWriter sw = new StreamWriter(ns, codificacion))
                {
                    sw.AutoFlush = true;
                    string welcome = "Bienvenido al chat de Alberto Carril";
                    sw.WriteLine(welcome);
                    sw.WriteLine("Escribe tu nombre de usuario:");
                    string nombre = sr.ReadLine();
                    if (!nombre.Equals(""))
                    {

                        Cliente cliente = new Cliente(nombre, ieClient.Address.ToString(), sw);

                        lock (l)
                        {
                            clientes.Add(cliente);
                            foreach (Cliente c in clientes)
                            {
                                c.Sw.WriteLine($"{cliente.NombreUsuario}@{cliente.Ip} entró al chat.");
                            }
                        }

                        try
                        {
                            while (true)
                            {
                                string msg = sr.ReadLine();
                                if (msg == null)
                                {
                                    break;
                                }
                                if (msg.Equals("#exit"))
                                {

                                    lock (l)
                                    {
                                        
                                        foreach (Cliente c in clientes)
                                        {
                                            c.Sw.WriteLine($"{cliente.NombreUsuario}@{cliente.Ip} se ha desconectado.");
                                            Console.WriteLine($"{cliente.NombreUsuario}@{cliente.Ip} se ha desconectado");
                                        }
                                    }
                                    break;
                                }

                                else if (msg.Equals("#list"))
                                {
                                    lock (l)
                                    {
                                        cliente.Sw.WriteLine($"Clientes conectados: ");
                                        foreach (Cliente c in clientes)
                                        {
                                            cliente.Sw.WriteLine($"Nombre: {c.NombreUsuario,15} Ip: {c.Ip,8}");

                                        }
                                    }
                                }
                                else
                                {
                                    lock (l)
                                    {
                                        foreach (Cliente c in clientes)
                                        {
                                            c.Sw.WriteLine($"{cliente.NombreUsuario}@{cliente.Ip}: {msg}");
                                        }
                                    }

                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            lock (l)
                            {
                                
                                foreach (Cliente c in clientes)
                                {
                                    c.Sw.WriteLine($"{cliente.NombreUsuario}@{cliente.Ip} se ha desconectado.");
                                    Console.WriteLine($"{cliente.NombreUsuario}@{cliente.Ip} se ha desconectado");
                                }
                            }
                        }
                        catch (SocketException ex)
                        {
                            lock (l)
                            {
                                
                                foreach (Cliente c in clientes)
                                {
                                    c.Sw.WriteLine($"{cliente.NombreUsuario}@{cliente.Ip} se ha desconectado.");
                                    Console.WriteLine($"{cliente.NombreUsuario}@{cliente.Ip} se ha desconectado");
                                }
                            }
                        }
                        clientes.Remove(cliente);

                    }

                }
            }
        }

    }
}

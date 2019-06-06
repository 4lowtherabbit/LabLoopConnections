using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;

namespace Loops.Controllers
{
    public class LoopController : Controller
    {
        static private TcpListener listener = null;
        static Int32 port;
        static IPAddress localAddr;

        static List<TcpClient> connections = new List<TcpClient>();
        static List<TcpClient> clients = new List<TcpClient>();

        // GET: Loop
        public ActionResult Index()
        {
            return View();
        }

        public string StartListening()
        {
            if(listener != null)
            {
                listener.Stop();
                listener = null;
            }

            // Set the TcpListener on port 13000.
            port = 13000;
            localAddr = IPAddress.Parse("127.0.0.1");

            // TcpListener server = new TcpListener(port);
            listener = new TcpListener(localAddr, port);

            // Start listening for client requests.
            listener.Start();

            return "Started";
        }

        public string StopListening()
        {
            foreach(var conn in connections)
            {
                conn.Close();
            }
            foreach(var client in clients)
            {
                client.Close();
            }
            listener.Stop();

            return "Stopped";
        }

        public string AddLoop()
        {
            TcpClient sender = new TcpClient();
            sender.Connect(localAddr, port);
            sender.GetStream().WriteByte(Convert.ToByte('a'));

            TcpClient client = listener.AcceptTcpClient();
            lock(connections)
            {
                connections.Add(client);
            }
            lock(clients)
            {
                clients.Add(sender);
            }
            return $"Done. Current loop connnections count:{connections.Count}";
        }

        public string AddWeakLoop()
        {
            TcpClient sender = new TcpClient();
            sender.Connect(localAddr, port);
            sender.GetStream().WriteByte(Convert.ToByte('a'));

            TcpClient client = listener.AcceptTcpClient();

            return $"Done.";
        }
    }
}
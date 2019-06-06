using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        public string Index()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Usage:<br/>");
            builder.AppendLine("Start listening on local port: /Loop/StartListening<br/>");
            builder.AppendLine("Add one more local loop connnection: /loop/AddLoop<br/>");
            builder.AppendLine("Add one more weeak local loop connnection: /loop/AddWeakLoop<br/>");
            builder.AppendLine("Close all connections and shut down listening: /loop/StopListening<br/>");
            builder.AppendLine("<br/>");
            builder.AppendLine($"Currently loops count: {connections.Count}<br/>");
            return builder.ToString();
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
            connections = new List<TcpClient>();
            foreach(var client in clients)
            {
                client.Close();
            }
            clients = new List<TcpClient>();
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
            return $"Done. Current loop connnections count: {connections.Count}";
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
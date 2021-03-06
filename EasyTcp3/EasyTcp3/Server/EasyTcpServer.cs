using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace EasyTcp3.Server
{
    /// <summary>
    /// Class that holds all the information and some functions of an EasyTcpServer
    /// See ServerUtils for more functions
    /// </summary>
    public class EasyTcpServer : IDisposable
    {
        /// <summary>
        /// BaseSocket of server,
        /// Gets disposed when calling Dispose()
        /// Null if disconnected
        /// </summary>
        public Socket BaseSocket { get; protected internal set; }

        /// <summary>
        /// Determines whether the server is running,
        /// set to true when server is started, set to false when server is disposed
        /// </summary>
        public bool IsRunning { get; protected internal set; }

        /// <summary>
        /// List with all the connected clients,
        /// clients get added when connected and removed when disconnected
        /// </summary>
        protected internal List<EasyTcpClient> ConnectedClients = new List<EasyTcpClient>();

        /// <summary>
        /// Get the number of connected clients
        /// </summary>
        public int ConnectedClientsCount => ConnectedClients.Count;

        /// <summary>
        /// IEnumerable of all connected clients
        /// Creates copy of ConnectedClients because this variable is used by async functions
        /// </summary>
        /// <returns>Copy of ConnectedClients</returns>
        public IEnumerable<EasyTcpClient> GetConnectedClients() => ConnectedClients.ToList();

        /// <summary>
        /// List of all connected sockets
        /// Creates copy of ConnectedClients because this variable is used by async functions
        /// </summary>
        /// <returns>Copy of the sockets in ConnectedClients</returns>
        public IEnumerable<Socket> GetConnectedSockets() => GetConnectedClients().Select(c => c.BaseSocket);

        /// <summary>
        /// Fired when a client connects to the server
        /// Dispose client in this event to dismiss the connection
        /// </summary>
        public event EventHandler<EasyTcpClient> OnConnect;

        /// <summary>
        /// Fired when a client disconnects from the server (After the client is disconnected!)
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;

        /// <summary>
        /// Fired when a client sends data to this server
        /// </summary>
        public event EventHandler<Message> OnDataReceive;

        /// <summary>
        /// Fired when an error occurs,
        /// if not set errors will be thrown
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Function used to fire the OnConnect event
        /// </summary>
        /// <param name="client"></param>
        protected internal void FireOnConnect(EasyTcpClient client) => OnConnect?.Invoke(this, client);

        /// <summary>
        /// Function used to fire the OnDisconnect event
        /// </summary>
        /// <param name="client"></param>
        protected internal void FireOnDisconnect(EasyTcpClient client)
        {
            lock (ConnectedClients) ConnectedClients.Remove(client);
            OnDisconnect?.Invoke(this, client);
        }

        /// <summary>
        /// Function used to fire the OnDataReceive event
        /// </summary>
        /// <param name="message"></param>
        protected internal void FireOnDataReceive(Message message) => OnDataReceive?.Invoke(this, message);

        /// <summary>
        /// Function used to fire the OnError event,
        /// or if event is null, throw an exception
        /// </summary>
        /// <param name="exception"></param>
        protected internal void FireOnError(Exception exception)
        {
            if (OnError != null) OnError.Invoke(this, exception);
#if DEBUG
            else throw exception;
#endif
        }

        /// <summary>
        /// Dispose current instance of the baseSocket if not null
        /// </summary>
        public void Dispose()
        {
            if (BaseSocket == null) return;
            IsRunning = false;
            lock (ConnectedClients)
            {
                foreach (var client in ConnectedClients) client.Dispose();
            }

            ConnectedClients.Clear();
            BaseSocket.Dispose();
            BaseSocket = null;
        }
    }
}
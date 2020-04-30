using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyTcp3.ClientUtils.Async
{
    /// <summary>
    /// Functions to async send a message to a remote host and then return the reply
    /// ! These functions do not work in the OnDataReceive event
    /// </summary>
    public static class SendAndGetReplyAsyncUtil
    {
        /// <summary>
        /// Default timeout used when no parameter is passed
        /// </summary>
        private const int DefaultTimeout = 5_000;

        /// <summary>
        /// Send data (byte[]) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns> 
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, byte[] data,
            TimeSpan? timeout = null)
        {
            if (client == null) throw new ArgumentException("Could not send: client is null");

            Message reply = null;
            using var signal = new SemaphoreSlim(0, 1); //Use SemaphoreSlim as async ManualResetEventSlim

            client.FireOnDataReceive = message =>
            {
                reply = message;
                client.FireOnDataReceive = client.FireOnDataReceiveEvent;
                // Function is no longer used when signal is disposed, therefore ignore this warning
                // ReSharper disable once AccessToDisposedClosure
                signal.Release();
            };
            client.Send(data);

            await signal.WaitAsync(timeout ?? TimeSpan.FromMilliseconds(DefaultTimeout));
            if (reply == null) client.FireOnDataReceive = client.FireOnDataReceiveEvent;
            return reply;
        }

        /// <summary>
        /// Send data (ushort) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, ushort data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (short) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, short data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (uint) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, uint data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (int) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, int data,
            TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (ulong) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, ulong data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (long) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, long data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (double) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, double data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (bool) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <returns>received data or null</returns>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, bool data, TimeSpan? timeout = null) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (string) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">data to send to server</param>
        /// <param name="timeout">maximum time to wait for a reply, if time expired: return null</param>
        /// <param name="encoding">Encoding type (Default: UTF8)</param>
        /// <returns>received data or null</returns>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, string data,
            TimeSpan? timeout = null,
            Encoding encoding = null)
            => await client.SendAndGetReplyAsync((encoding ?? Encoding.UTF8).GetBytes(data), timeout);
    }
}
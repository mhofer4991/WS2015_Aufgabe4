//-----------------------------------------------------------------------
// <copyright file="ExternalNetworkWindow.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents an external window, which communicates with the local window over the network.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents an external window, which communicates with the local window over the network.
    /// </summary>
    [Serializable]
    public class ExternalNetworkWindow : BasicWindow
    {
        /// <summary> The interval between each alive packets in milliseconds. </summary>
        public const int KeepAliveInterval = 10000;

        /// <summary> Used for locking certain objects. </summary>
        private readonly object lockObject = new object();

        /// <summary> The connected, opposite window of this window. </summary>
        [NonSerialized]
        private InternalNetworkWindow opposite;

        /// <summary> The TCP client, which represents the connection. </summary>
        [NonSerialized]
        private TcpClient client;

        /// <summary> The network stream of the TCP client. </summary>
        [NonSerialized]
        private NetworkStream stream;

        /// <summary> The last time a keep alive packet has been received. </summary>
        [NonSerialized]
        private long lastAlivePacket;

        /// <summary> Indicates whether the window is alive or not. </summary>
        [NonSerialized]
        private bool alive;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalNetworkWindow"/> class.
        /// </summary>
        /// <param name="basic">The basic window of this window.</param>
        /// <param name="client">The TCP client representing the connection.</param>
        public ExternalNetworkWindow(IWindow basic, TcpClient client) : base(basic.GetID(), basic.GetUniqueID())
        {
            this.client = client;
            this.SetParent(basic.GetParent());
            this.SetChildren(basic.GetChildren());
            this.SetClusterIdentifier(basic.GetClusterIdentifier());

            // this.client.NoDelay = true;
            // this.client.SendTimeout = 10000;
            // this.client.ReceiveTimeout = 10000;
            this.stream = client.GetStream();

            Thread thread = new Thread(new ThreadStart(this.WaitForMessage));
            thread.IsBackground = true;
            thread.Start();

            this.lastAlivePacket = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            this.alive = true;
        }

        /// <summary>
        /// Sets the connected, opposite window of this window.
        /// </summary>
        /// <param name="opposite">The connected, opposite window.</param>
        public void SetOpposite(InternalNetworkWindow opposite)
        {
            this.opposite = opposite;
        }

        /// <summary>
        /// Sends the given window message to this window.
        /// </summary>
        /// <param name="message">The given message.</param>
        public override void Send(WindowMessage message)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    message.Sender = this;

                    // Send message code
                    byte[] request = new byte[1] { (byte)NetworkMessageCode.Window_message_transfer };

                    // Send content
                    byte[] content = message.GetSerializedByteArray();

                    byte[] length = BitConverter.GetBytes(content.Length);

                    Monitor.Enter(this.lockObject);

                    try
                    {
                        this.stream.Write(request, 0, request.Length);

                        this.stream.Write(length, 0, length.Length);
                        this.stream.Write(content, 0, content.Length);

                        this.stream.Flush();
                    }
                    finally
                    {
                        Monitor.Exit(this.lockObject);
                    }

                    /*IFormatter formatter = new BinaryFormatter();

                    formatter.Serialize(this.stream, message);
                    this.stream.Flush();*/
                }
                catch (IOException)
                {
                    this.Kill();
                }
            });            
        }

        /// <summary>
        /// Checks if the window is still alive by investigating the last time a keep alive packet has been sent.
        /// </summary>
        public void CheckAliveStatus()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(ExternalNetworkWindow.KeepAliveInterval / 2);

                this.alive = false;
                this.SendKeepAlive();

                Thread.Sleep(ExternalNetworkWindow.KeepAliveInterval / 2);

                if (!this.alive)
                {
                    this.Kill();
                }
                else
                {
                    this.CheckAliveStatus();
                }
            });
        }

        /// <summary>
        /// Kills the window by removing it from the connected, opposite window.
        /// </summary>
        public void Kill()
        {
            if (this.opposite.GetParent() != null && this.opposite.GetParent().GetUniqueID().Equals(this.GetUniqueID()))
            {
                this.opposite.RemoveParent(true);
            }
            else
            {
                this.opposite.RemoveChild(this, true);
            }
        }

        /// <summary>
        /// Sends a Keep Alive packet to the window.
        /// </summary>
        public void SendKeepAlive()
        {
            try
            {
                byte[] response = new byte[1] { (byte)NetworkMessageCode.Keep_alive };
                
                Monitor.Enter(this.lockObject);

                try
                {
                    this.stream.Write(response, 0, response.Length);
                    this.stream.Flush();
                }
                finally
                {
                    Monitor.Exit(this.lockObject);
                }
            }
            catch (IOException)
            {
                this.Kill();
            }
        }

        /// <summary>
        /// Waits for a message and handles it.
        /// </summary>
        private void WaitForMessage()
        {
            try
            {
                while (true)
                {
                    if (this.stream.DataAvailable)
                    {
                        byte[] code = new byte[1];

                        if (this.stream.Read(code, 0, code.Length) == code.Length)
                        {
                            if (code[0] == (byte)NetworkMessageCode.Window_message_transfer)
                            {
                                // WindowMessage message = (WindowMessage)formatter.Deserialize(this.stream); 
                                byte[] length = new byte[4];

                                if (this.stream.Read(length, 0, length.Length) == length.Length)
                                {
                                    int size = BitConverter.ToInt32(length, 0);

                                    byte[] content = new byte[size];

                                    byte[] msg = new byte[content.Length];
                                    int bytesRead = 0;

                                    while (bytesRead < msg.Length)
                                    {
                                        int read = this.stream.Read(msg, bytesRead, msg.Length - bytesRead);
                                        bytesRead += read;

                                        if (read == 0)
                                        {
                                            break;
                                        }
                                    }

                                    WindowMessage message = WindowMessage.DeserializeByteArray(msg);

                                    message.Sender = this;

                                    this.opposite.Send(message);
                                }
                            }
                            else if (code[0] == (byte)NetworkMessageCode.Keep_alive)
                            {
                                this.lastAlivePacket = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                                this.alive = true;

                                this.SendKeepAlive();
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (IOException)
            {
                this.Kill();
            }
        }
    }
}

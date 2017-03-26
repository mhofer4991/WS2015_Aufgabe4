//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>The user can use this program to manage virtual windows over the network.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// The user can use this program to manage virtual windows over the network.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary> Represents the internal window, which can communicate with external windows. </summary>
        private InternalNetworkWindow internalWindow;

        /// <summary> Will be used to render the windows and interact with them. </summary>
        private NetworkView networkRenderer;

        /// <summary> Provides details about the current selected window. </summary>
        private WindowVM currentWindow;

        /// <summary> Provides details about the current selected message. </summary>
        private WindowMessageDetailVM currentMessageDetails;

        /// <summary> A list of messages which have been delivered to the internal window. </summary>
        private ObservableCollection<WindowMessageVM> deliveredMessages;
        
        /// <summary> A list of messages which have been forwarded by the internal window. </summary>
        private ObservableCollection<WindowMessageVM> forwardedMessages;

        /// <summary> A list of messages which have been received by the internal window. </summary>
        private ObservableCollection<WindowMessageVM> receivedMessages;

        /// <summary> A list of messages which have been sent by the internal window. </summary>
        private ObservableCollection<WindowMessageVM> sentMessages;

        /// <summary> A list of messages, which affect the prime generator. </summary>
        private ObservableCollection<WindowMessageVM> primeGeneratorMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.receivedMessages = new ObservableCollection<WindowMessageVM>();
            this.deliveredMessages = new ObservableCollection<WindowMessageVM>();
            
            this.forwardedMessages = new ObservableCollection<WindowMessageVM>();
            this.sentMessages = new ObservableCollection<WindowMessageVM>();

            this.primeGeneratorMessages = new ObservableCollection<WindowMessageVM>();

            this.currentWindow = new WindowVM();
            this.currentMessageDetails = new WindowMessageDetailVM();

            this.Loaded += this.MainWindow_Loaded;
            this.Closing += this.MainWindow_Closing;
            this.SizeChanged += this.MainWindow_SizeChanged;
        }

        /// <summary>
        /// Gets called when the window has been closed.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.internalWindow != null && this.internalWindow.IsListening)
            {
                this.internalWindow.Stop();
            }
        }

        /// <summary>
        /// Gets called when size of the window has been changed.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.networkRenderer != null)
            {
                this.networkRenderer.Render();
            }
        }

        /// <summary>
        /// Gets called when the program has been loaded.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.currentWindowFrame.DataContext = this.currentWindow;

            this.listOfMessages.ItemsSource = this.deliveredMessages;

            this.messageDetailsTabControl.DataContext = this.currentMessageDetails;
        }

        /// <summary>
        /// Gets called when the user selects another message list.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void TypesOfMessagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listOfMessages != null)
            {
                if (this.typesOfMessagesComboBox.SelectedIndex == 0)
                {
                    this.listOfMessages.ItemsSource = this.deliveredMessages;
                }
                else if (this.typesOfMessagesComboBox.SelectedIndex == 1)
                {
                    this.listOfMessages.ItemsSource = this.sentMessages;
                }
                else if (this.typesOfMessagesComboBox.SelectedIndex == 2)
                {
                    this.listOfMessages.ItemsSource = this.forwardedMessages;
                }
                else if (this.typesOfMessagesComboBox.SelectedIndex == 3)
                {
                    this.listOfMessages.ItemsSource = this.primeGeneratorMessages;
                }
                else
                {
                    // this.listOfMessages.ItemsSource = this.receivedMessages;
                }
            }
        }

        /// <summary>
        /// Gets called when the user creates the internal window.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void SetIDButton_Click(object sender, RoutedEventArgs e)
        {
            int id = 0;

            if (int.TryParse(this.windowIDTextBox.Text, out id))
            {
                this.internalWindow = new InternalNetworkWindow(id);
                this.internalWindow.SetClusterIdentifier(System.Environment.MachineName);

                this.internalWindow.OnStartSucceed += this.InternalWindow_OnStartSucceed;
                this.internalWindow.OnStartFailedDueToAlreadyUsedPort += this.InternalWindow_OnStartFailedDueToAlreadyUsedPort;

                this.internalWindow.OnConnectionSucceed += this.InternalWindow_OnConnectionSucceed;
                this.internalWindow.OnConnectionFailedDueToCycle += this.InternalWindow_OnConnectionFailedDueToCycle;
                this.internalWindow.OnConnectionFailedDueToExceed += this.InternalWindow_OnConnectionFailedDueToExceed;
                this.internalWindow.OnConnectionFailedDueToUnknown += this.InternalWindow_OnConnectionFailedDueToUnknown;

                this.internalWindow.OnMessageReceived += this.InternalWindow_OnMessageReceived;
                this.internalWindow.OnMessageDelivered += this.InternalWindow_OnMessageDelivered;
                this.internalWindow.OnMessageForwarded += this.InternalWindow_OnMessageForwarded;

                this.networkRenderer = new NetworkView(this.networkCanvas);
                this.networkRenderer.SetWindow(this.internalWindow);

                this.networkRenderer.OnWindowSelected += this.NetworkRenderer_OnWindowSelected;
                this.networkRenderer.OnWindowToolTipOpened += this.NetworkRenderer_OnWindowToolTipOpened;

                this.networkRenderer.Update();
                this.networkRenderer.Render();

                this.windowIDFrame.Visibility = Visibility.Collapsed;
                this.serverStartFrame.Visibility = Visibility.Visible;
                this.sendTextFrame.Visibility = Visibility.Visible;
                this.startPrimeFrame.Visibility = Visibility.Visible;

                this.internalWindowInformation.Text = "Window ID: " + id;
                this.internalWindowInformation.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("The ID must only contain numeric values!");
            }
        }

        /// <summary>
        /// Gets called when the user wants to see the tooltip content of the given window.
        /// </summary>
        /// <param name="window">The given window, whose tooltip content should be shown.</param>
        private void NetworkRenderer_OnWindowToolTipOpened(IWindow window)
        {
            WindowMessage message = new WindowMessage(Guid.NewGuid(), this.internalWindow, window, WindowMessageCode.List_of_messages, WindowMessageStatus.Transfer);
            message.TransferType = WindowMessageTransferType.LookForChilds;
            message.ExpectsResponse = false;
            message.Sender = this.internalWindow;

            message.AddressingType = WindowMessageAddressingType.UniqueID;

            this.internalWindow.Send(message);
        }

        /// <summary>
        /// Gets called when the user selects a window.
        /// </summary>
        /// <param name="window">The selected window.</param>
        private void NetworkRenderer_OnWindowSelected(IWindow window)
        {
            this.currentWindow.SetWindow(window);
        }

        /// <summary>
        /// Gets called when the internal window receives a message.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void InternalWindow_OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.receivedMessages.Insert(0, new WindowMessageVM(e.Message));
            }));
        }

        /// <summary>
        /// Gets called when the internal window receives a message and forwards it.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void InternalWindow_OnMessageForwarded(object sender, MessageReceivedEventArgs e)
        {
            if (!e.Message.SourceWindow.GetUniqueID().Equals(this.internalWindow.GetUniqueID()))
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.forwardedMessages.Insert(0, new WindowMessageVM(e.Message));
                }));
            }
        }

        /// <summary>
        /// Gets called when a message has been delivered to the internal window.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void InternalWindow_OnMessageDelivered(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message.Code == WindowMessageCode.Nodes_update)
            {
                this.networkCanvas.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.networkRenderer.Update();
                    this.networkRenderer.Render();
                }));
            }
            else if (e.Message.Code == WindowMessageCode.List_of_messages)
            {
                if (e.Message.Content == null)
                {
                    WindowMessage message = new WindowMessage(e.Message.ID, e.Message.TargetWindow, e.Message.SourceWindow, WindowMessageCode.List_of_messages, WindowMessageStatus.Transfer);
                    message.TransferType = WindowMessageTransferType.UseRoute;
                    message.SetRoute(e.Message.Route);
                    message.ExpectsResponse = false;

                    message.AddressingType = WindowMessageAddressingType.UniqueID;
                    
                    List<WindowMessageVM> messages = this.sentMessages.ToList();

                    // messages.AddRange(this.forwardedMessages);
                    message.Content = messages;

                    this.internalWindow.Send(message);
                }
                else
                {
                    this.networkRenderer.SetToolTipContent(e.Message.Content, e.Message.SourceWindow);
                }
            }
            else if (e.Message.Code == WindowMessageCode.Text_message)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.deliveredMessages.Insert(0, new WindowMessageVM(e.Message));
                }));
            }
            else if (e.Message.Code == WindowMessageCode.Start_prime_generator)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.deliveredMessages.Insert(0, new WindowMessageVM(e.Message));
                }));

                PrimeGeneratorStarter starter = new PrimeGeneratorStarter(e.Message.SourceWindow);

                starter.OnPrimeFound += this.Starter_OnPrimeFound;
                starter.OnGeneratorExited += this.Starter_OnGeneratorExited;
                starter.OnGeneratorNotFound += this.Starter_OnGeneratorNotFound;

                starter.Start((PrimeStartArguments)e.Message.Content);
            }
            else if (e.Message.Code == WindowMessageCode.Prime_found)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.primeGeneratorMessages.Insert(0, new WindowMessageVM(e.Message));
                }));
            }
            else if (e.Message.Code == WindowMessageCode.Prime_generator_exited)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.primeGeneratorMessages.Insert(0, new WindowMessageVM(e.Message));
                }));
            }
            else if (e.Message.Code == WindowMessageCode.Prime_generator_not_found)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.primeGeneratorMessages.Insert(0, new WindowMessageVM(e.Message));
                }));
            }
        }

        /// <summary>
        /// Gets called when the prime generator could not be found.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        private void Starter_OnGeneratorNotFound(PrimeGeneratorStarter sender)
        {
            WindowMessage message = new WindowMessage(Guid.NewGuid(), this.internalWindow, sender.InitiatingWindow, WindowMessageCode.Prime_generator_not_found, WindowMessageStatus.Transfer);
            message.TransferType = WindowMessageTransferType.LookForChilds;
            message.ExpectsResponse = false;
            message.Sender = this.internalWindow;

            // I assume that if there are multiple windows with the same ID, the prime results should NOT be sent to all these windows,
            // but only to the initiating window.
            message.AddressingType = WindowMessageAddressingType.UniqueID;

            // The window, which started the prime generator, also has to know that the prime generator could not be found.
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.primeGeneratorMessages.Insert(0, new WindowMessageVM(message));
            }));

            // If it is a local prime generator, we don't have to send a message.
            if (!this.internalWindow.GetUniqueID().Equals(message.TargetWindow.GetUniqueID()))
            {
                this.internalWindow.Send(message);
            }
        }

        /// <summary>
        /// Gets called when prime generator exited.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void Starter_OnGeneratorExited(object sender, PrimeGeneratorExitedEventArgs e)
        {
            PrimeGeneratorStarter starter = (PrimeGeneratorStarter)sender;

            WindowMessage message = new WindowMessage(Guid.NewGuid(), this.internalWindow, starter.InitiatingWindow, WindowMessageCode.Prime_generator_exited, WindowMessageStatus.Transfer);
            message.TransferType = WindowMessageTransferType.LookForChilds;
            message.ExpectsResponse = false;
            message.Content = e;
            message.Sender = this.internalWindow;

            // I assume that if there are multiple windows with the same ID, the prime results should NOT be sent to all these windows,
            // but only to the initiating window.
            message.AddressingType = WindowMessageAddressingType.UniqueID;

            // The window, which started the prime generator, also has to show the exit information.
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.primeGeneratorMessages.Insert(0, new WindowMessageVM(message));
            }));

            // If it is a local prime generator, we don't have to send a message.
            if (!this.internalWindow.GetUniqueID().Equals(message.TargetWindow.GetUniqueID()))
            {
                this.internalWindow.Send(message);
            }
        }

        /// <summary>
        /// Gets called when the prime generator found a prime.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void Starter_OnPrimeFound(object sender, PrimeFoundEventArgs e)
        {
            PrimeGeneratorStarter starter = (PrimeGeneratorStarter)sender;

            WindowMessage message = new WindowMessage(Guid.NewGuid(), this.internalWindow, starter.InitiatingWindow, WindowMessageCode.Prime_found, WindowMessageStatus.Transfer);
            message.TransferType = WindowMessageTransferType.LookForChilds;
            message.ExpectsResponse = false;
            message.Content = e;
            message.Sender = this.internalWindow;

            // I assume that if there are multiple windows with the same ID, the prime results should NOT be sent to all these windows,
            // but only to the initiating window.
            message.AddressingType = WindowMessageAddressingType.UniqueID;

            this.internalWindow.Send(message);
        }

        /// <summary>
        /// Gets called when the user wants to start the listener of the internal window.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int port = 0;

            if (int.TryParse(this.internalPortTextBox.Text, out port))
            {
                this.internalWindow.Start(port);
            }
            else
            {
                MessageBox.Show("The port must only contain numeric values!");
            }
        }

        /// <summary>
        /// Gets called when the user wants to connect to an external window.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            IPAddress address;

            if (IPAddress.TryParse(this.addressTextBox.Text, out address))
            {
                int port;

                if (int.TryParse(this.portTextBox.Text, out port))
                {
                    this.internalWindow.ConnectToExternalWindow(address, port);

                    this.connectButton.Visibility = Visibility.Collapsed;
                    this.connectingButton.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("The port must only contain numeric values!");
                }
            }
            else
            {
                MessageBox.Show("The IP address has an invalid format!");
            }
        }

        /// <summary>
        /// Gets called when the user wants to send a message to the currently selected window.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void SendToCurrentTextButton_Click(object sender, RoutedEventArgs e)
        {
            WindowMessage message = new WindowMessage(Guid.NewGuid(), this.internalWindow, this.currentWindow.Window, WindowMessageCode.Text_message, WindowMessageStatus.Transfer);
            message.TransferType = WindowMessageTransferType.LookForChilds;
            message.ExpectsResponse = false;
            message.Content = sendTextBox.Text;
            message.Sender = this.internalWindow;

            this.sentMessages.Insert(0, new WindowMessageVM(message));

            if (this.internalWindow.TransferMessage(message) < 1)
            {
                MessageBox.Show("The message could not be sent to the selected window!");
            }
        }

        /// <summary>
        /// Gets called when the user wants to send a message to all connected windows.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void SendToAllTextButton_Click(object sender, RoutedEventArgs e)
        {
            WindowMessage message = new WindowMessage(Guid.NewGuid(), this.internalWindow, null, WindowMessageCode.Text_message, WindowMessageStatus.Transfer);
            message.TransferType = WindowMessageTransferType.Broadcast;
            message.ExpectsResponse = false;
            message.Content = sendTextBox.Text;
            message.Sender = this.internalWindow;

            this.sentMessages.Insert(0, new WindowMessageVM(message));

            // this.SendMessage(message);
            this.internalWindow.TransferMessage(message);
        }

        /// <summary>
        /// Gets called when the listener of the internal window started successfully.
        /// </summary>
        private void InternalWindow_OnStartSucceed()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.serverStartFrame.Visibility = Visibility.Collapsed;
                this.serverConnectFrame.Visibility = Visibility.Visible;
            }));
        }

        /// <summary>
        /// Gets called when the listener of the internal window could not be started
        /// because the port is already used.
        /// </summary>
        private void InternalWindow_OnStartFailedDueToAlreadyUsedPort()
        {
            MessageBox.Show("The port is already used!");
        }

        /// <summary>
        /// Gets called when the internal window connected successfully to an external window.
        /// </summary>
        private void InternalWindow_OnConnectionSucceed()
        {
            this.networkCanvas.Dispatcher.BeginInvoke(new Action(() =>
            {
                // this.networkRenderer.Render();
            }));

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.serverConnectFrame.Visibility = Visibility.Collapsed;
                this.connectButton.Visibility = Visibility.Visible;
                this.connectingButton.Visibility = Visibility.Collapsed;
            }));
        }

        /// <summary>
        /// Gets called when the window could not connect to an 
        /// external window because it would exceed the maximum amount of windows.
        /// </summary>
        private void InternalWindow_OnConnectionFailedDueToExceed()
        {
            MessageBox.Show("The connection to the window failed because the maximum amount of windows exceeded!");
            
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.connectButton.Visibility = Visibility.Visible;
                this.connectingButton.Visibility = Visibility.Collapsed;
            }));
        }

        /// <summary>
        /// Gets called when the window could not connect to an external window because it would create a cycle.
        /// </summary>
        private void InternalWindow_OnConnectionFailedDueToCycle()
        {
            MessageBox.Show("The connection to the window was not possible because it would represent a cycle!");

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.connectButton.Visibility = Visibility.Visible;
                this.connectingButton.Visibility = Visibility.Collapsed;
            }));
        }

        /// <summary>
        /// Gets called when the window could not connect to an external window because it could not be found.
        /// </summary>
        private void InternalWindow_OnConnectionFailedDueToUnknown()
        {
            MessageBox.Show("The connection to the window failed because it could not be found!");

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.connectButton.Visibility = Visibility.Visible;
                this.connectingButton.Visibility = Visibility.Collapsed;
            }));
        }

        /// <summary>
        /// Gets called when the user clicked on an item in the messages list.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void ListOfMessages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                WindowMessageVM clicked = (WindowMessageVM)e.AddedItems[0];

                this.currentMessageDetails.SetMessage(clicked.Message);
            }
        }

        /// <summary>
        /// Gets called when the user wants to start a local prime generator.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void StartLocalPrimeButton_Click(object sender, RoutedEventArgs e)
        {
            this.StartPrimeGeneratorAtWindow(this.internalWindow);
        }

        /// <summary>
        /// Gets called when the user wants to start a prime generator at the currently selected window.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void StartCurrentPrimeButton_Click(object sender, RoutedEventArgs e)
        {
            this.StartPrimeGeneratorAtWindow(this.currentWindow.Window);
        }

        /// <summary>
        /// Starts the prime generator on the given window.
        /// </summary>
        /// <param name="window">The given window, which starts the prime generator.</param>
        private void StartPrimeGeneratorAtWindow(IWindow window)
        {
            Views.PrimeGeneratorPrompt prompt = new Views.PrimeGeneratorPrompt();
            prompt.Left = this.Left + ((this.ActualWidth - prompt.Width) / 2);
            prompt.Top = this.Top + ((this.ActualHeight - prompt.Height) / 2);

            if (prompt.ShowDialog() == true)
            {
                PrimeStartArguments args = new PrimeStartArguments(prompt.ThreadCount, prompt.Offset, prompt.UpdateInterval);

                WindowMessage message = new WindowMessage(Guid.NewGuid(), this.internalWindow, window, WindowMessageCode.Start_prime_generator, WindowMessageStatus.Transfer);
                message.TransferType = WindowMessageTransferType.LookForChilds;
                message.AddressingType = WindowMessageAddressingType.UniqueID;
                message.ExpectsResponse = false;
                message.Content = args;
                message.Sender = this.internalWindow;

                this.sentMessages.Insert(0, new WindowMessageVM(message));

                this.internalWindow.Send(message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace WAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Client's socket
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public MainWindow()
        {
            InitializeComponent();

            // Starts the client connection to server in the background
            runClient();
        }

        // Running the socket connection and reading in the background (Async)
        // If there is no need to run the program in the background, async can be replaced with public
        // Only need to run in the background if the client is constantly waiting on info (without the use of button clicks)
        // Alternative is to use connection handler
        #region runClient() function
        async void runClient()
        {
            await Task.Run(() =>
            {
                try
                {
                    // Connect to server at IP address 127.0.0.1 with port 9000
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);
                    socket.Connect(remoteEP);
                }
                catch (SocketException e)
                {
                    MessageBox.Show("Unable to connect to server.");
                    return;
                }

                NetworkStream stream = new NetworkStream(socket);
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);

                while (true)
                {
                    // Checks if still connected and close connection if !connected
                    if (!socket.Connected)
                    {
                        // not sure if this is the proper way to shutdown
                        MessageBox.Show("Server disconnected");
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                        break;
                    }
                    try
                    {
                        // If connected, waits input from server
                        string input = reader.ReadLine();

                        // do whatever with input
                    }
                    catch (Exception ex)
                    {
                        // Assuming something went wrong when reading from server (server disconnected/etc)
                        // not sure if this is the proper way to shutdown
                        MessageBox.Show("Server disconnected");
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                        break;
                    }
                }
            });
        }
        #endregion

        // Prevent cross-threading and setting text
        #region setText() function
        public void SetText(string msg)
        {
            // I assume this is the way to prevent cross threading from happening, not sure yet to be modified later
            //Action action = () => myTextBlock.Text = "Test";
            //var dispatcher = myTextBlock.Dispatcher;
            //if (dispatcher.CheckAccess())
            //    action();
                //myTextBlock.Text = msg;
                //myLabel.Content = msg;
            //else
            //    dispatcher.Invoke(action);
            //if (this.InvokeRequired)
            //{
            //    SetTextCallback d = new SetTextCallback(SetText);
            //    this.Invoke(d, msg);
            //    return;
            //}
            //lblTurn.Text = msg;
        }
        #endregion

        // Base load function (example)
        // Use of openFileDialogue to be user friendly
        // Not sure about the exact code for WPF https://marlongrech.wordpress.com/2008/05/28/wpf-dialogs-and-dialogresult/
        #region loadFile() function
        public void loadFile()
        {
            //DialogResult result = new DialogResult();
            //result = ofdLoad.ShowDialog();
            //int count = 0;
            //if (result == DialogResult.OK)
            //{
            //    string[] data = File.ReadAllLines(ofdLoad.FileName);
            //    foreach (var name in data)
            //    {
            //        string[] temp = Convert.ToString(name).Split(';');
            //        teleList.Add(temp[0], temp[1]);
            //        count++;
            //    }
            //    txtDisplay.Text = "Number of records loaded: " + count;
            //}
            //else if (result == DialogResult.Cancel)
            //{
            //    MessageBox.Show("Loading is cancelled!");
        }
        #endregion

        // Base save function (example)
        // Use of SaveFileDialog to be user friendly
        // Not sure about WPF counterpart
        #region saveFile() function
        public void saveFile()
        {
            //SaveFileDialog save = new SaveFileDialog();
            //save.Filter = "Text File | *.txt";
            //if (save.ShowDialog() == DialogResult.OK)
            //{
            //    StreamWriter writer = new StreamWriter(save.OpenFile());
            //    foreach (var kvp in teleList)
            //    {
            //        writer.WriteLine(kvp.Key + ";" + kvp.Value);
            //    }
            //    writer.Dispose();
            //    writer.Close();
            //    txtDisplay.Text = "Successfully saved!";
            //}
        }
        #endregion
    }
}

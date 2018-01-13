using Microsoft.Win32;
using System;
using System.Collections;
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
using System.Timers;
using System.Threading;
using System.Windows.Media.Animation;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace WAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static user currentUser = new user();
        // Client's socket
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // If running as server, we can contain all client's socket
        public static ArrayList arrSocket = new ArrayList();

        public static List<Movie> movieList = new List<Movie>();

        public static DateTime updatedTime;

        public static int listStart = 0;

        public static int currentSelectedMovie = 0;

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Starts the client connection to server in the background
            // We can change this to on button click, run client
            runClient();
            startAnimationHandler();

        }

        // we can help the user specify ip address + port to connect to.

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
                    // We can make a popup window to specify what address and port
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
                        //string input = reader.ReadLine();

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

        // If needed, we can run this GUI as a server. code is here.
        #region runServer() function
        async void runServer()
        {
            // again we can make the user specify the port if needed
            int port = 7000;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(endpoint);
            socket.Listen(10);
            //SetText("Waiting for clients on port " + port);
            await Task.Run(() =>
            {
                while (true)
                {
                    //try
                    //{
                    //    // if we want every client to have the possibility of being a server, we have to add connection handler class
                    //    Socket client = socket.Accept();
                    //    ConnectionHandler handler = new ConnectionHandler(client, this);
                    //    ThreadPool.QueueUserWorkItem(new WaitCallback(handler.HandleConnection));
                    //}
                    //catch (Exception)
                    //{
                    //    SetText("Connection falied on port " + port);
                    //}
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
        // Use of openFileDialog to be user friendly
        #region loadFile() function
        public void loadFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            // default file name, file extension, filter file extension
            dlg.FileName = "Document";
            dlg.DefaultExt = ".text";
            dlg.Filter = "Text documents (.txt)|*.txt";

            if (dlg.ShowDialog() == true)
            {
                // read lines from filename
                string[] data = File.ReadAllLines(dlg.FileName);
                foreach (var key in data)
                {
                    string[] temp = key.Split(';');
                    //dict.Add(temp[0], temp[1]);
                }
                //txtDsiplay.Text = "File loaded!";
            }
        }
        #endregion

        // Base save function (example)
        // Use of SaveFileDialog to be user friendly
        #region saveFile() function
        public void saveFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            // default file name, file extension, filter file extension
            dlg.FileName = "newDocument";
            dlg.DefaultExt = ".text";
            dlg.Filter = "Text documents (.txt)|*.txt";

            //string info = "things to be saved";
            //StreamWriter writer = new StreamWriter(dlg.OpenFile());

            if (dlg.ShowDialog() == true)
            {

                // Save document
                //File.WriteAllText(dlg.FileName, info);
                StreamWriter writer = new StreamWriter(dlg.OpenFile());
                //foreach (var kvp in dict)
                //{
                //    writer.WriteLine(kvp.Key + ";" + kvp.Value);
                //}
                writer.Dispose();
                writer.Close();
                //txtDsiplay.Text = "Successfully saved!";
            }
        }
        #endregion


        public void startAnimationHandler()
        {
            DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
            startCanvas.BeginAnimation(Canvas.OpacityProperty, ani);
        }

        private void imgHome1_MouseEnter(object send, MouseEventArgs e)
        {
            DoubleAnimation ani = new DoubleAnimation(0.5, TimeSpan.FromSeconds(0.3));
            rctHomeOpacity1.BeginAnimation(Rectangle.OpacityProperty, ani);
            lblHomeMovie1.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFFFF");
        }
        private void imgHome1_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.3));
            rctHomeOpacity1.BeginAnimation(Rectangle.OpacityProperty, ani);
            lblHomeMovie1.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF000000");
        }
        private void imgHome1_ClickEvent(object sender, MouseEventArgs e)
        {
            if (currentUser.getEmail() == null)
            {
                hideHomeGrid();
                showLoginGrid();
            }
            else
            {
                lblHomeMovie1.Content = "Not working";
            }
        }

        private void btnHomeRegister_Click(object sender, RoutedEventArgs e)
        {
            hideHomeGrid();
            showRegisterGrid();
        }
        private void btnHomeSignIn_Click(object sender, RoutedEventArgs e)
        {
            hideHomeGrid();
            showLoginGrid();
        }

        private void hideHomeGrid()
        {
            DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            homeGrid.BeginAnimation(Grid.OpacityProperty, ani);
            homeGrid.IsEnabled = false;
            homeGrid.Visibility = Visibility.Hidden;
        }
        private void showHomeGrid()
        {
            homeGrid.Opacity = 0;
            homeGrid.IsEnabled = true;
            homeGrid.Visibility = Visibility.Visible;
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            homeGrid.BeginAnimation(Grid.OpacityProperty, ani);
        }
        private void hideLoginGrid()
        {
            DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            loginGrid.BeginAnimation(Grid.OpacityProperty, ani);
            loginGrid.IsEnabled = false;
            loginGrid.Visibility = Visibility.Hidden;
        }
        private void showLoginGrid()
        {
            loginGrid.Opacity = 0;
            loginGrid.IsEnabled = true;
            loginGrid.Visibility = Visibility.Visible;
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            loginGrid.BeginAnimation(Grid.OpacityProperty, ani);
        }
        private void hideRegisterGrid()
        {
            DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            registerGrid.BeginAnimation(Grid.OpacityProperty, ani);
            registerGrid.IsEnabled = false;
            registerGrid.Visibility = Visibility.Hidden;
        }
        private void showRegisterGrid()
        {
            registerGrid.Opacity = 0;
            registerGrid.IsEnabled = true;
            registerGrid.Visibility = Visibility.Visible;
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            registerGrid.BeginAnimation(Grid.OpacityProperty, ani);
        }
        private void hideListGrid()
        {
            listStart = 0;
            DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            listGrid.BeginAnimation(Grid.OpacityProperty, ani);
            listGrid.IsEnabled = false;
            listGrid.Visibility = Visibility.Hidden;
        }
        private void showListGrid()
        {
            lblListLabel1.Content = "";
            lblListLabel2.Content = "";
            lblListLabel3.Content = "";
            lblListLabel4.Content = "";
            lblListLabel5.Content = "";
            lblListLabel6.Content = "";
            lblListLabel7.Content = "";
            lblListLabel8.Content = "";
            lblListLabel9.Content = "";
            lblListLabel10.Content = "";
            imgListImage1.Source = null;
            imgListImage2.Source = null;
            imgListImage3.Source = null;
            imgListImage4.Source = null;
            imgListImage5.Source = null;
            imgListImage6.Source = null;
            imgListImage7.Source = null;
            imgListImage8.Source = null;
            imgListImage9.Source = null;
            imgListImage10.Source = null;
            if (movieList.Count() == 0)
            {
                NetworkStream stream = new NetworkStream(socket);
                StreamWriter writer = new StreamWriter(stream);
                StreamReader read = new StreamReader(stream);
                writer.AutoFlush = true;
                writer.WriteLine("request_movie");
                string xml = "";
                string line;
                //string randomVar = read.ReadLine();
                var xs = new XmlSerializer(typeof(HashSet<Movie>));
                while ((line = read.ReadLine()) != "endofxml")
                {
                    xml += line;
                }
                HashSet<Movie> newSet = new HashSet<Movie>();
                using (var reader = new StringReader(xml))
                {
                    newSet = (HashSet<Movie>)xs.Deserialize(reader);
                }
                foreach (Movie details in newSet)
                {
                    if (details.Status == true)
                    {
                        movieList.Add(details);
                    }
                }
                updatedTime = DateTime.Now;
            }
            else
            {
                TimeSpan diff = DateTime.Now - updatedTime;
                if (diff.TotalDays > 0)
                {
                    NetworkStream stream = new NetworkStream(socket);
                    StreamWriter writer = new StreamWriter(stream);
                    StreamReader read = new StreamReader(stream);
                    writer.AutoFlush = true;
                    writer.WriteLine("request_movie");
                    string xml = "";
                    string line;
                    //string randomVar = read.ReadLine();
                    var xs = new XmlSerializer(typeof(HashSet<Movie>));
                    while ((line = read.ReadLine()) != "endofxml")
                    {
                        xml += line;
                    }
                    HashSet<Movie> newSet = new HashSet<Movie>();
                    using (var reader = new StringReader(xml))
                    {
                        newSet = (HashSet<Movie>)xs.Deserialize(reader);
                    }
                    foreach (Movie details in newSet)
                    {
                        if (details.Status == true)
                        {
                            movieList.Add(details);
                        }
                    }
                    updatedTime = DateTime.Now;
                }
            }
            int counter = 1;
            int x = 0;
            if (movieList.Count < listStart + 10)
            {
                x = movieList.Count;
            }
            else
            {
                x = listStart + 10;
            }
            for (int i = listStart; i < x; i ++)
            {
                if (counter == 1)
                {
                    lblListLabel1.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage1.Source = image;
                    counter += 1;
                }
                else if (counter == 2)
                {
                    lblListLabel2.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage2.Source = image;
                    counter += 1;
                }
                else if (counter == 3)
                {
                    lblListLabel3.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage3.Source = image;
                    counter += 1;
                }
                else if (counter == 4)
                {
                    lblListLabel4.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage4.Source = image;
                    counter += 1;
                }
                else if (counter == 5)
                {
                    lblListLabel5.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage5.Source = image;
                    counter += 1;
                }
                else if (counter == 6)
                {
                    lblListLabel6.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage6.Source = image;
                    counter += 1;
                }
                else if (counter == 7)
                {
                    lblListLabel7.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage7.Source = image;
                    counter += 1;
                }
                else if (counter == 8)
                {
                    lblListLabel8.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage8.Source = image;
                    counter += 1;
                }
                else if (counter == 9)
                {
                    lblListLabel9.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage9.Source = image;
                    counter += 1;
                }
                else if (counter == 10)
                {
                    lblListLabel10.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage10.Source = image;
                    counter += 1;
                }
            }
            if (listStart > 10)
            {
                btnListPrev.Visibility = Visibility.Visible;
                btnListPrev.IsEnabled = true;
            }
            else
            {
                btnListPrev.Visibility = Visibility.Hidden;
                btnListPrev.IsEnabled = false;
            }
            listStart += 10;
            if (movieList.ElementAtOrDefault(listStart) != null)
            {
                btnListNext.Visibility = Visibility.Visible;
                btnListNext.IsEnabled = true;
            }
            else
            {
                btnListNext.Visibility = Visibility.Hidden;
                btnListNext.IsEnabled = false;
            }
            listGrid.Opacity = 0;
            listGrid.IsEnabled = true;
            listGrid.Visibility = Visibility.Visible;
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            listGrid.BeginAnimation(Grid.OpacityProperty, ani);
        }

        private void hideMovieGrid()
        {
            currentSelectedMovie = 0;
            DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            movieGrid.BeginAnimation(Grid.OpacityProperty, ani);
            movieGrid.IsEnabled = false;
            movieGrid.Visibility = Visibility.Hidden;
        }

        private void showMovieGrid()
        {
            var image = LoadImage(movieList[currentSelectedMovie].FileData);
            imgMovie.Source = image;
            lblMovieTitle.Content = movieList[currentSelectedMovie].Title;
            lblMovieType.Content = movieList[currentSelectedMovie].MovieType;
            lblMoviePrice.Content = "$ " + String.Format("{0:.00}", movieList[currentSelectedMovie].Price);
            movieGrid.IsEnabled = true;
            movieGrid.Visibility = Visibility.Visible;
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            movieGrid.BeginAnimation(Grid.OpacityProperty, ani);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            NetworkStream stream = new NetworkStream(socket);
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            try
            {
                writer.AutoFlush = true;
                writer.WriteLine("login");
                writer.WriteLine(txtLoginEmail.Text);
                string password = sha256_hash(txtLoginPassword.Text);
                writer.WriteLine(password);
                if (reader.ReadLine() == "authorized")
                {
                    currentUser.setFirstName(reader.ReadLine());
                    currentUser.setMiddleName(reader.ReadLine());
                    currentUser.setLastName(reader.ReadLine());
                    currentUser.setDOB(reader.ReadLine());
                    currentUser.setEmail(txtLoginEmail.Text);
                    currentUser.setPassword(password);
                    txtLoginEmail.Text = "";
                    txtLoginPassword.Text = "";
                    lblLoginIncorrect.Visibility = Visibility.Hidden;
                    hideLoginGrid();
                    showHomeGrid();
                    btnHomeSignIn.IsEnabled = false;
                    btnHomeSignIn.Visibility = Visibility.Hidden;
                    btnHomeRegister.IsEnabled = false;
                    btnHomeRegister.Visibility = Visibility.Hidden;
                    btnHomeSignIn.Visibility = Visibility.Hidden;
                    lblHomeHello.Visibility = Visibility.Visible;
                    lblHomeName.Visibility = Visibility.Visible;
                    if ((currentUser.getFirstName() + " " + currentUser.getMiddleName() + " " + currentUser.getLastName()).Length > 20)
                    {
                        if ((currentUser.getFirstName() + " " + (currentUser.getMiddleName())[0] + ". " + currentUser.getLastName()).Length > 20)
                        {
                            if ((currentUser.getFirstName() + " " + (currentUser.getMiddleName())[0] + ". " + currentUser.getLastName()[0] + ".").Length > 20)
                            {
                                lblHomeName.Content = currentUser.getFirstName()[0] + ". " + (currentUser.getMiddleName())[0] + ". " + currentUser.getLastName()[0] + ".";
                            }
                            else
                            {
                                lblHomeName.Content = currentUser.getFirstName() + " " + (currentUser.getMiddleName())[0] + ". " + currentUser.getLastName()[0] + ".";
                            }
                        }
                        else
                        {
                            lblHomeName.Content = currentUser.getFirstName() + " " + (currentUser.getMiddleName())[0] + ". " + currentUser.getLastName();
                        }
                    }
                    else
                    {
                        lblHomeName.Content = currentUser.getFirstName() + " " + currentUser.getMiddleName() + " " + currentUser.getLastName();
                    }
                }
                else
                {
                    lblLoginIncorrect.Visibility = Visibility.Visible;
                    txtLoginPassword.Text = "";
                }
            }
            catch (Exception error)
            {
                throw error;
            }
            
        }
        public static String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        private void btnLoginRegister_Click(object sender, RoutedEventArgs e)
        {
            hideLoginGrid();
            showRegisterGrid();
        }

        private void btnRegisterRegister_Click(object sender, RoutedEventArgs e)
        {
            if (txtRegisterPassword.Text == txtRegisterConfirmPassword.Text)
            {
                NetworkStream stream = new NetworkStream(socket);
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                writer.AutoFlush = true;
                writer.WriteLine("register");
                writer.WriteLine(txtRegisterEmail.Text);
                writer.WriteLine(sha256_hash(txtRegisterPassword.Text));
                writer.WriteLine(txtRegisterFirstName.Text);
                writer.WriteLine(txtRegisterMiddleName.Text);
                writer.WriteLine(txtRegisterLastName.Text);
                writer.WriteLine(txtRegisterDOB.Text);
                writer.Flush();
                if (reader.ReadLine() == "success")
                {
                    currentUser.setEmail(txtRegisterEmail.Text);
                    currentUser.setPassword(sha256_hash(txtRegisterPassword.Text));
                    currentUser.setFirstName(txtRegisterFirstName.Text);
                    currentUser.setMiddleName(txtRegisterMiddleName.Text);
                    currentUser.setLastName(txtRegisterLastName.Text);
                    currentUser.setDOB(txtRegisterDOB.Text);
                    txtRegisterEmail.Text = "";
                    txtRegisterPassword.Text = "";
                    txtRegisterConfirmPassword.Text = "";
                    txtRegisterFirstName.Text = "";
                    txtRegisterMiddleName.Text = "";
                    txtRegisterLastName.Text = "";
                    txtRegisterDOB.Text = "";
                    hideRegisterGrid();
                    showHomeGrid();
                    btnHomeSignIn.IsEnabled = false;
                    btnHomeSignIn.Visibility = Visibility.Hidden;
                    btnHomeRegister.IsEnabled = false;
                    btnHomeRegister.Visibility = Visibility.Hidden;
                    btnHomeSignIn.Visibility = Visibility.Hidden;
                    lblHomeHello.Visibility = Visibility.Visible;
                    lblHomeName.Visibility = Visibility.Visible;
                    if ((currentUser.getFirstName() + " " + currentUser.getMiddleName() + " " + currentUser.getLastName()).Length > 20)
                    {
                        if ((currentUser.getFirstName() + " " + (currentUser.getMiddleName())[0] + ". " + currentUser.getLastName()).Length > 20)
                        {
                            if ((currentUser.getFirstName() + " " + (currentUser.getMiddleName())[0] + ". " + currentUser.getLastName()[0] + ".").Length > 20)
                            {
                                lblHomeName.Content = currentUser.getFirstName()[0] + ". " + (currentUser.getMiddleName())[0] + ". " + currentUser.getLastName()[0] + ".";
                            }
                            else
                            {
                                lblHomeName.Content = currentUser.getFirstName() + " " + (currentUser.getMiddleName())[0] + ". " + currentUser.getLastName()[0] + ".";
                            }
                        }
                        else
                        {
                            lblHomeName.Content = currentUser.getFirstName() + " " + (currentUser.getMiddleName())[0] + ". " + currentUser.getLastName();
                        }
                    }
                    else
                    {
                        lblHomeName.Content = currentUser.getFirstName() + " " + currentUser.getMiddleName() + " " + currentUser.getLastName();
                    }
                }
                else
                {

                }
            }
            else
            {

            }
        }

        private void rctHomeOpacity2_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation ani = new DoubleAnimation(0.5, TimeSpan.FromSeconds(0.3));
            rctHomeOpacity2.BeginAnimation(Rectangle.OpacityProperty, ani);
        }
        private void rctHomeOpacity2_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.3));
            rctHomeOpacity2.BeginAnimation(Rectangle.OpacityProperty, ani);
        }

        private void rctHomeOpacity2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            hideHomeGrid();
            showListGrid();
        }

        private void btnListNext_Click(object sender, RoutedEventArgs e)
        {
            lblListLabel1.Content = "";
            lblListLabel2.Content = "";
            lblListLabel3.Content = "";
            lblListLabel4.Content = "";
            lblListLabel5.Content = "";
            lblListLabel6.Content = "";
            lblListLabel7.Content = "";
            lblListLabel8.Content = "";
            lblListLabel9.Content = "";
            lblListLabel10.Content = "";
            imgListImage1.Source = null;
            imgListImage2.Source = null;
            imgListImage3.Source = null;
            imgListImage4.Source = null;
            imgListImage5.Source = null;
            imgListImage6.Source = null;
            imgListImage7.Source = null;
            imgListImage8.Source = null;
            imgListImage9.Source = null;
            imgListImage10.Source = null;
            int counter = 1;
            int x = 0;
            if (movieList.Count < listStart + 10)
            {
                x = movieList.Count;
            }
            else
            {
                x = listStart + 10;
            }
            for (int i = listStart; i < x; i++)
            {
                if (counter == 1)
                {
                    lblListLabel1.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage1.Source = image;
                    counter += 1;
                }
                else if (counter == 2)
                {
                    lblListLabel2.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage2.Source = image;
                    counter += 1;
                }
                else if (counter == 3)
                {
                    lblListLabel3.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage3.Source = image;
                    counter += 1;
                }
                else if (counter == 4)
                {
                    lblListLabel4.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage4.Source = image;
                    counter += 1;
                }
                else if (counter == 5)
                {
                    lblListLabel5.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage5.Source = image;
                    counter += 1;
                }
                else if (counter == 6)
                {
                    lblListLabel6.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage6.Source = image;
                    counter += 1;
                }
                else if (counter == 7)
                {
                    lblListLabel7.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage7.Source = image;
                    counter += 1;
                }
                else if (counter == 8)
                {
                    lblListLabel8.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage8.Source = image;
                    counter += 1;
                }
                else if (counter == 9)
                {
                    lblListLabel9.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage9.Source = image;
                    counter += 1;
                }
                else if (counter == 10)
                {
                    lblListLabel10.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage10.Source = image;
                    counter += 1;
                }
            }
            if (listStart > 0)
            {
                btnListPrev.Visibility = Visibility.Visible;
                btnListPrev.IsEnabled = true;
            }
            else
            {
                btnListPrev.Visibility = Visibility.Visible;
                btnListPrev.IsEnabled = true;
            }
            listStart += 10;
            if (movieList.ElementAtOrDefault(listStart) != null)
            {
                btnListNext.Visibility = Visibility.Visible;
                btnListNext.IsEnabled = true;
            }
            else
            {
                btnListNext.Visibility = Visibility.Hidden;
                btnListNext.IsEnabled = false;
            }
        }

        private void btnListPrev_Click(object sender, RoutedEventArgs e)
        {
            listStart -= 20;
            lblListLabel1.Content = "";
            lblListLabel2.Content = "";
            lblListLabel3.Content = "";
            lblListLabel4.Content = "";
            lblListLabel5.Content = "";
            lblListLabel6.Content = "";
            lblListLabel7.Content = "";
            lblListLabel8.Content = "";
            lblListLabel9.Content = "";
            lblListLabel10.Content = "";
            imgListImage1.Source = null;
            imgListImage2.Source = null;
            imgListImage3.Source = null;
            imgListImage4.Source = null;
            imgListImage5.Source = null;
            imgListImage6.Source = null;
            imgListImage7.Source = null;
            imgListImage8.Source = null;
            imgListImage9.Source = null;
            imgListImage10.Source = null;
            int counter = 1;
            int x = 0;
            if (movieList.Count < listStart + 10)
            {
                x = movieList.Count;
            }
            else
            {
                x = listStart + 10;
            }
            for (int i = listStart; i < x; i++)
            {
                if (counter == 1)
                {
                    lblListLabel1.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage1.Source = image;
                    counter += 1;
                }
                else if (counter == 2)
                {
                    lblListLabel2.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage2.Source = image;
                    counter += 1;
                }
                else if (counter == 3)
                {
                    lblListLabel3.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage3.Source = image;
                    counter += 1;
                }
                else if (counter == 4)
                {
                    lblListLabel4.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage4.Source = image;
                    counter += 1;
                }
                else if (counter == 5)
                {
                    lblListLabel5.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage5.Source = image;
                    counter += 1;
                }
                else if (counter == 6)
                {
                    lblListLabel6.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage6.Source = image;
                    counter += 1;
                }
                else if (counter == 7)
                {
                    lblListLabel7.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage7.Source = image;
                    counter += 1;
                }
                else if (counter == 8)
                {
                    lblListLabel8.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage8.Source = image;
                    counter += 1;
                }
                else if (counter == 9)
                {
                    lblListLabel9.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage9.Source = image;
                    counter += 1;
                }
                else if (counter == 10)
                {
                    lblListLabel10.Content = movieList[i].Title;
                    var image = LoadImage(movieList[i].FileData);
                    imgListImage10.Source = image;
                    counter += 1;
                }
            }
            if (listStart > 10)
            {
                btnListPrev.Visibility = Visibility.Visible;
                btnListPrev.IsEnabled = true;
            }
            else
            {
                btnListPrev.Visibility = Visibility.Hidden;
                btnListPrev.IsEnabled = false;
            }
            listStart += 10;
            if (movieList.ElementAtOrDefault(listStart) != null)
            {
                btnListNext.Visibility = Visibility.Visible;
                btnListNext.IsEnabled = true;
            }
            else
            {
                btnListNext.Visibility = Visibility.Hidden;
                btnListNext.IsEnabled = false;
            }
        }

        private void rctList1_MouseEnter(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 10) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0.2, TimeSpan.FromSeconds(0.1));
                rctList1.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList2_MouseEnter(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 9) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0.2, TimeSpan.FromSeconds(0.1));
                rctList2.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList3_MouseEnter(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 8) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0.2, TimeSpan.FromSeconds(0.1));
                rctList3.BeginAnimation(Rectangle.OpacityProperty, ani);
            } 
        }

        private void rctList4_MouseEnter(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart -7) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0.2, TimeSpan.FromSeconds(0.1));
                rctList4.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList5_MouseEnter(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart -6) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0.2, TimeSpan.FromSeconds(0.1));
                rctList5.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList6_MouseEnter(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 5) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0.2, TimeSpan.FromSeconds(0.1));
                rctList6.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList7_MouseEnter(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 4) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0.2, TimeSpan.FromSeconds(0.1));
                rctList7.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList8_MouseEnter(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 3) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0.2, TimeSpan.FromSeconds(0.1));
                rctList8.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList9_MouseEnter(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 2) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0.2, TimeSpan.FromSeconds(0.1));
                rctList9.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList10_MouseEnter(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 1) != null) {
                DoubleAnimation ani = new DoubleAnimation(0.2, TimeSpan.FromSeconds(0.1));
                rctList10.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList1_MouseLeave(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 10) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                rctList1.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList2_MouseLeave(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 9) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                rctList2.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList3_MouseLeave(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 8) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                rctList3.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList4_MouseLeave(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 7) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                rctList4.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList5_MouseLeave(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 6) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                rctList5.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList6_MouseLeave(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 5) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                rctList6.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList7_MouseLeave(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 4) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                rctList7.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList8_MouseLeave(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 3) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                rctList8.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList9_MouseLeave(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 2) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                rctList9.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList10_MouseLeave(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 1) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
                rctList10.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSelectedMovie = listStart - 10;
            hideListGrid();
            showMovieGrid();
        }

        private void rctList2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSelectedMovie = listStart - 9;
            hideListGrid();
            showMovieGrid();
        }

        private void rctList3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSelectedMovie = listStart - 8;
            hideListGrid();
            showMovieGrid();
        }

        private void rctList4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSelectedMovie = listStart - 7;
            hideListGrid();
            showMovieGrid();
        }

        private void rctList5_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSelectedMovie = listStart - 6;
            hideListGrid();
            showMovieGrid();
        }

        private void rctList6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSelectedMovie = listStart - 5;
            hideListGrid();
            showMovieGrid();
        }

        private void rctList7_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSelectedMovie = listStart - 4;
            hideListGrid();
            showMovieGrid();
        }

        private void rctList8_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSelectedMovie = listStart - 3;
            hideListGrid();
            showMovieGrid();
        }

        private void rctList9_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSelectedMovie = listStart - 2;
            hideListGrid();
            showMovieGrid();
        }

        private void rctList10_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSelectedMovie = listStart - 1;
            hideListGrid();
            showMovieGrid();
        }

        private void btnMovieBack_Click(object sender, RoutedEventArgs e)
        {
            hideMovieGrid();
            showListGrid();
        }

        private void btnMovieBook_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

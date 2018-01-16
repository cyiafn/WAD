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

        public static List<String> showtimeList = new List<string>();

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
                    Thread.Sleep(5000);
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
        private void hideConfirmGrid()
        {
            DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            confirmationGrid.BeginAnimation(Grid.OpacityProperty, ani);
            confirmationGrid.IsEnabled = false;
            confirmationGrid.Visibility = Visibility.Hidden;
        }
        private void showConfirmGrid()
        {
            confirmationGrid.Opacity = 0;
            confirmationGrid.IsEnabled = true;
            confirmationGrid.Visibility = Visibility.Visible;
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            confirmationGrid.BeginAnimation(Grid.OpacityProperty, ani);
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
                if (diff.Days > 0)
                {
                    movieList.Clear();
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
            imgLoading.IsEnabled = true;
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
            wbMovie.IsEnabled = true;
            wbMovie.Address = string.Format("https://www.youtube.com/embed/{0}?version=3&playlist=1&hd=1&autoplay=1&fs=0&autohide=1&loop=1&controls=0", movieList[currentSelectedMovie].VideoId);
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
                string password = sha256_hash(txtLoginPassword.Password);
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
                    txtLoginPassword.Password = "";
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
                    txtLoginPassword.Password = "";
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
            if (txtRegisterPassword.Password == txtRegisterConfirmPassword.Text)
            {
                NetworkStream stream = new NetworkStream(socket);
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                writer.AutoFlush = true;
                writer.WriteLine("register");
                writer.WriteLine(txtRegisterEmail.Text);
                writer.WriteLine(sha256_hash(txtRegisterPassword.Password));
                writer.WriteLine(txtRegisterFirstName.Text);
                writer.WriteLine(txtRegisterMiddleName.Text);
                writer.WriteLine(txtRegisterLastName.Text);
                writer.WriteLine(txtRegisterDOB.Text);
                writer.Flush();
                if (reader.ReadLine() == "success")
                {
                    currentUser.setEmail(txtRegisterEmail.Text);
                    currentUser.setPassword(sha256_hash(txtRegisterPassword.Password));
                    currentUser.setFirstName(txtRegisterFirstName.Text);
                    currentUser.setMiddleName(txtRegisterMiddleName.Text);
                    currentUser.setLastName(txtRegisterLastName.Text);
                    currentUser.setDOB(txtRegisterDOB.Text);
                    txtRegisterEmail.Text = "";
                    txtRegisterPassword.Password = "";
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
            if (movieList.ElementAtOrDefault(listStart - 7) != null)
            {
                DoubleAnimation ani = new DoubleAnimation(0.2, TimeSpan.FromSeconds(0.1));
                rctList4.BeginAnimation(Rectangle.OpacityProperty, ani);
            }
        }

        private void rctList5_MouseEnter(object sender, MouseEventArgs e)
        {
            if (movieList.ElementAtOrDefault(listStart - 6) != null)
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
            showBookingGrid();
        }

        private void wbMovie_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == false)
            {
                this.Dispatcher.Invoke(() =>
                {
                    imgLoading.IsEnabled = false;
                });

            }
        }
        private void showBookingGrid()
        {
            bookingGrid.IsEnabled = true;
            bookingGrid.Visibility = Visibility.Visible;
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            bookingGrid.BeginAnimation(Grid.OpacityProperty, ani);
            BrushConverter bc = new BrushConverter();
            btnBookingA1.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingA2.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingA3.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingA4.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingA5.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingB1.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingB2.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingB3.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingB4.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingB5.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingC1.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingC2.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingC3.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingC4.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingC5.Background = (Brush)bc.ConvertFrom("#FFFF0000");
            btnBookingA1.IsEnabled = false;
            btnBookingA2.IsEnabled = false;
            btnBookingA3.IsEnabled = false;
            btnBookingA4.IsEnabled = false;
            btnBookingA5.IsEnabled = false;
            btnBookingB1.IsEnabled = false;
            btnBookingB2.IsEnabled = false;
            btnBookingB3.IsEnabled = false;
            btnBookingB4.IsEnabled = false;
            btnBookingB5.IsEnabled = false;
            btnBookingC1.IsEnabled = false;
            btnBookingC2.IsEnabled = false;
            btnBookingC3.IsEnabled = false;
            btnBookingC4.IsEnabled = false;
            btnBookingC5.IsEnabled = false;
            btnBookingConfirm.IsEnabled = false;
            NetworkStream stream = new NetworkStream(socket);
            StreamWriter writer = new StreamWriter(stream);
            StreamReader read = new StreamReader(stream);
            writer.AutoFlush = true;
            writer.WriteLine("request_showtime");
            writer.WriteLine(movieList[currentSelectedMovie].Title);
            lblBookingTicketPrice.Content = "$ " + String.Format("{0:.00}", movieList[currentSelectedMovie].Price);
            lblBookingTotal.Content = "$ 0.00";
            string xml = "";
            string line;
            //string randomVar = read.ReadLine();
            var xs = new XmlSerializer(typeof(List<String>));
            while ((line = read.ReadLine()) != "endofxml")
            {
                xml += line;
            }
            using (var reader = new StringReader(xml))
            {
                showtimeList = (List<String>)xs.Deserialize(reader);
            }
            lblBookingTitle.Content = movieList[currentSelectedMovie].Title;
            for (int i = 0; i < showtimeList.Count; i++)
            {
                string[] tempList = showtimeList[i].Split(';');
                ddlBookingDate.Items.Add(tempList[1] + " on " + tempList[0]);
            }
        }
        private void hideBookingGrid()
        {
            DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            bookingGrid.BeginAnimation(Grid.OpacityProperty, ani);
            bookingGrid.IsEnabled = false;
            bookingGrid.Visibility = Visibility.Hidden;
            lblBookingSeats.Text = "";
            lblBookingTotalSeats.Content = "0";
            showtimeList.Clear();
        }

        private void btnBookingBack_Click(object sender, RoutedEventArgs e)
        {
            hideBookingGrid();
            showMovieGrid();
        }

        private void ddlBookingDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnBookingA1.IsEnabled = true;
            btnBookingA2.IsEnabled = true;
            btnBookingA3.IsEnabled = true;
            btnBookingA4.IsEnabled = true;
            btnBookingA5.IsEnabled = true;
            btnBookingB1.IsEnabled = true;
            btnBookingB2.IsEnabled = true;
            btnBookingB3.IsEnabled = true;
            btnBookingB4.IsEnabled = true;
            btnBookingB5.IsEnabled = true;
            btnBookingC1.IsEnabled = true;
            btnBookingC2.IsEnabled = true;
            btnBookingC3.IsEnabled = true;
            btnBookingC4.IsEnabled = true;
            btnBookingC5.IsEnabled = true;
            btnBookingConfirm.IsEnabled = true;
            BrushConverter bc = new BrushConverter();
            string tempVal = ddlBookingDate.SelectedValue.ToString().Replace(" on ", ";");
            String[] tempArray = tempVal.Split(';');

            for (int i = 0; i < showtimeList.Count; i++)
            {
                if (showtimeList[i].StartsWith(tempArray[1] + ";" + tempArray[0]))
                {
                    if (showtimeList[i].Contains("C1"))
                    {
                        btnBookingC1.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("C2"))
                    {
                        btnBookingC2.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("C3"))
                    {
                        btnBookingC3.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("C4"))
                    {
                        btnBookingC4.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("C5"))
                    {
                        btnBookingC5.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("B1"))
                    {
                        btnBookingB1.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("B2"))
                    {
                        btnBookingB2.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("B3"))
                    {
                        btnBookingB3.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("B4"))
                    {
                        btnBookingB4.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("B5"))
                    {
                        btnBookingB5.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("A1"))
                    {
                        btnBookingA1.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("A2"))
                    {
                        btnBookingA2.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("A3"))
                    {
                        btnBookingA3.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("A4"))
                    {
                        btnBookingA4.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                    if (showtimeList[i].Contains("A5"))
                    {
                        btnBookingA5.Background = (Brush)bc.ConvertFrom("#FF033A00");
                    }
                }
            }
        }

        private void btnBookingC1_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingC1.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingC1.Background.ToString() == "#FF033A00")
            {
                btnBookingC1.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "C1";
                }
                else
                {
                    lblBookingSeats.Text += ", C1";
                }
            }
            else
            {
                btnBookingC1.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "C1")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", C1", "");
                }
            }
        }

        private void btnBookingC2_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingC2.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingC2.Background.ToString() == "#FF033A00")
            {
                btnBookingC2.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "C2";
                }
                else
                {
                    lblBookingSeats.Text += ", C2";
                }
            }
            else
            {
                btnBookingC2.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "C2")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", C2", "");
                }
            }
        }

        private void btnBookingC3_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingC3.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingC3.Background.ToString() == "#FF033A00")
            {
                btnBookingC3.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "C3";
                }
                else
                {
                    lblBookingSeats.Text += ", C3";
                }
            }
            else
            {
                btnBookingC3.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "C3")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", C3", "");
                }
            }
        }

        private void btnBookingC4_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingC4.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingC4.Background.ToString() == "#FF033A00")
            {
                btnBookingC4.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "C4";
                }
                else
                {
                    lblBookingSeats.Text += ", C4";
                }
            }
            else
            {
                btnBookingC4.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "C4")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", C4", "");
                }
            }
        }

        private void btnBookingC5_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingC5.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingC5.Background.ToString() == "#FF033A00")
            {
                btnBookingC5.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "C5";
                }
                else
                {
                    lblBookingSeats.Text += ", C5";
                }
            }
            else
            {
                btnBookingC5.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "C5")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", C5", "");
                }
            }
        }

        private void btnBookingB1_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingB1.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingB1.Background.ToString() == "#FF033A00")
            {
                btnBookingB1.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "B1";
                }
                else
                {
                    lblBookingSeats.Text += ", B1";
                }
            }
            else
            {
                btnBookingB1.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "B1")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", B1", "");
                }
            }
        }

        private void btnBookingB2_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingB2.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingB2.Background.ToString() == "#FF033A00")
            {
                btnBookingB2.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "B2";
                }
                else
                {
                    lblBookingSeats.Text += ", B2";
                }
            }
            else
            {
                btnBookingB2.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "B2")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", B2", "");
                }
            }
        }

        private void btnBookingB3_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingB3.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingB3.Background.ToString() == "#FF033A00")
            {
                btnBookingB3.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "B3";
                }
                else
                {
                    lblBookingSeats.Text += ", B3";
                }
            }
            else
            {
                btnBookingB3.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "B3")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", B3", "");
                }
            }
        }

        private void btnBookingB4_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingB4.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingB4.Background.ToString() == "#FF033A00")
            {
                btnBookingB4.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "B4";
                }
                else
                {
                    lblBookingSeats.Text += ", B4";
                }
            }
            else
            {
                btnBookingB4.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "B4")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", B4", "");
                }
            }
        }

        private void btnBookingB5_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingB5.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingB5.Background.ToString() == "#FF033A00")
            {
                btnBookingB5.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "B5";
                }
                else
                {
                    lblBookingSeats.Text += ", B5";
                }
            }
            else
            {
                btnBookingB5.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "B5")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", B5", "");
                }
            }
        }

        private void btnBookingA1_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingA1.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingA1.Background.ToString() == "#FF033A00")
            {
                btnBookingA1.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "A1";
                }
                else
                {
                    lblBookingSeats.Text += ", A1";
                }
            }
            else
            {
                btnBookingA1.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "A1")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", A1", "");
                }
            }
        }

        private void btnBookingA2_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingA2.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingA2.Background.ToString() == "#FF033A00")
            {
                btnBookingA2.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "A2";
                }
                else
                {
                    lblBookingSeats.Text += ", A2";
                }
            }
            else
            {
                btnBookingA2.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "A2")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", A2", "");
                }
            }
        }

        private void btnBookingA3_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingA3.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingA3.Background.ToString() == "#FF033A00")
            {
                btnBookingA3.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "A3";
                }
                else
                {
                    lblBookingSeats.Text += ", A3";
                }
            }
            else
            {
                btnBookingA3.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "A3")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", A3", "");
                }
            }
        }

        private void btnBookingA4_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingA4.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingA4.Background.ToString() == "#FF033A00")
            {
                btnBookingA4.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "A4";
                }
                else
                {
                    lblBookingSeats.Text += ", A4";
                }
            }
            else
            {
                btnBookingA4.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "A4")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", A4", "");
                }
            }
        }

        private void btnBookingA5_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            if (btnBookingA5.Background.ToString() == "#FFFF0000")
            {
                System.Windows.MessageBox.Show("This seat is already taken!");
            }
            else if (btnBookingA5.Background.ToString() == "#FF033A00")
            {
                btnBookingA5.Background = (Brush)bc.ConvertFrom("#FF004385");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats += 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "")
                {
                    lblBookingSeats.Text += "A5";
                }
                else
                {
                    lblBookingSeats.Text += ", A5";
                }
            }
            else
            {
                btnBookingA5.Background = (Brush)bc.ConvertFrom("#FF033A00");
                int seats = Convert.ToInt32(lblBookingTotalSeats.Content);
                seats -= 1;
                lblBookingTotalSeats.Content = seats.ToString();
                double total = Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", ""));
                double price = Convert.ToDouble(lblBookingTicketPrice.Content.ToString().Replace("$ ", ""));
                total = seats * price;
                lblBookingTotal.Content = "$ " + String.Format("{0:.00}", total);

                if (lblBookingSeats.Text.ToString() == "A5")
                {
                    lblBookingSeats.Text = "";
                }
                else
                {
                    lblBookingSeats.Text = lblBookingSeats.Text.ToString().Replace(", A5", "");
                }
            }
        }

        private void btnBookingConfirm_Click(object sender, RoutedEventArgs e)
        {
            NetworkStream stream = new NetworkStream(socket);
            StreamWriter writer = new StreamWriter(stream);
            StreamReader read = new StreamReader(stream);
            writer.AutoFlush = true;
            writer.WriteLine("book_movie");
            writer.WriteLine(Guid.NewGuid().ToString());
            writer.WriteLine(movieList[currentSelectedMovie].Title);
            writer.WriteLine(currentUser.getEmail());
            writer.WriteLine(Convert.ToDouble(lblBookingTotal.Content.ToString().Replace("$ ", "")));
            string dt = ddlBookingDate.SelectedValue.ToString().Replace(" on ", ";");
            String[] datetime = dt.Split(';');
            writer.WriteLine(datetime[1]);
            writer.WriteLine(datetime[0]);
            writer.WriteLine(lblBookingSeats.Text.ToString().Replace(". ", "|"));
            string status = read.ReadLine();
            if (status == "success")
            {
                txtConfirmSeats.Text = "Booked Seats: " + lblBookingSeats.Text;
                lblConfirmTime.Text = ddlBookingDate.SelectedValue.ToString();
                hideBookingGrid();
                showConfirmGrid();
            }
            else
            {
                System.Windows.MessageBox.Show("Error! Please try again later!");
            }
            
        }

        private void btnConfirmRecept_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            save.FileName = "BookingReceipt.txt";

            save.Filter = "Text File | *.txt";

            if (save.ShowDialog() == true)

            {

                using (StreamWriter writer = new StreamWriter(save.FileName))
                {
                    writer.WriteLine("Ticketter");
                    writer.WriteLine("================================================");
                    writer.WriteLine("Booking for: " + currentUser.getFirstName() + " " + currentUser.getMiddleName() + " " + currentUser.getLastName());
                    writer.WriteLine("================================================");
                    writer.WriteLine("Seats booked: " + txtConfirmSeats.Text.ToString().Replace("Booked Seats: ", ""));
                    writer.WriteLine("Date and Time of Movie: " + lblConfirmTime.Text);
                    writer.WriteLine("");
                    writer.WriteLine("");
                    writer.WriteLine("");
                    writer.WriteLine("");
                    writer.WriteLine("");
                    writer.WriteLine("");
                    writer.WriteLine("");
                    writer.WriteLine("Total paid: " + String.Format("{0:.00}", lblBookingTotal.Content.ToString()));

                    writer.Flush();

                    writer.Close();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using Un4seen.Bass;
using System.Data.SqlClient;
using System.Timers;
using System.ComponentModel;
using Un4seen.Bass.Misc;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace Wpf_Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int playState, songLenght, selectedSong,songNumber;
        private bool looping, shuffling, dbClient;
        DatabaseManager db;
        DataTable dt;
        private System.Drawing.Bitmap spectrum;
        private Visuals _vis = new Visuals();
        private int specIdx = 0;
        private DataRowView drv;
        private BitmapSource im;
        private Player player = new Player();
        private System.Timers.Timer progressTimer = new System.Timers.Timer();
        private System.Timers.Timer spectrumTimer = new System.Timers.Timer();
        private int currentPos = 0,sliderProgressMax=0;
        private string currentPosition = String.Empty;
        private BitmapImage coverIm=new BitmapImage();
        public BitmapImage CoverImage
        {
            set
            {
                coverIm = value;
                OnPropertyChanged("CoverImage");
            }
            get { return coverIm; }
        }
        public string CurrentPosition
        {
            get { return currentPosition; }
            set
            {
                currentPosition = value;
                OnPropertyChanged("CurrentPosition");
            }
        }
        
        public int SliderProgressMax
        {
            get { return sliderProgressMax; }
            set
            {
                sliderProgressMax = value;
                OnPropertyChanged("SliderProgressMax");
            }
        }
        private int spectrumImageWidth = 0;
        private int spectrumImageHeight = 0;
        public double SpectrumImageHeight
        {
            get { return (double)spectrumImageHeight; }
            set
            {
                spectrumImageHeight = (int)value;
                OnPropertyChanged("SpectrumImageHeight");
            }
        }
        public double SpectrumImageWidth
        {
            get { return (double)spectrumImageWidth; }
            set
            {
                spectrumImageWidth = (int)value;
                OnPropertyChanged("SpectrumImageWidht");


            }
        }
        public int CurrentPos
        {
            get { return currentPos; }
            set { 
                currentPos = value;
                OnPropertyChanged("CurrentPos");
            }
        }
        private int selectedIndex = 0;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }
        private string pass, login;
        public MainWindow()
        {
            InitializeComponent();
            pass = "test";
            login = "test";
            
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }


        private void listview1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            player.StopSong();
            int index = listview1.SelectedIndex;
            drv = (DataRowView)listview1.Items[index];
            //5 kolumna to jest nazwa pliku drv.Row[5].ToString()
            player.LoadSong(drv.Row[5].ToString());
            Song song = new Song(drv.Row[5].ToString());
            CoverImage = song.CoverImage;
            
            int songLenght=0;
            Int32.TryParse(drv.Row[6].ToString(),out songLenght);
           
            slProgress.Minimum = 0;
            slProgress.Maximum = songLenght;
            sliderProgressMax = songLenght;
            player.PlaySong();
            player.Playing = true;
            spectrumTimer.Enabled = true;
            progressTimer.Enabled = true;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            db = new DatabaseManager("192.168.1.20", "musicclient",login,pass);

          

            listview1.DataContext = db.getQueryFromDB("select * from music");
            progressTimer.Elapsed += new ElapsedEventHandler(OnProgressEvent);
            progressTimer.Interval = 1000;
            spectrumTimer.Elapsed += new ElapsedEventHandler(SpectrumEvent);
            spectrumTimer.Interval = 100;
            slVolume.Maximum = 100;
            slVolume.Minimum = 0;
            shuffling = false;
            looping = false;
            slVolume.Value = 100;
            dbClient = true;
            spectrumImageWidth = 450;
            spectrumImageHeight = 70;
        }

       
        private void OnProgressEvent(object source, ElapsedEventArgs e)
        {

            CurrentPos = player.CurrentPossition();
            CurrentPosition = GetTimeMinutsAndSeconds(currentPos) + " / " + GetTimeMinutsAndSeconds(sliderProgressMax);
            if (currentPos > ((int)drv[6]-2))
            {
                song_change("next");

            }
       
            
        }
        private string GetTimeMinutsAndSeconds(int seconds)
        {
            int minut = 0;
            minut = seconds / 60;

            return String.Format("{0:00}", (float)minut) + ":" + String.Format("{0:00}", (float)(seconds % 60));
        }
        private void SpectrumEvent(object source, ElapsedEventArgs e)
        {
            if (player.Playing)
            {
      
                Dispatcher.Invoke(new Action(DrawSpectrum));
            }
        }
        private void DrawSpectrum()
        {
               // Width="{Binding ElementName=mywindow, Path=SpectrumImageWidth,Mode=OneWay,UpdateSourceTrigger=PropertyChanged}" 
              // Height="{Binding ElementName=mywindow, Path=SpectrumImageHeight,Mode=OneWay,UpdateSourceTrigger=PropertyChanged}" 
          
           // spectrum = _vis.CreateSpectrumWave(_stream, width, height, System.Drawing.Color.Yellow, System.Drawing.Color.Orange, System.Drawing.Color.Black, 1, false, false, false);

            int width = 450;
            int height = 70;
            switch (specIdx)
            {
                // normal spectrum (width = resolution)
                case 0:
                    spectrum = _vis.CreateSpectrum(player.Stream, width, height, System.Drawing.Color.Lime, System.Drawing.Color.Red, System.Drawing.Color.Black, false, false, false);
                    break;
                // normal spectrum (full resolution)
                case 1:
                    spectrum = _vis.CreateSpectrum(player.Stream, width, height, System.Drawing.Color.SteelBlue, System.Drawing.Color.Pink, System.Drawing.Color.Black, false, true, true);
                    break;
                // line spectrum (width = resolution)
                case 2:
                    spectrum = _vis.CreateSpectrumLine(player.Stream, width, height, System.Drawing.Color.Lime, System.Drawing.Color.Red, System.Drawing.Color.Black, 2, 2, false, false, false);
                    break;
                // line spectrum (full resolution)
                case 3:
                    spectrum = _vis.CreateSpectrumLine(player.Stream, width, height, System.Drawing.Color.SteelBlue, System.Drawing.Color.Pink, System.Drawing.Color.Black, 16, 4, false, true, true);
                    break;
                // ellipse spectrum (width = resolution)
                case 4:
                    spectrum = _vis.CreateSpectrumEllipse(player.Stream, width, height, System.Drawing.Color.Lime, System.Drawing.Color.Red, System.Drawing.Color.Black, 1, 2, false, false, false);
                    break;
                // ellipse spectrum (full resolution)
                case 5:
                    spectrum = _vis.CreateSpectrumEllipse(player.Stream, width, height, System.Drawing.Color.SteelBlue, System.Drawing.Color.Pink, System.Drawing.Color.Black, 2, 4, false, true, true);
                    break;
                // dot spectrum (width = resolution)
                case 6:
                    spectrum = _vis.CreateSpectrumDot(player.Stream, width, height, System.Drawing.Color.Lime, System.Drawing.Color.Red, System.Drawing.Color.Black, 1, 0, false, false, false);
                    break;
                // dot spectrum (full resolution)
                case 7:
                    spectrum = _vis.CreateSpectrumDot(player.Stream, width, height, System.Drawing.Color.SteelBlue, System.Drawing.Color.Pink, System.Drawing.Color.Black, 2, 1, false, false, true);
                    break;
                // peak spectrum (width = resolution)
                case 8:
                    spectrum = _vis.CreateSpectrumLinePeak(player.Stream, width, height, System.Drawing.Color.SeaGreen, System.Drawing.Color.LightGreen, System.Drawing.Color.Orange, System.Drawing.Color.Black, 2, 1, 2, 10, false, false, false);
                    break;
                // peak spectrum (full resolution)
                case 9:
                    spectrum = _vis.CreateSpectrumLinePeak(player.Stream, width, height, System.Drawing.Color.GreenYellow, System.Drawing.Color.RoyalBlue, System.Drawing.Color.DarkOrange, System.Drawing.Color.Black, 23, 5, 3, 5, false, true, true);
                    break;
                // wave spectrum (width = resolution)
                case 10:
                    spectrum = _vis.CreateSpectrumWave(player.Stream, width, height, System.Drawing.Color.Yellow, System.Drawing.Color.Orange, System.Drawing.Color.Black, 1, false, false, false);
                    break;
                // dancing beans spectrum (width = resolution)
                case 11:
                    spectrum = _vis.CreateSpectrumBean(player.Stream, width, height, System.Drawing.Color.Chocolate, System.Drawing.Color.DarkGoldenrod, System.Drawing.Color.Black, 4, false, false, true);
                    break;
                // dancing text spectrum (width = resolution)
                case 12:
                    spectrum = _vis.CreateSpectrumText(player.Stream, width, height, System.Drawing.Color.White, System.Drawing.Color.Tomato, System.Drawing.Color.Black, "BASS .NET IS GREAT PIECE! UN4SEEN ROCKS...", false, false, true);
                    break;

                case 13:
                    spectrum = _vis.CreateWaveForm(player.Stream, width, height, System.Drawing.Color.Green, System.Drawing.Color.Red, System.Drawing.Color.Gray, System.Drawing.Color.Black, 1, true, false, true);
                    break;
            }
            if (spectrum == null)
            {
                System.Drawing.Graphics g;
                System.Drawing.Bitmap b = new System.Drawing.Bitmap(200, 200);
                g = System.Drawing.Graphics.FromImage(b);
                g.Clear(System.Drawing.Color.AliceBlue);
                spectrum = b;
            }
            im = GetBitmapSource(spectrum);
            spectrumImage.Source = im; 
            //SpectrumImage = im;
           
           
        }
        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);
        private BitmapSource spectrumSource;
        public BitmapSource SpectrumImage
        {
            get { return spectrumSource; }
            set
            {
                spectrumSource = value;
                OnPropertyChanged("SpectrumImage");
            }
        }
        private System.Windows.Media.Imaging.BitmapSource GetBitmapSource(System.Drawing.Bitmap _image)
        {
            System.Drawing.Bitmap bitmap = _image;
            IntPtr hBitmap = bitmap.GetHbitmap();
            System.Windows.Media.Imaging.BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            DeleteObject(hBitmap);
            return bitmapSource;
        }
        
        private void song_change(string key)
        {
            
            if (looping)
            {
                player.PlaySong();
            }
            else if (shuffling)
            {
               
                Random rand = new Random();
                int i = rand.Next(0, listview1.Items.Count);
                spectrumTimer.Enabled = false;
                player.StopSong();
                drv = (DataRowView)listview1.Items[i];
                //5 kolumna to jest nazwa pliku drv.Row[5].ToString()
                player.LoadSong(drv.Row[5].ToString());
               
                CurrentPos=0; //wlasciwosc zbindowana z wlasciwoscia Value Slidera postepu
                songLenght = (int)drv[6];
                //slProgress.Maximum=songLenght; //max slidera ustawiony na dlugosc piosenki 
                SliderProgressMax = songLenght;
                updateEqualizer();
                player.PlaySong();
                player.Playing = true;
                progressTimer.Enabled = true;
                spectrumTimer.Enabled = true;


                selectedSong = i;
                SelectedIndex=i;
                
            }
            else if ((listview1.Items.Count >= 2 && (selectedSong + 1) < listview1.Items.Count) && key == "next")
            {

                player.StopSong();
                drv = (DataRowView)listview1.Items[selectedSong+1];
                player.LoadSong(drv.Row[5].ToString());
                CurrentPos=0; //wlasciwosc zbindowana z wlasciwoscia Value Slidera postepu
                songLenght = (int)drv[6];
                //slProgress.Maximum=songLenght; //max slidera ustawiony na dlugosc piosenki 
                SliderProgressMax = songLenght;
                progressTimer.Enabled = true;
                spectrumTimer.Enabled = true;
                updateEqualizer();
                player.PlaySong();
                player.Playing = true; 
                selectedSong++;
                SelectedIndex=selectedSong;

            }

            else if ((listview1.Items.Count >= 2 && (selectedSong - 1) >= 0) && key == "prev")
            {
                player.StopSong();
                drv = (DataRowView)listview1.Items[selectedSong-1];
                player.LoadSong(drv.Row[5].ToString());
                CurrentPos=0; //wlasciwosc zbindowana z wlasciwoscia Value Slidera postepu
                songLenght = (int)drv[6];
                //slProgress.Maximum=songLenght; //max slidera ustawiony na dlugosc piosenki 
                SliderProgressMax = songLenght;
                progressTimer.Enabled = true;
                spectrumTimer.Enabled = true;
                updateEqualizer();
                player.PlaySong();
                player.Playing = true; 
                selectedSong--;
                SelectedIndex = selectedSong;
            }
            else if (listview1.SelectedIndex==(listview1.Items.Count-1) && key=="next")

            {    // nastepna piosenka to jest 1
                player.StopSong();
                drv = (DataRowView)listview1.Items[0];
                player.LoadSong(drv.Row[5].ToString());
                CurrentPos=0; //wlasciwosc zbindowana z wlasciwoscia Value Slidera postepu
                songLenght = (int)drv[6];
                //slProgress.Maximum=songLenght; //max slidera ustawiony na dlugosc piosenki 
                SliderProgressMax = songLenght;
                progressTimer.Enabled = true;
                spectrumTimer.Enabled = true;
                updateEqualizer();
                player.PlaySong();
                player.Playing = true; 
                selectedSong=0;
                SelectedIndex = selectedSong;


            }
            else if (listview1.SelectedIndex==0 && key=="prev")
            {
                //poprzednia bedzie ostatnia na liscie
                player.StopSong();
                drv = (DataRowView)listview1.Items[listview1.Items.Count-1];
                player.LoadSong(drv.Row[5].ToString());
                CurrentPos=0; //wlasciwosc zbindowana z wlasciwoscia Value Slidera postepu
                songLenght = (int)drv[6];
                //slProgress.Maximum=songLenght; //max slidera ustawiony na dlugosc piosenki 
                SliderProgressMax = songLenght;
                progressTimer.Enabled = true;
                spectrumTimer.Enabled = true;
                updateEqualizer();
                player.PlaySong();
                player.Playing = true; 
                selectedSong=listview1.Items.Count-1;
                SelectedIndex = selectedSong;



            }
         
            // zapisywac glosnosc

        }
        

      
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
        private void SetVolume(int volume)
        {
            // Calculate the volume that's being set
            int NewVolume = ((ushort.MaxValue / 100) * volume);
            // Set the same volume for both the left and the right channels
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            // Set the volume
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);

        }
       
        private void slProgress_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            player.SeekSong(CurrentPos);
        }

        private void spectrumImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                specIdx++;
            else
                specIdx--;

            if (specIdx > 13)
                specIdx = 0;
            if (specIdx < 0)
                specIdx = 13;

            spectrum = null;
            _vis.ClearPeaks();
        }


        private void slVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVolume((int)slVolume.Value);
        }

        private void listview1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSong = listview1.SelectedIndex;
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            player.StopSong();
            progressTimer.Enabled = false;
            progressTimer.Stop();
            spectrumTimer.Enabled = false;
            spectrumTimer.Stop();
        }

        private void btn_play_Click(object sender, RoutedEventArgs e)
        {
            if(listview1.Items[selectedSong]!=null)
            {
                if (player.Paused)
                {
                    player.PlaySong();
                    player.Playing = true;
                    player.Paused = false;

                }
                else
                {
                    player.StopSong();
                    drv = (DataRowView)listview1.Items[selectedSong];
                    player.LoadSong(drv.Row[5].ToString());
                    CurrentPos = 0;
                    SliderProgressMax = (int)drv[6];
                    updateEqualizer();
                    player.PlaySong();
                    player.Playing = true;
                }

                progressTimer.Enabled = true;
                spectrumTimer.Enabled = true;

            }
            spectrumTimer.Start();
        }

        private void btn_pause_Click(object sender, RoutedEventArgs e)
        {
            if (player.Paused)
            {
                updateEqualizer();
                player.PlaySong();
                player.Playing = true;
                player.Paused = false;
                progressTimer.Enabled = true;
                spectrumTimer.Enabled = true;
            }
            else
            {
                player.PauseSong();
                playState = player.CurrentPossition();
                player.Paused = true;
                progressTimer.Enabled = false;
                spectrumTimer.Enabled = false;
            }
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            song_change("next");
        }

        private void btn_prev_Click(object sender, RoutedEventArgs e)
        {
            song_change("prev");
        }


       

    

        private void server_Click(object sender, RoutedEventArgs e)
        {
            if (!dbClient)
            {
                try
                {
                    db = new DatabaseManager("192.168.1.20", "musicclient",login,pass);
                    listview1.DataContext = db.getQueryFromDB("select * from music");
                    dbClient = true;
                }
                catch (SqlException ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                db = new DatabaseManager("192.168.1.20", "musicserver",login,pass);
                listview1.DataContext = db.getQueryFromDB("select * from music");
                dbClient = false;
            }
        }
        
        private void updateEqualizer()
        {
            Dispatcher.Invoke(new Action(delegate()
            {
                foreach (object c in LogicalTreeHelper.GetChildren(gridequalizer))
                {

                    if (c is Slider && ((Slider)c).Tag.ToString() != "echochorus")
                    {
                        int index = int.Parse(((Slider)c).Tag.ToString());
                        player.UpdateEQ(index, (float)((Slider)c).Value / 10f);
                    }
                }
            }));
        }


        #region ControlEvents

        private void sl50EQ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.UpdateEQ(0, ((float)sl50EQ.Value / 10f));
        }

        private void sl100EQ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.UpdateEQ(1, (float)sl100EQ.Value / 10f);
        }

        private void sl200EQ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.UpdateEQ(2, (float)sl200EQ.Value / 10f);
        }

        private void sl400EQ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.UpdateEQ(3, (float)sl400EQ.Value / 10f);
        }

        private void sl700EQ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.UpdateEQ(4, (float)sl700EQ.Value / 10f);
        }

        private void sl1EQ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.UpdateEQ(5, (float)sl1EQ.Value / 10f);
        }

        private void sl2EQ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.UpdateEQ(6, (float)sl2EQ.Value / 10f);
        }

        private void sl4EQ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.UpdateEQ(7, (float)sl4EQ.Value / 10f);
        }

        private void sl6EQ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.UpdateEQ(8, (float)sl6EQ.Value / 10f);
        }

        private void sl8EQ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.UpdateEQ(9, (float)sl8EQ.Value / 10f);
        }

        private void slEcho_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.SetEcho((float)slEcho.Value);
        }

        private void slChorus_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.SetChorus((float)slChorus.Value);
        }
        private bool eqVisible = true;
        private GridLength Row5Hight;
        private void equalizer_Click(object sender, RoutedEventArgs e)
        {
            if (eqVisible)
            {
                groupBox2.Visibility = Visibility.Hidden;
                Row5Hight = MainGrid.RowDefinitions[5].Height;
                MainGrid.RowDefinitions[5].Height = new GridLength(0);
                eqVisible = false;
            }
            else
            {
                groupBox2.Visibility = Visibility.Visible;
                MainGrid.RowDefinitions[5].Height = Row5Hight;
                eqVisible = true;
            }
        }


        private void checkBox3_Checked(object sender, RoutedEventArgs e)
        {
            SetVolume(0);
        }

        private void checkBox3_Unchecked(object sender, RoutedEventArgs e)
        {
            SetVolume((int)slVolume.Value);
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            if (!shuffling)
            {
                shuffling = true;
            }
        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            if (shuffling)
            {
                shuffling = false;
            }
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            if (!looping)
            {
                looping = true;
            }
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            if (looping)
            {
                looping = false;
            }
        }
        private void btn_open_Click(object sender, RoutedEventArgs e)
        {

            openfile_Click(sender, e);

        }

        private void btn_openf_Click(object sender, RoutedEventArgs e)
        {
            opendirectory_Click(sender, e);


        }
        private void openfile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "mp3 files|*.mp3";

            if ("OK" == fd.ShowDialog().ToString())
            {
                Song song = new Song(fd.FileName);
                songLenght = song.Duration;
                song.Number = songNumber;

                string request = String.Format("insert into music (Artist,Title,Genre,Year,FileName,Duration) values ('{0}','{1}','{2}',{3},'{4}',{5});", song.Artist, song.Title,song.Genre,song.Year, song.FileName, song.Duration);

                db.insertDataIntoDB(request);
                request = "select * from music;";
                db = new DatabaseManager("192.168.1.20", "musicclient", login, pass);



                dt = new DataTable();
                dt = db.getQueryFromDB("select * from music");
                listview1.DataContext = dt.DefaultView;
            }
        }

        private void opendirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if ("OK" == fbd.ShowDialog().ToString())
            {
                System.Collections.Specialized.StringCollection fileCol = new System.Collections.Specialized.StringCollection();
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(fbd.SelectedPath);

                fileCol.Add(fbd.SelectedPath);
                FileInfo[] files = dir.GetFiles("*.mp3");
                Song song;
                string request = "";
                db = new DatabaseManager("192.168.1.20", "musicclient", login, pass);
                foreach (FileInfo fi in files)
                {
                    if (fi.Extension == ".mp3")
                    {
                        song = new Song(fi.FullName);
                        song.Number = songNumber;

                        if (song.Artist == null || song.Title == null)
                        {
                        }
                        else
                        {
                           // request = "insert into music (Artist,Title,FileName,Duration) values ('" + song.Artist.Replace("'", "''") + "','" + song.Title.Replace("'", "''") + "','" + song.FileName.Replace("'", "''") + "'," + song.Duration + ")";
                            request = String.Format("insert into music (Artist,Title,Genre,Year,FileName,Duration) values ('{0}','{1}','{2}',{3},'{4}',{5});", song.Artist.Replace("'", "''"), song.Title.Replace("'", "''"), song.Genre.Replace("'", "''"), song.Year, song.FileName.Replace("'", "''"), song.Duration);
                            db.insertDataIntoDB(request);
                        }

                    }

                }
                request = "select * from music;";
                listview1.DataContext = db.getQueryFromDB(request);
            }
        }
        

   

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (dbClient)
            {
               
                drv = (DataRowView)listview1.Items[selectedSong];
                string requestedFile = (string)drv[0] + "-" + (string)drv[1];
                string filePath = getFileFromServer(requestedFile);
                Song song = new Song(filePath);
                songLenght = song.Duration;
                string request = String.Format("insert into music (Artist,Title,FileName,Duration) values ('{0}','{1}','{2}',{3});", song.Artist.Replace("'", "''"), song.Title.Replace("'", "''"), song.FileName.Replace("'", "''"), song.Duration);
                db = new DatabaseManager("192.168.1.20", "musicclient", login, pass);
                db.insertDataIntoDB(request);
               
                listview1.DataContext = db.getQueryFromDB("select * from music");
                
            }
        }
        #endregion
        private string getFileFromServer(string file)
        {

            Network net = new Network();
            net.RequestedFile = file;
            //net.RequestedFile = "Andy Moor & Ashley Wallbridge feat. Gabriela-World To Turn";
            if(!Directory.Exists("C:\\mp3"))
            {
                Directory.CreateDirectory(@"C:\mp3");
            }
            net.FolderPath = @"C:\mp3\";
            Thread client1 = new Thread(new ThreadStart(net.SendMessage));
            client1.Start();
            client1.Join();

            Thread.Sleep(5000);
            Thread client = new Thread(new ThreadStart(net.FetchFileFromServer));
            client.Start();
            client.Join();


            return net.RequestedFilePath;
        }

        private void rysuj_Click(object sender, RoutedEventArgs e)
        {
            
            ChartWindow chartWindow = new ChartWindow();
           
            chartWindow.Show();
        }
    








    }   
}

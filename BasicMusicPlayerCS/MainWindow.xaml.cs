using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
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

namespace BasicMusicPlayerCS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        //lets start with the classic folder
        public string searchPath = "\\";
        //lets do a folder for files to not clout the current folder of audio files
        public string musicFolder = "./music/"; //lets make it compatible with web servers like html5
        //lets just do it the classic way
        public string musicListFile = "musicList.txt";
        public List<string> musicArray = new List<string>();
        public WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
        public int curIdx = 0;
        public bool repeatMusic = false;
        public bool doneSearching = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region events
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //setupConsole(Properties.Settings.Default.showLogConsole); //why tho
            setupConsole(true); //why tho
            setupFiles();
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            setPlayerState("check");
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            setPlayerState("prev");
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            setPlayerState("next");
        }

        private void repeatButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void availableSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //automatically plays on open
            var oldIdx = curIdx;
            curIdx = availableSongs.SelectedIndex;
            if (Properties.Settings.Default.playOnOpen == true)
            {
                setPlayerState("play");
            }
            else
            {
                if(curIdx != oldIdx)
                {
                    setPlayerState("play");
                }
            }
        }
        #endregion

        #region functions
        public dynamic funnyRead(string filepath, bool turnintoarray)
        {
            dynamic daFileContent = "";
            try
            {
                if (turnintoarray == true)
                {
                    daFileContent = System.IO.File.ReadAllText(filepath).Trim().Split('\n');
                }
                else
                {
                    daFileContent = System.IO.File.ReadAllText(filepath);
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
            return daFileContent;
        }

        public void setupFiles()
        {
            Console.WriteLine("stopping player");
            setPlayerState("stop");
            Console.WriteLine("cleaning stuff just i ncase");
            cleanLists();
            Console.WriteLine("setting up");
            doneSearching = false;
            string daPath = "";
            string[] details = { };
            if (searchPath == "\\")
            {
                daPath = System.IO.Directory.GetCurrentDirectory() + searchPath;
                musicFolder = System.IO.Directory.GetCurrentDirectory() + "\\music\\";
            }
            else
            {
                daPath = searchPath;
            }
            string daMusicListDir = daPath + musicListFile;

            if (doneSearching == false)
            {
                if (System.IO.File.Exists(daMusicListDir))
                {
                    try
                    {
                        details = funnyRead(daMusicListDir, true);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    foreach (var i in details)
                    {
                        var advancedDetails = i.Split("|");
                        if (System.IO.Directory.Exists(musicFolder))
                        {
                            //music folder shoudl come formatted already
                            var daMusicFileDir = musicFolder + advancedDetails[0];
                            if (System.IO.File.Exists(daMusicFileDir))
                            {
                                musicArray.Add(daMusicFileDir);
                                availableSongs.Items.Add(advancedDetails[1]);
                                postSetupFiles();
                            }
                        }
                    }
                }
            }
        }

        public void setPlayerState(string state)
        {
            switch (state)
            {
                case "play":
                    if(doneSearching == true)
                    {
                        player.URL = musicArray[curIdx];
                        player.controls.play();
                        playButton.Content = "Pause";
                    }
                    break;
                case "check":
                    if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                    {
                        player.controls.pause();
                        playButton.Content = "Resume";
                    }
                    else if (player.playState == WMPLib.WMPPlayState.wmppsPaused)
                    {
                        player.controls.play();
                        playButton.Content = "Pause";
                    }
                    else if (player.playState == WMPLib.WMPPlayState.wmppsUndefined)
                    {
                        setPlayerState("play");
                    }
                    break;
                case "prev":
                    if (curIdx != 0)
                    {
                        curIdx--;
                        setPlayerState("play");
                    }
                    break;
                case "next":
                    if (curIdx != musicArray.Count - 1)
                    {
                        curIdx++;
                        setPlayerState("play");
                    }
                    break;
                case "setrepeat":
                    break;
                case "stop":
                    if(player.playState == WMPLib.WMPPlayState.wmppsPlaying || player.playState == WMPLib.WMPPlayState.wmppsPaused || player.playState == WMPLib.WMPPlayState.wmppsUndefined)
                    {
                        player.controls.stop();
                        player.close();
                    }
                    break;
            }
        }

        private void cleanLists()
        {
            Console.WriteLine("cleaning up");
            if (musicArray.Count > 0)
            {
                Console.WriteLine("cleaning up music file list");
                musicArray.Clear();
            }
            if (availableSongs.Items.Count > 0)
            {
                Console.WriteLine("cleaning up music list");
                availableSongs.Items.Clear();
            }
        }

        private void setupConsole(bool? show = false)
        {
            AllocConsole();

            if (show == true)
            {
                Console.Title = "Basic Music Player Log Console";
            }
            else
            {
                FreeConsole();
            }
        }

        private void postSetupFiles()
        {
            /*
            if(musicArray.Count > 0)
            {
                availableSongs.SelectedIndex = curIdx; //what
            }*/
            doneSearching = true;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
        //lets start with the classic folder
        public string searchPath = "\\";
        //lets do a folder for files to not clout the current folder of audio files
        public string musicFolder = "./music/"; //lets make it compatible with web servers like html5
        //lets just do it the classic way
        public string musicListFile = "musicList.txt";
        public List<string> musicArray = new List<string>();
        public List<string> musicNameArray = new List<string>();
        public WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
        public int curIdx = 0;
        public bool repeatMusic = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            setupFiles();
        }

        public dynamic funnyRead(string filepath, bool turnintoarray)
        {
            dynamic daFileContent = "";
            try
            {
                if(turnintoarray == true)
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
            output.Text = "";
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

            output.Text = daMusicListDir + "\n" + musicFolder;

            if (System.IO.File.Exists(daMusicListDir))
            {
                try
                {
                    details = funnyRead(daMusicListDir, true);
                }
                catch(Exception ex)
                {
                    output.Text += ex;
                }
                foreach(var i in details)
                {
                    var advancedDetails = i.Split("|");
                    output.Text += "\n" + advancedDetails[0];
                    output.Text += "\n" + advancedDetails[1];
                    if (System.IO.Directory.Exists(musicFolder))
                    {
                        //music folder shoudl come formatted already
                        var daMusicFileDir = musicFolder + advancedDetails[0];
                        if (System.IO.File.Exists(daMusicFileDir))
                        {
                            musicArray.Add(daMusicFileDir);
                        }
                    }
                }
            }
            /*
            else if(System.IO.Directory.Exists(musicFolder) && !System.IO.File.Exists(daMusicListDir))
            {
                output.Text += "\npath exists but the file doesnt, maybe it isnt there?? check it up man oh btw music path exists";
            }
            else if(!System.IO.Directory.Exists(musicFolder) && !System.IO.File.Exists(daMusicListDir) )
            {
                output.Text += "\npath exists but the file doesnt, maybe it isnt there?? check it up man oh btw music path doesnt exist";
            }*/
        }

        private void setPlayerState(string state)
        {
            switch (state)
            {
                case "play":
                    player.URL = musicArray[curIdx];
                    player.controls.play();
                    playButton.Content = "Pause";
                    break;
                case "check":
                    if(player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                    {
                        player.controls.pause();
                        playButton.Content = "Resume";
                    } 
                    else if(player.playState == WMPLib.WMPPlayState.wmppsPaused)
                    {
                        player.controls.play();
                        playButton.Content = "Pause";
                    }
                    else if(player.playState == WMPLib.WMPPlayState.wmppsUndefined)
                    {
                        setPlayerState("play");
                    }
                    break;
                case "prev":
                    if(curIdx != 0)
                    {
                        curIdx--;
                        setPlayerState("play");
                    }
                    break;
                case "next":
                    if(curIdx != musicArray.Count - 1)
                    {
                        curIdx++;
                        setPlayerState("play");
                    }
                    break;
                case "setrepeat":
                    break;
            }
        }

        private void searchFiles_Click(object sender, RoutedEventArgs e)
        {
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
    }
}

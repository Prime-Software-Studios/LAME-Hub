using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;

namespace LAME_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum LauncherStatus
    {
        OpeningHub,
        DownloadingHub,
        DownloadingUpdate,
        CheckingForUpdates,
        Failed,
    }

    public partial class MainWindow : Window
    {
        private string rootPath;
        private string versionFile;
        private string hubZip;
        private string hubExe;

        private LauncherStatus _status;

        internal LauncherStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                switch (_status)
                {
                     case LauncherStatus.OpeningHub:
                        LauncherLabel.Content = "Ready To Launch";
                        readyToLaunch();
                        break;
                    case LauncherStatus.DownloadingHub:
                        LauncherLabel.Content = "Downloading LAME Hub";
                        break;
                    case LauncherStatus.DownloadingUpdate:
                        LauncherLabel.Content = "Downloading LAME Hub Update";
                        break;
                    case LauncherStatus.CheckingForUpdates:
                        LauncherLabel.Content = "Checking For LAME Hub Updates";
                        break;
                    case LauncherStatus.Failed:
                        LauncherLabel.Content = "Failed";
                        break;
                    default:
                        break;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            rootPath = Directory.GetCurrentDirectory();
            versionFile = Path.Combine(rootPath, "Version.txt");
            hubZip = Path.Combine(rootPath, "release.zip");
            hubExe = Path.Combine(rootPath, "release", "LAME Hub.exe");
        }

        private void CheckForUpdates()
        {
            if (File.Exists(versionFile))
            {
                Version localVersion = new Version(File.ReadAllText(versionFile));
                VersionLabel.Content = localVersion.ToString();

                try
                {
                    WebClient webClient = new WebClient();
                    Version onlineVersion = new Version(webClient.DownloadString("https://drive.google.com/uc?export=download&id=1cUF1Xo-xp2dFdKhHfoYS5aXSYnfyCY-s"));

                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        InstallHub(false, onlineVersion);
                    }
                    else
                    {
                        Status = LauncherStatus.OpeningHub;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error While Checking For Updates: {ex}");
                }
            }
            else
            {
                InstallHub(false, Version.zero);
            }
        }

        private void InstallHub(bool _isUpdate, Version _onlineVersion)
        {
            try
            {
                WebClient webclient = new WebClient();
                if (_isUpdate)
                {
                    Status = LauncherStatus.DownloadingUpdate;
                }
                else
                {
                    Status = LauncherStatus.DownloadingHub;
                    _onlineVersion = new Version(webclient.DownloadString("https://drive.google.com/uc?export=download&id=1cUF1Xo-xp2dFdKhHfoYS5aXSYnfyCY-s"));
                }

                webclient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadingFilesCompletedCallback);
                webclient.DownloadFileAsync(new Uri("https://github.com/LeopoldPrime/LAME/releases/download/0.0/release.zip"), hubZip, _onlineVersion);
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.Failed;
                MessageBox.Show($"Error Downloading Files: {ex}");
            }
        }

        async private void LaunchHub()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(hubExe);
            startInfo.WorkingDirectory = Path.Combine(rootPath, "release");
            await Task.Delay(3000);
            Process.Start(startInfo);
            Close();
        }

        private void DownloadingFilesCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string onlineVersion = ((Version)e.UserState).ToString();
                ZipFile.ExtractToDirectory(hubZip, rootPath, true);
                File.Delete(hubZip);

                File.WriteAllText(versionFile, onlineVersion);

                VersionLabel.Content = "v" + onlineVersion;
                Status = LauncherStatus.OpeningHub;
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.Failed;
                MessageBox.Show($"Error Finishing Download: {ex}");
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void StabeButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchHub();
        }

        private void readyToLaunch()
        {
            Logo.Visibility = Visibility.Hidden;
            StabeButton.Visibility = Visibility.Visible;
            BetaButton.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    struct Version
    {
        internal static Version zero = new Version(0, 0, 0, 0);

        private short HubVersion;
        private short DestinyVersion;
        private short WarframeVersion;
        private short RustVersion;

        internal Version(short _HubVersion, short _DestinyVersion, short _WarframeVersion, short _RustVersion)
        {
            HubVersion = _HubVersion;
            DestinyVersion = _DestinyVersion;
            WarframeVersion = _WarframeVersion;
            RustVersion = _RustVersion;

        }

        internal Version(string _version)
        {
            string[] _versionStrings = _version.Split('.');

            HubVersion = short.Parse(_versionStrings[0]);
            DestinyVersion = short.Parse(_versionStrings[1]);
            WarframeVersion = short.Parse(_versionStrings[2]);
            RustVersion = short.Parse(_versionStrings[3]);
        }

        internal bool IsDifferentThan(Version _otherVersion)
        {
            if (HubVersion != _otherVersion.HubVersion)
            {
                return true;
            }
            else
            {
                if (DestinyVersion != _otherVersion.WarframeVersion)
                {
                    if (WarframeVersion != _otherVersion.WarframeVersion)
                    {
                        if (RustVersion != _otherVersion.RustVersion)
                        {
                            return true;
                        }
                        else { return false; }
                    }
                    else { return false; }
                }
                else { return false; }
            }
        }

        public override string ToString()
        {
            return $"{HubVersion}.{DestinyVersion}.{WarframeVersion}.{RustVersion}";
        }
    }
}
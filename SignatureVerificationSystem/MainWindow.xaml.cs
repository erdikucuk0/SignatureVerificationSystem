using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IronPython.Hosting;
using System.Diagnostics;
using MessageBox = System.Windows.Forms.MessageBox;
using SignatureVerificationSystem.Models;
using SignatureVerificationSystem.Models.Authorization;
using SignatureVerificationSystem.Utils;
using SignatureVerificationSystem.Windows.SettingsWindows;
using SignatureVerificationSystem.Windows.UserControlWindows;
using SignatureVerificationSystem.Resources;
using System.IO;
using Path = System.IO.Path;

namespace SignatureVerificationSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Member Fields

        private AuthorizationModel authorization { get; set; } = new AuthorizationModel();
        public AuthorizationModel Authorization
        {
            get
            {
                return authorization;
            }
            set
            {
                if (value != authorization)
                {
                    authorization = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Authorization"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        public MainWindow()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException; ;
            InitializeComponent();
            DataContext = this;
            Loaded += MainWindow_Loaded;
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }

        private async void MainWindow_Loaded(object sender, EventArgs e)
        {
            LoginButton.IsEnabled = false;
            Settings.LoadSettings();
            await DBHelper.InitDb();
            LoginButton.IsEnabled = true;

            HelperProcedures.InitTextBlocks(SupportSetGrid);
        }

        #endregion

        #region UI Events

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            Authorization.LoginSucceed(SessionHelper.CurrentUser);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void ManageUsersButton_Click(object sender, RoutedEventArgs e)
        {
            ManageUsersWindow manageUsersWindow = new ManageUsersWindow();
            manageUsersWindow.ShowDialog();
        }

        private void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (PartModelHelper.Instance.SupportSetFileName.Count < 10)
            {
                MessageBox.Show(LocalizedStrings.Instance["FillUpSupportSet"], LocalizedStrings.Instance["EmptySupportSet"]);
                return;
            }

            if (PartModelHelper.Instance.QuerySetFileName.Count < 1)
            {
                MessageBox.Show(LocalizedStrings.Instance["FillUpQuerySet"], LocalizedStrings.Instance["EmptyQuerySet"]);
                return;
            }

            if (Directory.Exists(Paths.SupportSetImages))
                Directory.Delete(Paths.SupportSetImages, true);

            if (Directory.Exists(Paths.QuerySetImages))
                Directory.Delete(Paths.QuerySetImages, true);

            foreach (var img in PartModelHelper.Instance.SupportSetFileName.Values)
            {
                string sourceFilePath = img;
                string destinationFilePath = Path.Combine(Paths.SupportSetImages, img.Split('\\').ElementAtOrDefault(img.Split('\\').Length - 2), img.Split('\\').ElementAtOrDefault(img.Split('\\').Length - 1));

                Directory.CreateDirectory(Path.Combine(Paths.SupportSetImages, img.Split('\\').ElementAtOrDefault(img.Split('\\').Length - 2)));

                File.Copy(sourceFilePath, destinationFilePath, true);
            }

            for (int i = 0; i < 5; i++)
            {
                string sourceFilePath = PartModelHelper.Instance.QuerySetFileName.Values.ToList()[0];
                string destinationFilePath = Path.Combine(Paths.QuerySetImages, $"{sourceFilePath.Split('\\').ElementAtOrDefault(sourceFilePath.Split('\\').Length - 2)}_Copy_{i}", sourceFilePath.Split('\\').ElementAtOrDefault(sourceFilePath.Split('\\').Length - 1));

                Directory.CreateDirectory(Path.Combine(Paths.QuerySetImages, $"{sourceFilePath.Split('\\').ElementAtOrDefault(sourceFilePath.Split('\\').Length - 2)}_Copy_{i}"));

                File.Copy(sourceFilePath, destinationFilePath, true);
            }

            var controlStartTime = DateTime.Now;

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(Paths.PythonExe, Paths.PythonScript)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            p.Start();
            p.WaitForExit();

            ControlTimeTextBlock.Text = string.Format(LocalizedStrings.Instance["ControlTimeTextBlock"], (DateTime.Now - controlStartTime).TotalMilliseconds);

            string estimatedClass = File.ReadAllText(Paths.EstimatedClass);
            string[] estimatedDistances = File.ReadAllLines(Paths.EstimatedDistances);
            string[] classNames = File.ReadAllLines(Paths.ClassNames);

            EstimatedClassTextBlock.Text = estimatedClass;

            EstimatedDistancesTextBlock.Text = "";

            for (int i = 0; i < estimatedDistances.Count(); i++)
            {
                EstimatedDistancesTextBlock.Text += classNames[i] + ": " + estimatedDistances[i] + "\n";
            }
        }

        private void CloseAppButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SupportSetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var supportImage = (Image)((System.Windows.Controls.ContextMenu)(sender as System.Windows.Controls.MenuItem).Parent).PlacementTarget;

            Grid grid = (Grid)supportImage.Parent;

            var openFileDialog = new OpenFileDialog
            {
                Title = LocalizedStrings.Instance["SelectImage"],
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...",
                Multiselect = true,
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (openFileDialog.FileNames.Length != 2)
                {
                    MessageBox.Show(LocalizedStrings.Instance["LoadTwoImages"], LocalizedStrings.Instance["ImagesNotUploaded"]);
                    return;
                }

                DispImages(grid.Name, openFileDialog.FileNames);
            }
        }

        private void QuerySetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = LocalizedStrings.Instance["ChooseAnImage"],
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...",
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                FirstQueryImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));

            if (PartModelHelper.Instance.QuerySetSafeFileName.ContainsKey(FirstQueryImage.Name))
                PartModelHelper.Instance.QuerySetSafeFileName.Remove(FirstQueryImage.Name);

            PartModelHelper.Instance.QuerySetSafeFileName.Add(FirstQueryImage.Name, openFileDialog.FileName.Split('\\').ElementAtOrDefault(openFileDialog.FileName.Split('\\').Length - 2));

            if (PartModelHelper.Instance.QuerySetFileName.ContainsKey(FirstQueryImage.Name))
                PartModelHelper.Instance.QuerySetFileName.Remove(FirstQueryImage.Name);

            PartModelHelper.Instance.QuerySetFileName.Add(FirstQueryImage.Name, openFileDialog.FileName);

            FirstQueryTextBlock.Text = openFileDialog.FileName.Split('\\').ElementAtOrDefault(openFileDialog.FileName.Split('\\').Length - 2);

        }

        #endregion

        #region Helper Procedures

        private void DispImages(string gridName, string[] fileNames)
        {
            switch (gridName)
            {
                case "Grid0":
                    if (PartModelHelper.Instance.SupportSetFileName.ContainsKey("FirstSupportImage1"))
                        PartModelHelper.Instance.SupportSetFileName.Remove("FirstSupportImage1");

                    if (PartModelHelper.Instance.SupportSetFileName.ContainsKey("FirstSupportImage2"))
                        PartModelHelper.Instance.SupportSetFileName.Remove("FirstSupportImage2");

                    if (PartModelHelper.Instance.SupportSetSafeFileName.ContainsKey("FirstSupportImage"))
                        PartModelHelper.Instance.SupportSetSafeFileName.Remove("FirstSupportImage");

                    PartModelHelper.Instance.SupportSetFileName.Add("FirstSupportImage1", fileNames[0]);
                    PartModelHelper.Instance.SupportSetFileName.Add("FirstSupportImage2", fileNames[1]);

                    PartModelHelper.Instance.SupportSetSafeFileName.Add("FirstSupportImage", fileNames[0].Split('\\').ElementAtOrDefault(fileNames[0].Split('\\').Length - 2));

                    PartModelHelper.Instance.SupportSetTextBlocks[1].Text = PartModelHelper.Instance.SupportSetSafeFileName["FirstSupportImage"];

                    FirstSupportImage1.Source = new BitmapImage(new Uri(fileNames[0]));
                    FirstSupportImage2.Source = new BitmapImage(new Uri(fileNames[1]));

                    break;

                case "Grid1":
                    if (PartModelHelper.Instance.SupportSetFileName.ContainsKey("SecondSupportImage1"))
                        PartModelHelper.Instance.SupportSetFileName.Remove("SecondSupportImage1");

                    if (PartModelHelper.Instance.SupportSetFileName.ContainsKey("SecondSupportImage2"))
                        PartModelHelper.Instance.SupportSetFileName.Remove("SecondSupportImage2");

                    if (PartModelHelper.Instance.SupportSetSafeFileName.ContainsKey("SecondSupportImage"))
                        PartModelHelper.Instance.SupportSetSafeFileName.Remove("SecondSupportImage");

                    PartModelHelper.Instance.SupportSetFileName.Add("SecondSupportImage1", fileNames[0]);
                    PartModelHelper.Instance.SupportSetFileName.Add("SecondSupportImage2", fileNames[1]);

                    PartModelHelper.Instance.SupportSetSafeFileName.Add("SecondSupportImage", fileNames[0].Split('\\').ElementAtOrDefault(fileNames[0].Split('\\').Length - 2));

                    PartModelHelper.Instance.SupportSetTextBlocks[3].Text = PartModelHelper.Instance.SupportSetSafeFileName["SecondSupportImage"];

                    SecondSupportImage1.Source = new BitmapImage(new Uri(fileNames[0]));
                    SecondSupportImage2.Source = new BitmapImage(new Uri(fileNames[1]));

                    break;

                case "Grid2":
                    if (PartModelHelper.Instance.SupportSetFileName.ContainsKey("ThirdSupportImage1"))
                        PartModelHelper.Instance.SupportSetFileName.Remove("ThirdSupportImage1");

                    if (PartModelHelper.Instance.SupportSetFileName.ContainsKey("ThirdSupportImage2"))
                        PartModelHelper.Instance.SupportSetFileName.Remove("ThirdSupportImage2");

                    if (PartModelHelper.Instance.SupportSetSafeFileName.ContainsKey("ThirdSupportImage"))
                        PartModelHelper.Instance.SupportSetSafeFileName.Remove("ThirdSupportImage");

                    PartModelHelper.Instance.SupportSetFileName.Add("ThirdSupportImage1", fileNames[0]);
                    PartModelHelper.Instance.SupportSetFileName.Add("ThirdSupportImage2", fileNames[1]);

                    PartModelHelper.Instance.SupportSetSafeFileName.Add("ThirdSupportImage", fileNames[0].Split('\\').ElementAtOrDefault(fileNames[0].Split('\\').Length - 2));

                    PartModelHelper.Instance.SupportSetTextBlocks[4].Text = PartModelHelper.Instance.SupportSetSafeFileName["ThirdSupportImage"];

                    ThirdSupportImage1.Source = new BitmapImage(new Uri(fileNames[0]));
                    ThirdSupportImage2.Source = new BitmapImage(new Uri(fileNames[1]));

                    break;

                case "Grid3":
                    if (PartModelHelper.Instance.SupportSetFileName.ContainsKey("FourthSupportImage1"))
                        PartModelHelper.Instance.SupportSetFileName.Remove("FourthSupportImage1");

                    if (PartModelHelper.Instance.SupportSetFileName.ContainsKey("FourthSupportImage2"))
                        PartModelHelper.Instance.SupportSetFileName.Remove("FourthSupportImage2");

                    if (PartModelHelper.Instance.SupportSetSafeFileName.ContainsKey("FourthSupportImage"))
                        PartModelHelper.Instance.SupportSetSafeFileName.Remove("FourthSupportImage");

                    PartModelHelper.Instance.SupportSetFileName.Add("FourthSupportImage1", fileNames[0]);
                    PartModelHelper.Instance.SupportSetFileName.Add("FourthSupportImage2", fileNames[1]);

                    PartModelHelper.Instance.SupportSetSafeFileName.Add("FourthSupportImage", fileNames[0].Split('\\').ElementAtOrDefault(fileNames[0].Split('\\').Length - 2));

                    PartModelHelper.Instance.SupportSetTextBlocks[2].Text = PartModelHelper.Instance.SupportSetSafeFileName["FourthSupportImage"];

                    FourthSupportImage1.Source = new BitmapImage(new Uri(fileNames[0]));
                    FourthSupportImage2.Source = new BitmapImage(new Uri(fileNames[1]));

                    break;

                case "Grid4":
                    if (PartModelHelper.Instance.SupportSetFileName.ContainsKey("FifthSupportImage1"))
                        PartModelHelper.Instance.SupportSetFileName.Remove("FifthSupportImage1");

                    if (PartModelHelper.Instance.SupportSetFileName.ContainsKey("FifthSupportImage2"))
                        PartModelHelper.Instance.SupportSetFileName.Remove("FifthSupportImage2");

                    if (PartModelHelper.Instance.SupportSetSafeFileName.ContainsKey("FifthSupportImage"))
                        PartModelHelper.Instance.SupportSetSafeFileName.Remove("FifthSupportImage");

                    PartModelHelper.Instance.SupportSetFileName.Add("FifthSupportImage1", fileNames[0]);
                    PartModelHelper.Instance.SupportSetFileName.Add("FifthSupportImage2", fileNames[1]);

                    PartModelHelper.Instance.SupportSetSafeFileName.Add("FifthSupportImage", fileNames[0].Split('\\').ElementAtOrDefault(fileNames[0].Split('\\').Length - 2));

                    PartModelHelper.Instance.SupportSetTextBlocks[0].Text = PartModelHelper.Instance.SupportSetSafeFileName["FifthSupportImage"];

                    FifthSupportImage1.Source = new BitmapImage(new Uri(fileNames[0]));
                    FifthSupportImage2.Source = new BitmapImage(new Uri(fileNames[1]));

                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}
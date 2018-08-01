using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

namespace MkLink_Creator
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Properties

        string _LinkPath = default(string);
        public string LinkPath { get { return _LinkPath; } set { Set(ref _LinkPath, value); } }

        string _DestPath = default(string);
        public string DestPath { get { return _DestPath; } set { Set(ref _DestPath, value); } }

        string _LinkName = default(string);
        public string LinkName { get { return _LinkName; } set { Set(ref _LinkName, value); } }

        string _Description = default(string);
        public string Description { get { return _Description; } set { Set(ref _Description, value); } }

        public string ChoosenCommand { get { return ((MkLinkCommands)Combo.SelectedItem ?? new MkLinkCommands()).Command ?? ""; } }
        public Paths? ChoosenFileType { get; set; } = null;

        public enum Paths{ File, Folder }
        List<MkLinkCommands> Commands;
        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Commands = new List<MkLinkCommands>(new MkLinkCommands[]
            {
                new MkLinkCommands { Name = "Choose!", Command = "", Description = ""},
                new MkLinkCommands { Name = "Directory", Command = "/d", Description = "Creates a directory symbolic link. By default, mklink creates a file symbolic link." },
                new MkLinkCommands { Name = "Hard link", Command = "/h", Description = "A hard link is a file system feature that cannot cross a file system boundary and can only target files." },
                new MkLinkCommands { Name = "Junction", Command = "/j", Description = "Creates a Directory Junction." }
            });
            Combo.ItemsSource = Commands;
            Combo.SelectedIndex = 0;
        }
        #endregion

        #region Events

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LinkPath == "")
            {
                MessageBox.Show("Link can't be emty!", "Error");
                return;
            }
            if (DestPath == "")
            {
                MessageBox.Show("Path can't be emty!", "Error");
                return;
            }
            if (LinkName == "")
            {
                MessageBox.Show("PLinkName can't be emty!", "Error");
                return;
            }
            if (ChoosenCommand == "")
            {
                MessageBox.Show("Please choose a command!", "Error");
                return;
            }

            string link = InsertQuotes(LinkPath + "\\" + LinkName);
            string dest = InsertQuotes(DestPath);
            string command = $"/C mklink {ChoosenCommand} {link} {dest}";
            RunCommand(command);
        }

        private void GetPath_Click(object sender, RoutedEventArgs e)
        {
            string explorer = (sender as Button).Name;
            if (explorer == nameof(LinkButton))
            {
                LinkPath = GetPath(Paths.Folder);
                return;
            }

            if (ChoosenCommand == "")
            {
                MessageBox.Show("Please choose a command First.", "Error");
                return;
            }

            if(ChoosenFileType == Paths.File)
            {
                DestPath = GetPath(Paths.File);
            }
            else
            {
                DestPath = GetPath(Paths.Folder);
            }            
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = Combo.SelectedItem as MkLinkCommands;   
            if (item.Command == "/h")
            {
                if (ChoosenFileType != Paths.File)
                    DestPath = "";

                ChoosenFileType = Paths.File;
                PathButton.IsEnabled = true;

                Destination.Text = "File location:";
                Description = $"Command {item.Command}: {item.Description}";
            }
            else if (item.Command == "/d" || item.Command == "/j")
            {
                if (ChoosenFileType != Paths.Folder)
                    DestPath = "";

                ChoosenFileType = Paths.Folder;
                PathButton.IsEnabled = true;

                Destination.Text = "Folder location:";
                Description = $"Command {item.Command}: {item.Description}";
            }
            else
            {
                PathButton.IsEnabled = false;

                Destination.Text = "Physical location:";
                Description = "";
            }
        }
        #endregion

        #region Helper methods

        private string InsertQuotes(string word)
        {
            word = word.Insert(0, "\"");
            word = word.Insert(word.Count(), "\"");
            return word;
        }

        private string GetPath(Paths path)
        {
            if(path == Paths.File)
            {
                using (var dialog = new WinForms.OpenFileDialog())
                {
                    dialog.Title = "Choose a file!";
                    dialog.Multiselect = false;
                    dialog.ShowDialog();
                    return dialog.FileName;
                }
            }
            else
            {
                using (var dialog = new WinForms.FolderBrowserDialog())
                {
                    dialog.Description = "Choose a folder!";
                    dialog.ShowDialog();
                    return dialog.SelectedPath;
                }
            }
        }

        private void RunCommand(string command, bool runAdmin = false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = command
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                MessageBox.Show("Something went wrong please try again, sometimes the application needs admistrator rights to work.", "Error");
                DestPath = "";
            }                
            else
            {
                string message = $"Link created successfully at \"{LinkPath + "\\" + LinkName}\"";
                MessageBox.Show(message, "Success!");
            }
        }
        #endregion

        #region Helper Class

        public class MkLinkCommands
        {
            public string Name { get; set; }
            public string Command { get; set; }
            public string Description { get; set; }
        }
        #endregion
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                RaisePropertyChanged(propertyName);
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

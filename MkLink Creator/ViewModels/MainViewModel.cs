using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MkLink_Creator.Models;
using MkLink_Creator.Services;
using WinForms = System.Windows.Forms;


namespace MkLink_Creator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties

        private string linkPath = string.Empty;
        public string LinkPath 
        { 
            get { return linkPath; } 
            set { Set(ref linkPath, value); } 
        }

        private string destPath = string.Empty;
        public string DestPath 
        { 
            get { return destPath; } 
            set { Set(ref destPath, value); } 
        }

        private string linkName = string.Empty;
        public string LinkName 
        { 
            get { return linkName; } 
            set { Set(ref linkName, value); } 
        }

        private MklinkCommand choosenCommand;
        public MklinkCommand ChoosenCommand
        {
            get { return choosenCommand; }
            set { Set(ref choosenCommand, value); }
        }

        private MklinkFacade mklinkFacade;
        public MklinkFacade MklinkFacade
        {
            get { return mklinkFacade ?? (mklinkFacade = new MklinkFacade()); }
        }
        #endregion

        public MainViewModel() { }

        #region Commands
        private RelayCommand createLinkCommand;
        public RelayCommand CreateLinkCommand => 
            createLinkCommand ?? (createLinkCommand = new RelayCommand(CreateLink, CanCreateLink));

        private void CreateLink() =>
            MklinkFacade.ExecuteCommand(choosenCommand.Command, LinkPath, DestPath, LinkName);

        private bool CanCreateLink()
        {
            if (linkPath == "" || DestPath == "" || LinkName == "" || ChoosenCommand == null)
                return false;
            return true;
        }

        private RelayCommand getLinkPathCommand;
        public RelayCommand GetLinkPathCommand =>
            getLinkPathCommand ?? (getLinkPathCommand = new RelayCommand(GetLinkPath, CanGetPath));

        private void GetLinkPath()
        {
            using (var dialog = new WinForms.FolderBrowserDialog())
            {
                dialog.Description = "Choose a folder!";
                dialog.ShowDialog();
                LinkPath = dialog.SelectedPath;
            }
        }

        private RelayCommand getDestPathCommand;
        public RelayCommand GetDestPathCommand =>
            getDestPathCommand ?? (getDestPathCommand = new RelayCommand(GetDestPath, CanGetPath));

        private void GetDestPath()
        {
            if(choosenCommand.Command == "/h")
            {
                using (var dialog = new WinForms.OpenFileDialog())
                {
                    dialog.Title = "Choose a file!";
                    dialog.Multiselect = false;
                    dialog.ShowDialog();
                    DestPath = dialog.FileName;
                }
            }
            else
            {
                using (var dialog = new WinForms.FolderBrowserDialog())
                {
                    dialog.Description = "Choose a folder!";
                    dialog.ShowDialog();
                    DestPath = dialog.SelectedPath;
                }
            }
        }

        private bool CanGetPath()
        {
            if (choosenCommand == null)
                return false;
            return true;
        }
        #endregion
    }
}

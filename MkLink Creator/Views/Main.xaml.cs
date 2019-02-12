using MkLink_Creator.Models;
using MkLink_Creator.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MkLink_Creator.Views
{
    public partial class Main : Window
    {
        MainViewModel VM = new MainViewModel();
        public Main()
        {
            InitializeComponent();
            DataContext = VM;
            
            Combo.ItemsSource = VM.MklinkFacade.Commands;
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var command = (MklinkCommand)Combo.SelectedItem;
            Description.Text = command.Description;
            VM.ChoosenCommand = command;
        }
    }
}

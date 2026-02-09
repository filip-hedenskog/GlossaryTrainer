using GlossaryTrainer.ViewModels;
using System.Windows;

namespace GlossaryTrainer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (DataContext is MainWindowViewModel vm)
        {
            vm.NewWordLoaded += FocusInput;
        }

        FocusInput();
    }

    // Call this from ViewModel when a new word is loaded
    public void FocusInput()
    {
        UserInputTextBox.Focus();            // set keyboard focus
        UserInputTextBox.SelectAll();        // optionally select existing text
    }
}
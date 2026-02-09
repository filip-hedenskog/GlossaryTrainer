using GlossaryTrainer.Views;
using Prism.Ioc;
using Prism.Unity;
using System.ComponentModel;
using System.Windows;

namespace GlossaryTrainer;

public partial class App : PrismApplication
{
    protected override Window CreateShell()
        => Container.Resolve<MainWindow>();

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Nothing to register yet
    }
}
using OzzUtils.Wpf;

namespace OzzCodeGen.Wpf.Models
{
    public abstract class MainWindowCommand : AbstractCommand
    {
        public MainWindowCommand(MainWindow window)
        {
            MainWindow = window;
        }

        public MainWindow MainWindow { private set; get; }
    }

    public class CloseWindow : MainWindowCommand
    {
        public CloseWindow(MainWindow window) : base(window) { }

        public override void Execute(object parameter)
        {
            MainWindow.Close();
        }
    }

    public class SaveProjectCommand : MainWindowCommand
    {
        public SaveProjectCommand(MainWindow window) : base(window) { }

        public override void Execute(object parameter)
        {
            MainWindow.SaveProject();
        }
    }

    public class OpenProjectCommand : MainWindowCommand
    {
        public OpenProjectCommand(MainWindow window) : base(window) { }

        public override void Execute(object parameter)
        {
            MainWindow.OpenProject();
        }
    }

    public class NewProjectCommand : MainWindowCommand
    {
        public NewProjectCommand(MainWindow window) : base(window) { }

        public override void Execute(object parameter)
        {
            MainWindow.NewProject();
        }
    }
}

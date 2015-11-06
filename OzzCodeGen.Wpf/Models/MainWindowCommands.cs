using OzzUtils.Wpf;
using System.ComponentModel;
using System;

namespace OzzCodeGen.Wpf.Models
{
    public abstract class MainWindowCommand : AbstractCommand
    {
        public MainWindowCommand(MainWindow window)
        {
            MainWindow = window;
            MainWindow.PropertyChanged += OnMainWindowPropertyChanged;
        }

        public MainWindow MainWindow { private set; get; }
        protected abstract void OnMainWindowPropertyChanged(object sender, PropertyChangedEventArgs e);
    }

    public class CloseWindow : MainWindowCommand
    {
        public CloseWindow(MainWindow window) : base(window) { }

        public override void Execute(object parameter)
        {
            MainWindow.Close();
        }

        protected override void OnMainWindowPropertyChanged(object sender, PropertyChangedEventArgs e) { }
    }

    public class SaveProjectCommand : MainWindowCommand
    {
        public SaveProjectCommand(MainWindow window) : base(window) { }

        public override void Execute(object parameter)
        {
            MainWindow.SaveProject();
        }

        public override bool CanExecute(object parameter)
        {
            return MainWindow.Project != null;
        }

        protected override void OnMainWindowPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Project"))
                RaiseCanExecuteChanged();
        }
    }

    public class OpenProjectCommand : MainWindowCommand
    {
        public OpenProjectCommand(MainWindow window) : base(window) { }

        public override void Execute(object parameter)
        {
            MainWindow.OpenProject();
        }
        protected override void OnMainWindowPropertyChanged(object sender, PropertyChangedEventArgs e) { }
    }

    public class OpenMostRecentProjectCommand : MainWindowCommand
    {
        public OpenMostRecentProjectCommand(MainWindow window) : base(window) { }

        public override void Execute(object parameter)
        {
            MainWindow.OpenMostRecentProject();
        }

        protected override void OnMainWindowPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Project"))
                RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return MainWindow.Project == null &&
                    MainWindow.MostRecentProjectFile != null &&
                    MainWindow.MostRecentProjectFile.FileExists;
        }
    }

    public class NewProjectCommand : MainWindowCommand
    {
        public NewProjectCommand(MainWindow window) : base(window) { }

        public override void Execute(object parameter)
        {
            MainWindow.NewProject();
        }
        protected override void OnMainWindowPropertyChanged(object sender, PropertyChangedEventArgs e) { }
    }
}

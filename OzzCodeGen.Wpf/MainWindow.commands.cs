using OzzCodeGen.Wpf.Models;

namespace OzzCodeGen.Wpf
{
    public partial class MainWindow
    {
        public CloseWindow CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new CloseWindow(this);
                }
                return _closeCommand;
            }
        }
        private CloseWindow _closeCommand;

        public OpenProjectCommand OpenProjectCommand
        {
            get
            {
                if (_openProjectCommand == null)
                {
                    _openProjectCommand = new OpenProjectCommand(this);
                }
                return _openProjectCommand;
            }
        }
        private OpenProjectCommand _openProjectCommand;

        public SaveProjectCommand SaveProjectCommand
        {
            get
            {
                if (_saveProjectCommand == null)
                {
                    _saveProjectCommand = new SaveProjectCommand(this);
                }
                return _saveProjectCommand;
            }
        }
        private SaveProjectCommand _saveProjectCommand;

        public NewProjectCommand NewProjectCommand
        {
            get
            {
                if (_newProjectCommand == null)
                {
                    _newProjectCommand = new NewProjectCommand(this);
                }
                return _newProjectCommand;
            }
        }
        private NewProjectCommand _newProjectCommand;
    }
}

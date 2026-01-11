using OzzUtils.Savables;
using System.ComponentModel;

namespace OzzUtils.Wpf.Savables
{
    public class SavableViewModel : SavableObject, INotifyPropertyChanged
    {
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

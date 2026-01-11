using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OzzUtils.Wpf
{
    public abstract class AbstractCommand : ICommand
    {
        public abstract void Execute(object parameter);

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        protected virtual void RaiseCanExecuteChanged(object sender = null, EventArgs e = null)
        {
            if (sender == null)
                sender = this;
            if (e == null)
                e = new EventArgs();

            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(sender, e);
            }
        }
        public event EventHandler CanExecuteChanged;
    }
}

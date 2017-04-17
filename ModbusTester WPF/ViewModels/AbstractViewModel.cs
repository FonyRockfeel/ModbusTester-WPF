using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ModbusTester_WPF.ViewModels
{
    public abstract class AbstractViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void OnNavigatedFrom()
        {
            
        }
    }
}

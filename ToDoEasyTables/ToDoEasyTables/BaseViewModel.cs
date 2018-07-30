using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToDoEasyTables
{
    public abstract class BaseViewModel : INotifyPropertyChanged // This updates the properties in the ListView
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
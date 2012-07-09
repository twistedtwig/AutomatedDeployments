using System;
using System.ComponentModel;
using Clients.Common;

namespace IISBackup.ViewModels
{   
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, String propertyName)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(String propertyName)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public RelayCommand RefreshData
        {
            get { return new RelayCommand(param => RefreshPageData()); }
        }

        protected abstract void RefreshPageData();
    }
}

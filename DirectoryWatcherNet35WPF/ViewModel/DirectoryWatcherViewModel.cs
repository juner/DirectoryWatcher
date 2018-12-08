using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace DirectoryWatcher.ViewModel
{
    class DirectoryWatcherViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged([CallerMemberName] string PropertyName = null)
        {
            if (PropertyName == null)
                return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        bool CanClear() => Informations.Any();
        void ClearAction()
        {
            foreach(var index in Informations.Select((v,i)=>i).Reverse())
                Informations.RemoveAt(index);
        }
        /// <summary>
        /// 削除ボタン
        /// </summary>
        public ICommand ClearCommand { get; private set; }
        public ObservableCollection<NotifyInformation> Informations { get; private set; } = new ObservableCollection<NotifyInformation>();
        
        
    }
    public class NotifyInformation
    {

    }

}

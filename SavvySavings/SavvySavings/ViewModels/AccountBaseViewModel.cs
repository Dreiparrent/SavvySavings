using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using SavvySavings.Models;
using SavvySavings.Services;
#if __ANDROID__
using SavvySavings.Droid.Services;
#endif
#if __IOS__
using SavvySavings.iOS.Services;
#endif

namespace SavvySavings.ViewModels
{
    public class AccountBaseViewModel : INotifyPropertyChanged
    {
        public ILocalAuth AuthStore => DependencyService.Get<AccountService>() ?? new AccountService();
        public IWatchItemStore<WatchItem> WatchItemStore => DependencyService.Get<AccountService>();
        public ICheckInStore<CheckIn> ChDataStore => DependencyService.Get<AccountService>();

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        string name = string.Empty;
        public string Name
        {
            get { return AuthStore.AuthAccount.Name; }
            set { SetProperty(ref name, value); OnPropertyChanged(); }
        }

        public DateTime Birthday
        {
            get { return AuthStore.AuthAccount.Birthday; }
            set { OnPropertyChanged(); }
        }

        public Double Age
        {
            get
            {
                var age = DateTime.Now - Birthday;
                return age.Days / 360;
            }
            set { OnPropertyChanged("Birthday"); }
        }

        public Double Radius
        {
            get { return AuthStore.AuthAccount.SalesRadius; }
            set { OnPropertyChanged(); }
        }

        Double sPoints = 0;
        public Double SPoints
        {
            get { return sPoints; }
            set { SetProperty(ref sPoints, value); }
        }

        public List<string> SPLevels
        {
            get { return PointsModel.SPLevels; }
        }

        public List<string> SPText
        {
            get { return PointsModel.SPText; }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

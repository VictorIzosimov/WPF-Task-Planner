using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ViewModel.Command;

namespace WPF_Task_Planner
{

    public class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Fields

        private ObservableCollection<Task> tasks;

        private int page = 1;

        private int itemCount = 3;

        private string sortColumn = "Id";

        private string sortDirection = "asc";

        private int totalItems = 0;

        private ICommand firstCommand;

        private ICommand previousCommand;

        private ICommand nextCommand;

        private ICommand lastCommand;
        private bool showAuthorization = true;
        private bool showLogout = false;

        #endregion

        public MainViewModel()
        {
            RefreshTasks();
        }

        public ObservableCollection<Task> Tasks
        {
            get
            {
                return tasks;
            }
            private set
            {
                if (object.ReferenceEquals(tasks, value) != true)
                {
                    tasks = value;
                    NotifyPropertyChanged("Tasks");
                }
            }
        }


        public bool ShowAuthorization
        {
            get { return showAuthorization; }
        }
        public bool ShowLogout
        {
            get { return showLogout; }
        }
        public int TotalItems { get { return totalItems; } }

        public int Page { get { return page; } }


        public ICommand FirstCommand
        {
            get
            {
                if (firstCommand == null)
                {
                    firstCommand = new RelayCommand
                    (
                        param =>
                        {
                            page = 1;
                            RefreshTasks();
                        },
                        param =>
                        {
                            return page > 1 ? true : false;
                        }
                    );
                }

                return firstCommand;
            }
        }

        public ICommand PreviousCommand
        {
            get
            {
                if (previousCommand == null)
                {
                    previousCommand = new RelayCommand
                    (
                        param =>
                        {
                            page --;
                            RefreshTasks();
                        },
                        param =>
                        {
                            return page > 1 ? true : false;
                        }
                    );
                }

                return previousCommand;
            }
        }

        public ICommand NextCommand
        {
            get
            {
                if (nextCommand == null)
                {
                    nextCommand = new RelayCommand
                    (
                        param =>
                        {
                            page ++;
                            RefreshTasks();
                        },
                        param =>
                        {
                            return page < (totalItems - 1) / itemCount + 1 ? true : false;
                        }
                    );
                }

                return nextCommand;
            }
        }

        public ICommand LastCommand
        {
            get
            {
                if (lastCommand == null)
                {
                    lastCommand = new RelayCommand
                    (
                        param =>
                        {
                            page = (totalItems - 1) / itemCount + 1;
                            RefreshTasks();
                        },
                        param =>
                        {
                            return page < (totalItems - 1) / itemCount + 1 ? true : false;
                        }
                    );
                }

                return lastCommand;
            }
        }

        public void Sort(string sortColumn, string direction)
        {
            this.sortColumn = sortColumn;
            this.sortDirection = direction;

            RefreshTasks();
        }
        public void Authorize(bool authorized)
        {
            if (authorized)
            {
                this.showAuthorization = false;
                this.showLogout = true;

            } else
            {
                this.showAuthorization = true;
                this.showLogout = false;
            }
            NotifyPropertyChanged("ShowAuthorization");
            NotifyPropertyChanged("ShowLogout");
        }
        public async void RefreshTasks()
        {
            var result = await DataAccess.GetTasks(page, itemCount, sortColumn, sortDirection);

            totalItems = result.Item1;
            Tasks = result.Item2;
            NotifyPropertyChanged("TotalItems");
            NotifyPropertyChanged("Page");

        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

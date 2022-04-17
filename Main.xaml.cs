using Newtonsoft.Json;
using System.ComponentModel;
using System.Data;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPF_Task_Planner
{
    public partial class MainWindow : Window
    {
        static HttpClient client = new HttpClient();

        private DataGridColumn currentSortColumn;
        private ListSortDirection currentSortDirection;
        private string token;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

        }

        private void TasksDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;

            MainViewModel mainViewModel = (MainViewModel)DataContext;

            string sortField = e.Column.SortMemberPath;

            ListSortDirection direction = (currentSortDirection != ListSortDirection.Ascending && currentSortColumn == e.Column) ?
                ListSortDirection.Ascending : ListSortDirection.Descending;

            var sortDir = direction == ListSortDirection.Ascending ? "asc" : "desc";
            mainViewModel.Sort(sortField, sortDir);


            e.Column.SortDirection = direction;

            currentSortColumn = e.Column;

            currentSortDirection = direction;
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameInput.Text))
            {
                MessageBox.Show("Имя пользователя обязательно для заполнения");
            }
            else if (string.IsNullOrEmpty(TextInput.Text))
            {
                MessageBox.Show("Текст задачи обязателен для заполнения");
            }
            else if (string.IsNullOrEmpty(EmailInput.Text))
            {
                MessageBox.Show("Email обязателен для заполнения");
            }
            else
            {
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(NameInput.Text), "username");
                form.Add(new StringContent(EmailInput.Text), "email");
                form.Add(new StringContent(TextInput.Text), "text");

                HttpResponseMessage response = await client.PostAsync("https://uxcandy.com/~shapoval/test-task-backend/v2/create?developer=Victor", form);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic stuff = JsonConvert.DeserializeObject(json);
                    var status = stuff["status"];
                    if (status == "ok")
                    {
                        MessageBox.Show("Задача добавлена успешно");
                        MainViewModel mainViewModel = (MainViewModel)DataContext;

                        mainViewModel.RefreshTasks();
                    }
                    else
                    {
                        string errorMessage = stuff["message"]["email"];
                        MessageBox.Show(errorMessage);
                    }

                }

            }

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginInput.Text))
            {
                MessageBox.Show("Логин обязателен для заполнения");
            }
            else if (string.IsNullOrEmpty(PasswordInput.Text))
            {
                MessageBox.Show("Пароль обязателен для заполнения");
            }
            else
            {
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(LoginInput.Text), "username");
                form.Add(new StringContent(PasswordInput.Text), "password");

                HttpResponseMessage response = await client.PostAsync("https://uxcandy.com/~shapoval/test-task-backend/v2/login?developer=Victor", form);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic stuff = JsonConvert.DeserializeObject(json);
                    var status = stuff["status"];
                    if (status == "ok")
                    {
                        token = stuff["message"]["token"];
                        MainViewModel mainViewModel = (MainViewModel)DataContext;

                        mainViewModel.Authorize(true);
                        MessageBox.Show("Авторизация прошла успешно");

                    }
                    else
                    {
                        string errorMessage = stuff["message"]["password"];
                        MessageBox.Show(errorMessage);
                    }
                }
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show("Для редактирования необходимо авторизоваться");
            } else
            if (string.IsNullOrEmpty(IdEditInput.Text))
            {
                MessageBox.Show("ID обязателен для заполнения");
            }
            else if (string.IsNullOrEmpty(TextEditInput.Text))
            {
                MessageBox.Show("Текст Задачи обязателен для заполнения");
            }
            else if (string.IsNullOrEmpty(StatusEditInput.Text))
            {
                MessageBox.Show("Статус обязателен для заполнения");
            }
            else
            {
                var id = IdEditInput.Text;
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(token), "token");
                form.Add(new StringContent(IdEditInput.Text), "id");
                form.Add(new StringContent(TextEditInput.Text), "text");
                var statusCode = 0;
                switch(StatusEditInput.Text)
                {
                    case "Выполнена":
                        statusCode = (int)Status.CompleteEdited;
                        break;
                    case "Не выполнена":
                        statusCode = (int)Status.IncompleteEdited;
                        break;
                    default:
                        statusCode = (int)Status.IncompleteEdited;
                        break;
                }
                form.Add(new StringContent(statusCode.ToString()), "status");

                HttpResponseMessage response = await client.PostAsync($"https://uxcandy.com/~shapoval/test-task-backend/v2/edit/{id}?developer=Victor", form);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic stuff = JsonConvert.DeserializeObject(json);
                    var status = stuff["status"];
                    if (status == "ok")
                    {
                        MessageBox.Show("Изменения внесены в задачу #" + id);
                        MainViewModel mainViewModel = (MainViewModel)DataContext;

                        mainViewModel.RefreshTasks();
                    }
                    else
                    {
                        string errorMessage = stuff["message"]["token"];
                        if (string.IsNullOrEmpty(errorMessage)) { 
                            errorMessage = stuff["message"]["id"];
                        }
                        MessageBox.Show(errorMessage);
                    }
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            token = string.Empty;
            MainViewModel mainViewModel = (MainViewModel)DataContext;

            mainViewModel.Authorize(false);
            MessageBox.Show("Вы вышли из профиля");
        }
    }
}

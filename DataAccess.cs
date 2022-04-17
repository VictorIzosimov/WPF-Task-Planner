using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;

namespace WPF_Task_Planner
{

    public static class DataAccess
    {

        private static ObservableCollection<Task> tasks = new ObservableCollection<Task>
        {      
        };
        static HttpClient client = new HttpClient();

        public static async Task<Tuple<int,ObservableCollection<Task>>> GetTasks(int page, int itemCount, string sortColumn, string sortDir)
        {
            var totalItems = tasks.Count;

            ObservableCollection<Task> sortedTasks = new ObservableCollection<Task>();
            var url = $"https://uxcandy.com/~shapoval/test-task-backend/v2/?developer=Victor&sort_field={sortColumn}&sort_direction={sortDir}&page={page}";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic stuff = JsonConvert.DeserializeObject(json);
                var tasks = stuff["message"]["tasks"];
                 totalItems = stuff["message"]["total_task_count"];
                foreach (var task in tasks)
                {
                    Task newTask = new Task()
                    {
                        Id = task["id"],
                        Username = task["username"],
                        Email = task["email"],
                        Text = task["text"],
                        Status = GetEnumDescription((Status)task["status"])
                    };

                    sortedTasks.Add(newTask);
                }
            }


            return new Tuple<int, ObservableCollection<Task>> (totalItems, sortedTasks);
        }
        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}

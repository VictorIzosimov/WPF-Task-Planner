using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF_Task_Planner
{
    public class Task
    {
        public Task()
        {
        }

        public string Id { get; set; }
        public string Username { get; set; } 
        public string Status { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }

    }
}

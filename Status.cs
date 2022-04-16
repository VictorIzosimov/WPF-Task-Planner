using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WPF_Task_Planner
{
    enum Status
    {
        [Description("Нет")]
        None = 0,
        [Description("Задача не выполнена")]
        Incomplete = 1,
        [Description("Задача не выполнена, отредактирована админом")]
        IncompleteEdited = 2,
        [Description("Задача выполнена")]
        Complete = 10,
        [Description("Задача отредактирована админом и выполнена")]
        CompleteEdited = 11
    }
}

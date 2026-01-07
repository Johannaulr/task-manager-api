using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Api.Services.TaskResults
{
    public enum CreateTaskResult
    {
        Success, 
        ValidationFailed,
        DuplicateTaskTitle,
        DueDateInPast
    }
}
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DIA.Core.Models;

namespace DoItApi.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<DiaTask>> GetTasksAsync(string userId);
        Task AddTaskAsync(DiaTask task);
        Task UpdateTaskAsync(DiaTask task);
        Task DeleteTaskAsync(string id, string userId);
    }
}
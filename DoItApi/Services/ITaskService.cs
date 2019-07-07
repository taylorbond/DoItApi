using System.Collections.Generic;
using System.Threading.Tasks;
using DIA.Core.Models;

namespace DoItApi.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<DiaTask>> GetTasks(string userId);
    }
}
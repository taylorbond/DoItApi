using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIA.Core.Exceptions;
using DIA.Core.Models;
using DoItApi.Data;
using Microsoft.EntityFrameworkCore;

namespace DoItApi.Services
{
    public class TaskService : ITaskService
    {
        private readonly DoItDbContext _doItDbContext;

        public TaskService(DoItDbContext doItDbContext)
        {
            _doItDbContext = doItDbContext;
        }

        public async Task<IEnumerable<DiaTask>> GetTasksAsync(string userId)
        {
            var tasks = await _doItDbContext.Tasks
                .Include("Comments").Include("AlertTimes")
                .Where(x => x.UserId == userId)
                .ToListAsync().ConfigureAwait(false);

            if (!tasks.Any()) throw new NoTasksFoundException();

            return tasks;
        }
    }
}
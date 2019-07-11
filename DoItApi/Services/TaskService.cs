using System;
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

        public async Task AddTaskAsync(DiaTask task)
        {
            try
            {
                _doItDbContext.Tasks.Add(task);
                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task UpdateTaskAsync(DiaTask task)
        {
            var entity = await FindTaskByIdForUserAsync(task.Id, task.UserId).ConfigureAwait(false);

            if (entity == null) throw new NoDatabaseObjectFoundException(task.Id);

            try
            {

                entity.TaskDescription = task.TaskDescription;
                entity.DueDateTime = task.DueDateTime;
                entity.AlertTimes = task.AlertTimes;
                entity.Comments = task.Comments;

                _doItDbContext.Entry(entity).State = EntityState.Modified;

                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task DeleteTaskAsync(string id, string userId)
        {
            var entity = await FindTaskByIdForUserAsync(id, userId).ConfigureAwait(false);

            if (entity == null) throw new NoDatabaseObjectFoundException(id);

            try
            {
                _doItDbContext.Tasks.Remove(entity);
                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        private Task<DiaTask> FindTaskByIdForUserAsync(string id, string userId)
        {
            if (_doItDbContext.Tasks == null)
                return Task.FromResult<DiaTask>(null);

            return _doItDbContext.Tasks
                .Include("Comments").Include("AlertTimes")
                .Where(x => x.UserId == userId && x.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
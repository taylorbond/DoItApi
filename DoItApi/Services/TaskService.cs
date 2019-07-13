using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIA.Core.Exceptions;
using DIA.Core.Models;
using DoItApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            var tasks = await GetTasksForUserIdAsync(userId).ConfigureAwait(false);

            var taskList = tasks.ToList();
            if (!taskList.Any()) throw new NoTasksFoundException("No tasks found.");

            return taskList.OrderBy(x => x.CreatedDate);
        }

        public async Task<IEnumerable<DiaTask>> GetTasksWithDetailsAsync(string userId)
        {
            var tasks = await GetTasksWithDetailsForUserIdAsync(userId).ConfigureAwait(false);

            var taskList = tasks.ToList();
            if (!taskList.Any()) throw new NoTasksFoundException("No tasks found.");

            return taskList.OrderBy(x => x.CreatedDate);
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
            try
            {
                var entity = await FindTaskByIdForUserAsync(task.Id, task.UserId).ConfigureAwait(false);

                if (entity == null) throw new NoDatabaseObjectFoundException($"Task {task.Id} was not found.");

                UpdateEntityWithTask(task, entity);

                _doItDbContext.Entry(entity).State = EntityState.Modified;

                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (NoDatabaseObjectFoundException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        private static void UpdateEntityWithTask(DiaTask task, DiaTask entity)
        {
            entity.UpdatedDate = DateTimeOffset.UtcNow;
            entity.TaskDescription = task.TaskDescription;
            entity.DueDateTime = task.DueDateTime;
            entity.AlertTimes = task.AlertTimes;
            entity.Comments = task.Comments;
            entity.IsCompleted = task.IsCompleted;
        }

        public async Task DeleteTaskAsync(string id, string userId)
        {
            try
            {
                var entity = await FindTaskByIdForUserAsync(id, userId).ConfigureAwait(false);

                if (entity == null) throw new NoDatabaseObjectFoundException($"Task {id} was not found.");

                entity.UpdatedDate = DateTimeOffset.UtcNow;
                entity.IsDeleted = true;
                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (NoDatabaseObjectFoundException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task MarkTaskCompleteStatusAsync(string taskId, string userId, bool isComplete)
        {
            try
            {
                var entity = await FindTaskByIdForUserAsync(taskId, userId).ConfigureAwait(false);
                if (entity == null) throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

                entity.UpdatedDate = DateTimeOffset.UtcNow;
                entity.IsCompleted = isComplete;

                _doItDbContext.Entry(entity).State = EntityState.Modified;

                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (NoDatabaseObjectFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync(string userId, string taskId)
        {
            var task = await FindTaskByIdForUserAsync(taskId, userId).ConfigureAwait(false);
            if (task == null) throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

            var comments = task.Comments.Where(x => x.IsDeleted == false);
            var commentsList = comments.ToList();
            if (!commentsList.Any())
                throw new NoCommentsFoundException($"No comments found for Task {taskId}.");

            return commentsList;
        }

        public async Task AddCommentAsync(string taskId, Comment comment)
        {
            try
            {
                var task = await FindTaskByIdForUserAsync(taskId, comment.UserId).ConfigureAwait(false);

                if (task == null)
                    throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

                task.Comments.Add(comment);
                _doItDbContext.Entry(task).State = EntityState.Modified;
                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (NoDatabaseObjectFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task UpdateCommentAsync(string taskId, Comment comment)
        {
            try
            {
                var task = await FindTaskByIdForUserAsync(taskId, comment.UserId).ConfigureAwait(false);
                if (task == null) throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

                var foundComment = task.Comments.FirstOrDefault(x => x.Id == comment.Id);
                if (foundComment == null)
                    throw new NoDatabaseObjectFoundException($"Comment {comment.Id} was not found for Task {taskId}.");

                foundComment.UpdatedDate = DateTimeOffset.UtcNow;
                foundComment.Text = comment.Text;

                _doItDbContext.Entry(foundComment).State = EntityState.Modified;

                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (NoDatabaseObjectFoundException) { throw; }
            catch (Exception e) { throw new DatabaseUpdateException(e); }
        }

        public async Task DeleteCommentAsync(string taskId, string commentId, string userId)
        {
            try
            {
                var task = await FindTaskByIdForUserAsync(taskId, userId).ConfigureAwait(false);
                if (task == null) throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

                var foundComment = task.Comments.FirstOrDefault(x => x.Id == commentId);
                if (foundComment == null) throw new NoDatabaseObjectFoundException($"Comment {commentId} was not found for Task {taskId}.");

                foundComment.UpdatedDate = DateTimeOffset.UtcNow;
                foundComment.IsDeleted = true;

                _doItDbContext.Entry(foundComment).State = EntityState.Modified;

                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (NoDatabaseObjectFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task<IEnumerable<AlertTime>> GetAlertsAsync(string userId, string taskId)
        {
            var task = await FindTaskByIdForUserAsync(taskId, userId).ConfigureAwait(false);
            if (task == null) throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

            var alerts = task.AlertTimes.Where(x => x.IsDeleted == false);
            var alertsList = alerts.ToList();
            if (!alertsList.Any())
                throw new NoAlertsFoundException($"No alerts found for task {taskId}.");

            return alertsList;
        }

        public async Task AddAlertAsync(string taskId, AlertTime alert)
        {
            if (alert.Time <= DateTimeOffset.UtcNow) throw new ArgumentException("Alert time cannot be in the past.");
            try
            {
                var task = await FindTaskByIdForUserAsync(taskId, alert.UserId).ConfigureAwait(false);

                if (task == null)
                    throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

                task.AlertTimes.Add(alert);
                _doItDbContext.Entry(task).State = EntityState.Modified;
                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (NoDatabaseObjectFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task UpdateAlertAsync(string taskId, AlertTime alert)
        {
            if (alert.Time <= DateTimeOffset.UtcNow) throw new ArgumentException("Alert time cannot be in the past.");
            try
            {
                var task = await FindTaskByIdForUserAsync(taskId, alert.UserId).ConfigureAwait(false);

                if (task == null)
                    throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

                var foundAlert = task.AlertTimes.FirstOrDefault(x => x.Id == alert.Id);
                if (foundAlert == null)
                    throw new NoDatabaseObjectFoundException($"Alert {alert.Id} was not found for Task {taskId}.");

                foundAlert.UpdatedDate = DateTimeOffset.UtcNow;
                foundAlert.Time = alert.Time;

                _doItDbContext.Entry(foundAlert).State = EntityState.Modified;

                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (NoDatabaseObjectFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task DeleteAlertAsync(string taskId, string alertId, string userId)
        {
            try
            {
                var task = await FindTaskByIdForUserAsync(taskId, userId).ConfigureAwait(false);
                if (task == null) throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

                var foundAlert = task.AlertTimes.FirstOrDefault(x => x.Id == alertId);
                if (foundAlert == null) throw new NoDatabaseObjectFoundException($"Alert {alertId} was not found for Task {taskId}.");

                foundAlert.UpdatedDate = DateTimeOffset.UtcNow;
                foundAlert.IsDeleted = true;

                _doItDbContext.Entry(foundAlert).State = EntityState.Modified;

                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (NoDatabaseObjectFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        private async Task<IEnumerable<DiaTask>> GetTasksForUserIdAsync(string userId, bool includeDeleted = false)
        {
            return await _doItDbContext.Tasks
                .Where(x => x.UserId == userId && (x.IsDeleted == false || includeDeleted))
                .ToListAsync().ConfigureAwait(false);
        }

        private async Task<IEnumerable<DiaTask>> GetTasksWithDetailsForUserIdAsync(string userId, bool includeDeleted = false)
        {
            return await _doItDbContext.Tasks
                .Include("Comments").Include("AlertTimes")
                .Where(x => x.UserId == userId && (x.IsDeleted == false || includeDeleted))
                .ToListAsync().ConfigureAwait(false);
        }

        private Task<DiaTask> FindTaskByIdForUserAsync(string id, string userId, bool includeDeleted = false)
        {
            if (_doItDbContext.Tasks == null)
                return Task.FromResult<DiaTask>(null);

            return _doItDbContext.Tasks
                .Include("Comments").Include("AlertTimes")
                .Where(x => x.UserId == userId && x.Id == id && (x.IsDeleted == false || includeDeleted))
                .FirstOrDefaultAsync();
        }
    }
}
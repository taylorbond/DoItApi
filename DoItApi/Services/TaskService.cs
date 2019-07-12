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
            if (!taskList.Any()) throw new NoTasksFoundException();

            return taskList;
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
                entity.UpdatedDate = DateTimeOffset.UtcNow;
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
                entity.UpdatedDate = DateTimeOffset.UtcNow;
                entity.IsDeleted = true;
                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync(string userId, string taskId)
        {
            var comments =
                (await FindTaskByIdForUserAsync(taskId, userId).ConfigureAwait(false)).Comments.Where(x =>
                    x.IsDeleted == false);
            var commentsList = comments.ToList();
            if (!commentsList.Any())
                throw new NoCommentsFoundException();

            return commentsList;
        }

        public async Task AddCommentAsync(string taskId, Comment comment)
        {
            // Verify task exists
            var task = await FindTaskByIdForUserAsync(taskId, comment.UserId).ConfigureAwait(false);

            if (task == null)
                throw new NoTasksFoundException();

            try
            {
                task.Comments.Add(comment);
                _doItDbContext.Entry(task).State = EntityState.Modified;
                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task UpdateCommentAsync(string taskId, Comment comment)
        {
            var task = await FindTaskByIdForUserAsync(taskId, comment.UserId).ConfigureAwait(false);
            if (task == null) throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

            var foundComment = task.Comments.FirstOrDefault(x => x.Id == comment.Id);
            if (foundComment == null) throw new NoDatabaseObjectFoundException($"Comment {comment.Id} was not found for Task {taskId}.");

            try
            {
                foundComment.UpdatedDate = DateTimeOffset.UtcNow;
                foundComment.Text = comment.Text;

                _doItDbContext.Entry(foundComment).State = EntityState.Modified;

                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task DeleteCommentAsync(string taskId, string commentId, string userId)
        {
            var task = await FindTaskByIdForUserAsync(taskId, userId).ConfigureAwait(false);
            if (task == null) throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

            var foundComment = task.Comments.FirstOrDefault(x => x.Id == commentId);
            if (foundComment == null) throw new NoDatabaseObjectFoundException($"Comment {commentId} was not found for Task {taskId}.");

            try
            {
                foundComment.UpdatedDate = DateTimeOffset.UtcNow;
                foundComment.IsDeleted = true;

                _doItDbContext.Entry(foundComment).State = EntityState.Modified;

                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task<IEnumerable<AlertTime>> GetAlertsAsync(string userId, string taskId)
        {
            var alerts =
                (await FindTaskByIdForUserAsync(taskId, userId).ConfigureAwait(false)).AlertTimes.Where(x =>
                    x.IsDeleted == false);
            var alertsList = alerts.ToList();
            if (!alertsList.Any())
                throw new NoCommentsFoundException();

            return alertsList;
        }

        public async Task AddAlertAsync(string taskId, AlertTime alert)
        {
            // Verify task exists
            var task = await FindTaskByIdForUserAsync(taskId, alert.UserId).ConfigureAwait(false);

            if (task == null)
                throw new NoTasksFoundException();

            try
            {
                task.AlertTimes.Add(alert);
                _doItDbContext.Entry(task).State = EntityState.Modified;
                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task UpdateAlertAsync(string taskId, AlertTime alert)
        {
            var task = await FindTaskByIdForUserAsync(taskId, alert.UserId).ConfigureAwait(false);
            if (task == null) throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

            var foundAlert = task.AlertTimes.FirstOrDefault(x => x.Id == alert.Id);
            if (foundAlert == null) throw new NoDatabaseObjectFoundException($"Alert {alert.Id} was not found for Task {taskId}.");

            try
            {
                foundAlert.UpdatedDate = DateTimeOffset.UtcNow;
                foundAlert.Time = alert.Time;

                _doItDbContext.Entry(foundAlert).State = EntityState.Modified;

                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        public async Task DeleteAlertAsync(string taskId, string alertId, string userId)
        {
            var task = await FindTaskByIdForUserAsync(taskId, userId).ConfigureAwait(false);
            if (task == null) throw new NoDatabaseObjectFoundException($"Task {taskId} was not found.");

            var foundAlert = task.AlertTimes.FirstOrDefault(x => x.Id == alertId);
            if (foundAlert == null) throw new NoDatabaseObjectFoundException($"Alert {alertId} was not found for Task {taskId}.");

            try
            {
                foundAlert.UpdatedDate = DateTimeOffset.UtcNow;
                foundAlert.IsDeleted = true;

                _doItDbContext.Entry(foundAlert).State = EntityState.Modified;

                await _doItDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new DatabaseUpdateException(e);
            }
        }

        private async Task<IEnumerable<DiaTask>> GetTasksForUserIdAsync(string userId, bool includeDeleted = false)
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
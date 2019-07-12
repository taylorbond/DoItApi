using System.Collections.Generic;
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
        Task<IEnumerable<Comment>> GetCommentsAsync(string userId, string taskId);
        Task AddCommentAsync(string taskId, Comment comment);
        Task UpdateCommentAsync(string taskId, Comment comment);
        Task DeleteCommentAsync(string taskId, string commentId, string userId);
        Task<IEnumerable<AlertTime>> GetAlertsAsync(string userId, string taskId);
        Task AddAlertAsync(string taskId, AlertTime alert);
        Task UpdateAlertAsync(string taskId, AlertTime alert);
        Task DeleteAlertAsync(string taskId, string alertId, string userId);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Box.Core.Services {

    public class AsyncTaskService {

        private static object createTaskLock = new object();
        private MemoryStream memoryStream = new MemoryStream();
        private GZipStream compress;

        public AsyncTaskService() {
           
        }

        public void Write(string asyncTaskUId, string currentActivity, short pct, string text) {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(text);
            compress.Write(bytes, 0, bytes.Length);
            UpdateTask(asyncTaskUId, currentActivity, pct);
        }

        /// <summary>
        /// Starts a new async task by inserting a registry at the AsyncTask table.
        /// </summary>
        /// <param name="type">The type of the task</param>
        /// <param name="currentActivity">The current activity</param>
        /// <param name="singleInstance">True if only one task per type is allowed</param>
        /// <returns>The task UId</returns>
        public string StartTask(string type, string requiredRole, string currentActivity, string description, string fileName, string contentType, string mimeType, Action<string> action, bool singleInstance = true) {

            string taskUId = Guid.NewGuid().ToString();

            //Check if there any 
            if (GetCurrentTask(type) != null)
                return taskUId;

            lock (createTaskLock) {

                // if is single instance, first removes older tasks
                if (singleInstance) {
                    string[] oldTasks = GetTasksUIds(type);
                    foreach (string id in oldTasks)
                        RemoveTask(id);
                }

                // create task at db and executes it async 
                CreateTask(taskUId, type, requiredRole, currentActivity, description, fileName, contentType, mimeType);
            }

            // create brand new streams
            memoryStream = new MemoryStream();
            compress = new GZipStream(memoryStream, CompressionMode.Compress);

            System.Threading.Thread thread = new System.Threading.Thread(p => action((string)p));

            thread.Start(taskUId);

            return taskUId;
        }

        /// <summary>
        /// Creates tasks percentage.
        /// </summary>
        /// <param name="asyncTaskUId">The task UId to be updated</param>
        /// <param name="currentActivity">The current activity</param>
        /// <param name="pct">The current percentage</param>
        public void CreateTask(string asyncTaskUId, string type, string requiredRole, string currentActivity, string description, string fileName, string contentType, string mimeType) {
            AsyncTask asyncTask = new AsyncTask();
            asyncTask.AsyncTaskUId = asyncTaskUId;
            asyncTask.Type = type;
            asyncTask.CurrentAtivity = currentActivity;
            asyncTask.RequiredRole = requiredRole;
            asyncTask.Description = description;
            asyncTask.CurrentPercentage = 0;
            asyncTask.StartDate = DateTime.Now.ToUniversalTime();
            asyncTask.FileName = fileName;
            asyncTask.ResultContentType = contentType;
            asyncTask.ResultMimeType = mimeType;

            using (Data.CoreContext context = new Data.CoreContext()) {
                context.AsyncTasks.Add(asyncTask);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates tasks percentage.
        /// </summary>
        /// <param name="asyncTaskUId">The task UId to be updated</param>
        /// <param name="currentActivity">The current activity</param>
        /// <param name="pct">The current percentage</param>
        public AsyncTask UpdateTask(string asyncTaskUId, string currentActivity, short pct) {
            AsyncTask asyncTask = null;
            using (Data.CoreContext context = new Data.CoreContext()) {
                try {
                    asyncTask = context.AsyncTasks.SingleOrDefault(a => a.AsyncTaskUId == asyncTaskUId);
                    if (asyncTask == null)
                        return null;
                    asyncTask.CurrentAtivity = currentActivity;
                    asyncTask.CurrentPercentage = pct;
                    context.SaveChanges();
                }
                catch {
                    asyncTask = null;
                }

                return asyncTask;
            }
        }

        /// <summary>
        /// Finishes the task.
        /// </summary>
        /// <param name="asyncTaskUId">The task UId to be updated</param>
        /// <param name="result">The byte array result</param>
        public void FinishTask(string asyncTaskUId) {
            using (Data.CoreContext context = new Data.CoreContext()) {
                AsyncTask asyncTask = context.AsyncTasks.SingleOrDefault(a => a.AsyncTaskUId == asyncTaskUId);
                if (asyncTask == null)
                    return;

                compress.Close();
                compress.Dispose();

                asyncTask.Result = memoryStream.ToArray();
                asyncTask.FileSize = asyncTask.Result.Length;
                asyncTask.EndDate = DateTime.Now.ToUniversalTime();
                context.SaveChanges();

                memoryStream.Close();
                memoryStream.Dispose();

                GC.Collect();

            }
        }

        /// <summary>
        /// Removes/Cancel the task.
        /// </summary>
        /// <param name="asyncTaskUId">The task UId to be updated</param>
        public void RemoveTask(string asyncTaskUId) {
            using (Data.CoreContext context = new Data.CoreContext()) {
                AsyncTask asyncTask = context.AsyncTasks.SingleOrDefault(a => a.AsyncTaskUId == asyncTaskUId);
                context.AsyncTasks.Remove(asyncTask);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Gets a task.
        /// </summary>
        /// <param name="asyncTaskUId">The task UId</param>
        /// <returns>The task</returns>
        public AsyncTask GetTask(string asyncTaskUId) {
            using (Data.CoreContext context = new Data.CoreContext())
                return context.AsyncTasks.SingleOrDefault(a => a.AsyncTaskUId == asyncTaskUId);
        }

        public AsyncTask GetTaskWithoutResult(string asyncTaskUId) {
            return null;
        }

        public IEnumerable<AsyncTask> GetTasksWithoutResult(string taskType) {
            using (Data.CoreContext context = new Data.CoreContext()) {
                IEnumerable<AsyncTask> list = context.AsyncTasks.Where(a => a.Type == taskType).ToList();
                list.ToList().ForEach(t => t.Result = null);
                return list;
            }
        }

        public AsyncTask GetCurrentTask(string taskType) {
            using (Data.CoreContext context = new Data.CoreContext()) {
                return context.AsyncTasks.SingleOrDefault(a => a.Type == taskType && a.EndDate == null);
            }
        }

        /// <summary>
        /// Gets all tasks uids of a given type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The ids array</returns>
        public string[] GetTasksUIds(string type) {
            using (Data.CoreContext context = new Data.CoreContext())
                return context.AsyncTasks.Where(a => a.Type == type).Select(a => a.AsyncTaskUId).ToArray();
        }
    }
}
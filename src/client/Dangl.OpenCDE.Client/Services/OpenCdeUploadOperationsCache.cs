using Dangl.OpenCDE.Client.Models;
using System.Collections.Generic;

namespace Dangl.OpenCDE.Client.Services
{
    public class OpenCdeUploadOperationsCache
    {
        public Dictionary<string, DocumentUploadInitializationParameters> UploadTasksByState { get; } = new Dictionary<string, DocumentUploadInitializationParameters>();

        public Queue<OpenCdeUploadTask> UploadTasks { get; } = new Queue<OpenCdeUploadTask>();
    }
}

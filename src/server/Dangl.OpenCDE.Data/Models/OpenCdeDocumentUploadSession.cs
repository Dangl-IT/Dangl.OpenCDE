﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Data.Models
{
    public class OpenCdeDocumentUploadSession
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public CdeUser User { get; set; }

        public DateTimeOffset ValidUntilUtc { get; set; }

        [Required]
        public string ClientCallbackUrl { get; set; }

        /// <summary>
        /// This holds the JWT for the current user, to allow direct login
        /// via the OpenCDE Documents API. The content is a <see cref="TokenStorage"/>
        /// </summary>
        public string AuthenticationInformationJson { get; set; }

        public List<PendingOpenCdeUploadFile> PendingFiles { get; set; }

        public Guid? SelectedProjectId { get; set; }

        public Project SelectedProject { get; set; }
    }
}

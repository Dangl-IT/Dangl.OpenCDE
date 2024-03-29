﻿using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Controllers.Status
{
    /// <summary>
    /// This object represents status information about the app.
    /// </summary>
    public class StatusGet
    {
        /// <summary>
        /// If this is false, there's an internal problem in the app. Otherwise, this should return true
        /// to indicate a healthy operation.
        /// </summary>
        [Required]
        public bool IsHealthy { get; set; }

        /// <summary>
        /// The version of the server.
        /// </summary>
        [Required]
        public string Version { get; set; }

        /// <summary>
        /// The informational version contains additional data about the branch and commit from which this version was built.
        /// </summary>
        [Required]
        public string InformationalVersion { get; set; }

        /// <summary>
        /// The current environment name of the backend.
        /// </summary>
        [Required]
        public string Environment { get; set; }
    }
}

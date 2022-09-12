/*
 * OpenCDE Documents API
 *
 * OpenCDE Documents API Specification
 *
 * The version of the OpenAPI document: 1.0
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Converters;

namespace Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class FileToUpload : IEquatable<FileToUpload>
    {
        /// <summary>
        /// The CDE UI will display this value to the User when entering document metadata
        /// </summary>
        /// <value>The CDE UI will display this value to the User when entering document metadata</value>
        [Required]
        [MinLength(1)]
        [DataMember(Name="file_name", EmitDefaultValue=false)]
        public string FileName { get; set; }

        /// <summary>
        /// This is a client provided id to differentiate between multiple files that are being uploaded in the same session
        /// </summary>
        /// <value>This is a client provided id to differentiate between multiple files that are being uploaded in the same session</value>
        [Required]
        [MinLength(1)]
        [DataMember(Name="session_file_id", EmitDefaultValue=false)]
        public string SessionFileId { get; set; }

        /// <summary>
        /// When present, indicates that this upload is a new version of an existing document
        /// </summary>
        /// <value>When present, indicates that this upload is a new version of an existing document</value>
        [MinLength(1)]
        [DataMember(Name="document_id", EmitDefaultValue=false)]
        public string DocumentId { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class FileToUpload {\n");
            sb.Append("  FileName: ").Append(FileName).Append("\n");
            sb.Append("  SessionFileId: ").Append(SessionFileId).Append("\n");
            sb.Append("  DocumentId: ").Append(DocumentId).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((FileToUpload)obj);
        }

        /// <summary>
        /// Returns true if FileToUpload instances are equal
        /// </summary>
        /// <param name="other">Instance of FileToUpload to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(FileToUpload other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    FileName == other.FileName ||
                    FileName != null &&
                    FileName.Equals(other.FileName)
                ) && 
                (
                    SessionFileId == other.SessionFileId ||
                    SessionFileId != null &&
                    SessionFileId.Equals(other.SessionFileId)
                ) && 
                (
                    DocumentId == other.DocumentId ||
                    DocumentId != null &&
                    DocumentId.Equals(other.DocumentId)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (FileName != null)
                    hashCode = hashCode * 59 + FileName.GetHashCode();
                    if (SessionFileId != null)
                    hashCode = hashCode * 59 + SessionFileId.GetHashCode();
                    if (DocumentId != null)
                    hashCode = hashCode * 59 + DocumentId.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(FileToUpload left, FileToUpload right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FileToUpload left, FileToUpload right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
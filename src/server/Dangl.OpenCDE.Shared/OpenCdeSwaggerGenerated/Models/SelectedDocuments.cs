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
    public partial class SelectedDocuments : IEquatable<SelectedDocuments>
    {
        /// <summary>
        /// A CDE controlled identifier recording the user&#39;s context on the CDE. For example which project and folder the user was on. If the client provides the &#x60;server_context&#x60; in subsequent calls then the CDE will attemp to load the UI at the same place.
        /// </summary>
        /// <value>A CDE controlled identifier recording the user&#39;s context on the CDE. For example which project and folder the user was on. If the client provides the &#x60;server_context&#x60; in subsequent calls then the CDE will attemp to load the UI at the same place.</value>
        [DataMember(Name="server_context", EmitDefaultValue=true)]
        public string ServerContext { get; set; }

        /// <summary>
        /// An array containing all the documents selected by the user
        /// </summary>
        /// <value>An array containing all the documents selected by the user</value>
        [Required]
        [DataMember(Name="documents", EmitDefaultValue=false)]
        public List<DocumentVersion> Documents { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SelectedDocuments {\n");
            sb.Append("  ServerContext: ").Append(ServerContext).Append("\n");
            sb.Append("  Documents: ").Append(Documents).Append("\n");
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
            return obj.GetType() == GetType() && Equals((SelectedDocuments)obj);
        }

        /// <summary>
        /// Returns true if SelectedDocuments instances are equal
        /// </summary>
        /// <param name="other">Instance of SelectedDocuments to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SelectedDocuments other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    ServerContext == other.ServerContext ||
                    ServerContext != null &&
                    ServerContext.Equals(other.ServerContext)
                ) && 
                (
                    Documents == other.Documents ||
                    Documents != null &&
                    other.Documents != null &&
                    Documents.SequenceEqual(other.Documents)
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
                    if (ServerContext != null)
                    hashCode = hashCode * 59 + ServerContext.GetHashCode();
                    if (Documents != null)
                    hashCode = hashCode * 59 + Documents.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(SelectedDocuments left, SelectedDocuments right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SelectedDocuments left, SelectedDocuments right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}

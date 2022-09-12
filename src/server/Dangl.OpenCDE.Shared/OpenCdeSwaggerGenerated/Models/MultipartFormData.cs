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
    /// Multipart Form Data descriptor. If the CDE provided custom data here to be used in multipart requests then it is very likely that the CDE will also set a custom Content-Type header with the multipart/form-data type and a server-provided boundary
    /// </summary>
    [DataContract]
    public partial class MultipartFormData : IEquatable<MultipartFormData>
    {
        /// <summary>
        /// This is a server provided value. Its value must be prefixed to the binary content body when uploading this part
        /// </summary>
        /// <value>This is a server provided value. Its value must be prefixed to the binary content body when uploading this part</value>
        [Required]
        [DataMember(Name="prefix", EmitDefaultValue=false)]
        public byte[] Prefix { get; set; }

        /// <summary>
        /// This is a server provided value. Its value must be suffixed to the binary content body when uploading this part. Typically, this is the end boundary for a multipart/form-data request
        /// </summary>
        /// <value>This is a server provided value. Its value must be suffixed to the binary content body when uploading this part. Typically, this is the end boundary for a multipart/form-data request</value>
        [Required]
        [DataMember(Name="suffix", EmitDefaultValue=false)]
        public byte[] Suffix { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MultipartFormData {\n");
            sb.Append("  Prefix: ").Append(Prefix).Append("\n");
            sb.Append("  Suffix: ").Append(Suffix).Append("\n");
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
            return obj.GetType() == GetType() && Equals((MultipartFormData)obj);
        }

        /// <summary>
        /// Returns true if MultipartFormData instances are equal
        /// </summary>
        /// <param name="other">Instance of MultipartFormData to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MultipartFormData other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Prefix == other.Prefix ||
                    Prefix != null &&
                    Prefix.Equals(other.Prefix)
                ) && 
                (
                    Suffix == other.Suffix ||
                    Suffix != null &&
                    Suffix.Equals(other.Suffix)
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
                    if (Prefix != null)
                    hashCode = hashCode * 59 + Prefix.GetHashCode();
                    if (Suffix != null)
                    hashCode = hashCode * 59 + Suffix.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(MultipartFormData left, MultipartFormData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MultipartFormData left, MultipartFormData right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
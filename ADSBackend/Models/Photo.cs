using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ADSBackend.Models
{
    public class Photo
    {
        [Key]
        public int PhotoId { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public string Filename { get; set; }
        public string Metadata { get; set; } = "{}";
        public string ContentType { get; set; } = "image/jpeg";
        [JsonIgnore]
        public string FileSubPath { get; set; }
        public long Length { get; set; }
        [JsonIgnore]
        public string SecureFileName { get; set; }
        public string Url { get; set; }
        public string Caption { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
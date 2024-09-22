using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinguaPlatform.Models
{
    public class Arch
    {
        public string id { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public string reason { get; set; }
        public string sessionId { get; set; }
        public int projectId { get; set; }
        public Int64 createdAt { get; set; }
        public int size { get; set; }
        public int duration { get; set; }
        public string outputMode { get; set; }
        public bool hasAudio { get; set; }
        public bool hasVideo { get; set; }
        public string certificate { get; set; }
        public string sha256sum { get; set; }
        public string password { get; set; }
        public Int64 updatedAt { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string resolution { get; set; }
        public int partnerId { get; set; }        
    }
}

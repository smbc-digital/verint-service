using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace verint_service.Models
{
    public class NoteWithAttachments
    {
        
        public long CaseRef { get; set; }
        [Required]
        public List<Attachment> Attachments { get; set; }
        public string AttachmentsDescription { get; set; } = string.Empty;
        public int Interaction { get; set; } = 0;
    }
}

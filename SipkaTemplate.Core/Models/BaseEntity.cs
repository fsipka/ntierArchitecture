using System.ComponentModel.DataAnnotations.Schema;

namespace SipkaTemplate.Core.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public bool Status { get; set; } 
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; } 
    }
}

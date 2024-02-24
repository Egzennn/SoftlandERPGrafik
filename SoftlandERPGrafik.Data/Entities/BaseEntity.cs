using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SoftlandERPGrafik.Data.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            this.Id = Guid.NewGuid();
            this.Created = DateTime.Now;
        }

        [Key]
        [DisplayName("ID")]
        public Guid Id { get; set; }

        [DisplayName("Utworzono")]
        public DateTime Created { get; set; }

        [DisplayName("Utworzono przez")]
        public string? CreatedBy { get; set; }

        [DisplayName("Zmodyfikowano")]
        public DateTime? Updated { get; set; }

        [DisplayName("Zmodyfikowano przez")]
        public string? UpdatedBy { get; set; }

        [DisplayName("Stan")]
        public string? Stan { get; set; }

        [DisplayName("Status")]
        public string? Status { get; set; }
    }
}
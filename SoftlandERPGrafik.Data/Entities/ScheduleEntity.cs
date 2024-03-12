using System.ComponentModel.DataAnnotations.Schema;

namespace SoftlandERPGrafik.Data.Entities
{
    public class ScheduleEntity : BaseEntity
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string? Description { get; set; }

        public bool IsAllDay { get; set; }

        public int PRI_PraId { get; set; }

        public int DZL_DzlId { get; set; }

        public Guid? RecurrenceID { get; set; }

        public string? RecurrenceRule { get; set; }

        public string? RecurrenceException { get; set; }

        [NotMapped]
        public bool IsReadonly { get; set; }

        [NotMapped]
        public string Color
        {
            get
            {
                if (!this.colorHasBeenSet)
                {
                    if (this.Stan == "Odrzucenie" || this.Status == "Odrzucenie")
                    {
                        this.color = "#ff0303";
                    }
                    else
                    {
                        this.color = this.Description == null ? "#69DC68" : "#FFE376";
                    }

                    this.colorHasBeenSet = true;
                }

                return this.color;
            }

            set
            {
                this.color = value;
            }
        }

        [NotMapped]
        public string Style
        {
            get
            {
                this.style = (this.Stan == "Plan" || this.Status == "Plan") ? "repeating-linear-gradient(-45deg, rgba(74, 142, 214, 0.12), rgba(74, 142, 214, 0.12) 10px, rgba(249, 250, 252, 0.3) 10px, rgba(249, 250, 252, 0.3) 20px);" : "";
                return this.style;
            }

            set
            {
                this.style = value;
            }
        }

        [NotMapped]
        private bool colorHasBeenSet = false;
        [NotMapped]
        private string color;
        [NotMapped]
        private string style;

    }
}

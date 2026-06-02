using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public bool IsCompleted { get; set; }

        public int CreatedByUserId { get; set; }
        public virtual User CreatedByUser { get; set; }

        public int? AssignedToUserId { get; set; }
        public virtual User AssignedToUser { get; set; }

        public bool IsForAllWorkers { get; set; }

        [NotMapped]
        public string ReceiverName
        {
            get
            {
                if (IsForAllWorkers)
                    return "Все работники";

                if (AssignedToUserId == CreatedByUserId)
                    return "Личная";

                if (AssignedToUser != null)
                    return AssignedToUser.Name;

                return "Не указано";
            }
        }
    }
}

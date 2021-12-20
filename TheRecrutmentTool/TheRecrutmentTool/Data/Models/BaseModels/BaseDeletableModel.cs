namespace TheRecrutmentTool.Data.Models.BaseModels
{
    using System;

    public class BaseDeletableModel : IDeletableEntity
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}

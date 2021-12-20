namespace TheRecrutmentTool.Data.Models.BaseModels
{
    using System;

    public interface IDeletableEntity
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}

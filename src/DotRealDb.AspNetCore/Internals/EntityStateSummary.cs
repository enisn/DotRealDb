using Microsoft.EntityFrameworkCore;

namespace DotRealDb.AspNetCore.Internals
{
    internal class EntityStateSummary
    {
        public EntityStateSummary()
        {
        }

        public EntityStateSummary(string entityName, string dbContextName, object entity, EntityState entityState)
        {
            this.EntityName = entityName;
            this.DbContextName = dbContextName;
            this.Entity = entity;
            this.EntityState = entityState;
        }

        public string EntityName { get; set; }
        public string DbContextName { get; set; }
        public object Entity { get; set; }
        public EntityState EntityState { get; set; }
    }
}

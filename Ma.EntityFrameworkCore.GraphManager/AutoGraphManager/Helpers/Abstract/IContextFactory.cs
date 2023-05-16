namespace Ma.EntityFrameworkCore.GraphManager.AutoGraphManager.Helpers.Abstract
{
    interface IContextFactory
    {
        IContextHelper GetContextHelper();
        IGraphEntityManager<TEntity> GetEntityManager<TEntity>()
            where TEntity : class;
        IGraphEntityTypeManager GetEntityTypeManager(string typeName);
    }
}

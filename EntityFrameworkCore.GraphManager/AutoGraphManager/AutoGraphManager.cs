using EntityFrameworkCore.GraphManager.AutoGraphManager.Abstract;
using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.GraphManager.AutoGraphManager
{
    public class AutoGraphManager
        : IAutoGraphManager
    {
        internal DbContext Context { get; }

        public AutoGraphManager(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}

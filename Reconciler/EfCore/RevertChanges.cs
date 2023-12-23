﻿using System.Reflection;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Collections;
using Microsoft.EntityFrameworkCore;

#nullable enable

public abstract class CollectionHelper
{
    protected abstract void AddOrRemove(IEnumerable collection, Object entity, Boolean actuallyAdd);

    public abstract Boolean IsCollectionT(IEnumerable? collection);

    public static void AddOrRemove(IEnumerable? collection, Object entity, Type type, Boolean actuallyAdd, Boolean throwIfTypeMismatch = false)
    {
        var helper = Get(type);

        if (helper.IsCollectionT(collection))
        {
            helper.AddOrRemove(collection, entity, actuallyAdd);
        }
        else if (throwIfTypeMismatch)
        {
            throw new ArgumentException($"Enumerable type {collection.GetType()} was expected to be a ICollection<{type}>");
        }
    }

    static CollectionHelper Create(Type type)
    {
        var genericType = typeof(CollectionHelper<>).MakeGenericType(type);

        return Activator.CreateInstance(genericType) as CollectionHelper ?? throw new Exception("Activation failed");
    }

    static ConcurrentDictionary<Type, CollectionHelper> helpers = new ConcurrentDictionary<Type, CollectionHelper>();

    static CollectionHelper Get(Type type) => helpers.GetOrAdd(type, Create);
}

public class CollectionHelper<T> : CollectionHelper
{
    public override Boolean IsCollectionT(IEnumerable? collection) => collection is ICollection<T>;

    protected override void AddOrRemove(IEnumerable collection, Object entity, Boolean actuallyAdd)
    {
        if (collection is not ICollection<T> target) throw new ArgumentException($"Enumerable type {collection.GetType()} was expected to be a ICollection<{typeof(T)}>");

        if (entity is not T item) throw new ArgumentException($"Entity of type {entity.GetType()} was expected to be a {typeof(T)}");

        if (actuallyAdd)
        {
            target.Add(item);
        }
        else
        {
            target.Remove(item);
        }
    }
}

public class DbContextHasOnlyNavPropsAdvisoryException : Exception
{
    public static Boolean IsEnabled { get; set; } = true;

    const String message = @"
Your DbContext has entity types with collection navigation properties
that don't have a corresponding scalar navigation property on the related
entity. This is required for RevertChanges to work properly.

This is an advisory exception and can be disabled by setting
`DbContextHasOnlyNavPropsAdvisoryException.IsEnabled` to false in
your application setup code.

For the respective entity types, RevertChanges will not remove added
entities from these collections.

An implementation without this restriction is possible but either
very complex or requiring Entity Framework's internal APIs.

The respective entity types and their properties are:

";

    internal DbContextHasOnlyNavPropsAdvisoryException(String report)
        : base(message + report)
    {
    }
}

public static class Extensions
{
    public static String CheckContextForOnlyNavProps(this DbContext context, Boolean throwOnIssue = false)
    {
        var onlyNavProps =
            from t in context.Model.GetEntityTypes()
            from n in t.GetNavigations()
            where n.IsCollection && n.Inverse is null
            select $"{t.ShortName()}.{n.Name}";

        var report = String.Join("\n", onlyNavProps);

        if (throwOnIssue)
        {
            throw new DbContextHasOnlyNavPropsAdvisoryException(report);
        }

        return report;
    }

    static Boolean oneContextChecked = false;

    static void CheckContextForOnlyNavPropsOnce(this DbContext context)
    {
        // For efficiency reasons, we only check for the first context RevertChanges is
        // called on and only if a debugger is attached. This should create enough attention
        // without causing issues.

        if (!oneContextChecked && DbContextHasOnlyNavPropsAdvisoryException.IsEnabled && Debugger.IsAttached)
        {
            context.CheckContextForOnlyNavProps(throwOnIssue: true);
        }

        oneContextChecked = true;
    }

    public static void RevertChanges(this DbContext context)
    {
        context.CheckContextForOnlyNavPropsOnce();

        foreach (var entry in context.ChangeTracker.Entries().ToArray())
        {
            switch (entry.State)
            {
                case EntityState.Deleted:
                    // It could still be also modified.
                    entry.CurrentValues.SetValues(entry.OriginalValues);

                    entry.State = EntityState.Unchanged;

                    // No collections need to change as that is done by EF only on successful deletion at SaveChanges.

                    break;
                case EntityState.Modified:
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;

                    break;
                case EntityState.Added:
                    // Only in this case do collection need changing: EF modifies collections prior to SaveChanges'
                    // successful completion and those changes need reverting. The following will only work if
                    // all collection nav props have a corresponding scalar nav prop on the other end.

                    // Bizarrely, in the case of a missing scalar nav prop, entry.Navigations just as much missing
                    // the relationship entry as entry.References is. There's no easy way to check for this case.

                    foreach (var reference in entry.References)
                    {
                        var nav = reference.Metadata;

                        if (nav.Inverse?.PropertyInfo is not PropertyInfo relatedProperty) continue;

                        if (reference.CurrentValue is not Object relatedObject) continue;

                        var collectionObject = relatedProperty.GetValue(relatedObject);

                        CollectionHelper.AddOrRemove(collectionObject as IEnumerable, entry.Entity, entry.Metadata.ClrType, false, true);
                    }

                    entry.State = EntityState.Detached;

                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    break;
            }
        }
    }
}

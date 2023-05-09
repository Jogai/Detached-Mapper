using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Ma.EntityFramework.GraphManager.Models;

public class NavigationDetail
{
    public string SourceTypeName { get; set; }
    public List<NavigationRelation> Relations { get; set; }

    public NavigationDetail() { }

    public NavigationDetail(IEntityType entityType)
    {
        SourceTypeName = entityType.Name;
        Relations = ExtractNavigationRelations(entityType);
    }

    private static List<NavigationRelation> ExtractNavigationRelations(IEntityType entityType)
    {
        var relations = new List<NavigationRelation>();

        var entityNavigations = entityType.GetNavigations();
        foreach (var navigation in entityNavigations)
        {
            relations.Add(ExtractNavigationDetails(navigation));
        }

        return relations;
    }

    private static NavigationRelation ExtractNavigationDetails(INavigation navigation)
    {
        var navigationRelation = new NavigationRelation();

        var isOnDependent = navigation.IsOnDependent;
        var isCollection = navigation.IsCollection;
        var inverse = navigation.Inverse;
        var foreignKey = navigation.ForeignKey;

        var principialProperties = foreignKey.PrincipalKey.Properties.Select(property => property.Name).ToList();
        var properties = foreignKey.Properties.Select(property => property.Name).ToList();

        navigationRelation.PropertyName = navigation.Name;
        navigationRelation.PropertyTypeName = navigation.TargetEntityType.Name;
        navigationRelation.Direction = isOnDependent ? NavigationDirection.From : NavigationDirection.To;
        navigationRelation.FromKeyNames = principialProperties;
        navigationRelation.ToKeyNames = properties;

        var inverseNavigation = navigation.Inverse;
        navigationRelation.TargetMultiplicity = CalculateRelationshipMultiplicity(navigation);

        var sourceMultiplicity = RelationshipMultiplicity.One;

        if (inverseNavigation != null)
        {
            sourceMultiplicity = CalculateRelationshipMultiplicity(inverseNavigation);
        }

        navigationRelation.SourceMultiplicity = sourceMultiplicity;

        return navigationRelation;
    }

    private static RelationshipMultiplicity CalculateRelationshipMultiplicity(INavigation navigation)
    {
        if (navigation.IsCollection) return RelationshipMultiplicity.Many;

        var foreignKey = navigation.ForeignKey;
        if (foreignKey.IsRequired || navigation.IsOnDependent)
            return RelationshipMultiplicity.One;
        else
            return RelationshipMultiplicity.ZeroOrOne;
    }
}

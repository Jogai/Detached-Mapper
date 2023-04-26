using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Ma.EntityFramework.GraphManager.Models
{
    public class NavigationDetail
    {        
        public NavigationDetail()
        {
            Relations = new List<NavigationRelation>();
        }

        /// <summary>
        /// Construct NavigationDetail according to EntityType.
        /// </summary>
        /// <param name="entityType">EntityType to get navigation details.</param>
        public NavigationDetail(IEntityType entityType)            
        {
            if (entityType == null)
                return;

            Relations = new List<NavigationRelation>();
            Initialize(entityType);            
        }

        public string SourceTypeName { get; set; }
        public List<NavigationRelation> Relations { get; set; }

        /// <summary>
        /// Initialize navigation details according entity type.
        /// </summary>
        /// <param name="entityType">EntityType to get navigation details.</param>
        private void Initialize(IEntityType entityType)
        {
            if (entityType == null)
                return;

            SourceTypeName = entityType.Name;

            var entityNavigations = entityType.GetNavigations().ToList();

            if (!entityNavigations.Any())
                return;

            foreach (var navigation in entityNavigations)
            {
                var relation = new NavigationRelation { PropertyName = navigation.Name };

                if (navigation.FromEndMember != null)
                    relation.SourceMultiplicity =
                        navigation.FromEndMember.RelationshipMultiplicity;

                if (navigation.ToEndMember != null)
                {
                    relation.TargetMultiplicity =
                        navigation.ToEndMember.RelationshipMultiplicity;

                    AssociationType associationType = navigation.ToEndMember.DeclaringType as AssociationType;

                    if (associationType != null
                        && associationType.Constraint != null)
                    {

                        relation.FromKeyNames = associationType
                            .Constraint
                            .FromProperties
                            .Select(m => m.Name)
                            .ToList();

                        relation.ToKeyNames = associationType
                            .Constraint
                            .ToProperties
                            .Select(m => m.Name)
                            .ToList();

                        if (navigation.ToEndMember.Name == associationType.Constraint.ToRole.Name)
                            relation.Direction = NavigationDirection.To;
                        else if (navigation.ToEndMember.Name == associationType.Constraint.FromRole.Name)
                            relation.Direction = NavigationDirection.From;
                    }

                    RefType toRefType = navigation.ToEndMember.TypeUsage.EdmType as RefType;

                    if (toRefType != null)
                        relation.PropertyTypeName =
                            toRefType.ElementType.Name;
                }

                Relations.Add(relation);
            }
        }
    }

    public class NavigationRelation
    {
        public string PropertyName { get; set; }        
        public string PropertyTypeName { get; set; }
        public List<string> FromKeyNames { get; set; }
        public List<string> ToKeyNames { get; set; }
        public RelationshipMultiplicity SourceMultiplicity { get; set; }
        public RelationshipMultiplicity TargetMultiplicity { get; set; }
        public NavigationDirection Direction { get; set; }
    }

    public enum NavigationDirection
    {
        NotSpecified = 0, From, To
    }
}

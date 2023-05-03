using System.Collections.Generic;

namespace Ma.EntityFramework.GraphManager.Models;

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

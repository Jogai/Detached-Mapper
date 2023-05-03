namespace Ma.EntityFramework.GraphManager.Models;

public enum RelationshipMultiplicity
{
    //
    // Summary:
    //     Lower Bound is Zero and Upper Bound is One
    ZeroOrOne,
    //
    // Summary:
    //     Both lower bound and upper bound is one
    One,
    //
    // Summary:
    //     Lower bound is zero and upper bound is null
    Many
}

using System.Linq;

namespace EntityFrameworkCore.GraphManager.Models
{
    /// <summary>
    /// Name of From and To classes and list of names of From and To properties
    /// </summary>
    public class RelationshipDetail
    {
        internal RelationshipDetail()
        {
            FromDetails = new ForeignKeyDetail();
            ToDetails = new ForeignKeyDetail();
        }

        internal ForeignKeyDetail FromDetails { get; set; }
        internal ForeignKeyDetail ToDetails { get; set; }
    }
}

using System;
namespace PathwaysLib.GraphObjects
{
    public interface IGraphCatalyze
    {
        string ECNumber { get; set; }
        Guid GeneProductID { get; set; }
        Guid OrganismGroupID { get; set; }
        Guid ProcessID { get; set; }
    }
}

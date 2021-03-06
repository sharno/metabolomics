using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    [Table("viewState")]
    public partial class viewState
    {
        [Key]
        public Guid viewID { get; set; }

        [StringLength(32)]
        public string openSection { get; set; }

        [StringLength(32)]
        public string organism { get; set; }

        public Guid? openNode1ID { get; set; }

        [StringLength(32)]
        public string openNode1Type { get; set; }

        public Guid? openNode2ID { get; set; }

        [StringLength(32)]
        public string openNode2Type { get; set; }

        public Guid? openNode3ID { get; set; }

        [StringLength(32)]
        public string openNode3Type { get; set; }

        public Guid? displayItemID { get; set; }

        [StringLength(32)]
        public string displayItemType { get; set; }

        public byte? viewGraph { get; set; }

        public DateTime? timeStamp { get; set; }
    }
}

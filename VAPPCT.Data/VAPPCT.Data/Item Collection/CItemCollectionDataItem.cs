using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

public class CItemCollectionDataItem
{
    public CItemCollectionDataItem()
    {
    }

    public CItemCollectionDataItem(DataSet ds)
    {
    }

    public long CollectionItemID { get; set; }
    public long ItemID { get; set; }
    public long SortOrder { get; set; }
}

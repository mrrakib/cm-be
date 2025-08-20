using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class CollectionConfiguration
{
    public long Id { get; set; }

    public long MemberId { get; set; }

    public decimal CollectionAmount { get; set; }

    public int FromMonth { get; set; }

    public int FromYear { get; set; }

    public int? ToMonth { get; set; }

    public int? ToYear { get; set; }

    public int Status { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

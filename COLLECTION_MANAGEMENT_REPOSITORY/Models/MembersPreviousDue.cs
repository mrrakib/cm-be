using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class MembersPreviousDue
{
    public long Id { get; set; }

    public long MemberId { get; set; }

    public int? TotalMonth { get; set; }

    public decimal TotalDue { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal RemainingDue { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }
}

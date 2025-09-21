using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class MembersBill
{
    public long Id { get; set; }

    public long MemberId { get; set; }

    public long OrgId { get; set; }

    public decimal Amount { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }
}

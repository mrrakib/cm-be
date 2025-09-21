using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class MemberWiseBill
{
    public long Id { get; set; }

    public long MemberId { get; set; }

    public long MembersBillId { get; set; }

    public decimal PaymentAmount { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public bool IsDue { get; set; }

    public bool IsPaid { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }
}

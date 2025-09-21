using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class Collection
{
    public long Id { get; set; }

    public decimal CollectionAmount { get; set; }

    public decimal BillAmount { get; set; }

    public DateTime? CollectionDate { get; set; }

    public bool IsPaid { get; set; }

    public string? InvoiceNo { get; set; }

    public long MemberId { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public long? FinancialYearId { get; set; }

    public long? CollectionTypeId { get; set; }

    public long OrganizationId { get; set; }

    public long? MemberWiseBillId { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }
}

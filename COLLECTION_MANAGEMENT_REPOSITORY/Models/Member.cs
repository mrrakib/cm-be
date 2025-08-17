using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class Member
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ContactNo { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public long VillageId { get; set; }

    public int Status { get; set; }

    public long OrgId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }
}

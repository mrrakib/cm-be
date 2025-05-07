using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class Organization
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? MobileNo { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }
}

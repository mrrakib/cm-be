using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class Menu
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string? ClientUrl { get; set; }

    public long ModuleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual ICollection<MenuPermission> MenuPermissions { get; set; } = new List<MenuPermission>();

    public virtual Module Module { get; set; } = null!;
}

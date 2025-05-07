using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class Module
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
}

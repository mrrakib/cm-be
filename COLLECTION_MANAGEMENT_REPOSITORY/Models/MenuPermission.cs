using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class MenuPermission
{
    public long Id { get; set; }

    public long RoleId { get; set; }

    public long MenuId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual Menu Menu { get; set; } = null!;
}

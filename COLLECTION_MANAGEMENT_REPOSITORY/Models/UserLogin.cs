using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class UserLogin
{
    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public string? ProviderDisplayName { get; set; }

    public long UserId { get; set; }

    public virtual User User { get; set; } = null!;
}

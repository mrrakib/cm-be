using System;
using System.Collections.Generic;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models;

public partial class ResponseMessage
{
    public long Id { get; set; }

    public string? StatusCode { get; set; }

    public string? MessageEn { get; set; }

    public string? MessageBn { get; set; }

    public string? Key { get; set; }
}

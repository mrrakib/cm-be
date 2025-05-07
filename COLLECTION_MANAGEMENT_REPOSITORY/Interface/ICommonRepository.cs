using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Interface
{
    public interface ICommonRepository
    {
        Task<List<ResponseMessage>> GetResponseMessages();
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStoreData
{
    interface IAppEntity
    {
        void Build(ModelBuilder builder);
    }
}

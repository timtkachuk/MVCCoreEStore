using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStoreData
{
    public abstract class AppEntity : IAppEntity

    {
        public abstract void Build(ModelBuilder builder);
    }
}

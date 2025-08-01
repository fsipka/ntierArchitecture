using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Namespace.Core.Models;

namespace Namespace.Repository.Configurations
{
    public class ModelNameConfiguration : BaseEntityConfiguration<ModelName>
    {
        public override void Configure(EntityTypeBuilder<ModelName> builder)
        {
            base.Configure(builder);


        }
    }
}

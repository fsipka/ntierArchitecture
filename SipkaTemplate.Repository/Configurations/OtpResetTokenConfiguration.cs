using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SipkaTemplate.Core.Models;

namespace SipkaTemplate.Repository.Configurations
{
    public class OtpResetTokenConfiguration : BaseEntityConfiguration<OtpResetToken>
    {
        public override void Configure(EntityTypeBuilder<OtpResetToken> builder)
        {
           base.Configure(builder);
            //builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedBy)
            //    .OnDelete(DeleteBehavior.NoAction);


        }
    }
}

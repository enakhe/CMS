using CMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.Data.Configurations
{
    internal class CriminalConfiguration : IEntityTypeConfiguration<Criminal>
    {
        public void Configure(EntityTypeBuilder<Criminal> builder)
        {
            builder.HasKey(c => c.Id);
        }
    }
}

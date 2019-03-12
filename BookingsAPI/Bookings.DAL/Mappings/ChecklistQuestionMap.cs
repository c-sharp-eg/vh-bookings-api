using System;
using System.Collections.Generic;
using System.Text;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.DAL.Mappings
{
    public class ChecklistQuestionMap : IEntityTypeConfiguration<ChecklistQuestion>
    {
        public void Configure(EntityTypeBuilder<ChecklistQuestion> builder)
        {
            builder.ToTable("ChecklistQuestion");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Description);
            builder.HasOne<UserRole>("UserRole").WithMany().HasForeignKey("UserRoleId");
        }
    }
}

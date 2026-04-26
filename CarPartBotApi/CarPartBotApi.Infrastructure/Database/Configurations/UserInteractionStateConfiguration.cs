using CarPartBotApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarPartBotApi.Infrastructure.Database.Configurations;

internal class UserInteractionStateConfiguration : IEntityTypeConfiguration<UserInteractionState>
{
    public void Configure(EntityTypeBuilder<UserInteractionState> builder)
    {
        builder
            .Property(x => x.ActionState)
            .HasColumnType("jsonb");
    }
}

using System;
using FinanceZakatManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FinanceZakatManager.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("FinanceZakatManager.Domain.Entities.Account", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<DateTime>("CreatedUtc").HasColumnType("timestamp with time zone");
                b.Property<string>("Currency").HasMaxLength(3).HasColumnType("character varying(3)");
                b.Property<bool>("IncludeInZakat").HasColumnType("boolean");
                b.Property<bool>("IsArchived").HasColumnType("boolean");
                b.Property<bool>("IsDebtAccount").HasColumnType("boolean");
                b.Property<string>("Name").HasColumnType("text");
                b.Property<decimal>("OpeningBalance").HasPrecision(18, 2).HasColumnType("numeric(18,2)");
                b.Property<int>("Type").HasColumnType("integer");
                b.Property<Guid>("UserId").HasColumnType("uuid");
                b.HasKey("Id");
                b.HasIndex("UserId", "Name").IsUnique();
                b.ToTable("accounts");
            });

            modelBuilder.Entity("FinanceZakatManager.Domain.Entities.AuditLog", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<string>("Action").HasMaxLength(100).HasColumnType("character varying(100)");
                b.Property<string>("Entity").HasMaxLength(100).HasColumnType("character varying(100)");
                b.Property<string>("EntityId").HasMaxLength(100).HasColumnType("character varying(100)");
                b.Property<Guid?>("UserId").HasColumnType("uuid");
                b.Property<DateTime>("TimestampUtc").HasColumnType("timestamp with time zone");
                b.Property<string>("RequestId").HasColumnType("text");
                b.HasKey("Id");
                b.ToTable("audit_logs");
            });

            modelBuilder.Entity("FinanceZakatManager.Domain.Entities.Budget", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<decimal>("Amount").HasPrecision(18, 2).HasColumnType("numeric(18,2)");
                b.Property<string>("Currency").HasMaxLength(3).HasColumnType("character varying(3)");
                b.Property<DateTime>("CreatedUtc").HasColumnType("timestamp with time zone");
                b.Property<int>("Period").HasColumnType("integer");
                b.Property<string>("Name").HasColumnType("text");
                b.Property<DateTime>("StartDate").HasColumnType("timestamp with time zone");
                b.Property<DateTime>("EndDate").HasColumnType("timestamp with time zone");
                b.Property<Guid>("UserId").HasColumnType("uuid");
                b.HasKey("Id");
                b.ToTable("budgets");
            });

            modelBuilder.Entity("FinanceZakatManager.Domain.Entities.Category", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<string>("ColorHex").HasMaxLength(7).HasColumnType("character varying(7)");
                b.Property<DateTime>("CreatedUtc").HasColumnType("timestamp with time zone");
                b.Property<string>("Name").HasColumnType("text");
                b.Property<int>("Type").HasColumnType("integer");
                b.Property<Guid>("UserId").HasColumnType("uuid");
                b.HasKey("Id");
                b.HasIndex("UserId", "Name").IsUnique();
                b.ToTable("categories");
            });

            modelBuilder.Entity("FinanceZakatManager.Domain.Entities.PriceQuote", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<string>("Currency").HasMaxLength(3).HasColumnType("character varying(3)");
                b.Property<DateTime>("CreatedUtc").HasColumnType("timestamp with time zone");
                b.Property<decimal>("PricePerUnit").HasPrecision(18, 4).HasColumnType("numeric(18,4)");
                b.Property<DateTime>("QuotedAtUtc").HasColumnType("timestamp with time zone");
                b.Property<string>("Source").HasColumnType("text");
                b.Property<string>("Symbol").HasMaxLength(3).HasColumnType("character varying(3)");
                b.HasKey("Id");
                b.ToTable("price_quotes");
            });

            modelBuilder.Entity("FinanceZakatManager.Domain.Entities.Transaction", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<Guid>("AccountId").HasColumnType("uuid");
                b.Property<decimal>("Amount").HasPrecision(18, 2).HasColumnType("numeric(18,2)");
                b.Property<DateTime>("BookedOn").HasColumnType("timestamp with time zone");
                b.Property<Guid?>("CategoryId").HasColumnType("uuid");
                b.Property<DateTime>("CreatedUtc").HasColumnType("timestamp with time zone");
                b.Property<string>("Currency").HasMaxLength(3).HasColumnType("character varying(3)");
                b.Property<string>("ExternalId").HasMaxLength(128).HasColumnType("character varying(128)");
                b.Property<bool>("IsTransfer").HasColumnType("boolean");
                b.Property<string>("Note").HasColumnType("text");
                b.Property<string>("Tags").HasColumnType("text");
                b.Property<Guid>("UserId").HasColumnType("uuid");
                b.HasKey("Id");
                b.HasIndex("UserId", "BookedOn");
                b.HasIndex("UserId", "AccountId", "BookedOn");
                b.ToTable("transactions");
            });

            modelBuilder.Entity("FinanceZakatManager.Domain.Entities.UserProfile", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<DateTime>("CreatedUtc").HasColumnType("timestamp with time zone");
                b.Property<string>("DisplayName").HasColumnType("text");
                b.Property<string>("Email").HasColumnType("text");
                b.Property<string>("KeycloakSub").HasColumnType("text");
                b.HasKey("Id");
                b.ToTable("user_profiles");
            });

            modelBuilder.Entity("FinanceZakatManager.Domain.Entities.ZakatCalculation", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<decimal>("AmountDue").HasPrecision(18, 2).HasColumnType("numeric(18,2)");
                b.Property<string>("Currency").HasMaxLength(3).HasColumnType("character varying(3)");
                b.Property<DateTime>("CreatedUtc").HasColumnType("timestamp with time zone");
                b.Property<int>("HawlDays").HasColumnType("integer");
                b.Property<decimal>("NisabThreshold").HasPrecision(18, 2).HasColumnType("numeric(18,2)");
                b.Property<Guid>("UserId").HasColumnType("uuid");
                b.Property<bool>("UsedGoldStandard").HasColumnType("boolean");
                b.Property<decimal>("ZakatableWealth").HasPrecision(18, 2).HasColumnType("numeric(18,2)");
                b.HasKey("Id");
                b.ToTable("zakat_calculations");
            });

            modelBuilder.Entity("FinanceZakatManager.Domain.Entities.ZakatRule", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<DateTime>("CreatedUtc").HasColumnType("timestamp with time zone");
                b.Property<DateTime?>("LastCalculatedUtc").HasColumnType("timestamp with time zone");
                b.Property<decimal>("NisabMultiplier").HasPrecision(18, 4).HasColumnType("numeric(18,4)");
                b.Property<Guid>("UserId").HasColumnType("uuid");
                b.Property<bool>("UseGoldStandard").HasColumnType("boolean");
                b.HasKey("Id");
                b.ToTable("zakat_rules");
            });
#pragma warning restore 612, 618
        }
    }
}

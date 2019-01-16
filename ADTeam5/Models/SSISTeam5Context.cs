using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ADTeam5.Models
{
    public partial class SSISTeam5Context : DbContext
    {
        public SSISTeam5Context()
        {
        }

        public SSISTeam5Context(DbContextOptions<SSISTeam5Context> options)
            : base(options)
        {
        }

        public virtual DbSet<AdjustmentRecord> AdjustmentRecord { get; set; }
        public virtual DbSet<Catalogue> Catalogue { get; set; }
        public virtual DbSet<CollectionPoint> CollectionPoint { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<DepartmentCoveringHeadRecord> DepartmentCoveringHeadRecord { get; set; }
        public virtual DbSet<DisbursementList> DisbursementList { get; set; }
        public virtual DbSet<EmployeeRequestRecord> EmployeeRequestRecord { get; set; }
        public virtual DbSet<InventoryTransRecord> InventoryTransRecord { get; set; }
        public virtual DbSet<PurchaseOrderRecord> PurchaseOrderRecord { get; set; }
        public virtual DbSet<RecordDetails> RecordDetails { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAccount> UserAccount { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=SSISTeam5;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity<AdjustmentRecord>(entity =>
            {
                entity.HasKey(e => e.VoucherNo);

                entity.Property(e => e.VoucherNo)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ApproveDate).HasColumnType("datetime");

                entity.Property(e => e.ClerkId).HasColumnName("ClerkID");

                entity.Property(e => e.IssueDate).HasColumnType("datetime");

                entity.Property(e => e.ManagerId).HasColumnName("ManagerID");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SuperviserId).HasColumnName("SuperviserID");

                entity.HasOne(d => d.Clerk)
                    .WithMany(p => p.AdjustmentRecordClerk)
                    .HasForeignKey(d => d.ClerkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdjustmentRecord_User");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.AdjustmentRecordManager)
                    .HasForeignKey(d => d.ManagerId)
                    .HasConstraintName("FK_AdjustmentRecord_User1");

                entity.HasOne(d => d.Superviser)
                    .WithMany(p => p.AdjustmentRecordSuperviser)
                    .HasForeignKey(d => d.SuperviserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdjustmentRecord_User2");
            });

            modelBuilder.Entity<Catalogue>(entity =>
            {
                entity.HasKey(e => e.ItemNumber);

                entity.Property(e => e.ItemNumber)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ItemName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Last1OrderDate).HasColumnType("datetime");

                entity.Property(e => e.Last2OrderDate).HasColumnType("datetime");

                entity.Property(e => e.Last3OrderDate).HasColumnType("datetime");

                entity.Property(e => e.Location)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Supplier1)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Supplier1Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Supplier2)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Supplier2Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Supplier3)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Supplier3Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UnitOfMeasure)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Supplier1Navigation)
                    .WithMany(p => p.CatalogueSupplier1Navigation)
                    .HasForeignKey(d => d.Supplier1)
                    .HasConstraintName("FK_Catalogue_Supplier1");

                entity.HasOne(d => d.Supplier2Navigation)
                    .WithMany(p => p.CatalogueSupplier2Navigation)
                    .HasForeignKey(d => d.Supplier2)
                    .HasConstraintName("FK_Catalogue_Supplier2");

                entity.HasOne(d => d.Supplier3Navigation)
                    .WithMany(p => p.CatalogueSupplier3Navigation)
                    .HasForeignKey(d => d.Supplier3)
                    .HasConstraintName("FK_Catalogue_Supplier3");
            });

            modelBuilder.Entity<CollectionPoint>(entity =>
            {
                entity.Property(e => e.CollectionPointId).HasColumnName("CollectionPointID");

                entity.Property(e => e.CollectionPointName)
                    .IsRequired()
                    .HasColumnName("CollectionPointName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CollectionTime).HasColumnType("datetime");

                entity.Property(e => e.Location)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepartmentCode);

                entity.Property(e => e.DepartmentCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CollectionPassword)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CollectionPointId).HasColumnName("CollectionPointID");

                entity.Property(e => e.CoveringHeadId).HasColumnName("CoveringHeadID");

                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HeadId).HasColumnName("HeadID");

                entity.Property(e => e.RepId).HasColumnName("RepID");

                entity.HasOne(d => d.CollectionPoint)
                    .WithMany(p => p.Department)
                    .HasForeignKey(d => d.CollectionPointId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Department_CollectionPoint");

                entity.HasOne(d => d.CoveringHead)
                    .WithMany(p => p.DepartmentCoveringHead)
                    .HasForeignKey(d => d.CoveringHeadId)
                    .HasConstraintName("FK_Department_User2");

                entity.HasOne(d => d.Head)
                    .WithMany(p => p.DepartmentHead)
                    .HasForeignKey(d => d.HeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Department_User");

                entity.HasOne(d => d.Rep)
                    .WithMany(p => p.DepartmentRep)
                    .HasForeignKey(d => d.RepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Department_User1");
            });

            modelBuilder.Entity<DepartmentCoveringHeadRecord>(entity =>
            {
                entity.HasKey(e => e.Chindex);

                entity.Property(e => e.Chindex).HasColumnName("CHIndex");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DepartmentCoveringHeadRecord)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DepartmentCoveringHeadRecord_User");
            });

            modelBuilder.Entity<DisbursementList>(entity =>
            {
                entity.HasKey(e => e.Dlid);

                entity.Property(e => e.Dlid)
                    .HasColumnName("DLID")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CollectionPointId).HasColumnName("CollectionPointID");

                entity.Property(e => e.CompleteDate).HasColumnType("datetime");

                entity.Property(e => e.DepartmentCode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.RepId).HasColumnName("RepID");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.CollectionPoint)
                    .WithMany(p => p.DisbursementList)
                    .HasForeignKey(d => d.CollectionPointId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DisbursementList_CollectionPoint");

                entity.HasOne(d => d.DepartmentCodeNavigation)
                    .WithMany(p => p.DisbursementList)
                    .HasForeignKey(d => d.DepartmentCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DisbursementList_Department");

                entity.HasOne(d => d.Rep)
                    .WithMany(p => p.DisbursementList)
                    .HasForeignKey(d => d.RepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DisbursementList_User");
            });

            modelBuilder.Entity<EmployeeRequestRecord>(entity =>
            {
                entity.HasKey(e => e.Rrid)
                    .HasName("PK_DepartmentOrderRecord");

                entity.Property(e => e.Rrid)
                    .HasColumnName("RRID")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CompleteDate).HasColumnType("datetime");

                entity.Property(e => e.DepCode)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.DepEmpId).HasColumnName("DepEmpID");

                entity.Property(e => e.DepHeadId).HasColumnName("DepHeadID");

                entity.Property(e => e.Remark)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RequestDate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.DepCodeNavigation)
                    .WithMany(p => p.EmployeeRequestRecord)
                    .HasForeignKey(d => d.DepCode)
                    .HasConstraintName("FK_EmployeeRequestRecord_Department");

                entity.HasOne(d => d.DepEmp)
                    .WithMany(p => p.EmployeeRequestRecordDepEmp)
                    .HasForeignKey(d => d.DepEmpId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeRequestRecord_User");

                entity.HasOne(d => d.DepHead)
                    .WithMany(p => p.EmployeeRequestRecordDepHead)
                    .HasForeignKey(d => d.DepHeadId)
                    .HasConstraintName("FK_EmployeeRequestRecord_User1");
            });

            modelBuilder.Entity<InventoryTransRecord>(entity =>
            {
                entity.HasKey(e => e.TransId);

                entity.Property(e => e.TransId).HasColumnName("TransID");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.ItemNumber)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.RecordId)
                    .IsRequired()
                    .HasColumnName("RecordID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ItemNumberNavigation)
                    .WithMany(p => p.InventoryTransRecord)
                    .HasForeignKey(d => d.ItemNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryTransRecord_Catalogue");
            });

            modelBuilder.Entity<PurchaseOrderRecord>(entity =>
            {
                entity.HasKey(e => e.Poid);

                entity.Property(e => e.Poid)
                    .HasColumnName("POID")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CompleteDate).HasColumnType("datetime");

                entity.Property(e => e.ExpectedCompleteDate).HasColumnType("datetime");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StoreClerkId).HasColumnName("StoreClerkID");

                entity.Property(e => e.SupplierCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.HasOne(d => d.StoreClerk)
                    .WithMany(p => p.PurchaseOrderRecord)
                    .HasForeignKey(d => d.StoreClerkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseOrderRecord_User");

                entity.HasOne(d => d.SupplierCodeNavigation)
                    .WithMany(p => p.PurchaseOrderRecord)
                    .HasForeignKey(d => d.SupplierCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseOrderRecord_Supplier");
            });

            modelBuilder.Entity<RecordDetails>(entity =>
            {
                entity.HasKey(e => e.Rdid);

                entity.Property(e => e.Rdid).HasColumnName("RDID");

                entity.Property(e => e.ItemNumber)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Remark).IsUnicode(false);

                entity.Property(e => e.Rrid)
                    .IsRequired()
                    .HasColumnName("RRID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ItemNumberNavigation)
                    .WithMany(p => p.RecordDetails)
                    .HasForeignKey(d => d.ItemNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecordDetails_Catalogue");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierCode);

                entity.Property(e => e.SupplierCode)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ContactName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GstregistrationNo)
                    .HasColumnName("GSTRegistrationNo")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TitleOfCourtesy)
                    .HasMaxLength(5)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.DepartmentCode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TitleOfCourtesy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.DepartmentCodeNavigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.DepartmentCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Department");
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.LoginPassword)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}

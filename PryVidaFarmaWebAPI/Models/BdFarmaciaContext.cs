using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PryVidaFarmaWebAPI.Models;

public partial class BdFarmaciaContext : DbContext
{
    public BdFarmaciaContext()
    {
    }

    public BdFarmaciaContext(DbContextOptions<BdFarmaciaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbBanco> TbBancos { get; set; }

    public virtual DbSet<TbCarritoCompra> TbCarritoCompras { get; set; }

    public virtual DbSet<TbCategoria> TbCategorias { get; set; }

    public virtual DbSet<TbCliente> TbClientes { get; set; }

    public virtual DbSet<TbDetalleCompra> TbDetalleCompras { get; set; }

    public virtual DbSet<TbEmpleado> TbEmpleados { get; set; }

    public virtual DbSet<TbPersona> TbPersonas { get; set; }

    public virtual DbSet<TbProducto> TbProductos { get; set; }

    public virtual DbSet<TbPuesto> TbPuestos { get; set; }

    public virtual DbSet<TbTarjeta> TbTarjetas { get; set; }

    public virtual DbSet<TbTiposPago> TbTiposPagos { get; set; }

    public virtual DbSet<TbTransaccionesPaypal> TbTransaccionesPaypals { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
  //      => optionsBuilder.UseSqlServer("server=.;database=bd_farmacia;integrated security=true;TrustServerCertificate=false;Encrypt=false;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbBanco>(entity =>
        {
            entity.HasKey(e => e.IdBanco).HasName("PK__tb_banco__70BD1642B6BD378B");

            entity.ToTable("tb_bancos");

            entity.Property(e => e.IdBanco).HasColumnName("id_banco");
            entity.Property(e => e.NombreBanco)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre_banco");
        });

        modelBuilder.Entity<TbCarritoCompra>(entity =>
        {
            entity.HasKey(e => e.IdCarritoCompra).HasName("PK__tb_carri__9CED1A58151DBBF5");

            entity.ToTable("tb_carrito_compra");

            entity.Property(e => e.IdCarritoCompra).HasColumnName("id_carrito_compra");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.ImporteTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("importeTotal");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.TbCarritoCompras)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_carrito_compra_cliente");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.TbCarritoCompras)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_detalle_producto_producto");
        });

        modelBuilder.Entity<TbCategoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__tb_categ__CD54BC5AED8DEE78");

            entity.ToTable("tb_categorias");

            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre_categoria");
        });

        modelBuilder.Entity<TbCliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__tb_clien__677F38F5062ECA86");

            entity.ToTable("tb_clientes");

            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.Contrasenia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contrasenia");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.TbClientes)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_clientes_persona");
        });

        modelBuilder.Entity<TbDetalleCompra>(entity =>
        {
            entity.HasKey(e => e.IdDetalleCompra).HasName("PK__tb_detal__D48AAC0556137D3C");

            entity.ToTable("tb_detalle_Compra");

            entity.HasIndex(e => new { e.IdCarritoCompra, e.IdProducto }, "UQ__tb_detal__531E5B99FBD593F1").IsUnique();

            entity.Property(e => e.IdDetalleCompra).HasColumnName("id_detalle_Compra");
            entity.Property(e => e.FechaCompra).HasColumnName("fecha_compra");
            entity.Property(e => e.IdCarritoCompra).HasColumnName("id_carrito_compra");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_pago");

            entity.HasOne(d => d.IdCarritoCompraNavigation).WithMany(p => p.TbDetalleCompras)
                .HasForeignKey(d => d.IdCarritoCompra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_detalle_compra_carrito_compra");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.TbDetalleCompras)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_detalle_compra_producto");

            entity.HasOne(d => d.IdTipoPagoNavigation).WithMany(p => p.TbDetalleCompras)
                .HasForeignKey(d => d.IdTipoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_detalle_compra_tipo_pago");
        });

        modelBuilder.Entity<TbEmpleado>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("PK__tb_emple__88B513940A763379");

            entity.ToTable("tb_empleados");

            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
            entity.Property(e => e.FechaContratacion).HasColumnName("fecha_contratacion");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.IdPuesto).HasColumnName("id_puesto");
            entity.Property(e => e.Salario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("salario");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.TbEmpleados)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_empleados_persona");

            entity.HasOne(d => d.IdPuestoNavigation).WithMany(p => p.TbEmpleados)
                .HasForeignKey(d => d.IdPuesto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_empleados_puestos");
        });

        modelBuilder.Entity<TbPersona>(entity =>
        {
            entity.HasKey(e => e.IdPersona).HasName("PK__tb_perso__228148B0B3B7661B");

            entity.ToTable("tb_personas");

            entity.HasIndex(e => e.CorreoElectronico, "UQ__tb_perso__5B8A0682032F484A").IsUnique();

            entity.HasIndex(e => e.Dni, "UQ__tb_perso__D87608A77416F38F").IsUnique();

            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("apellidos");
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("correo_electronico");
            entity.Property(e => e.Direccion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Dni)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("dni");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.Nombres)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombres");
        });

        modelBuilder.Entity<TbProducto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__tb_produ__FF341C0D3FCAD909");

            entity.ToTable("tb_productos");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Detalles)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("detalles");
            entity.Property(e => e.Estado)
                .HasDefaultValue(1)
                .HasColumnName("estado");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Imagen)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("imagen");
            entity.Property(e => e.NombreProducto)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre_producto");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.TbProductos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_producto_categoria");
        });

        modelBuilder.Entity<TbPuesto>(entity =>
        {
            entity.HasKey(e => e.IdPuesto).HasName("PK__tb_puest__123AAB9939D98A07");

            entity.ToTable("tb_puestos");

            entity.Property(e => e.IdPuesto).HasColumnName("id_puesto");
            entity.Property(e => e.NombrePuesto)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre_puesto");
        });

        modelBuilder.Entity<TbTarjeta>(entity =>
        {
            entity.HasKey(e => e.IdTarjeta).HasName("PK__tb_tarje__E92BCFEAB7E3957A");

            entity.ToTable("tb_tarjetas");

            entity.Property(e => e.IdTarjeta).HasColumnName("id_tarjeta");
            entity.Property(e => e.Cvv)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cvv");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            entity.Property(e => e.IdBanco).HasColumnName("id_banco");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_pago");
            entity.Property(e => e.NombreTitular)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre_titular");
            entity.Property(e => e.NumeroTarjeta)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("numero_tarjeta");

            entity.HasOne(d => d.IdBancoNavigation).WithMany(p => p.TbTarjeta)
                .HasForeignKey(d => d.IdBanco)
                .HasConstraintName("fk_tarjetas_banco");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.TbTarjeta)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tarjetas_cliente");

            entity.HasOne(d => d.IdTipoPagoNavigation).WithMany(p => p.TbTarjeta)
                .HasForeignKey(d => d.IdTipoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tipo_pago_credito");
        });

        modelBuilder.Entity<TbTiposPago>(entity =>
        {
            entity.HasKey(e => e.IdTipoPago).HasName("PK__tb_tipos__F7E781E5D77DC699");

            entity.ToTable("tb_tipos_pago");

            entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_pago");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<TbTransaccionesPaypal>(entity =>
        {
            entity.HasKey(e => e.IdTransaccion).HasName("PK__tb_trans__1EDAC0993065FD69");

            entity.ToTable("tb_transacciones_paypal");

            entity.Property(e => e.IdTransaccion).HasColumnName("id_transaccion");
            entity.Property(e => e.Dni)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("dni");
            entity.Property(e => e.FechaTransaccion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_transaccion");
            entity.Property(e => e.IdBanco).HasColumnName("id_banco");
            entity.Property(e => e.IdCarritoCompra).HasColumnName("id_carrito_compra");
            entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_pago");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.NombreTitular)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre_titular");
            entity.Property(e => e.PaypalTransactionId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("paypal_transaction_id");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdBancoNavigation).WithMany(p => p.TbTransaccionesPaypals)
                .HasForeignKey(d => d.IdBanco)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transacciones_banco");

            entity.HasOne(d => d.IdCarritoCompraNavigation).WithMany(p => p.TbTransaccionesPaypals)
                .HasForeignKey(d => d.IdCarritoCompra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transacciones_carrito_compra");

            entity.HasOne(d => d.IdTipoPagoNavigation).WithMany(p => p.TbTransaccionesPaypals)
                .HasForeignKey(d => d.IdTipoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transacciones_tipo_pago");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelCliente : DbContext
    {
        public ModelCliente()
            : base("name=ModelCliente")
        {
        }

        public virtual DbSet<ConexionSAP> ConexionSAP { get; set; }
        public virtual DbSet<EncDocumento> EncDocumento { get; set; }
        public virtual DbSet<Impuestos> Impuestos { get; set; }
        public virtual DbSet<OtrosCargos> OtrosCargos { get; set; }
        public virtual DbSet<OtrosTextos> OtrosTextos { get; set; }
        public virtual DbSet<Sucursales> Sucursales { get; set; }
        public virtual DbSet<DetDocumento> DetDocumento { get; set; }
        public virtual DbSet<Parametros> Parametros { get; set; }
        public virtual DbSet<UnidadesMedida> UnidadesMedida { get; set; }
        public virtual DbSet<BitacoraErrores> BitacoraErrores { get; set; }
        public virtual DbSet<CondicionesVenta> CondicionesVenta { get; set; }
        public virtual DbSet<RespuestasCyber> RespuestasCyber { get; set; }
        public virtual DbSet<Cantones> Cantones { get; set; }
        public virtual DbSet<Distritos> Distritos { get; set; }
        public virtual DbSet<Barrios> Barrios { get; set; }
        //Parte grafica
        public virtual DbSet<Login> Login { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<SeguridadModulos> SeguridadModulos { get; set; }
        public virtual DbSet<SeguridadRolesModulos> SeguridadRolesModulos { get; set; }
        public virtual DbSet<CorreosRecepcion> CorreosRecepcion { get; set; }
        public virtual DbSet<BandejaEntrada> BandejaEntrada { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConexionSAP>()
                .Property(e => e.SQLUser)
                .IsUnicode(false);

            modelBuilder.Entity<ConexionSAP>()
                .Property(e => e.SQLServer)
                .IsUnicode(false);

            modelBuilder.Entity<ConexionSAP>()
                .Property(e => e.SQLPass)
                .IsUnicode(false);

            modelBuilder.Entity<ConexionSAP>()
                .Property(e => e.SQLBD)
                .IsUnicode(false);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.idSucursal)
                .IsUnicode(false);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.consecutivoSAP)
                .IsUnicode(false);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.TipoDocumento)
                .IsUnicode(false);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.condicionVenta)
                .IsUnicode(false);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.medioPago)
                .IsUnicode(false);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.montoOtrosCargos)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.moneda)
                .IsUnicode(false);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.tipoCambio)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalserviciogravado)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalservicioexento)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalservicioexonerado)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalmercaderiagravado)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalmercaderiaexonerado)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalexento)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalexonerado)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalventa)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totaldescuentos)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalventaneta)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalimpuestos)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalivadevuelto)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalotroscargos)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EncDocumento>()
                .Property(e => e.totalcomprobante)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Impuestos>()
                .Property(e => e.id)
                .IsUnicode(false);

            modelBuilder.Entity<Impuestos>()
                .Property(e => e.codigo)
                .IsUnicode(false);

            modelBuilder.Entity<Impuestos>()
                .Property(e => e.codigoTarifa)
                .IsUnicode(false);

            modelBuilder.Entity<Impuestos>()
                .Property(e => e.tarifa)
                .HasPrecision(4, 2);

            modelBuilder.Entity<Impuestos>()
                .Property(e => e.factorIVA)
                .HasPrecision(1, 1);

            modelBuilder.Entity<Impuestos>()
                .Property(e => e.exportacion)
                .HasPrecision(13, 5);

            modelBuilder.Entity<OtrosCargos>()
                .Property(e => e.tipoDocumento)
                .IsUnicode(false);

            modelBuilder.Entity<OtrosCargos>()
                .Property(e => e.detalle)
                .IsUnicode(false);

            modelBuilder.Entity<OtrosCargos>()
                .Property(e => e.porcentaje)
                .HasPrecision(5, 5);

            modelBuilder.Entity<OtrosTextos>()
                .Property(e => e.codigo)
                .IsUnicode(false);

            modelBuilder.Entity<OtrosTextos>()
                .Property(e => e.detalle)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.codSuc)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.NombreComercial)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.Terminal)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.TipoCedula)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.Cedula)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.Provincia)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.Canton)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.Distrito)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.Barrio)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.sennas)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.Telefono)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.Correo)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.Logo)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.ApiKey)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.codPais)
                .IsUnicode(false);

            modelBuilder.Entity<Sucursales>()
                .Property(e => e.CodActividadComercial)
                .IsUnicode(false);
        }
    }
}

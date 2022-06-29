CREATE TABLE [dbo].[BandejaEntrada](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[XmlFactura] [varbinary](max) NULL,
	[XmlConfirmacion] [varchar](max) NULL,
	[Pdf] [varbinary](max) NULL,
	[FechaIngreso] [datetime] NOT NULL,
	[Procesado] [varchar](1) NULL,
	[FechaProcesado] [datetime] NULL,
	[Mensaje] [varchar](max) NULL,
	[Asunto] [varchar](max) NULL,
	[Remitente] [varchar](max) NULL,
	[NumeroConsecutivo] [varchar](100) NULL,
	[TipoDocumento] [varchar](20) NULL,
	[FechaEmision] [varchar](20) NULL,
	[NombreEmisor] [varchar](200) NULL,
	[IdEmisor] [varchar](100) NULL,
	[CodigoMoneda] [varchar](20) NULL,
	[TotalComprobante] [money] NULL,
	[tipo] [varchar](3) NULL,
	[Impuesto] [money] NULL,
	[DetalleMensaje] [varchar](500) NULL,
	[CodigoActividad] [varchar](20) NULL,
	[CondicionImpuesto] [varchar](3) NULL,
	[impuestoAcreditar] [money] NULL,
	[gastoAplicable] [money] NULL,
	[situacionPresentacion] [varchar](2) NULL,
	[tipoIdentificacionEmisor] [varchar](3) NULL,
	[JSON] [varchar](max) NULL,
	[RespuestaHacienda] [varchar](50) NULL,
	[XMLRespuesta] [varchar](max) NULL,
	[ClaveReceptor] [varchar](100) NULL,
	[ConsecutivoReceptor] [varchar](50) NULL,
 CONSTRAINT [PK_BandejaEntrada] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[BandejaEntrada] ADD  CONSTRAINT [DF_BandejaEntrada_FechaIngreso]  DEFAULT (getdate()) FOR [FechaIngreso]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'05 aceptacion, 06 parcial, 07 rechazado' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BandejaEntrada', @level2type=N'COLUMN',@level2name=N'tipo'
GO

------------------------------------------------------Bandeja Entrada-----------------------------------
CREATE TABLE [dbo].[Barrios](
	[CodProvincia] [int] NOT NULL,
	[CodCanton] [int] NOT NULL,
	[CodDistrito] [int] NOT NULL,
	[CodBarrio] [int] NOT NULL,
	[NomBarrio] [varchar](100) NULL,
 CONSTRAINT [PK_Barrios] PRIMARY KEY CLUSTERED 
(
	[CodProvincia] ASC,
	[CodCanton] ASC,
	[CodDistrito] ASC,
	[CodBarrio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--------------------------------------Barrios --------------------------------------------
CREATE TABLE [dbo].[BitacoraErrores](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[DocNum] [varchar](max) NULL,
	[Type] [varchar](2) NULL,
	[Descripcion] [varchar](max) NULL,
	[StackTrace] [varchar](max) NULL,
	[Fecha] [datetime] NULL,
 CONSTRAINT [PK_BitacoraErrores] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[BitacoraErrores] ADD  CONSTRAINT [DF_BitacoraErrores_Fecha]  DEFAULT (getdate()) FOR [Fecha]
GO

---------------------------------------Bitacora Errores --------------------------------
CREATE TABLE [dbo].[Cantones](
	[CodProvincia] [int] NOT NULL,
	[CodCanton] [int] NOT NULL,
	[NomCanton] [varchar](30) NULL,
 CONSTRAINT [PK_Cantones] PRIMARY KEY CLUSTERED 
(
	[CodProvincia] ASC,
	[CodCanton] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
------------------------------------------Cantones--------------------------------------
CREATE TABLE [dbo].[CondicionesVenta](
	[codSAP] [varchar](2) NOT NULL,
	[codCyber] [varchar](2) NULL,
	[Nombre] [varchar](100) NULL,
 CONSTRAINT [PK_CondicionesVenta] PRIMARY KEY CLUSTERED 
(
	[codSAP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--------------------------------Condiciones Venta -------------------------------
CREATE TABLE [dbo].[ConexionSAP](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[SQLUser] [varchar](50) NULL,
	[SQLServer] [varchar](50) NULL,
	[SQLPass] [varchar](60) NULL,
	[SQLBD] [varchar](50) NULL,
 CONSTRAINT [PK_ConexionSAP] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

------------------------------Conexion SAP ----------------------------------------
CREATE TABLE [dbo].[CorreosRecepcion](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[RecepcionEmail] [varchar](500) NULL,
	[RecepcionPassword] [varchar](500) NULL,
	[RecepcionHostName] [varchar](50) NULL,
	[RecepcionUseSSL] [bit] NULL,
	[RecepcionPort] [int] NULL,
	[RecepcionUltimaLecturaImap] [datetime] NULL,
 CONSTRAINT [PK_CorreosRecepcion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


---------------------------------------Correos Recepcion---------------------------
CREATE TABLE [dbo].[DetDocumento](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idEncabezado] [int] NOT NULL,
	[NumLinea] [int] NULL,
	[partidaArancelaria] [varchar](15) NULL,
	[exportacion] [money] NULL,
	[CodCabys] [varchar](13) NULL,
	[tipoCod] [varchar](2) NULL,
	[codPro] [varchar](20) NULL,
	[cantidad] [money] NULL,
	[unidadMedida] [varchar](5) NULL,
	[unidadMedidaComercial] [varchar](20) NULL,
	[NomPro] [varchar](200) NULL,
	[PrecioUnitario] [money] NULL,
	[MontoTotal] [money] NULL,
	[MontoDescuento] [money] NULL,
	[NaturalezaDescuento] [varchar](80) NULL,
	[SubTotal] [money] NULL,
	[baseImponible] [money] NULL,
	[idImpuesto] [varchar](10) NULL,
	[montoImpuesto] [money] NULL,
	[factorIVA] [money] NULL,
	[exonTipoDoc] [varchar](2) NULL,
	[exonNumdoc] [varchar](40) NULL,
	[exonNomInst] [varchar](160) NULL,
	[exonFecEmi] [datetime] NULL,
	[exonPorExo] [int] NULL,
	[exonMonExo] [money] NULL,
	[impNeto] [money] NULL,
	[totalLinea] [money] NULL,
 CONSTRAINT [PK_DetDocumento] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
---------------------------------Det Documento ---------------------------------------------------
CREATE TABLE [dbo].[Distritos](
	[CodProvincia] [int] NOT NULL,
	[CodCanton] [int] NOT NULL,
	[CodDistrito] [int] NOT NULL,
	[NomDistrito] [varchar](30) NULL,
 CONSTRAINT [PK_Distritos] PRIMARY KEY CLUSTERED 
(
	[CodProvincia] ASC,
	[CodCanton] ASC,
	[CodDistrito] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

------------------------------Distritos ----------------------------------------------------

CREATE TABLE [dbo].[EncDocumento](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idSucursal] [varchar](3) NOT NULL,
	[consecutivoSAP] [varchar](max) NOT NULL,
	[consecutivoInterno] [int] NULL,
	[TipoDocumento] [varchar](2) NULL,
	[DocEntry] [varchar](max) NULL,
	[Fecha] [datetime] NULL,
	[CodActividadEconomica] [varchar](50) NULL,
	[CardCode] [varchar](10) NULL,
	[CardName] [varchar](100) NULL,
	[LicTradNum] [varchar](12) NULL,
	[Email] [varchar](100) NULL,
	[TipoIdentificacion] [varchar](2) NULL,
	[condicionVenta] [varchar](2) NULL,
	[plazoCredito] [int] NULL,
	[medioPago] [varchar](15) NULL,
	[montoOtrosCargos] [money] NULL,
	[moneda] [varchar](3) NULL,
	[tipoCambio] [money] NULL,
	[totalserviciogravado] [money] NULL,
	[totalservicioexento] [money] NULL,
	[totalservicioexonerado] [money] NULL,
	[totalmercaderiagravado] [money] NULL,
	[totalmercaderiaexonerado] [money] NULL,
	[totalmercaderiaexenta] [money] NULL,
	[totalgravado] [money] NULL,
	[totalexento] [money] NULL,
	[totalexonerado] [money] NULL,
	[totalventa] [money] NULL,
	[totaldescuentos] [money] NULL,
	[totalventaneta] [money] NULL,
	[totalimpuestos] [money] NULL,
	[totalivadevuelto] [money] NULL,
	[totalotroscargos] [money] NULL,
	[totalcomprobante] [money] NULL,
	[RefTipoDocumento] [varchar](2) NULL,
	[RefNumeroDocumento] [varchar](50) NULL,
	[RefFechaEmision] [datetime] NULL,
	[RefCodigo] [varchar](2) NULL,
	[RefRazon] [varchar](180) NULL,
	[procesadaHacienda] [bit] NULL,
	[RespuestaHacienda] [varchar](500) NULL,
	[XMLFirmado] [varchar](max) NULL,
	[ClaveHacienda] [varchar](55) NULL,
	[ConsecutivoHacienda] [varchar](25) NULL,
	[ErrorCyber] [varchar](max) NULL,
	[code] [int] NULL,
	[JSON] [varchar](max) NULL,
	[sincronizadaSAP] [bit] NULL,
 CONSTRAINT [PK_EncDocumento] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[EncDocumento] ADD  CONSTRAINT [DF_EncDocumento_sincronizadaSAP]  DEFAULT ((0)) FOR [sincronizadaSAP]
GO

--------------------------------------------Enc Documento----------------------------------------------
CREATE TABLE [dbo].[Impuestos](
	[id] [varchar](10) NOT NULL,
	[codigo] [varchar](2) NULL,
	[codigoTarifa] [varchar](2) NULL,
	[tarifa] [decimal](4, 2) NULL,
	[factorIVA] [decimal](1, 1) NULL,
	[exportacion] [decimal](13, 5) NULL,
 CONSTRAINT [PK_Impuestos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
-------------------------------------------Impuestos --------------------------------------
CREATE TABLE [dbo].[Login](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idRol] [int] NULL,
	[Email] [varchar](200) NULL,
	[Nombre] [varchar](100) NULL,
	[Activo] [bit] NULL,
	[Clave] [varchar](500) NULL,
 CONSTRAINT [PK_Login] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--------------------------------------------Login ---------------------------------------------
CREATE TABLE [dbo].[OtrosCargos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idEncabezado] [int] NULL,
	[tipoDocumento] [varchar](2) NULL,
	[detalle] [varchar](160) NULL,
	[porcentaje] [decimal](5, 5) NULL,
	[monto] [money] NULL,
 CONSTRAINT [PK_OtrosCargos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-------------------------------------------Otros Cargos ------------------------------------------

CREATE TABLE [dbo].[OtrosTextos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idEncabezado] [int] NULL,
	[codigo] [varchar](2) NULL,
	[detalle] [varchar](50) NULL,
 CONSTRAINT [PK_OtrosTextos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

---------------------------------------Otros Textos -------------------------------------

CREATE TABLE [dbo].[Parametros](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[FETEEnc] [varchar](max) NULL,
	[FETEDet] [varchar](max) NULL,
	[NCEnc] [varchar](max) NULL,
	[NCDet] [varchar](max) NULL,
	[FECEnc] [varchar](max) NULL,
	[FECDet] [varchar](max) NULL,
	[Exoneracion] [varchar](max) NULL,
	[UbicacionCliente] [varchar](max) NULL,
	[SerieFE] [varchar](3) NULL,
	[SerieTE] [varchar](3) NULL,
	[SerieNC] [varchar](3) NULL,
	[SerieND] [varchar](3) NULL,
	[SerieFEC] [varchar](3) NULL,
	[SerieFEE] [varchar](3) NULL,
	[urlCyber] [varchar](500) NULL,
	[urlCyberRespHacienda] [varchar](500) NULL,
	[urlCyberAceptacion] [varchar](500) NULL,
	[urlCyberReenvio] [varchar](500) NULL,
	[CampoConsecutivo] [varchar](500) NULL,
	[CampoClave] [varchar](500) NULL,
	[CampoEstado] [varchar](500) NULL,
	[urlWebApi] [varchar](500) NULL,
 CONSTRAINT [PK_Parametros] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

----------------------------------------Parametros --------------------------------
CREATE TABLE [dbo].[RespuestasCyber](
	[id] [int] NOT NULL,
	[Detalle] [varchar](300) NULL,
 CONSTRAINT [PK_RespuestasCyber] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--------------------------------------Respuestas Cyber -----------------------------------

CREATE TABLE [dbo].[Roles](
	[idRol] [int] IDENTITY(1,1) NOT NULL,
	[NombreRol] [varchar](50) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[idRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


---------------------------------------Roles ------------------------------------------------
CREATE TABLE [dbo].[SeguridadModulos](
	[CodModulo] [int] NOT NULL,
	[Descripcion] [varchar](150) NOT NULL,
 CONSTRAINT [PK_SeguridadModulos_1] PRIMARY KEY CLUSTERED 
(
	[CodModulo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-----------------------------------Seguridad Modulos -------------------------------------
CREATE TABLE [dbo].[SeguridadRolesModulos](
	[CodRol] [int] NOT NULL,
	[CodModulo] [int] NOT NULL,
 CONSTRAINT [PK_SeguridadRolesModulos_1] PRIMARY KEY CLUSTERED 
(
	[CodRol] ASC,
	[CodModulo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

------------------------------------Seguridad Roles Modulos ---------------------------------

CREATE TABLE [dbo].[Sucursales](
	[codSuc] [varchar](3) NOT NULL,
	[Nombre] [varchar](100) NULL,
	[NombreComercial] [varchar](100) NULL,
	[Terminal] [varchar](5) NULL,
	[TipoCedula] [varchar](2) NULL,
	[Cedula] [varchar](12) NULL,
	[Provincia] [varchar](1) NULL,
	[Canton] [varchar](2) NULL,
	[Distrito] [varchar](2) NULL,
	[Barrio] [varchar](2) NULL,
	[sennas] [varchar](250) NULL,
	[Telefono] [varchar](20) NULL,
	[Correo] [varchar](100) NULL,
	[Logo] [varchar](max) NULL,
	[ApiKey] [varchar](max) NULL,
	[consecFac] [int] NULL,
	[consecTiq] [int] NULL,
	[consecNC] [int] NULL,
	[consecND] [int] NULL,
	[consecFEC] [int] NULL,
	[consecFEE] [int] NULL,
	[consecAFC] [int] NULL,
	[codPais] [varchar](3) NULL,
	[idConexion] [int] NOT NULL,
	[CodActividadComercial] [varchar](6) NULL,
 CONSTRAINT [PK_Sucursaless] PRIMARY KEY CLUSTERED 
(
	[codSuc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-------------------------------------------Sucursales ------------------------------------------
CREATE TABLE [dbo].[UnidadesMedida](
	[codSAP] [varchar](7) NOT NULL,
	[codCyber] [varchar](7) NULL,
	[Nombre] [varchar](50) NULL,
 CONSTRAINT [PK_UnidadesMedida] PRIMARY KEY CLUSTERED 
(
	[codSAP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
--------------------------------------------Unidades Medida --------------------------------------------



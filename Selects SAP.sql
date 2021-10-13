select t0.DocEntry, t0.DocNum, t0.ObjType, t0.Series, t0.CardCode as CardCode, t2.CardName as NombreCliente, t2.LicTradNum as Identificacion, t2.E_Mail as Correo, t0.DocDate, t0.GroupNum as CondicionVenta , t1.ExtraDays as PlazoCredito, t0.U_BYL_MedioPago as MedioPago,
t0.DocCur as Moneda, t0.SysRate as TipoCambio, CASE WHEN t3.RefDocNum is null or t3.RefDocNum = '' then  ''
ELSE t3.RefDocNum END 
as Referencia, t3.Remark as Comentario
from oinv t0 
inner join octg t1 on t0.GroupNum = t1.GroupNum
inner join ocrd t2 on t0.CardCode = t2.CardCode
left join inv21 t3 on t0.DocEntry = t3.DocEntry
where t0.DocNum =


 
-------------------------------------------------------EncDocumento---------------------------------------------------
select distinct t0.DocEntry, t0.DocNum, t0.ObjType, t0.Series, t0.CardCode as CardCode, t0.CardName as NombreCliente, t2.LicTradNum as Identificacion, t2.E_Mail as Correo, t0.DocDate, t0.GroupNum as CondicionVenta, t3.ExtraDays as PlazoCredito, t0.U_BYL_MedioPago as MedioPago,
t0.DocCur as Moneda, t0.SysRate as TipoCambio,
CASE WHEN t1.BaseRef is null or t1.BaseRef = '' then  t4.RefDocNum
ELSE t1.BaseRef END 
as Referencia, t4.Remark as Comentario
from ORIN t0
inner join rin1 t1 on t0.DocEntry = t1.DocEntry
inner join ocrd t2 on t0.CardCode = t2.CardCode
inner join octg t3 on t0.GroupNum = t3.GroupNum
left join RIN21 t4 on t0.DocEntry = t4.DocEntry
where t0.DocNum =
 
   
 
-------------------------------------------------------EncNotaCredito-------------------------------------------------
 

select t0.DocEntry, t0.DocNum, t0.ObjType, t0.Series, t0.CardCode as CardCode, t2.CardName as NombreCliente, t2.LicTradNum as Identificacion, t2.E_Mail as Correo, t0.DocDate, t0.GroupNum as CondicionVenta, t1.ExtraDays as PlazoCredito, t0.U_BYL_MedioPago as MedioPago,
 t0.DocCur as Moneda, t0.SysRate as TipoCambio, t0.U_BYL_Actividad as ActividadComercial, '' as Referencia, '' as Comentario
 from OPCH t0
 inner join octg t1 on t0.GroupNum = t1.GroupNum
inner join ocrd t2 on t0.CardCode = t2.CardCode
where t0.DocNum = 
------------------------------------------------------EncFacturaElectronicaCompra-------------------------------------
  select t0.LineNum + 1 as NumLinea,'' as PartidaArancelaria, 0 as Exportacion, 
 CASE WHEN 
t1.U_BYL_CodigoMH IS NULL then t0.U_BYL_CodigoMH ELSE t1.U_BYL_CodigoMH END as CodigoCabys
 ,CASE WHEN 
t1.U_BYL_TipoCod IS NULL then '' ELSE t1.U_BYL_TipoCod END as tipoCod,
CASE WHEN 
t0.ItemCode IS NULL then '' ELSE t0.ItemCode END as CodPro, 
 t0.Quantity as Cantidad,
CASE WHEN t1.ItemCode is null then 'Sp' Else t1.U_BYL_UoM END as UnidadMedida,
t0.Dscription as NomPro, t0.PriceBefDi as PrecioUnitario, t0.DiscPrcnt as PorDesc, t0.TaxCode as idImpuesto
, t0.U_BYL_NatDescuento as NaturalezaDescuento, t0.U_BYL_ExDoc as DocumentoExoneracion, 
CASE WHEN 
t1.U_FactorDeVenta IS NULL then 0 ELSE t1.U_FactorDeVenta END as FactorIVA
from INV1 t0 
left join oitm t1 on t0.ItemCode = t1.ItemCode
where t0.DocEntry = @reemplazo
order by t0.LineNum

-----------------------------------------------------DetDocumento------------------------------------------------------
select t0.LineNum + 1 as NumLinea, '' as PartidaArancelaria, 0 as Exportacion, 
CASE WHEN 
t1.U_BYL_CodigoMH IS NULL then t0.U_BYL_CodigoMH ELSE t1.U_BYL_CodigoMH END as CodigoCabys,
CASE WHEN 
t1.U_BYL_TipoCod IS NULL then '' ELSE t1.U_BYL_TipoCod END as tipoCod,
CASE WHEN 
t0.ItemCode IS NULL then '' ELSE t0.ItemCode END as CodPro,
t0.Quantity as Cantidad,
CASE WHEN t1.ItemCode is null then 'Sp' Else t1.U_BYL_UoM END as UnidadMedida ,
t0.Dscription as NomPro, t0.PriceBefDi as PrecioUnitario, t0.DiscPrcnt as PorDesc, t0.TaxCode as idImpuesto
, t0.U_BYL_NatDescuento as NaturalezaDescuento, t0.U_BYL_ExDoc as DocumentoExoneracion,
CASE WHEN 
t1.U_FactorDeVenta IS NULL then 0 ELSE t1.U_FactorDeVenta END as FactorIVA
from PCH1 t0
left join oitm t1 on t0.ItemCode = t1.ItemCode
where t0.DocEntry = @reemplazo
order by t0.LineNum


 
 ----------------------------------------------------DetFEC-----------------------------------------------------------
  select t0.LineNum + 1 as NumLinea, '' as PartidaArancelaria, 0 as Exportacion,  
 CASE WHEN 
t1.U_BYL_CodigoMH IS NULL then t0.U_BYL_CodigoMH ELSE t1.U_BYL_CodigoMH END as  CodigoCabys
 
 , CASE WHEN 
t1.U_BYL_TipoCod IS NULL then '' ELSE t1.U_BYL_TipoCod END as tipoCod,
 CASE WHEN 
t0.ItemCode IS NULL then '' ELSE t0.ItemCode END as CodPro, 
t0.Quantity as Cantidad,
 CASE WHEN t1.ItemCode is null then 'Sp' Else t1.U_BYL_UoM END as UnidadMedida, 
 t0.Dscription as NomPro, t0.PriceBefDi as PrecioUnitario, t0.DiscPrcnt as PorDesc, t0.TaxCode as idImpuesto,
 t0.U_BYL_NatDescuento as NaturalezaDescuento, t0.U_BYL_ExDoc as DocumentoExoneracion, 
 CASE WHEN 
t1.U_FactorDeVenta IS NULL then 0 ELSE t1.U_FactorDeVenta END as FactorIVA
 from rin1 t0
   left join oitm t1 on t0.ItemCode = t1.ItemCode
   where t0.DocEntry = @reemplazo
   order by t0.LineNum
 ----------------------------------------------------DetNotaCredito-------------------------------------------------------
 select t0.U_TipoDoc as TipoDocumento, t0.U_Documento as NumeroDocumento, t0.U_Institucion as Emisora, t0.U_FeEmi as FechaEmision,
 t0.U_OSTACode as CodTarifa
 from [dbo].[@BYL_OEXC] t0
 where t0.DocEntry = 
 ------------------------------------------------------Exoneracion Documentos----------------------------------------

 INSERT INTO [ConectorFacturaElectronica].[dbo].[Impuestos]

 select  distinct top 2 t1.Code, '01', '01', t1.Rate, NULL, NULL 
 from [dbo].[@BYL_OEXC] t0
 inner join ostc t1 on t0.U_OSTACode = t1.Code
 ----------------------------------------------------INSERTAR IMPUESTOS DE EXONERACIONES------------------------------------------

 select * from NNM1 --Series
 
 
 
 
 -----------------------------------------SP--------------
 
 
 CREATE PROCEDURE [dbo].[SP_NotaCredito]
	-- Add the parameters for the stored procedure here
	@numFactura varchar(50),
	@Sucursal varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 DECLARE @URL NVARCHAR(MAX) =    CONCAT( CONCAT(   CONCAT(CONCAT('http://watickets.dydconsultorescr.com/conectorfejdsc/api/Documentos?DocNum=',@numFactura),'&ObjType=14') , '&CodSucursal=') , @Sucursal );
		Declare @Object as Int;
	Declare @ResponseText as Varchar(8000);

	Exec sp_OACreate 'MSXML2.XMLHTTP', @Object OUT;
	Exec sp_OAMethod @Object, 'open', NULL, 'get',
       @URL,
       'False'
	 

Exec sp_OAMethod @Object, 'send'
Exec sp_OAMethod @Object, 'responseText', @ResponseText OUTPUT
END


----------------------------------------------------------------------------

 CREATE PROCEDURE [dbo].[SP_FacturaElectronica_ND]
	-- Add the parameters for the stored procedure here
	@numFactura varchar(50), 
	@ND bit,
	@Sucursal varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 DECLARE @URL NVARCHAR(MAX) = CASE WHEN @ND = 1 THEN  CONCAT( CONCAT( CONCAT(CONCAT('http://watickets.dydconsultorescr.com/conectorfejdsc/api/Documentos?DocNum=',@numFactura),'&ND=true'), '&CodSucursal='),@Sucursal) ELSE CONCAT( CONCAT ( CONCAT('http://watickets.dydconsultorescr.com/conectorfejdsc/api/Documentos?DocNum=',@numFactura), '&CodSucursal='),@Sucursal) END;
		Declare @Object as Int;
	Declare @ResponseText as Varchar(8000);

	Exec sp_OACreate 'MSXML2.XMLHTTP', @Object OUT;
	Exec sp_OAMethod @Object, 'open', NULL, 'get',
       @URL,
       'False'
	 

Exec sp_OAMethod @Object, 'send'
Exec sp_OAMethod @Object, 'responseText', @ResponseText OUTPUT
END

-------------------------------------------------------------

 CREATE PROCEDURE [dbo].[SP_FacturaCompra]
	-- Add the parameters for the stored procedure here
	@numFactura varchar(50),
	@Sucursal varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 DECLARE @URL NVARCHAR(MAX) =   CONCAT( CONCAT (CONCAT('http://watickets.dydconsultorescr.com/conectorfejdsc/api/Compras?DocNum=',@numFactura), '&CodSucursal=' ), @Sucursal);
		Declare @Object as Int;
	Declare @ResponseText as Varchar(8000);

	Exec sp_OACreate 'MSXML2.XMLHTTP', @Object OUT;
	Exec sp_OAMethod @Object, 'open', NULL, 'get',
       @URL,
       'False'
	 

Exec sp_OAMethod @Object, 'send'
Exec sp_OAMethod @Object, 'responseText', @ResponseText OUTPUT
END
--------------------------------------------------------------

 CREATE PROCEDURE [dbo].[SP_ConsultaEstadoHacienda]
	-- Add the parameters for the stored procedure here
	 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 DECLARE @URL NVARCHAR(MAX) =    'http://watickets.dydconsultorescr.com/conectorfejdsc/api/Documentos/ConsultarEstado';
		Declare @Object as Int;
	Declare @ResponseText as Varchar(8000);

	Exec sp_OACreate 'MSXML2.XMLHTTP', @Object OUT;
	Exec sp_OAMethod @Object, 'open', NULL, 'get',
       @URL,
       'False'
	 

Exec sp_OAMethod @Object, 'send'
Exec sp_OAMethod @Object, 'responseText', @ResponseText OUTPUT
END
--------------------------------------------------------

CREATE VIEW [dbo].[VistaConectorFacturaElectronica]
AS
SELECT id, idSucursal, consecutivoSAP, consecutivoInterno, TipoDocumento, DocEntry, Fecha, CodActividadEconomica, CardCode, CardName, LicTradNum, Email, TipoIdentificacion, condicionVenta, plazoCredito, medioPago, montoOtrosCargos, 
                  moneda, tipoCambio, totalserviciogravado, totalservicioexento, totalservicioexonerado, totalmercaderiagravado, totalmercaderiaexonerado, totalmercaderiaexenta, totalgravado, totalexento, totalexonerado, totalventa, totaldescuentos, 
                  totalventaneta, totalimpuestos, totalivadevuelto, totalotroscargos, totalcomprobante, RefTipoDocumento, RefNumeroDocumento, RefFechaEmision, RefCodigo, RefRazon, procesadaHacienda, RespuestaHacienda, XMLFirmado, 
                  ClaveHacienda, ConsecutivoHacienda, ErrorCyber, code, JSON, sincronizadaSAP
FROM     ConectorFacturaElectronica.dbo.EncDocumento
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "EncDocumento (ConectorFacturaElectronica.dbo)"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 320
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VistaConectorFacturaElectronica'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VistaConectorFacturaElectronica'
GO



---------------------------------------------------------------------------------

Declare @docNum varchar(50) 
declare @serie varchar(5)
declare @clave varchar(55)
declare @consecutivo varchar(25)
declare @retransmitir varchar(2)

---------------------------------------

If @object_type in ('13','14', '18') and @transaction_type in ('A')  

Begin
	
	if(@object_type = '18')
	begin
		set @docNum = (Select DocNum from opch where DocEntry = @list_of_cols_val_tab_del)

		begin
		exec [dbo].[SP_FacturaCompra] @docNum, '001'
		end


		 waitfor delay'00:00:02';
		 
		set @clave = (Select t0.ClaveHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento = '08')
		set @consecutivo = (Select t0.ConsecutivoHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento = '08')
	 
		Begin
			 UPDATE OPCH 
			set U_DYD_DocKey = @clave,
				U_DYD_DocNum = @consecutivo
			 where DocEntry = @list_of_cols_val_tab_del

	 

		 ENd
		


	end

	if (@object_type = '13')
	begin
	set @docNum = (Select DocNum from oinv where DocEntry = @list_of_cols_val_tab_del)
	set @serie = (Select Series from oinv where DocEntry = @list_of_cols_val_tab_del)

		if(@serie = '83')
		begin --Nota de debito
			begin
				exec SP_FacturaElectronica_ND @docNum, 1, '001'
			end
			 	begin
				 waitfor delay'00:00:01';
		 
				set @clave = (Select t0.ClaveHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08')
				set @consecutivo = (Select t0.ConsecutivoHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08')
	 
				Begin
					 UPDATE OINV 
					set U_DYD_DocKey = @clave,
						 U_DYD_DocNum = @consecutivo
					 where DocEntry = @list_of_cols_val_tab_del

	 

				 ENd

			 
			end
		end
		else 
		begin -- Facturas, tiquetes, exportacion
			begin
				exec SP_FacturaElectronica_ND  @docNum,  0, '001'
			end
			begin
				 waitfor delay'00:00:01';
		 
				set @clave = (Select t0.ClaveHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08')
				set @consecutivo = (Select t0.ConsecutivoHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08')
	 
				Begin
					 UPDATE OINV 
					set U_DYD_DocKey = @clave,
						 U_DYD_DocNum = @consecutivo
					 where DocEntry = @list_of_cols_val_tab_del

	 

				 ENd

			 
			end
		end

	 
	end


	If (@object_type = '14') --NOta de credito
	begin

		begin
		 set @docNum = (Select DocNum from ORIN where DocEntry = @list_of_cols_val_tab_del)

		EXEC [dbo].[SP_NotaCredito]  @docNum,'001';
		end
			begin
				 waitfor delay'00:00:01';
		 
				set @clave = (Select t0.ClaveHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08')
				set @consecutivo = (Select t0.ConsecutivoHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08')
	 
				Begin
					 UPDATE ORIN 
					set U_DYD_DocKey = @clave,
						 U_DYD_DocNum = @consecutivo
					 where DocEntry = @list_of_cols_val_tab_del

	 

				 ENd

			 
			end

	end
	

End


--- RETRANSMITIR
If @object_type in ('13','14', '18') and @transaction_type in ('U')  

Begin
	
	if(@object_type = '18')
	begin
		set @docNum = (Select DocNum from opch where DocEntry = @list_of_cols_val_tab_del)
	    set @retransmitir = (Select U_DYD_Retransmitir from opch where DocEntry = @list_of_cols_val_tab_del)

		If(@retransmitir = 'Y')
		begin
			begin
			exec [dbo].[SP_FacturaCompra] @docNum, '001'
			end


			 waitfor delay'00:00:02';
		 
			set @clave = (Select top 1 t0.ClaveHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento = '08'  order by id desc)
			set @consecutivo = (Select top 1 t0.ConsecutivoHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento = '08'  order by id desc)
	 
			Begin
				 UPDATE OPCH 
				set U_DYD_DocKey = @clave,
						 U_DYD_DocNum = @consecutivo,
						 U_DYD_Retransmitir = 'N'
				 where DocEntry = @list_of_cols_val_tab_del

	 

			 ENd
		end

		
		


	end

	if (@object_type = '13')
	begin
	set @docNum = (Select DocNum from oinv where DocEntry = @list_of_cols_val_tab_del)
	set @serie = (Select Series from oinv where DocEntry = @list_of_cols_val_tab_del)
	set @retransmitir = (Select U_DYD_Retransmitir from oinv where DocEntry =  @list_of_cols_val_tab_del)
	If(@retransmitir = 'Y')
		begin

			if(@serie = '83')
			begin --Nota de debito
				begin
					exec SP_FacturaElectronica_ND @docNum, 1, '001'
				end
			 		begin
					 waitfor delay'00:00:01';
		 
					set @clave = (Select top 1 t0.ClaveHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08'  order by id desc)
					set @consecutivo = (Select top 1 t0.ConsecutivoHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08'  order by id desc)
	 
					Begin
						 UPDATE OINV 
						set U_DYD_DocKey = @clave,
						 U_DYD_DocNum = @consecutivo,
						 U_DYD_Retransmitir = 'N'
						 where DocEntry = @list_of_cols_val_tab_del

	 

					 ENd

			 
				end
			end
			else 
			begin -- Facturas, tiquetes, exportacion
				begin
					exec SP_FacturaElectronica_ND  @docNum,  0, '001'
				end
				begin
					 waitfor delay'00:00:01';
		 
					set @clave = (Select top 1 t0.ClaveHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08'  order by id desc)
					set @consecutivo = (Select top 1 t0.ConsecutivoHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08'  order by id desc)
	 
					Begin
						 UPDATE OINV 
						set U_DYD_DocKey = @clave,
						 U_DYD_DocNum = @consecutivo,
						 U_DYD_Retransmitir = 'N'
						 where DocEntry = @list_of_cols_val_tab_del

	 

					 ENd

			 
				end
			end
		end
	 
	end


	If (@object_type = '14') --NOta de credito
	begin

		 set @docNum = (Select DocNum from ORIN where DocEntry = @list_of_cols_val_tab_del)
		 set @retransmitir = (Select U_DYD_Retransmitir from ORIN where DocEntry = @list_of_cols_val_tab_del) 

		 If(@retransmitir = 'Y')
		begin
			begin
		

			EXEC [dbo].[SP_NotaCredito]  @docNum, '001';
			end
				begin
					 waitfor delay'00:00:01';
		 
					set @clave = (Select top 1 t0.ClaveHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08'  order by id desc)
					set @consecutivo = (Select top 1 t0.ConsecutivoHacienda from [dbo].[VistaConectorFacturaElectronica] t0 where t0.DocEntry = @list_of_cols_val_tab_del and t0.TipoDocumento <> '08' order by id desc)
	 
					Begin
						 UPDATE ORIN 
						set U_DYD_DocKey = @clave,
						 U_DYD_DocNum = @consecutivo,
						 U_DYD_Retransmitir = 'N',
						 U_DYD_Estado = 'P'

						 where DocEntry = @list_of_cols_val_tab_del

	 

					 ENd

			 
				end
			
			end
			
	end
	

End


---------------------------------------------
http://watickets.dydconsultorescr.com/conectorfejdsc/api/Documentos/Respuesta
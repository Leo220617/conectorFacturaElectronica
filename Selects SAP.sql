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
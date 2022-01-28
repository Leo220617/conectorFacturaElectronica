Declare @docentry int
Declare @docNum int

DECLARE mi_cursor CURSOR  
    FOR SELECT id FROM VistaConectorFacturaElectronica where code = 44 and TipoDocumento = '01'
OPEN mi_cursor  
FETCH NEXT FROM mi_cursor into @docentry

while @@FETCH_STATUS=0
begin

	set @docNum = (select consecutivoSAP from VistaConectorFacturaElectronica where id = @docentry)


	delete from  [CFH].[dbo].[DetDocumento]
	where idEncabezado = @docEntry

	delete from [CFH].[dbo].[EncDocumento]
	where id = @docentry

	--select @docentry
	Exec SP_FacturaElectronica_ND @docNum,0,'001';


	

	FETCH NEXT FROM mi_cursor into @docentry

	WAITFOR Delay '00:00:01'; 

end

CLOSE mi_cursor;  
DEALLOCATE mi_cursor;  
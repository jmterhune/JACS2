/*
    JACS2 post-deployment script - module version 00.00.03

    TARGET DATABASE: JACS2 (the "jacs" connection string), not the DotNetNuke
    database. See README.md in this folder.

    Safe to re-run: CREATE OR ALTER replaces the procedure with the same text.

    ------------------------------------------------------------------------
    Contents
      1. Widen tjc_jacs_get_udf_paged @sortOrder
    ------------------------------------------------------------------------
*/

/*==========================================================================
  1. Widen tjc_jacs_get_udf_paged @sortOrder
  --------------------------------------------------------------------------
  @sortOrder was nvarchar(20). The User Defined Fields page sorts its "Use in
  Attorney Scheduling" column by use_in_attorany_scheduling, which is 26
  characters: the name was cut to "use_in_attorany_sche" and the ORDER BY
  failed with "Invalid column name". Widened to nvarchar(128), the length of
  a SQL Server identifier. Nothing else about the procedure changes.

  The column name is concatenated into the ORDER BY; it only ever comes from
  UserDefinedFieldAPIController.GetSortColumn's fixed list, never from the
  request.
==========================================================================*/
CREATE OR ALTER PROCEDURE [dbo].[tjc_jacs_get_udf_paged]
	@courtId bigint,@searchTerm nvarchar(250),@rowOffset int, @pageSize int,@sortOrder nvarchar(128), @direction nvarchar(6)
AS

BEGIN
	DECLARE @sql nvarchar(MAX)='SELECT * from [user_defined_fields] where ([court_id] = @courtId)';
	DECLARE @ParameterDef nvarchar(MAX)

    SET @ParameterDef = '@courtId bigint,@searchTerm nvarchar(250),@rowOffset int,@pageSize int,@sortOrder nvarchar(128)';

	SET NOCOUNT ON;

	IF LEN(@searchTerm) > 0
		BEGIN
			SET	@sql = @sql +' AND (field_name like  +''%'' + TRIM(@searchTerm) +''%'' OR field_type like  +''%'' + TRIM(@searchTerm) +''%'')';
		END
	SET	@sql = @sql + ' ORDER BY ' + @sortOrder + ' ' +@direction;
	SET	@sql = @sql + ' OFFSET @rowOffset ROWS FETCH NEXT @pagesize ROWS ONLY';
    -- Execute parameratized SQL
	EXEC sp_Executesql  @sql,  @ParameterDef, @courtId=@courtId,@searchTerm=@searchTerm,@rowOffset=@rowOffset,@pageSize=@pageSize,@sortOrder=@sortOrder;
END
GO

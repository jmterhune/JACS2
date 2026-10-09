/*============================================================================
  fn_GetServiceLength  --  completed whole years of service as of @ReportDate

  Problem fixed: the previous version computed DATEDIFF(month, ...) / 12, which
  ignores the day of the month. In the anniversary month the month-difference
  is always an exact multiple of 12, so the function credited a full year as of
  the 1st of that month -- up to ~30 days before the actual anniversary date --
  over-counting by 1 for anyone not hired on the 1st.

  Two pieces of dead code were also removed:
    * "IF MONTH(@ServiceDate) = MONTH(@ReportDate) ... @MonthsOfService + 1":
      in that branch the value is already a multiple of 12, so +1 never changed
      the integer result.
    * @yearofService / @monthofService: declared and assigned, never used.

  New version: standard "completed years" (age-style) calculation that counts
  the anniversary by month AND day, so a year is credited only once the actual
  anniversary date is reached. Future-dated service dates yield a negative
  result and NULLs yield NULL -- both still excluded by the report procedure's
  "fn_GetServiceLength(...) >= 0" filter, exactly as before.

  Deployed to dbo on: local (dev), 10.212.72.62 (test), judmansql03 (prod).
============================================================================*/
USE [intranet.jud12.local];
GO

ALTER FUNCTION [dbo].[fn_GetServiceLength]
(
    @ServiceDate DateTime,
    @ReportDate  DateTime
)
RETURNS int
AS
BEGIN
    RETURN DATEDIFF(YEAR, @ServiceDate, @ReportDate)
         - CASE
               WHEN MONTH(@ServiceDate) > MONTH(@ReportDate)
                 OR (MONTH(@ServiceDate) = MONTH(@ReportDate) AND DAY(@ServiceDate) > DAY(@ReportDate))
               THEN 1 ELSE 0
           END;
END
GO

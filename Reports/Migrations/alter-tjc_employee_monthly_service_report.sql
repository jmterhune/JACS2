/*============================================================================
  tjc_employee_monthly_service_report  (Service / Employment Report SP)

  Change: include two groups that were previously filtered out of the report.

    1. Employees with LESS THAN ONE YEAR of service.
       The cutoff "fn_GetServiceLength(...) > 0" is relaxed to ">= 0", so
       employees who have started but not yet completed a full year (0 years)
       now appear. Negative values -- a service / hire date dated more than a
       year in the FUTURE relative to the report date, i.e. a data error --
       stay excluded.

    2. Employees with EmploymentType = 'OPS'.
       The "EmploymentType <> 'OPS'" exclusion is removed. Interns are still
       excluded ("EmploymentType <> 'Intern'" is retained).

  All four branches are updated (All-Year / single-month  x  Service / Hire).
  The All-Year branches still require isEmployee = 1 (unchanged); the
  single-month branches remain without that filter (unchanged).

  Later change -- service-award date: the All-Year branches (@Month = 0) now
  anchor the report date on JULY 1 of the selected year ('7/1/' + @year) rather
  than January 1, because July 1 is the service-award date. Service is therefore
  measured as of July 1, capturing everyone who reaches a milestone by the award
  date. Single-month branches still anchor on the 1st of the chosen month.

  Deployed to dbo on: local (dev), 10.212.72.62 (test), judmansql03 (prod).
============================================================================*/
USE [intranet.jud12.local];
GO

ALTER Procedure [dbo].[tjc_employee_monthly_service_report]
    @Month int, @dateType int, @year int
AS
    DECLARE @yearofService int
    DECLARE @ReportDate DateTime
    SET @yearofService = @year

    IF @Month = 0
    BEGIN
        IF @dateType = 1
        BEGIN
            SET @ReportDate = CONVERT(Datetime, '7/1/' + Convert(varchar(4), @yearofService))

            SELECT AgencyOfEmployment, FirstName, LastName, ServiceDate,
                   dbo.fn_GetServiceLength(ServiceDate, @ReportDate) AS YearsOfService
            FROM tjc_employee
            WHERE (IsActive = 1) AND (EmploymentType <> 'Intern') AND isEmployee = 1
                AND dbo.fn_GetServiceLength(ServiceDate, @ReportDate) >= 0
            ORDER BY LastName, FirstName
        END
        ELSE
        BEGIN
            SET @ReportDate = CONVERT(Datetime, '7/1/' + Convert(varchar(4), @yearofService))

            SELECT AgencyOfEmployment, FirstName, LastName, HireDate AS 'ServiceDate',
                   dbo.fn_GetServiceLength(HireDate, @ReportDate) AS YearsOfService
            FROM tjc_employee
            WHERE (IsActive = 1) AND (EmploymentType <> 'Intern') AND isEmployee = 1
                AND dbo.fn_GetServiceLength(HireDate, @ReportDate) >= 0
            ORDER BY LastName, FirstName
        END
    END
    ELSE
    BEGIN
        IF @dateType = 1
        BEGIN
            SET @ReportDate = CONVERT(Datetime, Convert(varchar(2), @Month) + '/1/' + Convert(varchar(4), @yearofService))

            SELECT AgencyOfEmployment, FirstName, LastName, ServiceDate,
                   dbo.fn_GetServiceLength(ServiceDate, @ReportDate) AS YearsOfService
            FROM tjc_employee
            WHERE (IsActive = 1) AND (EmploymentType <> 'Intern')
                AND dbo.fn_GetServiceLength(ServiceDate, @ReportDate) >= 0 AND MONTH(serviceDate) = @Month
            ORDER BY LastName, FirstName
        END
        ELSE
        BEGIN
            SET @ReportDate = CONVERT(Datetime, Convert(varchar(2), @Month) + '/1/' + Convert(varchar(4), @yearofService))

            SELECT AgencyOfEmployment, FirstName, LastName, hireDate AS 'ServiceDate',
                   dbo.fn_GetServiceLength(hireDate, @ReportDate) AS YearsOfService
            FROM tjc_employee
            WHERE (IsActive = 1) AND (EmploymentType <> 'Intern')
                AND dbo.fn_GetServiceLength(hireDate, @ReportDate) >= 0 AND MONTH(hireDate) = @Month
            ORDER BY LastName, FirstName
        END
    END
GO

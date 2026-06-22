/*
 * Format existing phone numbers in MediationStatistics tables.
 *
 * For each non-empty Phone value in tjc_med_mediators and tjc_med_attorneys:
 *   1. Strip every non-digit character.
 *   2. If the cleaned value has exactly 10 digits, re-format as "(XXX) XXX-XXXX".
 *   3. If the cleaned value has 11 digits and starts with '1', drop the '1' and
 *      format the remaining 10 digits as "(XXX) XXX-XXXX".
 *   4. Otherwise, store just the cleaned digits (no mask).
 *
 * Empty / NULL phones are left untouched.
 * The Attorney.Extension column is NOT modified.
 *
 * The script wraps both updates in a single transaction and prints a preview
 * before committing. Review the preview rows, then run COMMIT TRAN manually
 * (or ROLLBACK if you don't like what you see).
 */

SET NOCOUNT ON;

BEGIN TRAN PhoneFormat;

------------------------------------------------------------------
-- 1) tjc_med_mediators
------------------------------------------------------------------
;WITH Tally(n) AS (
    SELECT TOP (50) ROW_NUMBER() OVER (ORDER BY (SELECT NULL))
    FROM sys.all_objects
),
Cleaned AS (
    SELECT
        m.MediatorId,
        DigitsOnly = STRING_AGG(SUBSTRING(m.Phone, t.n, 1), '')
                     WITHIN GROUP (ORDER BY t.n)
    FROM tjc_med_mediators m
    INNER JOIN Tally t ON t.n <= LEN(m.Phone)
    WHERE m.Phone IS NOT NULL
      AND m.Phone <> ''
      AND SUBSTRING(m.Phone, t.n, 1) BETWEEN '0' AND '9'
    GROUP BY m.MediatorId
)
UPDATE m
SET m.Phone =
    CASE
        WHEN LEN(c.DigitsOnly) = 10 THEN
            '(' + SUBSTRING(c.DigitsOnly, 1, 3) + ') ' +
                  SUBSTRING(c.DigitsOnly, 4, 3) + '-' +
                  SUBSTRING(c.DigitsOnly, 7, 4)
        WHEN LEN(c.DigitsOnly) = 11 AND LEFT(c.DigitsOnly, 1) = '1' THEN
            '(' + SUBSTRING(c.DigitsOnly, 2, 3) + ') ' +
                  SUBSTRING(c.DigitsOnly, 5, 3) + '-' +
                  SUBSTRING(c.DigitsOnly, 8, 4)
        ELSE c.DigitsOnly
    END
FROM tjc_med_mediators m
INNER JOIN Cleaned c ON c.MediatorId = m.MediatorId;

PRINT CONCAT('tjc_med_mediators: ', @@ROWCOUNT, ' row(s) updated.');

------------------------------------------------------------------
-- 2) tjc_med_attorneys
------------------------------------------------------------------
;WITH Tally(n) AS (
    SELECT TOP (50) ROW_NUMBER() OVER (ORDER BY (SELECT NULL))
    FROM sys.all_objects
),
Cleaned AS (
    SELECT
        a.AttorneyId,
        DigitsOnly = STRING_AGG(SUBSTRING(a.Phone, t.n, 1), '')
                     WITHIN GROUP (ORDER BY t.n)
    FROM tjc_med_attorneys a
    INNER JOIN Tally t ON t.n <= LEN(a.Phone)
    WHERE a.Phone IS NOT NULL
      AND a.Phone <> ''
      AND SUBSTRING(a.Phone, t.n, 1) BETWEEN '0' AND '9'
    GROUP BY a.AttorneyId
)
UPDATE a
SET a.Phone =
    CASE
        WHEN LEN(c.DigitsOnly) = 10 THEN
            '(' + SUBSTRING(c.DigitsOnly, 1, 3) + ') ' +
                  SUBSTRING(c.DigitsOnly, 4, 3) + '-' +
                  SUBSTRING(c.DigitsOnly, 7, 4)
        WHEN LEN(c.DigitsOnly) = 11 AND LEFT(c.DigitsOnly, 1) = '1' THEN
            '(' + SUBSTRING(c.DigitsOnly, 2, 3) + ') ' +
                  SUBSTRING(c.DigitsOnly, 5, 3) + '-' +
                  SUBSTRING(c.DigitsOnly, 8, 4)
        ELSE c.DigitsOnly
    END
FROM tjc_med_attorneys a
INNER JOIN Cleaned c ON c.AttorneyId = a.AttorneyId;

PRINT CONCAT('tjc_med_attorneys: ', @@ROWCOUNT, ' row(s) updated.');

------------------------------------------------------------------
-- 3) Preview - confirm the results look right
------------------------------------------------------------------
PRINT '----- tjc_med_mediators preview (first 25 non-empty) -----';
SELECT TOP (25) MediatorId, FirstName, LastName, Phone, Email
FROM tjc_med_mediators
WHERE Phone IS NOT NULL AND Phone <> ''
ORDER BY MediatorId;

PRINT '----- tjc_med_attorneys preview (first 25 non-empty) -----';
SELECT TOP (25) AttorneyId, FirstName, LastName, Phone, Extension, Email
FROM tjc_med_attorneys
WHERE Phone IS NOT NULL AND Phone <> ''
ORDER BY AttorneyId;

------------------------------------------------------------------
-- 4) Confirm / discard
------------------------------------------------------------------
-- If the preview looks correct, run:
--   COMMIT TRAN PhoneFormat;
--
-- If it doesn't, run:
--   ROLLBACK TRAN PhoneFormat;

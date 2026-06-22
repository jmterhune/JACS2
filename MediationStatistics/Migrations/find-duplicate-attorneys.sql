/*
 * Find duplicate attorney rows in tjc_med_attorneys (same FirstName + LastName)
 * and classify each group as:
 *   - SAFE        : every non-empty value in each field agrees across the group
 *                   (so the only differences are NULL/empty vs a value) — safe to merge
 *   - NEEDS REVIEW: some field has contradicting non-empty values (e.g., different Firms,
 *                   different emails) — these MIGHT actually be different attorneys
 *
 * Also reports how many tjc_med_sessions rows each duplicate attorney is referenced by
 * (via p1_AttorneyId or p2_AttorneyId), and computes a "winner" candidate for SAFE groups
 * (most session uses; tiebreak: most non-empty fields; final tiebreak: lowest AttorneyId).
 *
 * Read-only — no UPDATE or DELETE here.
 */

SET NOCOUNT ON;

------------------------------------------------------------------
-- 1) Name-duplicate groups (overview)
------------------------------------------------------------------
PRINT '----- duplicate name groups (FirstName + LastName) -----';
SELECT FirstName, LastName, COUNT(*) AS DuplicateRowCount
FROM tjc_med_attorneys
GROUP BY FirstName, LastName
HAVING COUNT(*) > 1
ORDER BY DuplicateRowCount DESC, LastName, FirstName;

------------------------------------------------------------------
-- 2) Classification + row-level detail + session usage
------------------------------------------------------------------
;WITH NameGroups AS (
    SELECT FirstName, LastName, COUNT(*) AS GrpSize
    FROM tjc_med_attorneys
    GROUP BY FirstName, LastName
    HAVING COUNT(*) > 1
),
-- For each duplicate group, count how many DISTINCT non-empty values appear in each field.
-- If any field has > 1 distinct non-empty value, the group has a real contradiction.
GroupConflicts AS (
    SELECT
        a.FirstName, a.LastName,
        DistFirm      = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(LOWER(ISNULL(a.Firm,      '')))), '')),
        DistEmail     = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(LOWER(ISNULL(a.Email,     '')))), '')),
        DistPhone     = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(      ISNULL(a.Phone,     ''))),  '')),
        DistExtension = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(      ISNULL(a.Extension, ''))),  '')),
        DistAddress   = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(LOWER(ISNULL(a.Address,   '')))), '')),
        DistCity      = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(LOWER(ISNULL(a.City,      '')))), '')),
        DistState     = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(LOWER(ISNULL(a.State,     '')))), '')),
        DistZip       = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(      ISNULL(a.Zip,       ''))),  ''))
    FROM tjc_med_attorneys a
    INNER JOIN NameGroups ng ON ng.FirstName = a.FirstName AND ng.LastName = a.LastName
    GROUP BY a.FirstName, a.LastName
),
Classified AS (
    SELECT
        gc.*,
        Classification = CASE
            WHEN gc.DistFirm > 1 OR gc.DistEmail > 1 OR gc.DistPhone > 1 OR gc.DistExtension > 1
              OR gc.DistAddress > 1 OR gc.DistCity > 1 OR gc.DistState > 1 OR gc.DistZip > 1
            THEN 'NEEDS REVIEW'
            ELSE 'SAFE'
        END
    FROM GroupConflicts gc
),
Usage AS (
    SELECT AttorneyId, SUM(c) AS UseCount
    FROM (
        SELECT p1_AttorneyId AS AttorneyId, COUNT(*) AS c
          FROM tjc_med_sessions
         WHERE p1_AttorneyId IS NOT NULL
         GROUP BY p1_AttorneyId
        UNION ALL
        SELECT p2_AttorneyId AS AttorneyId, COUNT(*) AS c
          FROM tjc_med_sessions
         WHERE p2_AttorneyId IS NOT NULL
         GROUP BY p2_AttorneyId
    ) u
    GROUP BY AttorneyId
),
DataFillScore AS (
    SELECT
        a.AttorneyId,
        Score =
            CASE WHEN LTRIM(RTRIM(ISNULL(a.Firm,     ''))) <> '' THEN 1 ELSE 0 END +
            CASE WHEN LTRIM(RTRIM(ISNULL(a.Email,    ''))) <> '' THEN 1 ELSE 0 END +
            CASE WHEN LTRIM(RTRIM(ISNULL(a.Phone,    ''))) <> '' THEN 1 ELSE 0 END +
            CASE WHEN LTRIM(RTRIM(ISNULL(a.Extension,''))) <> '' THEN 1 ELSE 0 END +
            CASE WHEN LTRIM(RTRIM(ISNULL(a.Address,  ''))) <> '' THEN 1 ELSE 0 END +
            CASE WHEN LTRIM(RTRIM(ISNULL(a.City,     ''))) <> '' THEN 1 ELSE 0 END +
            CASE WHEN LTRIM(RTRIM(ISNULL(a.State,    ''))) <> '' THEN 1 ELSE 0 END +
            CASE WHEN LTRIM(RTRIM(ISNULL(a.Zip,      ''))) <> '' THEN 1 ELSE 0 END
    FROM tjc_med_attorneys a
),
-- For each SAFE group, rank rows so #1 = the winner (most uses, then most data, then lowest Id).
SafeRanked AS (
    SELECT
        a.AttorneyId, a.FirstName, a.LastName, a.Firm, a.Email, a.Phone,
        a.Extension, a.Address, a.City, a.State, a.Zip,
        UseCount = ISNULL(u.UseCount, 0),
        DataScore = d.Score,
        Rk = ROW_NUMBER() OVER (
            PARTITION BY a.FirstName, a.LastName
            ORDER BY ISNULL(u.UseCount, 0) DESC, d.Score DESC, a.AttorneyId ASC
        )
    FROM tjc_med_attorneys a
    INNER JOIN Classified c ON c.FirstName = a.FirstName AND c.LastName = a.LastName
    LEFT  JOIN Usage u ON u.AttorneyId = a.AttorneyId
    LEFT  JOIN DataFillScore d ON d.AttorneyId = a.AttorneyId
    WHERE c.Classification = 'SAFE'
)
SELECT
    c.Classification,
    a.AttorneyId,
    a.FirstName,
    a.LastName,
    a.Firm,
    a.Email,
    a.Phone,
    a.Extension,
    a.City,
    a.State,
    a.Zip,
    UseCount  = ISNULL(u.UseCount, 0),
    DataScore = d.Score,
    [Action] = CASE
        WHEN c.Classification = 'NEEDS REVIEW' THEN 'review'
        WHEN sr.Rk = 1 THEN 'keep'
        ELSE 'merge into ' + CAST(winner.AttorneyId AS NVARCHAR(20))
    END
FROM tjc_med_attorneys a
INNER JOIN Classified c ON c.FirstName = a.FirstName AND c.LastName = a.LastName
LEFT  JOIN Usage u ON u.AttorneyId = a.AttorneyId
LEFT  JOIN DataFillScore d ON d.AttorneyId = a.AttorneyId
LEFT  JOIN SafeRanked sr ON sr.AttorneyId = a.AttorneyId
LEFT  JOIN SafeRanked winner ON winner.FirstName = a.FirstName AND winner.LastName = a.LastName AND winner.Rk = 1
ORDER BY c.Classification, a.LastName, a.FirstName, ISNULL(u.UseCount, 0) DESC, a.AttorneyId;

------------------------------------------------------------------
-- 3) NEEDS REVIEW summary - the groups that look like potentially-different attorneys
------------------------------------------------------------------
PRINT '';
PRINT '----- NEEDS REVIEW: duplicates with conflicting field values -----';

;WITH NameGroups AS (
    SELECT FirstName, LastName
    FROM tjc_med_attorneys
    GROUP BY FirstName, LastName
    HAVING COUNT(*) > 1
),
GroupConflicts AS (
    SELECT
        a.FirstName, a.LastName,
        DistFirm      = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(LOWER(ISNULL(a.Firm,      '')))), '')),
        DistEmail     = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(LOWER(ISNULL(a.Email,     '')))), '')),
        DistPhone     = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(      ISNULL(a.Phone,     ''))),  '')),
        DistExtension = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(      ISNULL(a.Extension, ''))),  '')),
        DistAddress   = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(LOWER(ISNULL(a.Address,   '')))), '')),
        DistCity      = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(LOWER(ISNULL(a.City,      '')))), '')),
        DistState     = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(LOWER(ISNULL(a.State,     '')))), '')),
        DistZip       = COUNT(DISTINCT NULLIF(LTRIM(RTRIM(      ISNULL(a.Zip,       ''))),  ''))
    FROM tjc_med_attorneys a
    INNER JOIN NameGroups ng ON ng.FirstName = a.FirstName AND ng.LastName = a.LastName
    GROUP BY a.FirstName, a.LastName
)
SELECT a.AttorneyId, a.FirstName, a.LastName, a.Firm, a.Email, a.Phone, a.Extension, a.Address, a.City, a.State, a.Zip
FROM tjc_med_attorneys a
INNER JOIN GroupConflicts gc ON gc.FirstName = a.FirstName AND gc.LastName = a.LastName
WHERE gc.DistFirm > 1 OR gc.DistEmail > 1 OR gc.DistPhone > 1 OR gc.DistExtension > 1
   OR gc.DistAddress > 1 OR gc.DistCity > 1 OR gc.DistState > 1 OR gc.DistZip > 1
ORDER BY a.LastName, a.FirstName, a.AttorneyId;

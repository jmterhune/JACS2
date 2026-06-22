/*
 * Dedupe tjc_med_attorneys rows that share the same FirstName + LastName AND have no
 * contradicting non-empty field values ("SAFE" groups per find-duplicate-attorneys.sql).
 *
 * For each SAFE group:
 *   1. Pick the winner = the row with the most tjc_med_sessions references
 *      (sum of p1_AttorneyId + p2_AttorneyId occurrences). Ties break by most
 *      non-empty data fields, then by lowest AttorneyId.
 *   2. UPDATE tjc_med_sessions so any session that referenced a loser now references the winner.
 *   3. DELETE the loser rows from tjc_med_attorneys.
 *
 * Rows in NEEDS REVIEW groups (conflicting Firm/Phone/Email/Address/etc.) are not touched.
 *
 * The whole thing runs in a transaction with preview SELECTs at the end.
 * Review the preview, then run COMMIT or ROLLBACK manually.
 */

SET NOCOUNT ON;

BEGIN TRAN AttorneyDedupe;

------------------------------------------------------------------
-- 1) Build the winner/loser map for SAFE duplicate groups
------------------------------------------------------------------
IF OBJECT_ID('tempdb..#WinnerMap') IS NOT NULL DROP TABLE #WinnerMap;

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
),
SafeGroups AS (
    SELECT FirstName, LastName
    FROM GroupConflicts
    WHERE DistFirm <= 1 AND DistEmail <= 1 AND DistPhone <= 1 AND DistExtension <= 1
      AND DistAddress <= 1 AND DistCity <= 1 AND DistState <= 1 AND DistZip <= 1
),
Usage AS (
    SELECT AttorneyId, SUM(c) AS UseCount
    FROM (
        SELECT p1_AttorneyId AS AttorneyId, COUNT(*) AS c FROM tjc_med_sessions WHERE p1_AttorneyId IS NOT NULL GROUP BY p1_AttorneyId
        UNION ALL
        SELECT p2_AttorneyId AS AttorneyId, COUNT(*) AS c FROM tjc_med_sessions WHERE p2_AttorneyId IS NOT NULL GROUP BY p2_AttorneyId
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
Ranked AS (
    SELECT
        a.AttorneyId, a.FirstName, a.LastName,
        UseCount  = ISNULL(u.UseCount, 0),
        DataScore = d.Score,
        Rk = ROW_NUMBER() OVER (
            PARTITION BY a.FirstName, a.LastName
            ORDER BY ISNULL(u.UseCount, 0) DESC, d.Score DESC, a.AttorneyId ASC
        )
    FROM tjc_med_attorneys a
    INNER JOIN SafeGroups sg ON sg.FirstName = a.FirstName AND sg.LastName = a.LastName
    LEFT  JOIN Usage u         ON u.AttorneyId = a.AttorneyId
    LEFT  JOIN DataFillScore d ON d.AttorneyId = a.AttorneyId
)
SELECT
    r.FirstName, r.LastName,
    r.AttorneyId,
    LoserId  = r.AttorneyId,
    WinnerId = w.AttorneyId,
    LoserUses = r.UseCount,
    WinnerUses = w.UseCount
INTO #WinnerMap
FROM Ranked r
INNER JOIN Ranked w ON w.FirstName = r.FirstName AND w.LastName = r.LastName AND w.Rk = 1
WHERE r.Rk > 1;   -- only the losers

PRINT CONCAT('Loser rows queued for dedupe: ', (SELECT COUNT(*) FROM #WinnerMap));

------------------------------------------------------------------
-- 2) Redirect tjc_med_sessions to the winners
--    (each loser may appear in p1_AttorneyId, p2_AttorneyId, or both)
------------------------------------------------------------------
UPDATE s
SET s.p1_AttorneyId = wm.WinnerId
FROM tjc_med_sessions s
INNER JOIN #WinnerMap wm ON wm.LoserId = s.p1_AttorneyId;

PRINT CONCAT('tjc_med_sessions.p1_AttorneyId redirected: ', @@ROWCOUNT);

UPDATE s
SET s.p2_AttorneyId = wm.WinnerId
FROM tjc_med_sessions s
INNER JOIN #WinnerMap wm ON wm.LoserId = s.p2_AttorneyId;

PRINT CONCAT('tjc_med_sessions.p2_AttorneyId redirected: ', @@ROWCOUNT);

------------------------------------------------------------------
-- 3) Delete the loser attorney rows
------------------------------------------------------------------
DELETE a
FROM tjc_med_attorneys a
INNER JOIN #WinnerMap wm ON wm.LoserId = a.AttorneyId;

PRINT CONCAT('tjc_med_attorneys rows deleted: ', @@ROWCOUNT);

------------------------------------------------------------------
-- 4) Preview - what just happened
------------------------------------------------------------------
PRINT '';
PRINT '----- winner/loser map (still in transaction) -----';
SELECT * FROM #WinnerMap ORDER BY LastName, FirstName, LoserId;

PRINT '----- sessions that were redirected (sanity check - winners should now own them) -----';
SELECT TOP (50) s.SessionId, s.p1_AttorneyId, s.p2_AttorneyId
FROM tjc_med_sessions s
WHERE s.p1_AttorneyId IN (SELECT WinnerId FROM #WinnerMap)
   OR s.p2_AttorneyId IN (SELECT WinnerId FROM #WinnerMap)
ORDER BY s.SessionId;

PRINT '----- remaining duplicate name groups (should ONLY be NEEDS REVIEW after this runs) -----';
SELECT FirstName, LastName, COUNT(*) AS RemainingRows
FROM tjc_med_attorneys
GROUP BY FirstName, LastName
HAVING COUNT(*) > 1
ORDER BY LastName, FirstName;

------------------------------------------------------------------
-- 5) Confirm / discard
------------------------------------------------------------------
-- If the preview is correct:
--   COMMIT TRAN AttorneyDedupe;
-- If not:
--   ROLLBACK TRAN AttorneyDedupe;

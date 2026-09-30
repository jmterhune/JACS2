/*
    JACS2 post-deployment script - module version 00.00.02

    TARGET DATABASE: JACS2 (the "jacs" connection string), not the DotNetNuke
    database. See README.md in this folder.

    Safe to re-run: every statement checks for what it is about to create.

    ------------------------------------------------------------------------
    Contents
      1. Attorney type-ahead indexes
      2. Timeslot / court join indexes
      3. Event join indexes
    ------------------------------------------------------------------------
*/

/*==========================================================================
  1. Attorney type-ahead
  --------------------------------------------------------------------------
  attorneys holds the entire Florida Bar (~106,000 rows) and had only its
  clustered primary key on id, so every dropdown search scanned the table. A
  two-character term matched over 10,000 rows, all of which were materialised
  and serialised; the browser cancelled the request on the next keystroke,
  surfacing as "Failed to fetch attorney dropdown entries".

  AttorneyController now caps a search at 50 rows ordered by name and searches
  one column rather than both with an OR -- a term with letters cannot match a
  numeric bar number, and the OR stopped either index being used.

    IX_attorneys_name     name is matched with a leading wildcard, which cannot
                          seek, but this index is narrow and already in name
                          order, so ORDER BY is free and the engine stops once
                          it has 50 rows.
    IX_attorneys_bar_num  bar numbers are matched as a prefix, so this seeks.

  Measured, ~106k rows, best of three, capped query, local database:

    term     before    after
    jo       180 ms     1 ms
    jon      185 ms     4 ms
    jame     187 ms     2 ms
    smith    204 ms    60 ms
    38946    146 ms     0 ms
==========================================================================*/

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_attorneys_name' AND object_id = OBJECT_ID('attorneys'))
    CREATE NONCLUSTERED INDEX IX_attorneys_name ON attorneys(name) INCLUDE (bar_num);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_attorneys_bar_num' AND object_id = OBJECT_ID('attorneys'))
    CREATE NONCLUSTERED INDEX IX_attorneys_bar_num ON attorneys(bar_num) INCLUDE (name);
GO

/*==========================================================================
  2. Timeslot / court joins
  --------------------------------------------------------------------------
  court_timeslots (~30,900 rows) and timeslots (~30,900 rows) carry foreign
  keys with no supporting index, so each join scanned the whole table.

  court_timeslots.timeslot_id is the hottest of these: EventController's
  GetCourtIdByEventId joins timeslot_events to court_timeslots on it, and that
  runs for every hearing notification. court_id backs GetEventsByCourtId and
  the calendar loads.
==========================================================================*/

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_court_timeslots_timeslot_id' AND object_id = OBJECT_ID('court_timeslots'))
    CREATE NONCLUSTERED INDEX IX_court_timeslots_timeslot_id ON court_timeslots(timeslot_id) INCLUDE (court_id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_court_timeslots_court_id' AND object_id = OBJECT_ID('court_timeslots'))
    CREATE NONCLUSTERED INDEX IX_court_timeslots_court_id ON court_timeslots(court_id) INCLUDE (timeslot_id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_timeslots_courtroom_id' AND object_id = OBJECT_ID('timeslots'))
    CREATE NONCLUSTERED INDEX IX_timeslots_courtroom_id ON timeslots(courtroom_id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_timeslots_template_id' AND object_id = OBJECT_ID('timeslots'))
    CREATE NONCLUSTERED INDEX IX_timeslots_template_id ON timeslots(template_id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_template_timeslots_court_template_id' AND object_id = OBJECT_ID('template_timeslots'))
    CREATE NONCLUSTERED INDEX IX_template_timeslots_court_template_id ON template_timeslots(court_template_id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_court_motions_court_id' AND object_id = OBJECT_ID('court_motions'))
    CREATE NONCLUSTERED INDEX IX_court_motions_court_id ON court_motions(court_id) INCLUDE (motion_id, allowed);
GO

/*==========================================================================
  3. Event joins
  --------------------------------------------------------------------------
  timeslot_events and events are empty at the time of writing -- the events
  were cleared while moving the counties from the mock API to the real clerk
  API -- but they are on the path of nearly every lookup once hearings exist
  again, and creating an index on an empty table is instant. Adding them now
  means the first busy day does not scan.
==========================================================================*/

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_timeslot_events_event_id' AND object_id = OBJECT_ID('timeslot_events'))
    CREATE NONCLUSTERED INDEX IX_timeslot_events_event_id ON timeslot_events(event_id) INCLUDE (timeslot_id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_timeslot_events_timeslot_id' AND object_id = OBJECT_ID('timeslot_events'))
    CREATE NONCLUSTERED INDEX IX_timeslot_events_timeslot_id ON timeslot_events(timeslot_id) INCLUDE (event_id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_events_attorney_id' AND object_id = OBJECT_ID('events'))
    CREATE NONCLUSTERED INDEX IX_events_attorney_id ON events(attorney_id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_events_opp_attorney_id' AND object_id = OBJECT_ID('events'))
    CREATE NONCLUSTERED INDEX IX_events_opp_attorney_id ON events(opp_attorney_id);
GO

/*
    Rollback -- dropping any of these is not destructive, the affected queries
    simply return to scanning:

        DROP INDEX IX_attorneys_name ON attorneys;
        DROP INDEX IX_attorneys_bar_num ON attorneys;
        DROP INDEX IX_court_timeslots_timeslot_id ON court_timeslots;
        DROP INDEX IX_court_timeslots_court_id ON court_timeslots;
        DROP INDEX IX_timeslots_courtroom_id ON timeslots;
        DROP INDEX IX_timeslots_template_id ON timeslots;
        DROP INDEX IX_template_timeslots_court_template_id ON template_timeslots;
        DROP INDEX IX_court_motions_court_id ON court_motions;
        DROP INDEX IX_timeslot_events_event_id ON timeslot_events;
        DROP INDEX IX_timeslot_events_timeslot_id ON timeslot_events;
        DROP INDEX IX_events_attorney_id ON events;
        DROP INDEX IX_events_opp_attorney_id ON events;

    Tables under ~100 rows (courts, judges, court_event_types and similar) are
    deliberately left alone: scanning twenty rows costs nothing and an index
    there is pure write overhead.
*/

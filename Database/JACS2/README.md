# JACS2 post-deployment scripts

Scripts in this folder run against the **JACS2 database** — the one reached through
the `jacs` connection string — after the module has been deployed.

They are deliberately kept out of `Providers\DataProviders\SqlDataProvider\`. DNN
executes `.SqlDataProvider` files against the DotNetNuke database via
`SiteSqlServer`, and JACS2 is a completely separate database that DNN knows nothing
about. A script placed there referencing `attorneys` or `timeslots` would run in the
wrong database and fail the install.

## Running them

Run the script whose name matches the module version being deployed, against JACS2,
in each environment (dev, test, and any new one):

```
sqlcmd -S <server> -d JACS2 -i 00.00.02.sql
```

Every script is written to be safe to re-run — each statement checks for what it is
about to create — so running one twice, or running an older one again, does nothing.

## Naming

`<module version>.sql`, matching the `version` attribute of the `<package>` element
in `JACS.dnn`. A deployment runs every script from the version after the one already
applied through the version being installed.

| Script | Applies to | Summary |
| --- | --- | --- |
| `00.00.02.sql` | module 00.00.02 | Indexes for the attorney type-ahead and the timeslot/court join paths |

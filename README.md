# ElronAPI
This is a web API project that serves train times and Elron account info.

Mainly used by my Android app `Eesti rongiajad`

# GTFS
GTFS data is pulled from https://peatus.ee/gtfs.

An automated import script is located in `gtfs/`

As of 2018-11-10 The import is automated with Hangfire jobs

# Database Scaffolding
Modify the connectionstring if needed:

`Scaffold-DbContext "User ID=postgres;Password=<password>;Host=localhost;Port=5431;Database=peatus;" "Npgsql.EntityFrameworkCore.PostgreSQL" -Context PeatusContext -o ./Data/ -f`

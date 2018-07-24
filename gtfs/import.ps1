if(Test-Path .\gtfs.zip){
    Remove-Item .\gtfs.zip
}
if(Test-Path .\gtfs){
    Remove-Item .\gtfs -Recurse
}
Invoke-WebRequest http://peatus.ee/gtfs/gtfs.zip -OutFile gtfs.zip
Expand-Archive gtfs.zip

Invoke-Expression "icacls gtfs /grant ""Everyone:(OI)(CI)R"""

$command = @"
psql -U postgres -d peatus -c "
truncate stop_times cascade;
truncate trips cascade;
truncate routes cascade;
truncate stops cascade;
truncate agencies cascade;
truncate calendar cascade;
copy agencies from '$PWD/gtfs/agency.txt' delimiter ',' ENCODING 'unicode' CSV HEADER;
copy routes from '$PWD/gtfs/routes.txt' delimiter ',' ENCODING 'unicode' CSV HEADER;
copy stops from '$PWD/gtfs/stops.txt' delimiter ',' ENCODING 'unicode' CSV HEADER;
copy trips from '$PWD/gtfs/trips.txt' delimiter ',' ENCODING 'unicode' CSV HEADER;
copy stop_times from '$PWD/gtfs/stop_times.txt' delimiter ',' ENCODING 'unicode' CSV HEADER;
copy calendar from '$PWD/gtfs/calendar.txt' delimiter ',' ENCODING 'unicode' CSV HEADER;
insert into public.""import_logs""(""import_date"") values(now());"
"@

Invoke-Expression $command
CREATE TABLE public.agencies (
	agency_id int8 NOT NULL,
	agency_name varchar(255) NULL,
	agency_url varchar(255) NULL,
	agency_timezone varchar(255) NULL,
	agency_phone varchar(255) NULL,
	agency_lang varchar(255) NULL,
	CONSTRAINT agencies_pkey PRIMARY KEY (agency_id)
)
WITH (
	OIDS=FALSE
);

CREATE TABLE public.routes (
	route_id varchar(32) NOT NULL,
	agency_id int8 NULL,
	route_short_name varchar(16) NULL,
	route_long_name varchar(256) NULL,
	route_type int4 NULL,
	route_color varchar(16) NULL,
	competent_authority varchar(32) NULL,
	CONSTRAINT routes_pkey PRIMARY KEY (route_id),
	CONSTRAINT routes_agencies_fkey FOREIGN KEY (agency_id) REFERENCES public.agencies(agency_id)
)
WITH (
	OIDS=FALSE
);

CREATE TABLE public.trips (
	route_id varchar(32) NOT NULL,
	service_id int4 NULL,
	trip_id int8 NOT NULL,
	trip_headsign varchar(64) NULL,
	trip_long_name varchar(256) NULL,
	direction_code varchar(32) NULL,
	wheelchair_accessible int4 NULL,
	shape_id int4 NULL,
	CONSTRAINT trips_pkey PRIMARY KEY (trip_id),
	CONSTRAINT trips_routes_fkey FOREIGN KEY (route_id) REFERENCES public.routes(route_id)
)
WITH (
	OIDS=FALSE
);

CREATE TABLE public.stops (
	stop_id int4 NOT NULL,
	stop_code varchar(16) NULL,
	stop_name varchar(64) NULL,
	stop_lat float8 NULL,
	stop_lon float8 NULL,
	zone_id int4 NULL,
	alias varchar(32) NULL,
	stop_area varchar(32) NULL,
	stop_desc varchar(64) NULL,
	lest_x numeric(19) NULL,
	lest_y numeric(19) NULL,
	zone_name varchar(100) NULL,
	CONSTRAINT stops_pkey PRIMARY KEY (stop_id)
)
WITH (
	OIDS=FALSE
);

CREATE TABLE public.stop_times (
	trip_id int8 NOT NULL,
	arrival_time varchar(16) NULL,
	departure_time varchar(16) NULL,
	stop_id int4 NOT NULL,
	stop_sequence int4 NOT NULL,
	pickup_type int4 NULL,
	drop_off_type int4 NULL,
	CONSTRAINT stop_times_pkey PRIMARY KEY (trip_id,stop_id,stop_sequence),
	CONSTRAINT stop_times_stops_fkey FOREIGN KEY (stop_id) REFERENCES public.stops(stop_id),
	CONSTRAINT stop_times_trips_fkey FOREIGN KEY (trip_id) REFERENCES public.trips(trip_id)
)
WITH (
	OIDS=FALSE
);


CREATE TABLE public.calendar (
	service_id int4 NOT NULL,
	monday bool NOT NULL,
	tuesday bool NOT NULL,
	wednesday bool NOT NULL,
	thursday bool NOT NULL,
	friday bool NOT NULL,
	saturday bool NOT NULL,
	sunday bool NOT NULL,
	start_date date NOT NULL,
	end_date date NOT NULL,
	CONSTRAINT calendar_pkey PRIMARY KEY (service_id)
)
WITH (
	OIDS=FALSE
);
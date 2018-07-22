-- Table: public."CachedResponses"

-- DROP TABLE public."CachedResponses";

CREATE TABLE public."CachedResponses"
(
    "Id" text COLLATE pg_catalog."default" NOT NULL,
    "Data" jsonb NOT NULL,
    "ExpireTime" timestamp without time zone NOT NULL,
    CONSTRAINT "CachedResponses_pkey" PRIMARY KEY ("Id")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."CachedResponses"
    OWNER to postgres;
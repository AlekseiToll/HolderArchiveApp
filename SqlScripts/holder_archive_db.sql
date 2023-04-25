SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;
COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';

SET search_path = public, pg_catalog;
SET default_tablespace = '';
SET default_with_oids = false;

CREATE TABLE public.holder_types
(
  id bigserial NOT NULL,
  type_name character varying(30) NOT NULL,
  color integer,
  container_type character varying(20),
  cell_hor smallint,
  cell_vert smallint,
  time_limit smallint,
  created_on timestamp without time zone,
  changed_on timestamp without time zone,
  lab character varying(30),
  CONSTRAINT holder_types_pk PRIMARY KEY (id),
  CONSTRAINT holder_type_name_unique UNIQUE (type_name)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.holder_types
  OWNER TO tracking;


CREATE TABLE public.holder_type__container_type
(
  record_id bigserial NOT NULL,
  holder_type_id integer NOT NULL,
  container_type character varying(20) NOT NULL,
  CONSTRAINT holder_type__container_type_pkey PRIMARY KEY (record_id),
  CONSTRAINT holder_type_id_fkey FOREIGN KEY (holder_type_id)
      REFERENCES public.holder_types (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.holder_type__container_type
  OWNER TO tracking;


CREATE TABLE public.holders
(
  id bigserial NOT NULL,
  holder_type_id integer,
  holder_status integer,
  created_on timestamp without time zone,
  deleted_on timestamp without time zone,
  archived_on timestamp without time zone,
  CONSTRAINT holders_pkey PRIMARY KEY (id),
  CONSTRAINT holder_type_fk FOREIGN KEY (holder_type_id)
      REFERENCES public.holder_types (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.holders
  OWNER TO tracking;


CREATE TABLE public.workflow_records
(
  id bigserial NOT NULL,
  workflow_name character varying(50),
  statuses integer,
  CONSTRAINT workflows_pkey PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.workflow_records
  OWNER TO tracking;
COMMENT ON TABLE public.workflow_records
  IS 'workflow settings';


CREATE TABLE public.workflow_record__container_type
(
  record_id bigserial NOT NULL,
  workflow_record_id integer NOT NULL,
  container_type character varying(20) NOT NULL,
  CONSTRAINT workflow_record__container_type_pkey PRIMARY KEY (record_id),
  CONSTRAINT record_id_fkey FOREIGN KEY (workflow_record_id)
      REFERENCES public.workflow_records (id) MATCH SIMPLE
      ON UPDATE CASCADE ON DELETE CASCADE
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.workflow_record__container_type
  OWNER TO tracking;


CREATE TABLE public.users
(
  user_id character varying(128) NOT NULL,
  lab character varying(30),
  CONSTRAINT users_pkey PRIMARY KEY (user_id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.users
  OWNER TO tracking;
COMMENT ON COLUMN public.users.user_id IS 'User id from Helixidentity';
COMMENT ON COLUMN public.users.lab IS 'Laboratory';


CREATE TABLE public.samples
(
  record_id bigserial NOT NULL,
  sample_number bigint NOT NULL UNIQUE,
  label_id character varying(20),
  status character varying(1),
  x_workflow character varying(20),
  container_type character varying(20),
  sample_template character varying(10),
  changed_on timestamp without time zone,
  login_on timestamp without time zone,
  holder_id bigint,
  holder_row character varying(10),
  holder_column character varying(10),
  CONSTRAINT samples_pkey PRIMARY KEY (record_id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.samples
  OWNER TO tracking;



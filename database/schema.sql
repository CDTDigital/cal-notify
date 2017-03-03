--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.1
-- Dumped by pg_dump version 9.6.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


--
-- Name: postgis; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS postgis WITH SCHEMA public;


--
-- Name: EXTENSION postgis; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION postgis IS 'PostGIS geometry, geography, and raster spatial types and functions';


--
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: Address; Type: TABLE; Schema: public; Owner: calnotify
--

CREATE TABLE "Address" (
    "Id" bigint NOT NULL,
    "City" character varying(80),
    "FormattedAddress" character varying(80),
    "GeoLocation" geometry,
    "Number" character varying(80),
    "State" character varying(80),
    "Street" character varying(80),
    "UserId" uuid NOT NULL,
    "Zip" character varying(80)
);


ALTER TABLE "Address" OWNER TO calnotify;

--
-- Name: Address_Id_seq; Type: SEQUENCE; Schema: public; Owner: calnotify
--

CREATE SEQUENCE "Address_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "Address_Id_seq" OWNER TO calnotify;

--
-- Name: Address_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: calnotify
--

ALTER SEQUENCE "Address_Id_seq" OWNED BY "Address"."Id";


--
-- Name: AllUsers; Type: TABLE; Schema: public; Owner: calnotify
--

CREATE TABLE "AllUsers" (
    "Id" uuid DEFAULT uuid_generate_v4() NOT NULL,
    "Avatar" bytea,
    "Discriminator" text NOT NULL,
    "Email" text,
    "Enabled" boolean NOT NULL,
    "EnabledEmail" boolean NOT NULL,
    "EnabledSms" boolean NOT NULL,
    "JoinDate" timestamp without time zone NOT NULL,
    "LastLogin" timestamp without time zone NOT NULL,
    "Name" text,
    "Password" text,
    "PhoneNumber" text,
    "UserName" text,
    "ValidatedEmail" boolean NOT NULL,
    "ValidatedSms" boolean NOT NULL
);


ALTER TABLE "AllUsers" OWNER TO calnotify;

--
-- Name: Configurations; Type: TABLE; Schema: public; Owner: calnotify
--

CREATE TABLE "Configurations" (
    "Id" bigint NOT NULL
);


ALTER TABLE "Configurations" OWNER TO calnotify;

--
-- Name: Configurations_Id_seq; Type: SEQUENCE; Schema: public; Owner: calnotify
--

CREATE SEQUENCE "Configurations_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "Configurations_Id_seq" OWNER TO calnotify;

--
-- Name: Configurations_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: calnotify
--

ALTER SEQUENCE "Configurations_Id_seq" OWNED BY "Configurations"."Id";


--
-- Name: NotificationLog; Type: TABLE; Schema: public; Owner: calnotify
--

CREATE TABLE "NotificationLog" (
    "Id" bigint NOT NULL,
    "NotificationId" bigint NOT NULL,
    "Type" integer NOT NULL,
    "UserId" uuid NOT NULL
);


ALTER TABLE "NotificationLog" OWNER TO calnotify;

--
-- Name: NotificationLog_Id_seq; Type: SEQUENCE; Schema: public; Owner: calnotify
--

CREATE SEQUENCE "NotificationLog_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "NotificationLog_Id_seq" OWNER TO calnotify;

--
-- Name: NotificationLog_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: calnotify
--

ALTER SEQUENCE "NotificationLog_Id_seq" OWNED BY "NotificationLog"."Id";


--
-- Name: Notifications; Type: TABLE; Schema: public; Owner: calnotify
--

CREATE TABLE "Notifications" (
    "Id" bigint NOT NULL,
    "AffectedArea" geometry,
    "AuthorId" uuid NOT NULL,
    "Category" integer NOT NULL,
    "Created" timestamp without time zone NOT NULL,
    "Details" text NOT NULL,
    "Location" geometry,
    "Published" timestamp without time zone,
    "PublishedById" uuid,
    "Severity" integer NOT NULL,
    "Source" character varying(50) NOT NULL,
    "SourceId" text,
    "Status" integer NOT NULL,
    "Title" text NOT NULL
);


ALTER TABLE "Notifications" OWNER TO calnotify;

--
-- Name: Notifications_Id_seq; Type: SEQUENCE; Schema: public; Owner: calnotify
--

CREATE SEQUENCE "Notifications_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "Notifications_Id_seq" OWNER TO calnotify;

--
-- Name: Notifications_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: calnotify
--

ALTER SEQUENCE "Notifications_Id_seq" OWNED BY "Notifications"."Id";


--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: calnotify
--

CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE "__EFMigrationsHistory" OWNER TO calnotify;

--
-- Name: Address Id; Type: DEFAULT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "Address" ALTER COLUMN "Id" SET DEFAULT nextval('"Address_Id_seq"'::regclass);


--
-- Name: Configurations Id; Type: DEFAULT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "Configurations" ALTER COLUMN "Id" SET DEFAULT nextval('"Configurations_Id_seq"'::regclass);


--
-- Name: NotificationLog Id; Type: DEFAULT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "NotificationLog" ALTER COLUMN "Id" SET DEFAULT nextval('"NotificationLog_Id_seq"'::regclass);


--
-- Name: Notifications Id; Type: DEFAULT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "Notifications" ALTER COLUMN "Id" SET DEFAULT nextval('"Notifications_Id_seq"'::regclass);


--
-- Name: Address PK_Address; Type: CONSTRAINT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "Address"
    ADD CONSTRAINT "PK_Address" PRIMARY KEY ("Id");


--
-- Name: AllUsers PK_AllUsers; Type: CONSTRAINT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "AllUsers"
    ADD CONSTRAINT "PK_AllUsers" PRIMARY KEY ("Id");


--
-- Name: Configurations PK_Configurations; Type: CONSTRAINT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "Configurations"
    ADD CONSTRAINT "PK_Configurations" PRIMARY KEY ("Id");


--
-- Name: NotificationLog PK_NotificationLog; Type: CONSTRAINT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "NotificationLog"
    ADD CONSTRAINT "PK_NotificationLog" PRIMARY KEY ("Id");


--
-- Name: Notifications PK_Notifications; Type: CONSTRAINT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "Notifications"
    ADD CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_Address_UserId; Type: INDEX; Schema: public; Owner: calnotify
--

CREATE UNIQUE INDEX "IX_Address_UserId" ON "Address" USING btree ("UserId");


--
-- Name: IX_Notifications_AuthorId; Type: INDEX; Schema: public; Owner: calnotify
--

CREATE INDEX "IX_Notifications_AuthorId" ON "Notifications" USING btree ("AuthorId");


--
-- Name: IX_Notifications_PublishedById; Type: INDEX; Schema: public; Owner: calnotify
--

CREATE INDEX "IX_Notifications_PublishedById" ON "Notifications" USING btree ("PublishedById");


--
-- Name: Address FK_Address_AllUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "Address"
    ADD CONSTRAINT "FK_Address_AllUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AllUsers"("Id") ON DELETE CASCADE;


--
-- Name: Notifications FK_Notifications_AllUsers_AuthorId; Type: FK CONSTRAINT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "Notifications"
    ADD CONSTRAINT "FK_Notifications_AllUsers_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "AllUsers"("Id") ON DELETE CASCADE;


--
-- Name: Notifications FK_Notifications_AllUsers_PublishedById; Type: FK CONSTRAINT; Schema: public; Owner: calnotify
--

ALTER TABLE ONLY "Notifications"
    ADD CONSTRAINT "FK_Notifications_AllUsers_PublishedById" FOREIGN KEY ("PublishedById") REFERENCES "AllUsers"("Id");


--
-- PostgreSQL database dump complete
--


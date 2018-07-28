/*
Navicat PGSQL Data Transfer

Source Server         : 144000.tv
Source Server Version : 90606
Source Host           : 144000.tv:5432
Source Database       : samp
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90606
File Encoding         : 65001

Date: 2018-07-28 17:38:21
*/


-- ----------------------------
-- Sequence structure for cashes_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."cashes_id_seq";
CREATE SEQUENCE "public"."cashes_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 31
 CACHE 1;
SELECT setval('"public"."cashes_id_seq"', 31, true);

-- ----------------------------
-- Sequence structure for chats_id_seq1
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."chats_id_seq1";
CREATE SEQUENCE "public"."chats_id_seq1"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."chats_id_seq1"', 1, true);

-- ----------------------------
-- Sequence structure for orders_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."orders_id_seq";
CREATE SEQUENCE "public"."orders_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Sequence structure for repays_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."repays_id_seq";
CREATE SEQUENCE "public"."repays_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 5
 CACHE 1;
SELECT setval('"public"."repays_id_seq"', 5, true);

-- ----------------------------
-- Sequence structure for users_id_seq1
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."users_id_seq1";
CREATE SEQUENCE "public"."users_id_seq1"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Table structure for chats
-- ----------------------------
DROP TABLE IF EXISTS "public"."chats";
CREATE TABLE "public"."chats" (
"id" int4 DEFAULT nextval('chats_id_seq1'::regclass) NOT NULL,
"ctrid" varchar(2) COLLATE "default" NOT NULL,
"subject" varchar(20) COLLATE "default",
"uid" int4 NOT NULL,
"uname" varchar(254) COLLATE "default",
"msgs" jsonb,
"replies" int2 NOT NULL,
"posted" timestamp(6),
"top" bool,
"img0" bytea,
"img1" bytea,
"img2" bytea,
"img3" bytea,
"img4" bytea,
"img5" bytea,
"img6" bytea,
"img7" bytea,
"img8" bytea,
"img9" bytea,
"imgs" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for items
-- ----------------------------
DROP TABLE IF EXISTS "public"."items";
CREATE TABLE "public"."items" (
"ctrid" varchar(2) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default" NOT NULL,
"descr" varchar(100) COLLATE "default",
"icon" bytea,
"unit" varchar(4) COLLATE "default",
"price" money,
"min" int2,
"step" int2,
"refrig" bool,
"mov" varchar(100) COLLATE "default",
"vdrid" varchar(3) COLLATE "default",
"demand" int2,
"cap7" int2[],
"status" int2,
"_vdr" money,
"_tm" money,
"_dlvy" money
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for ords
-- ----------------------------
DROP TABLE IF EXISTS "public"."ords";
CREATE TABLE "public"."ords" (
"id" int4 DEFAULT nextval('orders_id_seq'::regclass) NOT NULL,
"rev" int2 DEFAULT 0 NOT NULL,
"ctrid" varchar(2) COLLATE "default" NOT NULL,
"tmid" varchar(4) COLLATE "default",
"uid" int4 NOT NULL,
"uname" varchar(10) COLLATE "default",
"uwx" varchar(28) COLLATE "default",
"utel" varchar(11) COLLATE "default",
"uaddr" varchar(20) COLLATE "default",
"total" money,
"cash" money DEFAULT 0,
"score" money,
"posid" int4,
"created" timestamp(6),
"paid" timestamp(6),
"aborted" timestamp(6),
"ended" timestamp(6),
"status" int2,
"item" varchar(10) COLLATE "default",
"unit" varchar(4) COLLATE "default",
"price" numeric,
"qty" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for orgs
-- ----------------------------
DROP TABLE IF EXISTS "public"."orgs";
CREATE TABLE "public"."orgs" (
"id" varchar(4) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default",
"descr" varchar(50) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"x" float8,
"y" float8,
"icon" bytea,
"mgrid" int4,
"mgrname" varchar(10) COLLATE "default",
"mgrwx" varchar(28) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"status" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for recs
-- ----------------------------
DROP TABLE IF EXISTS "public"."recs";
CREATE TABLE "public"."recs" (
"id" int4 DEFAULT nextval('cashes_id_seq'::regclass) NOT NULL,
"orgid" varchar(4) COLLATE "default" NOT NULL,
"date" date,
"code" int2,
"descr" varchar(20) COLLATE "default",
"receive" money,
"pay" money,
"creator" varchar(10) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for repays
-- ----------------------------
DROP TABLE IF EXISTS "public"."repays";
CREATE TABLE "public"."repays" (
"id" int4 DEFAULT nextval('repays_id_seq'::regclass) NOT NULL,
"orgid" varchar(4) COLLATE "default" NOT NULL,
"fro" date NOT NULL,
"till" date NOT NULL,
"orders" int4,
"total" money,
"cash" money,
"payer" varchar(6) COLLATE "default",
"err" varchar(40) COLLATE "default",
"status" int2 DEFAULT 0
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for tuts
-- ----------------------------
DROP TABLE IF EXISTS "public"."tuts";
CREATE TABLE "public"."tuts" (
"id" varchar(4) COLLATE "default",
"name" varchar(10) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS "public"."users";
CREATE TABLE "public"."users" (
"id" int4 DEFAULT nextval('users_id_seq1'::regclass) NOT NULL,
"name" varchar(10) COLLATE "default" NOT NULL,
"ctrid" varchar(2) COLLATE "default" NOT NULL,
"wx" varchar(28) COLLATE "default" NOT NULL,
"tel" varchar(11) COLLATE "default" NOT NULL,
"addr" varchar(20) COLLATE "default",
"credential" varchar(32) COLLATE "default",
"score" int4,
"refid" int4,
"ctrat" varchar(2) COLLATE "default",
"ctr" int2 DEFAULT 0,
"vdrat" varchar(3) COLLATE "default",
"vdr" int2,
"tmat" varchar(4) COLLATE "default",
"tm" int2,
"plat" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."cashes_id_seq" OWNED BY "recs"."id";
ALTER SEQUENCE "public"."chats_id_seq1" OWNED BY "chats"."id";
ALTER SEQUENCE "public"."orders_id_seq" OWNED BY "ords"."id";
ALTER SEQUENCE "public"."repays_id_seq" OWNED BY "repays"."id";
ALTER SEQUENCE "public"."users_id_seq1" OWNED BY "users"."id";

-- ----------------------------
-- Indexes structure for table chats
-- ----------------------------
CREATE INDEX "chats_ctrid" ON "public"."chats" USING btree ("ctrid");

-- ----------------------------
-- Primary Key structure for table chats
-- ----------------------------
ALTER TABLE "public"."chats" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table items
-- ----------------------------
ALTER TABLE "public"."items" ADD PRIMARY KEY ("ctrid", "name");

-- ----------------------------
-- Indexes structure for table ords
-- ----------------------------
CREATE INDEX "orders_statuscustid" ON "public"."ords" USING btree ("status", "uid");
CREATE INDEX "orders_statusorgid" ON "public"."ords" USING btree ("status", "ctrid");

-- ----------------------------
-- Primary Key structure for table ords
-- ----------------------------
ALTER TABLE "public"."ords" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table orgs
-- ----------------------------
ALTER TABLE "public"."orgs" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table recs
-- ----------------------------
CREATE INDEX "recs_idx_orgiddate" ON "public"."recs" USING btree ("orgid", "date");

-- ----------------------------
-- Primary Key structure for table recs
-- ----------------------------
ALTER TABLE "public"."recs" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table repays
-- ----------------------------
CREATE INDEX "replays_idx_orgidstatus" ON "public"."repays" USING btree ("orgid", "status");

-- ----------------------------
-- Primary Key structure for table repays
-- ----------------------------
ALTER TABLE "public"."repays" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table users
-- ----------------------------
CREATE INDEX "users_ctrat" ON "public"."users" USING btree ("ctrat") WHERE ctrat IS NOT NULL;
CREATE INDEX "users_tel" ON "public"."users" USING hash ("tel") WHERE tel IS NOT NULL;

-- ----------------------------
-- Primary Key structure for table users
-- ----------------------------
ALTER TABLE "public"."users" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Foreign Key structure for table "public"."chats"
-- ----------------------------
ALTER TABLE "public"."chats" ADD FOREIGN KEY ("ctrid") REFERENCES "public"."orgs" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Key structure for table "public"."items"
-- ----------------------------
ALTER TABLE "public"."items" ADD FOREIGN KEY ("ctrid") REFERENCES "public"."orgs" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Key structure for table "public"."ords"
-- ----------------------------
ALTER TABLE "public"."ords" ADD FOREIGN KEY ("ctrid") REFERENCES "public"."orgs" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Key structure for table "public"."recs"
-- ----------------------------
ALTER TABLE "public"."recs" ADD FOREIGN KEY ("orgid") REFERENCES "public"."orgs" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Key structure for table "public"."repays"
-- ----------------------------
ALTER TABLE "public"."repays" ADD FOREIGN KEY ("orgid") REFERENCES "public"."orgs" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Key structure for table "public"."users"
-- ----------------------------
ALTER TABLE "public"."users" ADD FOREIGN KEY ("ctrid") REFERENCES "public"."orgs" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

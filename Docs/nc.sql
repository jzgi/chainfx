/*
Navicat PGSQL Data Transfer

Source Server         : 144000.tv
Source Server Version : 90606
Source Host           : 144000.tv:5432
Source Database       : nc
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90606
File Encoding         : 65001

Date: 2018-08-15 23:03:28
*/


-- ----------------------------
-- Sequence structure for chats_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."chats_id_seq";
CREATE SEQUENCE "public"."chats_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."chats_id_seq"', 1, true);

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
 START 1
 CACHE 1;
SELECT setval('"public"."repays_id_seq"', 1, true);

-- ----------------------------
-- Sequence structure for users_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."users_id_seq";
CREATE SEQUENCE "public"."users_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 28
 CACHE 1;
SELECT setval('"public"."users_id_seq"', 28, true);

-- ----------------------------
-- Table structure for chats
-- ----------------------------
DROP TABLE IF EXISTS "public"."chats";
CREATE TABLE "public"."chats" (
"id" int4 DEFAULT nextval('chats_id_seq'::regclass) NOT NULL,
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
"name" varchar(10) COLLATE "default" NOT NULL,
"descr" varchar(100) COLLATE "default",
"remark" varchar(500) COLLATE "default",
"mov" varchar(100) COLLATE "default",
"icon" bytea,
"unit" varchar(4) COLLATE "default",
"price" money,
"giverp" money,
"grperp" money,
"dvrerp" money,
"min" int2,
"step" int2,
"refrig" bool,
"demand" int2,
"giverid" int4,
"cap7" int2[],
"status" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for orders
-- ----------------------------
DROP TABLE IF EXISTS "public"."orders";
CREATE TABLE "public"."orders" (
"id" int4 DEFAULT nextval('orders_id_seq'::regclass) NOT NULL,
"uid" int4 NOT NULL,
"uname" varchar(10) COLLATE "default",
"uwx" varchar(28) COLLATE "default",
"utel" varchar(11) COLLATE "default",
"uaddr" varchar(20) COLLATE "default",
"grpid" varchar(4) COLLATE "default",
"item" varchar(10) COLLATE "default",
"unit" varchar(4) COLLATE "default",
"price" numeric(38),
"qty" int2,
"total" money,
"cash" money DEFAULT 0,
"created" timestamp(6),
"paid" timestamp(6),
"aborted" timestamp(6),
"giverid" int4,
"giving" timestamp(6),
"given" timestamp(6),
"dvrerid" int4,
"dvring" timestamp(6),
"dvred" timestamp(6),
"grperid" int4,
"ended" timestamp(6),
"status" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for orgs
-- ----------------------------
DROP TABLE IF EXISTS "public"."orgs";
CREATE TABLE "public"."orgs" (
"id" varchar(3) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"x" float8,
"y" float8,
"mgrid" int4,
"mgrname" varchar(10) COLLATE "default",
"mgrwx" varchar(28) COLLATE "default",
"mgrtel" varchar(11) COLLATE "default",
"status" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for repays
-- ----------------------------
DROP TABLE IF EXISTS "public"."repays";
CREATE TABLE "public"."repays" (
"id" int4 DEFAULT nextval('repays_id_seq'::regclass) NOT NULL,
"job" int2 NOT NULL,
"uid" int4 NOT NULL,
"uname" varchar(10) COLLATE "default" NOT NULL,
"uwx" varchar(28) COLLATE "default",
"fro" date NOT NULL,
"till" date NOT NULL,
"orders" int4,
"cash" money,
"paid" timestamp(6),
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
"id" int4 DEFAULT nextval('users_id_seq'::regclass) NOT NULL,
"wx" varchar(28) COLLATE "default",
"refid" int4,
"name" varchar(10) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"credential" varchar(32) COLLATE "default",
"grpat" varchar(3) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"grp" int2,
"ctr" int2 DEFAULT 0,
"created" timestamp(6) DEFAULT ('now'::text)::timestamp without time zone
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."chats_id_seq" OWNED BY "chats"."id";
ALTER SEQUENCE "public"."orders_id_seq" OWNED BY "orders"."id";
ALTER SEQUENCE "public"."repays_id_seq" OWNED BY "repays"."id";
ALTER SEQUENCE "public"."users_id_seq" OWNED BY "users"."id";

-- ----------------------------
-- Primary Key structure for table chats
-- ----------------------------
ALTER TABLE "public"."chats" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table items
-- ----------------------------
ALTER TABLE "public"."items" ADD PRIMARY KEY ("name");

-- ----------------------------
-- Indexes structure for table orders
-- ----------------------------
CREATE INDEX "orders_statusuid" ON "public"."orders" USING btree ("status", "uid");

-- ----------------------------
-- Primary Key structure for table orders
-- ----------------------------
ALTER TABLE "public"."orders" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table orgs
-- ----------------------------
ALTER TABLE "public"."orgs" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table repays
-- ----------------------------
ALTER TABLE "public"."repays" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table users
-- ----------------------------
CREATE INDEX "users_tel" ON "public"."users" USING hash ("tel") WHERE tel IS NOT NULL;
CREATE UNIQUE INDEX "users_wx" ON "public"."users" USING btree ("wx");

-- ----------------------------
-- Primary Key structure for table users
-- ----------------------------
ALTER TABLE "public"."users" ADD PRIMARY KEY ("id");

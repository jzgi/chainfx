/*
 Navicat PostgreSQL Data Transfer

 Source Server         : 144000.tv
 Source Server Type    : PostgreSQL
 Source Server Version : 90606
 Source Host           : 144000.tv:5432
 Source Catalog        : samp
 Source Schema         : public

 Target Server Type    : PostgreSQL
 Target Server Version : 90606
 File Encoding         : 65001

 Date: 03/10/2018 23:19:55
*/


-- ----------------------------
-- Sequence structure for chats_id_seq1
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."chats_id_seq1";
CREATE SEQUENCE "public"."chats_id_seq1" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for items_id_seq1
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."items_id_seq1";
CREATE SEQUENCE "public"."items_id_seq1" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for orders_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."orders_id_seq";
CREATE SEQUENCE "public"."orders_id_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for orgs_id_seq1
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."orgs_id_seq1";
CREATE SEQUENCE "public"."orgs_id_seq1" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for repays_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."repays_id_seq";
CREATE SEQUENCE "public"."repays_id_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for users_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."users_id_seq";
CREATE SEQUENCE "public"."users_id_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Table structure for chats
-- ----------------------------
DROP TABLE IF EXISTS "public"."chats";
CREATE TABLE "public"."chats" (
  "id" int4 NOT NULL DEFAULT nextval('chats_id_seq1'::regclass),
  "hubid" varchar(2) COLLATE "pg_catalog"."default" NOT NULL,
  "subject" varchar(20) COLLATE "pg_catalog"."default",
  "uname" varchar(254) COLLATE "pg_catalog"."default",
  "posts" jsonb,
  "posted" timestamp(6),
  "fcount" int2 NOT NULL,
  "fname" varchar(10) COLLATE "pg_catalog"."default",
  "img" bytea,
  "status" int2
)
;

-- ----------------------------
-- Table structure for hubs
-- ----------------------------
DROP TABLE IF EXISTS "public"."hubs";
CREATE TABLE "public"."hubs" (
  "id" varchar(2) COLLATE "pg_catalog"."default" NOT NULL,
  "name" varchar(10) COLLATE "pg_catalog"."default",
  "appid" varchar(18) COLLATE "pg_catalog"."default",
  "appsecret" varchar(32) COLLATE "pg_catalog"."default",
  "mchid" varchar(10) COLLATE "pg_catalog"."default",
  "noncestr" varchar(10) COLLATE "pg_catalog"."default",
  "spbillcreateip" varchar(15) COLLATE "pg_catalog"."default",
  "key" varchar(32) COLLATE "pg_catalog"."default",
  "status" int2,
  "watchurl" varchar(100) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Table structure for items
-- ----------------------------
DROP TABLE IF EXISTS "public"."items";
CREATE TABLE "public"."items" (
  "id" int2 NOT NULL DEFAULT nextval('items_id_seq1'::regclass),
  "hubid" varchar(2) COLLATE "pg_catalog"."default" NOT NULL,
  "name" varchar(10) COLLATE "pg_catalog"."default" NOT NULL,
  "descr" varchar(100) COLLATE "pg_catalog"."default",
  "remark" varchar(500) COLLATE "pg_catalog"."default",
  "mov" varchar(100) COLLATE "pg_catalog"."default",
  "icon" bytea,
  "unit" varchar(4) COLLATE "pg_catalog"."default",
  "price" money,
  "fee" money,
  "shopp" money,
  "teamp" money,
  "senderp" money,
  "min" int2,
  "step" int2,
  "refrig" bool,
  "cap" int4,
  "queue" int4,
  "shopid" int2,
  "status" int2
)
;

-- ----------------------------
-- Table structure for orders
-- ----------------------------
DROP TABLE IF EXISTS "public"."orders";
CREATE TABLE "public"."orders" (
  "id" int4 NOT NULL DEFAULT nextval('orders_id_seq'::regclass),
  "hubid" varchar(2) COLLATE "pg_catalog"."default" NOT NULL,
  "uid" int4 NOT NULL,
  "uname" varchar(10) COLLATE "pg_catalog"."default",
  "uwx" varchar(28) COLLATE "pg_catalog"."default",
  "utel" varchar(11) COLLATE "pg_catalog"."default",
  "uaddr" varchar(20) COLLATE "pg_catalog"."default",
  "teamid" int2,
  "itemid" int2,
  "itemname" varchar(10) COLLATE "pg_catalog"."default",
  "unit" varchar(4) COLLATE "pg_catalog"."default",
  "price" numeric(38),
  "qty" int2,
  "total" money,
  "cash" money DEFAULT 0,
  "paid" timestamp(6),
  "shopid" varchar(3) COLLATE "pg_catalog"."default",
  "giverid" int4,
  "given" timestamp(6),
  "senderid" int4,
  "sent" timestamp(6),
  "takerid" int4,
  "taken" timestamp(6),
  "receiverid" int4,
  "received" timestamp(6),
  "ended" timestamp(6),
  "status" int2
)
;

-- ----------------------------
-- Table structure for orgs
-- ----------------------------
DROP TABLE IF EXISTS "public"."orgs";
CREATE TABLE "public"."orgs" (
  "id" int2 NOT NULL DEFAULT nextval('orgs_id_seq1'::regclass),
  "hubid" varchar(2) COLLATE "pg_catalog"."default" NOT NULL,
  "typ" int2,
  "name" varchar(10) COLLATE "pg_catalog"."default",
  "tel" varchar(11) COLLATE "pg_catalog"."default",
  "addr" varchar(20) COLLATE "pg_catalog"."default",
  "x" float8,
  "y" float8,
  "mgrid" int4,
  "mgrname" varchar(10) COLLATE "pg_catalog"."default",
  "mgrwx" varchar(28) COLLATE "pg_catalog"."default",
  "status" int2
)
;

-- ----------------------------
-- Table structure for repays
-- ----------------------------
DROP TABLE IF EXISTS "public"."repays";
CREATE TABLE "public"."repays" (
  "id" int4 NOT NULL DEFAULT nextval('repays_id_seq'::regclass),
  "hubid" varchar(2) COLLATE "pg_catalog"."default" NOT NULL,
  "job" int2 NOT NULL,
  "uid" int4 NOT NULL,
  "uname" varchar(10) COLLATE "pg_catalog"."default" NOT NULL,
  "uwx" varchar(28) COLLATE "pg_catalog"."default",
  "fro" date NOT NULL,
  "till" date NOT NULL,
  "orders" int4,
  "cash" money,
  "paid" timestamp(6),
  "payer" varchar(6) COLLATE "pg_catalog"."default",
  "err" varchar(40) COLLATE "pg_catalog"."default",
  "status" int2 DEFAULT 0
)
;

-- ----------------------------
-- Table structure for tuts
-- ----------------------------
DROP TABLE IF EXISTS "public"."tuts";
CREATE TABLE "public"."tuts" (
  "id" varchar(4) COLLATE "pg_catalog"."default",
  "name" varchar(10) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS "public"."users";
CREATE TABLE "public"."users" (
  "id" int4 NOT NULL DEFAULT nextval('users_id_seq'::regclass),
  "hubid" varchar(2) COLLATE "pg_catalog"."default" NOT NULL,
  "name" varchar(10) COLLATE "pg_catalog"."default",
  "wx" varchar(28) COLLATE "pg_catalog"."default",
  "tel" varchar(11) COLLATE "pg_catalog"."default",
  "credential" varchar(32) COLLATE "pg_catalog"."default",
  "addr" varchar(20) COLLATE "pg_catalog"."default",
  "teamat" int2,
  "teamly" int2 NOT NULL DEFAULT 0,
  "shopat" int2,
  "shoply" int2 NOT NULL DEFAULT 0,
  "hubly" int2 NOT NULL DEFAULT 0,
  "created" timestamp(6) DEFAULT ('now'::text)::timestamp without time zone
)
;

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."chats_id_seq1"
OWNED BY "public"."chats"."id";
SELECT setval('"public"."chats_id_seq1"', 2, false);
ALTER SEQUENCE "public"."items_id_seq1"
OWNED BY "public"."items"."id";
SELECT setval('"public"."items_id_seq1"', 2, false);
ALTER SEQUENCE "public"."orders_id_seq"
OWNED BY "public"."orders"."id";
SELECT setval('"public"."orders_id_seq"', 2, false);
ALTER SEQUENCE "public"."orgs_id_seq1"
OWNED BY "public"."orgs"."id";
SELECT setval('"public"."orgs_id_seq1"', 2, false);
ALTER SEQUENCE "public"."repays_id_seq"
OWNED BY "public"."repays"."id";
SELECT setval('"public"."repays_id_seq"', 2, false);
ALTER SEQUENCE "public"."users_id_seq"
OWNED BY "public"."users"."id";
SELECT setval('"public"."users_id_seq"', 7, true);

-- ----------------------------
-- Primary Key structure for table chats
-- ----------------------------
ALTER TABLE "public"."chats" ADD CONSTRAINT "chats_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table hubs
-- ----------------------------
ALTER TABLE "public"."hubs" ADD CONSTRAINT "hubs_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table items
-- ----------------------------
ALTER TABLE "public"."items" ADD CONSTRAINT "items_pkey1" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table orders
-- ----------------------------
CREATE INDEX "orders_statusuid" ON "public"."orders" USING btree (
  "status" "pg_catalog"."int2_ops" ASC NULLS LAST,
  "uid" "pg_catalog"."int4_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table orders
-- ----------------------------
ALTER TABLE "public"."orders" ADD CONSTRAINT "orders_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table orgs
-- ----------------------------
ALTER TABLE "public"."orgs" ADD CONSTRAINT "orgs_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table repays
-- ----------------------------
ALTER TABLE "public"."repays" ADD CONSTRAINT "repays_pkey1" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table users
-- ----------------------------
CREATE INDEX "users_tel" ON "public"."users" USING hash (
  "tel" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops"
) WHERE tel IS NOT NULL;
CREATE UNIQUE INDEX "users_wx" ON "public"."users" USING btree (
  "wx" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table users
-- ----------------------------
ALTER TABLE "public"."users" ADD CONSTRAINT "users_pkey" PRIMARY KEY ("id");

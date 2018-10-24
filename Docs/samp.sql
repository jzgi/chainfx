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

 Date: 23/10/2018 15:03:23
*/


-- ----------------------------
-- Sequence structure for chats_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."chats_id_seq";
CREATE SEQUENCE "public"."chats_id_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for items_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."items_id_seq";
CREATE SEQUENCE "public"."items_id_seq" 
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
-- Sequence structure for shops_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."shops_id_seq";
CREATE SEQUENCE "public"."shops_id_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for teams_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."teams_id_seq";
CREATE SEQUENCE "public"."teams_id_seq" 
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
  "id" int4 NOT NULL DEFAULT nextval('chats_id_seq'::regclass),
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
  "watchurl" varchar(100) COLLATE "pg_catalog"."default",
  "p12" bytea,
  "status" int2
)
;

-- ----------------------------
-- Table structure for items
-- ----------------------------
DROP TABLE IF EXISTS "public"."items";
CREATE TABLE "public"."items" (
  "id" int2 NOT NULL DEFAULT nextval('items_id_seq'::regclass),
  "hubid" varchar(2) COLLATE "pg_catalog"."default" NOT NULL,
  "name" varchar(10) COLLATE "pg_catalog"."default" NOT NULL,
  "descr" varchar(100) COLLATE "pg_catalog"."default",
  "remark" varchar(500) COLLATE "pg_catalog"."default",
  "icon" bytea,
  "img" bytea,
  "mov" varchar(100) COLLATE "pg_catalog"."default",
  "unit" varchar(4) COLLATE "pg_catalog"."default",
  "price" money,
  "fee" money,
  "shopp" money,
  "teamp" money,
  "senderp" money,
  "min" int2,
  "step" int2,
  "refrig" bool,
  "cap7" int4,
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
  "teamid" int2,
  "uid" int4 NOT NULL,
  "uname" varchar(10) COLLATE "pg_catalog"."default",
  "creatorwx" varchar(28) COLLATE "pg_catalog"."default",
  "utel" varchar(11) COLLATE "pg_catalog"."default",
  "uaddr" varchar(20) COLLATE "pg_catalog"."default",
  "itemid" int2,
  "item" varchar(8) COLLATE "pg_catalog"."default",
  "unit" varchar(4) COLLATE "pg_catalog"."default",
  "price" numeric(38),
  "fee" money,
  "qty" int2,
  "total" money,
  "cash" money DEFAULT 0,
  "creatorid" int2,
  "creator" varchar(8) COLLATE "pg_catalog"."default",
  "paid" timestamp(6),
  "shopid" int2,
  "accepterid" int4,
  "accepted" timestamp(6),
  "stockerid" int4,
  "stocked" timestamp(6),
  "senderid" int4,
  "sent" timestamp(6),
  "receiverid" int4,
  "received" timestamp(6),
  "ended" timestamp(6),
  "status" int2,
  "accepter" varchar(8) COLLATE "pg_catalog"."default",
  "stocker" varchar(8) COLLATE "pg_catalog"."default",
  "sender" varchar(8) COLLATE "pg_catalog"."default",
  "receiver" varchar(8) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Table structure for repays
-- ----------------------------
DROP TABLE IF EXISTS "public"."repays";
CREATE TABLE "public"."repays" (
  "id" int4 NOT NULL DEFAULT nextval('repays_id_seq'::regclass),
  "hubid" varchar(2) COLLATE "pg_catalog"."default" NOT NULL,
  "typ" int2 NOT NULL,
  "orgid" int2,
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
-- Table structure for shops
-- ----------------------------
DROP TABLE IF EXISTS "public"."shops";
CREATE TABLE "public"."shops" (
  "id" int2 NOT NULL DEFAULT nextval('shops_id_seq'::regclass),
  "hubid" varchar(2) COLLATE "pg_catalog"."default" NOT NULL,
  "name" varchar(10) COLLATE "pg_catalog"."default",
  "addr" varchar(20) COLLATE "pg_catalog"."default",
  "x" float8,
  "y" float8,
  "mgrtel" varchar(11) COLLATE "pg_catalog"."default",
  "mgrname" varchar(10) COLLATE "pg_catalog"."default",
  "mgrwx" varchar(28) COLLATE "pg_catalog"."default",
  "status" int2
)
;

-- ----------------------------
-- Table structure for teams
-- ----------------------------
DROP TABLE IF EXISTS "public"."teams";
CREATE TABLE "public"."teams" (
  "id" int2 NOT NULL DEFAULT nextval('teams_id_seq'::regclass),
  "hubid" varchar(2) COLLATE "pg_catalog"."default" NOT NULL,
  "name" varchar(10) COLLATE "pg_catalog"."default",
  "addr" varchar(20) COLLATE "pg_catalog"."default",
  "x" float8,
  "y" float8,
  "mgrtel" varchar(11) COLLATE "pg_catalog"."default",
  "mgrname" varchar(10) COLLATE "pg_catalog"."default",
  "mgrwx" varchar(28) COLLATE "pg_catalog"."default",
  "status" int2
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
  "teamid" int2,
  "teamly" int2 NOT NULL DEFAULT 0,
  "shopid" int2 NOT NULL DEFAULT 0,
  "shoply" int2 NOT NULL DEFAULT 0,
  "hubly" int2 NOT NULL DEFAULT 0,
  "created" timestamp(6) DEFAULT ('now'::text)::timestamp without time zone
)
;

-- ----------------------------
-- Function structure for first_agg
-- ----------------------------
DROP FUNCTION IF EXISTS "public"."first_agg"(anyelement, anyelement);
CREATE OR REPLACE FUNCTION "public"."first_agg"(anyelement, anyelement)
  RETURNS "pg_catalog"."anyelement" AS $BODY$
SELECT $1;
$BODY$
  LANGUAGE sql IMMUTABLE STRICT
  COST 100;

-- ----------------------------
-- Function structure for last_agg
-- ----------------------------
DROP FUNCTION IF EXISTS "public"."last_agg"(anyelement, anyelement);
CREATE OR REPLACE FUNCTION "public"."last_agg"(anyelement, anyelement)
  RETURNS "pg_catalog"."anyelement" AS $BODY$
SELECT $2;
$BODY$
  LANGUAGE sql IMMUTABLE STRICT
  COST 100;

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."chats_id_seq"
OWNED BY "public"."chats"."id";
SELECT setval('"public"."chats_id_seq"', 4, true);
ALTER SEQUENCE "public"."items_id_seq"
OWNED BY "public"."items"."id";
SELECT setval('"public"."items_id_seq"', 11, false);
ALTER SEQUENCE "public"."orders_id_seq"
OWNED BY "public"."orders"."id";
SELECT setval('"public"."orders_id_seq"', 258, true);
ALTER SEQUENCE "public"."repays_id_seq"
OWNED BY "public"."repays"."id";
SELECT setval('"public"."repays_id_seq"', 2, false);
ALTER SEQUENCE "public"."shops_id_seq"
OWNED BY "public"."shops"."id";
SELECT setval('"public"."shops_id_seq"', 2, false);
ALTER SEQUENCE "public"."teams_id_seq"
OWNED BY "public"."teams"."id";
SELECT setval('"public"."teams_id_seq"', 2, false);
ALTER SEQUENCE "public"."users_id_seq"
OWNED BY "public"."users"."id";
SELECT setval('"public"."users_id_seq"', 18, true);

-- ----------------------------
-- Primary Key structure for table chats
-- ----------------------------
ALTER TABLE "public"."chats" ADD CONSTRAINT "chats_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table hubs
-- ----------------------------
ALTER TABLE "public"."hubs" ADD CONSTRAINT "hubs_pkey1" PRIMARY KEY ("id");

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
ALTER TABLE "public"."orders" ADD CONSTRAINT "orders_pkey1" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table repays
-- ----------------------------
ALTER TABLE "public"."repays" ADD CONSTRAINT "repays_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table shops
-- ----------------------------
ALTER TABLE "public"."shops" ADD CONSTRAINT "shops_pk" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table teams
-- ----------------------------
ALTER TABLE "public"."teams" ADD CONSTRAINT "teams_pk" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table users
-- ----------------------------
CREATE UNIQUE INDEX "users_tel" ON "public"."users" USING btree (
  "tel" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);
CREATE UNIQUE INDEX "users_wx" ON "public"."users" USING btree (
  "wx" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table users
-- ----------------------------
ALTER TABLE "public"."users" ADD CONSTRAINT "users_pkey" PRIMARY KEY ("id");

/*
Navicat PGSQL Data Transfer

Source Server         : 106.14.45.109
Source Server Version : 90505
Source Host           : 106.14.45.109:5432
Source Database       : samp
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90505
File Encoding         : 65001

Date: 2017-11-02 22:11:05
*/


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
 MINVALUE 1000
 MAXVALUE 9223372036854775807
 START 1293
 CACHE 16;
SELECT setval('"public"."repays_id_seq"', 1293, true);

-- ----------------------------
-- Sequence structure for shops_id_seq1
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."shops_id_seq1";
CREATE SEQUENCE "public"."shops_id_seq1"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Table structure for items
-- ----------------------------
DROP TABLE IF EXISTS "public"."items";
CREATE TABLE "public"."items" (
"shopid" varchar(4) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default" NOT NULL,
"descr" varchar(30) COLLATE "default",
"icon" bytea,
"unit" varchar(4) COLLATE "default",
"price" money,
"min" int2,
"step" int2,
"max" int2,
"customs" varchar(30)[] COLLATE "default",
"status" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for orders
-- ----------------------------
DROP TABLE IF EXISTS "public"."orders";
CREATE TABLE "public"."orders" (
"id" int8 DEFAULT nextval('orders_id_seq'::regclass) NOT NULL,
"shopid" varchar(4) COLLATE "default",
"shopname" varchar(10) COLLATE "default",
"wx" varchar(28) COLLATE "default",
"name" varchar(10) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"city" varchar(6) COLLATE "default",
"region" varchar(10) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"items" jsonb,
"total" money,
"created" timestamp(6),
"cash" money DEFAULT 0,
"paid" timestamp(6),
"prepare" bool,
"aborted" timestamp(6),
"received" timestamp(6),
"note" varchar(20) COLLATE "default",
"kick" varchar(40) COLLATE "default",
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
"shopid" varchar(4) COLLATE "default",
"shopname" varchar(10) COLLATE "default",
"till" date,
"orders" int4,
"total" money,
"cash" money,
"paid" timestamp(6),
"payer" varchar(6) COLLATE "default",
"status" int2 DEFAULT 0,
"err" varchar(40) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for shops
-- ----------------------------
DROP TABLE IF EXISTS "public"."shops";
CREATE TABLE "public"."shops" (
"id" varchar(4) COLLATE "default" DEFAULT nextval('shops_id_seq1'::regclass) NOT NULL,
"name" varchar(10) COLLATE "default",
"city" varchar(6) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"x" float8,
"y" float8,
"icon" bytea,
"areas" varchar(10)[] COLLATE "default",
"minimum" money,
"every" money,
"cut" money,
"mgrwx" varchar(28) COLLATE "default",
"mgrtel" varchar(11) COLLATE "default",
"mgrname" varchar(10) COLLATE "default",
"oprwx" varchar(28) COLLATE "default",
"oprtel" varchar(11) COLLATE "default",
"oprname" varchar(10) COLLATE "default",
"status" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for slides
-- ----------------------------
DROP TABLE IF EXISTS "public"."slides";
CREATE TABLE "public"."slides" (
"no" varchar(4) COLLATE "default",
"lesson" varchar(10) COLLATE "default",
"title" varchar(30) COLLATE "default",
"figure" varchar(254) COLLATE "default",
"text" varchar(254) COLLATE "default",
"mp3" bytea,
"modified" date
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS "public"."users";
CREATE TABLE "public"."users" (
"wx" varchar(28) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"credential" varchar(32) COLLATE "default",
"city" varchar(4) COLLATE "default",
"area" varchar(10) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"opr" int2 DEFAULT 0,
"oprat" varchar(4) COLLATE "default",
"oprname" varchar(10) COLLATE "default",
"adm" bool DEFAULT false
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."orders_id_seq" OWNED BY "orders"."id";
ALTER SEQUENCE "public"."repays_id_seq" OWNED BY "repays"."id";
ALTER SEQUENCE "public"."shops_id_seq1" OWNED BY "shops"."id";

-- ----------------------------
-- Primary Key structure for table items
-- ----------------------------
ALTER TABLE "public"."items" ADD PRIMARY KEY ("shopid", "name");

-- ----------------------------
-- Primary Key structure for table orders
-- ----------------------------
ALTER TABLE "public"."orders" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table repays
-- ----------------------------
ALTER TABLE "public"."repays" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table shops
-- ----------------------------
ALTER TABLE "public"."shops" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table users
-- ----------------------------
ALTER TABLE "public"."users" ADD PRIMARY KEY ("wx");

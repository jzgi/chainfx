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

Date: 2017-11-15 14:35:06
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
-- Table structure for items
-- ----------------------------
DROP TABLE IF EXISTS "public"."items";
CREATE TABLE "public"."items" (
"shopid" varchar(4) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default" NOT NULL,
"descr" varchar(30) COLLATE "default",
"mains" varchar(20) COLLATE "default",
"icon" bytea,
"unit" varchar(4) COLLATE "default",
"price" money,
"min" int2,
"step" int2,
"max" int2,
"customs" varchar(30)[] COLLATE "default",
"status" int2,
"idx" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for orders
-- ----------------------------
DROP TABLE IF EXISTS "public"."orders";
CREATE TABLE "public"."orders" (
"id" int4 DEFAULT nextval('orders_id_seq'::regclass) NOT NULL,
"rev" int2,
"shopid" varchar(4) COLLATE "default",
"shopname" varchar(10) COLLATE "default",
"wx" varchar(28) COLLATE "default",
"name" varchar(10) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"city" varchar(6) COLLATE "default",
"area" varchar(10) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"items" jsonb,
"min" money,
"notch" money,
"off" money,
"total" money,
"cash" money DEFAULT 0,
"paid" timestamp(6),
"preparing" bool,
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
"id" varchar(4) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default",
"city" varchar(6) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"icon" bytea,
"schedule" varchar(20) COLLATE "default",
"flags" varchar(10)[] COLLATE "default",
"areas" varchar(10)[] COLLATE "default",
"min" money,
"notch" money,
"off" money,
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

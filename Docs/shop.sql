/*
Navicat PGSQL Data Transfer

Source Server         : 106.14.45.109
Source Server Version : 90505
Source Host           : 106.14.45.109:5432
Source Database       : shop
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90505
File Encoding         : 65001

Date: 2017-05-15 00:08:37
*/


-- ----------------------------
-- Sequence structure for evtq_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."evtq_id_seq";
CREATE SEQUENCE "public"."evtq_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 100;

-- ----------------------------
-- Sequence structure for orders_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."orders_id_seq";
CREATE SEQUENCE "public"."orders_id_seq"
 INCREMENT 1
 MINVALUE 1000
 MAXVALUE 9223372036854775807
 START 1096
 CACHE 8;
SELECT setval('"public"."orders_id_seq"', 1096, true);

-- ----------------------------
-- Table structure for evtq
-- ----------------------------
DROP TABLE IF EXISTS "public"."evtq";
CREATE TABLE "public"."evtq" (
"id" int8 DEFAULT nextval('evtq_id_seq'::regclass) NOT NULL,
"name" varchar(40) COLLATE "default",
"shard" varchar(20) COLLATE "default",
"arg" varchar(40) COLLATE "default",
"type" varchar(40) COLLATE "default",
"body" bytea,
"time" timestamp(6)
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for evtu
-- ----------------------------
DROP TABLE IF EXISTS "public"."evtu";
CREATE TABLE "public"."evtu" (
"peerid" varchar(20) COLLATE "default" NOT NULL,
"evtid" int8
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for items
-- ----------------------------
DROP TABLE IF EXISTS "public"."items";
CREATE TABLE "public"."items" (
"shopid" varchar(6) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default" NOT NULL,
"descr" varchar(30) COLLATE "default",
"icon" bytea,
"unit" varchar(8) COLLATE "default",
"price" money,
"min" int2,
"step" int2,
"status" int2,
"qty" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for orders
-- ----------------------------
DROP TABLE IF EXISTS "public"."orders";
CREATE TABLE "public"."orders" (
"id" int8 DEFAULT nextval('orders_id_seq'::regclass) NOT NULL,
"shopname" varchar(10) COLLATE "default",
"shopid" varchar(6) COLLATE "default",
"custname" varchar(10) COLLATE "default",
"custwx" varchar(28) COLLATE "default",
"custtel" varchar(11) COLLATE "default",
"custdistr" varchar(6) COLLATE "default",
"custaddr" varchar(20) COLLATE "default",
"detail" jsonb,
"total" money,
"created" timestamp(6),
"paid" timestamp(6),
"pack" varchar(4) COLLATE "default",
"packtel" varchar(11) COLLATE "default",
"packed" timestamp(6),
"dvrat" varchar(6) COLLATE "default",
"dvr" varchar(4) COLLATE "default",
"dvrtel" varchar(11) COLLATE "default",
"dvred" timestamp(6),
"closed" timestamp(6),
"status" int2,
"prepay_id" varchar(60) COLLATE "default",
"repaid" timestamp(6),
"note" varchar(20) COLLATE "default",
"custcity" varchar(6) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for repays
-- ----------------------------
DROP TABLE IF EXISTS "public"."repays";
CREATE TABLE "public"."repays" (
"term" int4 NOT NULL,
"shopid" varchar(6) COLLATE "default",
"amount" money,
"status" int2,
"till" timestamp(6),
"created" timestamp(6),
"transfered" timestamp(6),
"total" money,
"orders" int4,
"city" varchar(4) COLLATE "default",
"mgrwx" varchar(28) COLLATE "default",
"creator" varchar(4) COLLATE "default",
"shopname" varchar(10) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for shops
-- ----------------------------
DROP TABLE IF EXISTS "public"."shops";
CREATE TABLE "public"."shops" (
"id" varchar(6) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default",
"descr" varchar(20) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"city" varchar(6) COLLATE "default",
"distr" varchar(6) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"x" float8,
"y" float8,
"lic" varchar(20) COLLATE "default",
"created" timestamp(6),
"status" int2,
"icon" bytea,
"mgrid" varchar(11) COLLATE "default",
"mgrwx" varchar(28) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS "public"."users";
CREATE TABLE "public"."users" (
"wx" varchar(28) COLLATE "default" NOT NULL,
"nickname" varchar(10) COLLATE "default",
"credential" varchar(32) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"city" varchar(4) COLLATE "default",
"distr" varchar(6) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"created" timestamp(6),
"oprat" varchar(6) COLLATE "default",
"sprat" varchar(4) COLLATE "default",
"adm" bool DEFAULT false,
"id" varchar(11) COLLATE "default",
"opr" int2,
"name" varchar(4) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."orders_id_seq" OWNED BY "orders"."id";

-- ----------------------------
-- Primary Key structure for table evtq
-- ----------------------------
ALTER TABLE "public"."evtq" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table evtu
-- ----------------------------
ALTER TABLE "public"."evtu" ADD PRIMARY KEY ("peerid");

-- ----------------------------
-- Primary Key structure for table items
-- ----------------------------
ALTER TABLE "public"."items" ADD PRIMARY KEY ("shopid", "name");

-- ----------------------------
-- Primary Key structure for table users
-- ----------------------------
ALTER TABLE "public"."users" ADD PRIMARY KEY ("wx");

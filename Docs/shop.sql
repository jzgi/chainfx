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

Date: 2017-03-21 12:27:22
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
"descr" varchar(20) COLLATE "default",
"icon" bytea,
"unit" varchar(4) COLLATE "default",
"oprice" money,
"price" money,
"min" int2,
"step" int2,
"sold" int4,
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
"shopid" varchar(6) COLLATE "default",
"shop" varchar(10) COLLATE "default",
"shopwx" varchar(20) COLLATE "default",
"shoptel" varchar(11) COLLATE "default",
"buy" varchar(10) COLLATE "default",
"buywx" varchar(20) COLLATE "default",
"buytel" varchar(11) COLLATE "default",
"buyaddr" varbit(20),
"lines" jsonb,
"total" money,
"status" int2,
"created" timestamp(6),
"paid" timestamp(6),
"locked" timestamp(6),
"reason" varchar(20) COLLATE "default",
"cancelled" varchar(20) COLLATE "default",
"closed" timestamp(6)
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for repays
-- ----------------------------
DROP TABLE IF EXISTS "public"."repays";
CREATE TABLE "public"."repays" (
"id" int4 DEFAULT nextval('repays_id_seq'::regclass) NOT NULL,
"shopid" varchar(6) COLLATE "default",
"amount" money,
"paid" money,
"state" int4,
"status" int2,
"endorderid" int4,
"time" timestamp(6)
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
"credential" varchar(32) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"mgr" varchar(4) COLLATE "default",
"mgrwx" varchar(28) COLLATE "default",
"province" varchar(4) COLLATE "default",
"city" varchar(4) COLLATE "default",
"x" float8,
"y" float8,
"scope" int2,
"icon" bytea,
"descr" varchar(20) COLLATE "default",
"lic" varchar(20) COLLATE "default",
"enabled" bool
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
"name" varchar(4) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"credential" varchar(32) COLLATE "default",
"province" varchar(4) COLLATE "default",
"city" varchar(4) COLLATE "default",
"created" timestamp(6),
"shopid" varchar(6) COLLATE "default",
"admin" int2,
"addup" money
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."orders_id_seq" OWNED BY "orders"."id";
ALTER SEQUENCE "public"."repays_id_seq" OWNED BY "repays"."id";

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

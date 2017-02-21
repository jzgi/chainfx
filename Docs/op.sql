/*
Navicat PGSQL Data Transfer

Source Server         : 106.14.45.109
Source Server Version : 90505
Source Host           : 106.14.45.109:5432
Source Database       : op
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90505
File Encoding         : 65001

Date: 2017-02-21 14:33:38
*/


-- ----------------------------
-- Sequence structure for orders_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."orders_id_seq";
CREATE SEQUENCE "public"."orders_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 3
 CACHE 1;
SELECT setval('"public"."orders_id_seq"', 3, true);

-- ----------------------------
-- Sequence structure for pays_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."pays_id_seq";
CREATE SEQUENCE "public"."pays_id_seq"
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
-- Table structure for items
-- ----------------------------
DROP TABLE IF EXISTS "public"."items";
CREATE TABLE "public"."items" (
"shopid" varchar(6) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default" NOT NULL,
"descr" varchar(100) COLLATE "default",
"price" money,
"unit" varchar(4) COLLATE "default",
"oprice" money,
"icon" bytea,
"min" int2,
"step" int2,
"sold" int4,
"enabled" bool,
"capacity" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of items
-- ----------------------------
INSERT INTO "public"."items" VALUES ('360001', '全麦馒头', '全麦馒头', '$2.00', '个', '$2.50', null, '20', '5', '40', 't', null);
INSERT INTO "public"."items" VALUES ('360001', '全麦馒头加', '全麦馒头加', '$3.00', '个', '$3.50', null, '10', '5', '10', 't', null);

-- ----------------------------
-- Table structure for orders
-- ----------------------------
DROP TABLE IF EXISTS "public"."orders";
CREATE TABLE "public"."orders" (
"id" int4 DEFAULT nextval('orders_id_seq'::regclass) NOT NULL,
"shopid" varchar(6) COLLATE "default",
"shop" varchar(10) COLLATE "default",
"shopwx" varchar(20) COLLATE "default",
"user" varchar(10) COLLATE "default",
"userwx" varchar(20) COLLATE "default",
"created" timestamp(6),
"pend" varchar(20) COLLATE "default",
"fixed" timestamp(6),
"detail" jsonb,
"payid" varchar(20) COLLATE "default",
"total" money,
"delivered" timestamp(6) NOT NULL,
"closed" timestamp(6),
"state" int4,
"status" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of orders
-- ----------------------------
INSERT INTO "public"."orders" VALUES ('3', '360001', '黄田', 'hismichael', null, null, null, null, null, null, null, null, '2017-02-17 17:08:16', null, null, '0');

-- ----------------------------
-- Table structure for pays
-- ----------------------------
DROP TABLE IF EXISTS "public"."pays";
CREATE TABLE "public"."pays" (
"id" int8 DEFAULT nextval('pays_id_seq'::regclass) NOT NULL,
"gateway" varchar(30) COLLATE "default",
"amount" money
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of pays
-- ----------------------------

-- ----------------------------
-- Table structure for repays
-- ----------------------------
DROP TABLE IF EXISTS "public"."repays";
CREATE TABLE "public"."repays" (
"id" int4 DEFAULT nextval('repays_id_seq'::regclass) NOT NULL,
"shopid" varchar(6) COLLATE "default",
"amount" money,
"paid" money,
"states" int4,
"status" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of repays
-- ----------------------------

-- ----------------------------
-- Table structure for shops
-- ----------------------------
DROP TABLE IF EXISTS "public"."shops";
CREATE TABLE "public"."shops" (
"id" varchar(6) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default",
"credential" char(32) COLLATE "default",
"city" varchar(4) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"status" int2,
"x" float8,
"y" float8,
"wx" varchar(255) COLLATE "default",
"descr" varchar(20) COLLATE "default",
"icon" bytea,
"license" varchar(20) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of shops
-- ----------------------------
INSERT INTO "public"."shops" VALUES ('360001', '黄田', '3391AA55C0221F8DEE00B7594DE3B378', '南昌', null, null, '115.9', '28.6', null, null, null, null);
INSERT INTO "public"."shops" VALUES ('360003', '涂红妹粗粮馒头', 'E690820BC5D7E1F1CEA517914FDB29E8', null, null, null, '115.9', '28.6', null, null, null, null);
INSERT INTO "public"."shops" VALUES ('360004', '刘玉红粮油', 'A5FB36057110F41CCD5392BB038EF259', null, null, null, '115.9', '28.6', null, null, null, null);
INSERT INTO "public"."shops" VALUES ('360005', '黄燕理疗康复中心', '0967E4E07F6526709996BC3BA923BA49', null, null, null, '115.9', '28.6', null, null, null, null);

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS "public"."users";
CREATE TABLE "public"."users" (
"wx" varchar(20) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default",
"nickname" varchar(10) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"orderon" date,
"orderup" money
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of users
-- ----------------------------

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."orders_id_seq" OWNED BY "orders"."id";
ALTER SEQUENCE "public"."pays_id_seq" OWNED BY "pays"."id";
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
-- Primary Key structure for table pays
-- ----------------------------
ALTER TABLE "public"."pays" ADD PRIMARY KEY ("id");

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

-- ----------------------------
-- Foreign Key structure for table "public"."items"
-- ----------------------------
ALTER TABLE "public"."items" ADD FOREIGN KEY ("shopid") REFERENCES "public"."shops" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

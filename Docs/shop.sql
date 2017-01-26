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

Date: 2017-01-25 10:48:01
*/


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
-- Table structure for buyers
-- ----------------------------
DROP TABLE IF EXISTS "public"."buyers";
CREATE TABLE "public"."buyers" (
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
-- Table structure for items
-- ----------------------------
DROP TABLE IF EXISTS "public"."items";
CREATE TABLE "public"."items" (
"shopid" char(6) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default" NOT NULL,
"descript" varchar(100) COLLATE "default",
"price" money
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for orders
-- ----------------------------
DROP TABLE IF EXISTS "public"."orders";
CREATE TABLE "public"."orders" (
"id" char(16) COLLATE "default" NOT NULL,
"buyerid" varchar(20) COLLATE "default",
"buyer" varchar(20) COLLATE "default",
"time" timestamp(6)
)
WITH (OIDS=FALSE)

;

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
-- Table structure for shops
-- ----------------------------
DROP TABLE IF EXISTS "public"."shops";
CREATE TABLE "public"."shops" (
"id" char(6) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default",
"credential" char(32) COLLATE "default",
"prov" varchar(4) COLLATE "default",
"city" varchar(4) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"status" int2,
"x" float8,
"y" float8,
"wx" varchar(255) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."pays_id_seq" OWNED BY "pays"."id";

-- ----------------------------
-- Primary Key structure for table buyers
-- ----------------------------
ALTER TABLE "public"."buyers" ADD PRIMARY KEY ("wx");

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
-- Primary Key structure for table shops
-- ----------------------------
ALTER TABLE "public"."shops" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Foreign Key structure for table "public"."items"
-- ----------------------------
ALTER TABLE "public"."items" ADD FOREIGN KEY ("shopid") REFERENCES "public"."shops" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

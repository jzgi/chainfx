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

Date: 2016-12-25 19:19:42
*/


-- ----------------------------
-- Table structure for customers
-- ----------------------------
DROP TABLE IF EXISTS "public"."customers";
CREATE TABLE "public"."customers" (
"shopid" char(6) COLLATE "default" NOT NULL,
"openid" varchar(20) COLLATE "default",
"name" varchar(10) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of customers
-- ----------------------------

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
-- Records of items
-- ----------------------------

-- ----------------------------
-- Table structure for shops
-- ----------------------------
DROP TABLE IF EXISTS "public"."shops";
CREATE TABLE "public"."shops" (
"id" char(6) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default",
"credential" char(32) COLLATE "default",
"location" point,
"district" varchar(10) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of shops
-- ----------------------------

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table customers
-- ----------------------------
ALTER TABLE "public"."customers" ADD PRIMARY KEY ("shopid");

-- ----------------------------
-- Primary Key structure for table items
-- ----------------------------
ALTER TABLE "public"."items" ADD PRIMARY KEY ("shopid", "name");

-- ----------------------------
-- Primary Key structure for table shops
-- ----------------------------
ALTER TABLE "public"."shops" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Foreign Key structure for table "public"."customers"
-- ----------------------------
ALTER TABLE "public"."customers" ADD FOREIGN KEY ("shopid") REFERENCES "public"."shops" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Key structure for table "public"."items"
-- ----------------------------
ALTER TABLE "public"."items" ADD FOREIGN KEY ("shopid") REFERENCES "public"."shops" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

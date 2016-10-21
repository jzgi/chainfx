/*
Navicat PGSQL Data Transfer

Source Server         : Aliyun
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : www
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-10-21 12:37:46
*/


-- ----------------------------
-- Sequence structure for links_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."links_id_seq";
CREATE SEQUENCE "public"."links_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."links_id_seq"', 1, true);

-- ----------------------------
-- Sequence structure for msgq_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."msgq_id_seq";
CREATE SEQUENCE "public"."msgq_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Table structure for cats
-- ----------------------------
DROP TABLE IF EXISTS "public"."cats";
CREATE TABLE "public"."cats" (
"id" int2 DEFAULT nextval('links_id_seq'::regclass) NOT NULL,
"title" text COLLATE "default",
"img" bytea,
"disabled" bool,
"filter" varchar(10) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for msgq
-- ----------------------------
DROP TABLE IF EXISTS "public"."msgq";
CREATE TABLE "public"."msgq" (
"id" int4 DEFAULT nextval('msgq_id_seq'::regclass) NOT NULL,
"time" timestamp(6),
"topic" varchar(20) COLLATE "default",
"shard" varchar(10) COLLATE "default",
"body" bytea
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for msgu
-- ----------------------------
DROP TABLE IF EXISTS "public"."msgu";
CREATE TABLE "public"."msgu" (
"addr" varchar(45) COLLATE "default" NOT NULL,
"lastid" int4
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."links_id_seq" OWNED BY "cats"."id";
ALTER SEQUENCE "public"."msgq_id_seq" OWNED BY "msgq"."id";

-- ----------------------------
-- Primary Key structure for table msgq
-- ----------------------------
ALTER TABLE "public"."msgq" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table msgu
-- ----------------------------
ALTER TABLE "public"."msgu" ADD PRIMARY KEY ("addr");

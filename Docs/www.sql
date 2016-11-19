/*
Navicat PGSQL Data Transfer

Source Server         : 60.205.104.239
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : www
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-11-19 23:13:25
*/


-- ----------------------------
-- Sequence structure for "public"."links_id_seq"
-- ----------------------------
DROP SEQUENCE "public"."links_id_seq";
CREATE SEQUENCE "public"."links_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Sequence structure for "public"."msgq_id_seq"
-- ----------------------------
DROP SEQUENCE "public"."msgq_id_seq";
CREATE SEQUENCE "public"."msgq_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Table structure for "public"."cats"
-- ----------------------------
DROP TABLE "public"."cats";
CREATE TABLE "public"."cats" (
"id" int2 DEFAULT nextval('links_id_seq'::regclass) NOT NULL,
"title" text,
"img" bytea,
"disabled" bool,
"filter" varchar(10)
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of cats
-- ----------------------------
INSERT INTO "public"."cats" VALUES ('0', '全部', null, 'f', '全部');
INSERT INTO "public"."cats" VALUES ('1', '歌手', null, 'f', '歌手');
INSERT INTO "public"."cats" VALUES ('2', '演员', null, 'f', '演员');
INSERT INTO "public"."cats" VALUES ('3', '主播', null, 'f', '主播');
INSERT INTO "public"."cats" VALUES ('4', '主持人', null, 'f', '主持人');
INSERT INTO "public"."cats" VALUES ('5', '模特', null, 'f', '模特');

-- ----------------------------
-- Table structure for "public"."msgq"
-- ----------------------------
DROP TABLE "public"."msgq";
CREATE TABLE "public"."msgq" (
"id" int4 DEFAULT nextval('msgq_id_seq'::regclass) NOT NULL,
"time" timestamp(6),
"topic" varchar(20),
"shard" varchar(10),
"body" bytea
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of msgq
-- ----------------------------

-- ----------------------------
-- Table structure for "public"."msgu"
-- ----------------------------
DROP TABLE "public"."msgu";
CREATE TABLE "public"."msgu" (
"addr" varchar(45) NOT NULL,
"lastid" int4
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of msgu
-- ----------------------------

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."links_id_seq" OWNED BY "cats"."id";
ALTER SEQUENCE "public"."msgq_id_seq" OWNED BY "msgq"."id";

-- ----------------------------
-- Primary Key structure for table "public"."msgq"
-- ----------------------------
ALTER TABLE "public"."msgq" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table "public"."msgu"
-- ----------------------------
ALTER TABLE "public"."msgu" ADD PRIMARY KEY ("addr");

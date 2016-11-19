/*
Navicat PGSQL Data Transfer

Source Server         : 60.205.104.239
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : dir
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-11-19 23:13:47
*/


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
-- Table structure for "public"."users"
-- ----------------------------
DROP TABLE "public"."users";
CREATE TABLE "public"."users" (
"id" char(11) NOT NULL,
"credential" char(32),
"name" varchar(4),
"date" timestamp,
"fame" bool,
"brand" bool
)
WITH (OIDS=FALSE)

;
COMMENT ON COLUMN "public"."users"."id" IS '用户登录名，通常为用户的手机号码';
COMMENT ON COLUMN "public"."users"."credential" IS '密码加密存储';
COMMENT ON COLUMN "public"."users"."name" IS '实名';
COMMENT ON COLUMN "public"."users"."date" IS '用户注册时间';
COMMENT ON COLUMN "public"."users"."fame" IS '红人标志';
COMMENT ON COLUMN "public"."users"."brand" IS '品牌商标识';

-- ----------------------------
-- Records of users
-- ----------------------------
INSERT INTO "public"."users" VALUES ('13307082524', '4b652bc722ea0f5824fe075fbd1ce5b5', '孔洪', null, null, null);
INSERT INTO "public"."users" VALUES ('13307083456', '7738e757a2fbe7dc2ad66b3764908687', '何东亮', null, null, null);
INSERT INTO "public"."users" VALUES ('18610745739', '23a8d9e4d4cc770869d0c018d434f2b8', '邹正良', null, null, null);
INSERT INTO "public"."users" VALUES ('18970072664', '3195133d23c5a058b776c2b73c147f3d', '黄田', null, null, null);

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."msgq_id_seq" OWNED BY "msgq"."id";

-- ----------------------------
-- Primary Key structure for table "public"."msgq"
-- ----------------------------
ALTER TABLE "public"."msgq" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table "public"."msgu"
-- ----------------------------
ALTER TABLE "public"."msgu" ADD PRIMARY KEY ("addr");

-- ----------------------------
-- Primary Key structure for table "public"."users"
-- ----------------------------
ALTER TABLE "public"."users" ADD PRIMARY KEY ("id");

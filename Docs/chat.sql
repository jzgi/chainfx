/*
Navicat PGSQL Data Transfer

Source Server         : 60.205.104.239
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : chat
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-11-19 22:55:50
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
-- Table structure for "public"."chats"
-- ----------------------------
DROP TABLE "public"."chats";
CREATE TABLE "public"."chats" (
"to" char(11)[] NOT NULL,
"from" char(11)[] NOT NULL,
"content" jsonb,
"subtype" int2,
"status" int2
)
WITH (OIDS=FALSE)

;
COMMENT ON TABLE "public"."chats" IS '消息和聊天记录';
COMMENT ON COLUMN "public"."chats"."to" IS '收方用户号';
COMMENT ON COLUMN "public"."chats"."from" IS '发方用户号';
COMMENT ON COLUMN "public"."chats"."content" IS '消息数组 （time,msg）';

-- ----------------------------
-- Records of chats
-- ----------------------------

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
ALTER SEQUENCE "public"."msgq_id_seq" OWNED BY "msgq"."id";

-- ----------------------------
-- Primary Key structure for table "public"."chats"
-- ----------------------------
ALTER TABLE "public"."chats" ADD PRIMARY KEY ("to", "from");

-- ----------------------------
-- Primary Key structure for table "public"."msgq"
-- ----------------------------
ALTER TABLE "public"."msgq" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table "public"."msgu"
-- ----------------------------
ALTER TABLE "public"."msgu" ADD PRIMARY KEY ("addr");

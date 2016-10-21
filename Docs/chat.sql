/*
Navicat PGSQL Data Transfer

Source Server         : Aliyun
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : chat
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-10-21 12:36:49
*/


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
-- Table structure for chats
-- ----------------------------
DROP TABLE IF EXISTS "public"."chats";
CREATE TABLE "public"."chats" (
"to" char(11)[] COLLATE "default" NOT NULL,
"from" char(11)[] COLLATE "default" NOT NULL,
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
ALTER SEQUENCE "public"."msgq_id_seq" OWNED BY "msgq"."id";

-- ----------------------------
-- Primary Key structure for table chats
-- ----------------------------
ALTER TABLE "public"."chats" ADD PRIMARY KEY ("to", "from");

-- ----------------------------
-- Primary Key structure for table msgq
-- ----------------------------
ALTER TABLE "public"."msgq" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table msgu
-- ----------------------------
ALTER TABLE "public"."msgu" ADD PRIMARY KEY ("addr");

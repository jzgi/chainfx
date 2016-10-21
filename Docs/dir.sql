/*
Navicat PGSQL Data Transfer

Source Server         : Aliyun
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : dir
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-10-21 12:37:27
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
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS "public"."users";
CREATE TABLE "public"."users" (
"id" char(11) COLLATE "default" NOT NULL,
"credential" char(16) COLLATE "default",
"name" varchar(4) COLLATE "default",
"date" timestamp(0),
"fame" bool,
"brand" bool,
"admin" bool,
"friends" jsonb,
"favs" jsonb,
"favposts" jsonb
)
WITH (OIDS=FALSE)

;
COMMENT ON COLUMN "public"."users"."id" IS '用户登录名，通常为用户的手机号码';
COMMENT ON COLUMN "public"."users"."credential" IS '密码加密存储';
COMMENT ON COLUMN "public"."users"."name" IS '实名';
COMMENT ON COLUMN "public"."users"."date" IS '用户注册时间';
COMMENT ON COLUMN "public"."users"."fame" IS '红人标志';
COMMENT ON COLUMN "public"."users"."brand" IS '品牌商标识';
COMMENT ON COLUMN "public"."users"."admin" IS '管理员标识';
COMMENT ON COLUMN "public"."users"."friends" IS '好友列表';
COMMENT ON COLUMN "public"."users"."favs" IS '关注的用户列表';
COMMENT ON COLUMN "public"."users"."favposts" IS '帖子收藏列表';

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."msgq_id_seq" OWNED BY "msgq"."id";

-- ----------------------------
-- Primary Key structure for table msgq
-- ----------------------------
ALTER TABLE "public"."msgq" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table msgu
-- ----------------------------
ALTER TABLE "public"."msgu" ADD PRIMARY KEY ("addr");

-- ----------------------------
-- Primary Key structure for table users
-- ----------------------------
ALTER TABLE "public"."users" ADD PRIMARY KEY ("id");

/*
Navicat PGSQL Data Transfer

Source Server         : Aliyun
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : cont
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-10-21 12:37:07
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
-- Sequence structure for posts_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."posts_id_seq";
CREATE SEQUENCE "public"."posts_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."posts_id_seq"', 1, true);

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
-- Table structure for notices
-- ----------------------------
DROP TABLE IF EXISTS "public"."notices";
CREATE TABLE "public"."notices" (
"id" int4 NOT NULL,
"loc" varchar(10) COLLATE "default",
"authorid" char(11) COLLATE "default",
"author" varchar(20) COLLATE "default",
"date" timestamp(0),
"duedate" timestamp(0),
"subject" text COLLATE "default",
"tel" char(11) COLLATE "default",
"text" text COLLATE "default",
"reads" int4,
"apps" jsonb
)
WITH (OIDS=FALSE)

;
COMMENT ON TABLE "public"."notices" IS '通告表';
COMMENT ON COLUMN "public"."notices"."loc" IS '通告地域，如北京';
COMMENT ON COLUMN "public"."notices"."duedate" IS '截止日期';

-- ----------------------------
-- Table structure for posts
-- ----------------------------
DROP TABLE IF EXISTS "public"."posts";
CREATE TABLE "public"."posts" (
"id" int4 DEFAULT nextval('posts_id_seq'::regclass) NOT NULL,
"time" timestamp(6),
"authorid" char(11) COLLATE "default",
"author" varchar(20) COLLATE "default",
"commentable" bool,
"comments" jsonb,
"shared" int4,
"text" text COLLATE "default",
"mset" varchar(9) COLLATE "default",
"m0" bytea,
"m1" bytea,
"m2" bytea,
"m3" bytea,
"m4" bytea,
"m5" bytea,
"m6" bytea,
"m7" bytea,
"m8" bytea
)
WITH (OIDS=FALSE)

;
COMMENT ON TABLE "public"."posts" IS '动态表（帖子）';
COMMENT ON COLUMN "public"."posts"."id" IS '动态ID';
COMMENT ON COLUMN "public"."posts"."time" IS '发布时间';
COMMENT ON COLUMN "public"."posts"."authorid" IS '发布者';
COMMENT ON COLUMN "public"."posts"."comments" IS '评论者信息，包含评论者ID、评论时间、评论的文字内容。图标评论可以转化为文字处理，转化的过程在客户端进行';
COMMENT ON COLUMN "public"."posts"."mset" IS '媒体字段标志位';

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."msgq_id_seq" OWNED BY "msgq"."id";
ALTER SEQUENCE "public"."posts_id_seq" OWNED BY "posts"."id";

-- ----------------------------
-- Primary Key structure for table msgq
-- ----------------------------
ALTER TABLE "public"."msgq" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table msgu
-- ----------------------------
ALTER TABLE "public"."msgu" ADD PRIMARY KEY ("addr");

-- ----------------------------
-- Primary Key structure for table notices
-- ----------------------------
ALTER TABLE "public"."notices" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table posts
-- ----------------------------
ALTER TABLE "public"."posts" ADD PRIMARY KEY ("id");

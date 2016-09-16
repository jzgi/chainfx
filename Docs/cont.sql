/*
Navicat PGSQL Data Transfer

Source Server         : aliyun
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : cont
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-09-16 10:00:01
*/


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
"subtype" int2,
"subject" text COLLATE "default",
"tel" char(11) COLLATE "default",
"wechat" char(20) COLLATE "default",
"remark" text COLLATE "default",
"reads" int4,
"joins" jsonb
)
WITH (OIDS=FALSE)

;
COMMENT ON TABLE "public"."notices" IS '通告表';
COMMENT ON COLUMN "public"."notices"."loc" IS '通告地域，如北京';
COMMENT ON COLUMN "public"."notices"."duedate" IS '截止日期';
COMMENT ON COLUMN "public"."notices"."subtype" IS '通告子类型';

-- ----------------------------
-- Records of notices
-- ----------------------------

-- ----------------------------
-- Table structure for posts
-- ----------------------------
DROP TABLE IF EXISTS "public"."posts";
CREATE TABLE "public"."posts" (
"id" int4 NOT NULL,
"date" timestamp(6),
"authorid" char(11) COLLATE "default",
"author" varchar(20) COLLATE "default",
"text" text COLLATE "default",
"m0" bytea,
"m1" bytea,
"m2" bytea,
"m3" bytea,
"m4" bytea,
"m5" bytea,
"m6" bytea,
"m7" bytea,
"m8" bytea,
"m9" bytea,
"mbits" varbit(10),
"comments" jsonb
)
WITH (OIDS=FALSE)

;
COMMENT ON TABLE "public"."posts" IS '动态表（帖子）';
COMMENT ON COLUMN "public"."posts"."id" IS '动态ID';
COMMENT ON COLUMN "public"."posts"."date" IS '发布时间';
COMMENT ON COLUMN "public"."posts"."authorid" IS '发布者';
COMMENT ON COLUMN "public"."posts"."mbits" IS '媒体字段标志位';
COMMENT ON COLUMN "public"."posts"."comments" IS '评论者信息，包含评论者ID、评论时间、评论的文字内容。图标评论可以转化为文字处理，转化的过程在客户端进行';

-- ----------------------------
-- Records of posts
-- ----------------------------

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table notices
-- ----------------------------
ALTER TABLE "public"."notices" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table posts
-- ----------------------------
ALTER TABLE "public"."posts" ADD PRIMARY KEY ("id");

/*
Navicat PGSQL Data Transfer

Source Server         : Aliyun
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : notice
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-09-09 16:09:16
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
-- Alter Sequences Owned By 
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table notices
-- ----------------------------
ALTER TABLE "public"."notices" ADD PRIMARY KEY ("id");

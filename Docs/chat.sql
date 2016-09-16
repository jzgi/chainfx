/*
Navicat PGSQL Data Transfer

Source Server         : aliyun
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : chat
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-09-16 09:59:06
*/


-- ----------------------------
-- Table structure for msgs
-- ----------------------------
DROP TABLE IF EXISTS "public"."msgs";
CREATE TABLE "public"."msgs" (
"to" char(11)[] COLLATE "default" NOT NULL,
"from" char(11)[] COLLATE "default" NOT NULL,
"content" jsonb,
"subtype" int2,
"status" int2
)
WITH (OIDS=FALSE)

;
COMMENT ON TABLE "public"."msgs" IS '消息和聊天记录';
COMMENT ON COLUMN "public"."msgs"."to" IS '收方用户号';
COMMENT ON COLUMN "public"."msgs"."from" IS '发方用户号';
COMMENT ON COLUMN "public"."msgs"."content" IS '消息数组 （time,msg）';

-- ----------------------------
-- Records of msgs
-- ----------------------------

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table msgs
-- ----------------------------
ALTER TABLE "public"."msgs" ADD PRIMARY KEY ("to", "from");

/*
Navicat PGSQL Data Transfer

Source Server         : aliyun
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : dir
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-09-16 10:00:52
*/


-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS "public"."users";
CREATE TABLE "public"."users" (
"id" char(11) COLLATE "default" NOT NULL,
"credential" char(32) COLLATE "default",
"name" varchar(4) COLLATE "default",
"date" timestamp(0),
"fame" bool,
"brand" bool,
"admin" bool,
"friends" jsonb,
"favs" jsonb,
"favposts" jsonb,
"nick" varchar(20) COLLATE "default"
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
COMMENT ON COLUMN "public"."users"."nick" IS '显示名网名';

-- ----------------------------
-- Records of users
-- ----------------------------

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table users
-- ----------------------------
ALTER TABLE "public"."users" ADD PRIMARY KEY ("id");

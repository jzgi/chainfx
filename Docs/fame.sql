/*
Navicat PGSQL Data Transfer

Source Server         : Aliyun
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : fame
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-09-09 16:09:43
*/


-- ----------------------------
-- Table structure for fames
-- ----------------------------
DROP TABLE IF EXISTS "public"."fames";
CREATE TABLE "public"."fames" (
"id" char(11) COLLATE "default" NOT NULL,
"name" varchar(20) COLLATE "default",
"quote" varchar(20) COLLATE "default",
"sex" char(1) COLLATE "default",
"icon" bytea,
"birthday" date,
"qq" varchar(11) COLLATE "default",
"wechat" varchar(20) COLLATE "default",
"email" varchar(30) COLLATE "default",
"province" varchar(4) COLLATE "default",
"city" varchar(4) COLLATE "default",
"rating" int2,
"height" int2,
"weight" int2,
"bust" int2,
"waist" int2,
"hip" int2,
"cup" int2,
"styles" text COLLATE "default",
"skills" text COLLATE "default",
"remark" text COLLATE "default",
"sites" json,
"friends" json,
"awards" text[] COLLATE "default",
"date" date
)
WITH (OIDS=FALSE)

;
COMMENT ON TABLE "public"."fames" IS '红人信息表';
COMMENT ON COLUMN "public"."fames"."id" IS '红人的用户ID';
COMMENT ON COLUMN "public"."fames"."name" IS '红人昵称,最长允许7个汉字';
COMMENT ON COLUMN "public"."fames"."quote" IS '红人签名';
COMMENT ON COLUMN "public"."fames"."sex" IS '性别，0表示男，1表示女';
COMMENT ON COLUMN "public"."fames"."icon" IS '红人头像';
COMMENT ON COLUMN "public"."fames"."birthday" IS '生日';
COMMENT ON COLUMN "public"."fames"."qq" IS 'qq号';
COMMENT ON COLUMN "public"."fames"."wechat" IS '微信号';
COMMENT ON COLUMN "public"."fames"."email" IS '邮箱';
COMMENT ON COLUMN "public"."fames"."province" IS '省份';
COMMENT ON COLUMN "public"."fames"."city" IS '城市';
COMMENT ON COLUMN "public"."fames"."rating" IS '级别';
COMMENT ON COLUMN "public"."fames"."height" IS '身高';
COMMENT ON COLUMN "public"."fames"."weight" IS '体重';
COMMENT ON COLUMN "public"."fames"."bust" IS '胸围';
COMMENT ON COLUMN "public"."fames"."waist" IS '腰围';
COMMENT ON COLUMN "public"."fames"."hip" IS '臀围';
COMMENT ON COLUMN "public"."fames"."cup" IS '罩杯，只有女性才填写这条信息';
COMMENT ON COLUMN "public"."fames"."styles" IS '风格标签';
COMMENT ON COLUMN "public"."fames"."skills" IS '技能标签';
COMMENT ON COLUMN "public"."fames"."remark" IS '备注';
COMMENT ON COLUMN "public"."fames"."sites" IS '社交平台信息，包括社交平台名称、url';
COMMENT ON COLUMN "public"."fames"."friends" IS '好友信息';
COMMENT ON COLUMN "public"."fames"."awards" IS '殊荣';
COMMENT ON COLUMN "public"."fames"."date" IS '注册日期';

-- ----------------------------
-- Records of fames
-- ----------------------------

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table fames
-- ----------------------------
ALTER TABLE "public"."fames" ADD PRIMARY KEY ("id");

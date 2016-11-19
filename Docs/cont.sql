/*
Navicat PGSQL Data Transfer

Source Server         : 60.205.104.239
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : cont
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-11-19 23:14:07
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
-- Sequence structure for "public"."notices_id_seq"
-- ----------------------------
DROP SEQUENCE "public"."notices_id_seq";
CREATE SEQUENCE "public"."notices_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 16
 CACHE 12;

-- ----------------------------
-- Sequence structure for "public"."posts_id_seq"
-- ----------------------------
DROP SEQUENCE "public"."posts_id_seq";
CREATE SEQUENCE "public"."posts_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 578
 CACHE 12;

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
-- Table structure for "public"."notices"
-- ----------------------------
DROP TABLE "public"."notices";
CREATE TABLE "public"."notices" (
"id" int4 DEFAULT nextval('notices_id_seq'::regclass) NOT NULL,
"loc" varchar(10),
"authorid" char(11),
"author" varchar(20),
"date" timestamp,
"duedate" timestamp,
"subject" varchar(50),
"tel" char(11),
"text" text,
"read" int4,
"apps" jsonb,
"type" varchar(20),
"shared" int4,
"commentable" bool,
"comments" jsonb
)
WITH (OIDS=FALSE)

;
COMMENT ON TABLE "public"."notices" IS '通告表';
COMMENT ON COLUMN "public"."notices"."loc" IS '通告地域，如北京';
COMMENT ON COLUMN "public"."notices"."duedate" IS '截止日期';
COMMENT ON COLUMN "public"."notices"."type" IS '通告类别';
COMMENT ON COLUMN "public"."notices"."comments" IS '评论者信息，包含评论者ID、评论时间、评论的文字内容。图标评论可以转化为文字处理，转化的过程在客户端进行';

-- ----------------------------
-- Records of notices
-- ----------------------------
INSERT INTO "public"."notices" VALUES ('1', '北京', '18970072664', '黄田', '2016-10-20 00:00:00', '2016-10-31 00:00:00', '北京拍摄宣传片', '18970072664', '北京拍摄宣传片,需要青春时尚,靓丽健康,运动型女孩, 不备注不加（时间紧任务重）群众勿扰,不在北京的勿扰', '19', '[{"user": "黄田", "userid": "18970072664"}]', null, '10', 't', '[{"text": "hahah", "time": "2016-10.23", "emoji": true, "author": "zou", "authorid": "18610745739"}]');
INSERT INTO "public"."notices" VALUES ('2', '南昌', '18970072664', '黄田', '2016-10-25 00:00:00', '2016-11-05 00:00:00', '南昌短片', '18970072664', '南昌短片', '0', null, null, '1', 't', null);
INSERT INTO "public"."notices" VALUES ('3', '长沙', '18970072664', '黄田', '2016-10-30 00:00:00', '2016-11-15 00:00:00', '明星沙龙', '18970072664', '沙龙聚会', null, null, null, null, null, null);
INSERT INTO "public"."notices" VALUES ('5', '上海', '18970072664', '黄田', '2016-11-01 17:26:56', '2016-12-12 00:00:00', '_subject_', '_telephone_', '_content_', '0', null, null, '0', 't', null);

-- ----------------------------
-- Table structure for "public"."postgrps"
-- ----------------------------
DROP TABLE "public"."postgrps";
CREATE TABLE "public"."postgrps" (
"id" varchar(20) NOT NULL,
"descript" varchar(100),
"creationdate" date,
"rating" int4,
"cat" varchar(10),
"icon" bytea
)
WITH (OIDS=FALSE)

;
COMMENT ON TABLE "public"."postgrps" IS '帖子话题';

-- ----------------------------
-- Records of postgrps
-- ----------------------------
INSERT INTO "public"."postgrps" VALUES ('星爷传奇', null, '2015-07-07', '234', '影视', null);
INSERT INTO "public"."postgrps" VALUES ('海贼王', null, '2016-11-11', '23', '娱乐', null);

-- ----------------------------
-- Table structure for "public"."posts"
-- ----------------------------
DROP TABLE "public"."posts";
CREATE TABLE "public"."posts" (
"id" int4 DEFAULT nextval('posts_id_seq'::regclass) NOT NULL,
"time" timestamp(6),
"authorid" char(11),
"author" varchar(20),
"commentable" bool,
"comments" jsonb,
"shared" int4,
"text" text,
"mset" varchar(9),
"m0" bytea,
"m1" bytea,
"m2" bytea,
"m3" bytea,
"m4" bytea,
"m5" bytea,
"m6" bytea,
"m7" bytea,
"m8" bytea,
"likes" varchar[],
"grp" varchar(20)
)
WITH (OIDS=FALSE)

;
COMMENT ON TABLE "public"."posts" IS '动态表（帖子）';
COMMENT ON COLUMN "public"."posts"."id" IS '动态ID';
COMMENT ON COLUMN "public"."posts"."time" IS '发布时间';
COMMENT ON COLUMN "public"."posts"."authorid" IS '发布者';
COMMENT ON COLUMN "public"."posts"."comments" IS '评论者信息，包含评论者ID、评论时间、评论的文字内容。图标评论可以转化为文字处理，转化的过程在客户端进行';
COMMENT ON COLUMN "public"."posts"."mset" IS '媒体字段标志位';
COMMENT ON COLUMN "public"."posts"."likes" IS '点赞人';
COMMENT ON COLUMN "public"."posts"."grp" IS '帖子的话题或类别';

-- ----------------------------
-- Records of posts
-- ----------------------------

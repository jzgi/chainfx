/*
Navicat PGSQL Data Transfer

Source Server         : aliyun
Source Server Version : 90503
Source Host           : 60.205.104.239:5432
Source Database       : www
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90503
File Encoding         : 65001

Date: 2016-09-16 10:01:33
*/


-- ----------------------------
-- Sequence structure for links_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."links_id_seq";
CREATE SEQUENCE "public"."links_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Table structure for links
-- ----------------------------
DROP TABLE IF EXISTS "public"."links";
CREATE TABLE "public"."links" (
"id" int4 DEFAULT nextval('links_id_seq'::regclass) NOT NULL,
"subtype" int2,
"title" text COLLATE "default",
"image" bytea,
"url" varchar(100) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Records of links
-- ----------------------------

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."links_id_seq" OWNED BY "links"."id";

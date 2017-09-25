/*
Navicat PGSQL Data Transfer

Source Server         : 106.14.45.109
Source Server Version : 90505
Source Host           : 106.14.45.109:5432
Source Database       : care
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90505
File Encoding         : 65001

Date: 2017-09-22 11:11:03
*/


-- ----------------------------
-- Sequence structure for orders_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."orders_id_seq";
CREATE SEQUENCE "public"."orders_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Sequence structure for repays_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."repays_id_seq";
CREATE SEQUENCE "public"."repays_id_seq"
 INCREMENT 1
 MINVALUE 1000
 MAXVALUE 9223372036854775807
 START 1293
 CACHE 16;
SELECT setval('"public"."repays_id_seq"', 1293, true);

-- ----------------------------
-- Sequence structure for shops_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."shops_id_seq";
CREATE SEQUENCE "public"."shops_id_seq"
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
"shopid" varchar(6) COLLATE "default" NOT NULL,
"wx" varchar(28) COLLATE "default" NOT NULL,
"msgs" jsonb,
"quested" timestamp(6),
"name" varchar(10) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for cities
-- ----------------------------
DROP TABLE IF EXISTS "public"."cities";
CREATE TABLE "public"."cities" (
"name" varchar(4) COLLATE "default",
"x1" float8,
"y1" float8,
"x2" float8,
"y2" float8,
"areas" jsonb,
"idx" varchar(3) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for items
-- ----------------------------
DROP TABLE IF EXISTS "public"."items";
CREATE TABLE "public"."items" (
"shopid" int2 NOT NULL,
"name" varchar(10) COLLATE "default" NOT NULL,
"descr" varchar(30) COLLATE "default",
"icon" bytea,
"price" money,
"min" int2,
"step" int2,
"max" int2,
"parts" jsonb,
"status" int2,
"unit" varchar(4) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for kicks
-- ----------------------------
DROP TABLE IF EXISTS "public"."kicks";
CREATE TABLE "public"."kicks" (
"id" int4,
"wx" varchar(28) COLLATE "default",
"city" varchar(6) COLLATE "default",
"report" varchar(50) COLLATE "default",
"creator" varchar(6) COLLATE "default",
"created" timestamp(6),
"status" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for lessons
-- ----------------------------
DROP TABLE IF EXISTS "public"."lessons";
CREATE TABLE "public"."lessons" (
"id" varchar(2) COLLATE "default" NOT NULL,
"name" varchar(20) COLLATE "default",
"refid" varchar(20) COLLATE "default",
"modified" timestamp(6)
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for orders
-- ----------------------------
DROP TABLE IF EXISTS "public"."orders";
CREATE TABLE "public"."orders" (
"id" int8 DEFAULT nextval('orders_id_seq'::regclass) NOT NULL,
"shopid" varchar(6) COLLATE "default",
"shopname" varchar(10) COLLATE "default",
"wx" varchar(28) COLLATE "default",
"name" varchar(10) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"city" varchar(6) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"details" jsonb,
"total" money,
"created" timestamp(6),
"cash" money DEFAULT 0,
"accepted" timestamp(6),
"abortly" varchar(20) COLLATE "default",
"aborted" timestamp(6),
"shipped" timestamp(6),
"note" varchar(20) COLLATE "default",
"status" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for repays
-- ----------------------------
DROP TABLE IF EXISTS "public"."repays";
CREATE TABLE "public"."repays" (
"id" int4 DEFAULT nextval('repays_id_seq'::regclass) NOT NULL,
"shopid" varchar(6) COLLATE "default",
"shop" varchar(10) COLLATE "default",
"till" date,
"orders" int4,
"total" money,
"cash" money,
"paid" timestamp(6),
"payer" varchar(6) COLLATE "default",
"status" int2 DEFAULT 0,
"err" varchar(40) COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for shops
-- ----------------------------
DROP TABLE IF EXISTS "public"."shops";
CREATE TABLE "public"."shops" (
"id" int2 DEFAULT nextval('shops_id_seq'::regclass) NOT NULL,
"name" varchar(10) COLLATE "default",
"city" varchar(6) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"x" float8,
"y" float8,
"icon" bytea,
"areas" varchar(10)[] COLLATE "default",
"mgrwx" varchar(28) COLLATE "default",
"mgrtel" varchar(11) COLLATE "default",
"mgrname" varchar(10) COLLATE "default",
"oprwx" varchar(28) COLLATE "default",
"oprtel" varchar(11) COLLATE "default",
"oprname" varchar(10) COLLATE "default",
"status" int2
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS "public"."users";
CREATE TABLE "public"."users" (
"wx" varchar(28) COLLATE "default" NOT NULL,
"name" varchar(10) COLLATE "default",
"id" varchar(11) COLLATE "default",
"credential" varchar(32) COLLATE "default",
"tel" varchar(11) COLLATE "default",
"city" varchar(4) COLLATE "default",
"addr" varchar(20) COLLATE "default",
"oprat" varchar(6) COLLATE "default",
"opr" int2 DEFAULT 0,
"adm" bool DEFAULT false,
"status" int2 DEFAULT 0 NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."orders_id_seq" OWNED BY "orders"."id";
ALTER SEQUENCE "public"."repays_id_seq" OWNED BY "repays"."id";
ALTER SEQUENCE "public"."shops_id_seq" OWNED BY "shops"."id";

-- ----------------------------
-- Primary Key structure for table chats
-- ----------------------------
ALTER TABLE "public"."chats" ADD PRIMARY KEY ("shopid", "wx");

-- ----------------------------
-- Primary Key structure for table repays
-- ----------------------------
ALTER TABLE "public"."repays" ADD PRIMARY KEY ("id");




DECLARE 

  cur CURSOR FOR SELECT id, shopid, shop, cash, status FROM orders WHERE status = 5 AND shipped < till AND cash > 0.00::money ORDER BY shopid FOR UPDATE;

  rshopid VARCHAR(6) DEFAULT NULL;
  rshop VARCHAR(10) DEFAULT NULL;
  rorders INT DEFAULT 0;
  rtotal MONEY DEFAULT 0.00; -- repay total
  rcash MONEY DEFAULT 0.00; -- repay cash
  
  gtotal MONEY DEFAULT 0.00; -- grand total
  gcash MONEY DEFAULT 0.00; -- grand cash
  
  ord RECORD;
  
BEGIN

  OPEN cur;

  LOOP
  
    FETCH cur INTO ord; -- fetch an order

    IF NOT FOUND OR rshopid <> ord.shopid THEN

      IF rshopid IS NOT NULL AND rtotal > 0.00::money THEN
        -- insert the accumulated repay
        INSERT INTO repays (shopid, shop, till, orders, total, cash) VALUES (rshopid, rshop, till, rorders, rtotal, rtotal * 0.994);
        -- reset repay 
        rshopid := NULL;
        rshop := NULL;
        rorders := 0;
        rtotal := 0;
      END IF;

      IF NOT FOUND THEN
        CLOSE cur;
        EXIT;
      END IF;
    
    END IF;

    rshopid := ord.shopid;

    -- test against the limitation
    IF rtotal + ord.cash <= rmax AND gtotal + ord.cash <= gmax THEN

			rshop := ord.shop;
      rorders := rorders + 1;
      rtotal := rtotal + ord.cash;
      gtotal := gtotal + ord.cash;

      -- set status to reckoned
      UPDATE orders SET status = 7  WHERE CURRENT OF cur; 

    END IF;

  END LOOP;

END

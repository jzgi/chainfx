-- Table: public.brands

-- DROP TABLE public.brands;

CREATE TABLE public.brands
(
  uid character(11) NOT NULL,
  name character varying(20),
  CONSTRAINT brands_pkey PRIMARY KEY (uid)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.brands
  OWNER TO postgres;


-- Table: public.fames

-- DROP TABLE public.fames;

CREATE TABLE public.fames
(
  id character(11) NOT NULL, -- 红人的用户ID
  name character varying(20), -- 红人昵称,最长允许7个汉字
  quote character varying(20), -- 红人签名
  sex character(1), -- 性别，0表示男，1表示女
  icon bytea, -- 红人头像
  birthday date, -- 生日
  qq character varying(11), -- qq号
  wechat character varying(20), -- 微信号
  email character varying(30), -- 邮箱
  city character varying(4), -- 城市
  rating smallint, -- 级别
  height smallint, -- 身高
  weight smallint, -- 体重
  bust smallint, -- 胸围
  waist smallint, -- 腰围
  hip smallint, -- 臀围
  cup smallint, -- 罩杯，只有女性才填写这条信息
  styles character varying(10)[], -- 风格标签
  skills character varying(10)[], -- 技能标签
  remark text, -- 备注
  sites json, -- 社交平台信息，包括社交平台名称、url
  friends json, -- 好友信息
  date date, -- 注册日期
  m0 bytea,
  m1 bytea,
  m2 bytea,
  m3 bytea,
  m4 bytea,
  mset character varying(5),
  CONSTRAINT fames_pkey PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.fames
  OWNER TO postgres;
COMMENT ON TABLE public.fames
  IS '红人信息表';
COMMENT ON COLUMN public.fames.id IS '红人的用户ID';
COMMENT ON COLUMN public.fames.name IS '红人昵称,最长允许7个汉字';
COMMENT ON COLUMN public.fames.quote IS '红人签名';
COMMENT ON COLUMN public.fames.sex IS '性别，0表示男，1表示女';
COMMENT ON COLUMN public.fames.icon IS '红人头像';
COMMENT ON COLUMN public.fames.birthday IS '生日';
COMMENT ON COLUMN public.fames.qq IS 'qq号';
COMMENT ON COLUMN public.fames.wechat IS '微信号';
COMMENT ON COLUMN public.fames.email IS '邮箱';
COMMENT ON COLUMN public.fames.city IS '城市';
COMMENT ON COLUMN public.fames.rating IS '级别';
COMMENT ON COLUMN public.fames.height IS '身高';
COMMENT ON COLUMN public.fames.weight IS '体重';
COMMENT ON COLUMN public.fames.bust IS '胸围';
COMMENT ON COLUMN public.fames.waist IS '腰围';
COMMENT ON COLUMN public.fames.hip IS '臀围';
COMMENT ON COLUMN public.fames.cup IS '罩杯，只有女性才填写这条信息';
COMMENT ON COLUMN public.fames.styles IS '风格标签';
COMMENT ON COLUMN public.fames.skills IS '技能标签';
COMMENT ON COLUMN public.fames.remark IS '备注';
COMMENT ON COLUMN public.fames.sites IS '社交平台信息，包括社交平台名称、url';
COMMENT ON COLUMN public.fames.friends IS '好友信息';
COMMENT ON COLUMN public.fames.date IS '注册日期';


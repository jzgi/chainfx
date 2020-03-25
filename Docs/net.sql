create schema net;

alter schema chain owner to postgres;

create table ents
(
	id varchar(10) not null
		constraint fed_pk
			primary key,
	name varchar(20),
	raddr varchar(50),
	stamp timestamp(0),
	status smallint default 0 not null
);

alter table ents owner to postgres;

create table typs
(
	id smallint not null
		constraint typs_pk
			primary key,
	remark varchar(20),
	r boolean,
	w boolean,
	status integer
);

alter table typs owner to postgres;

create table chains
(
	entid varchar(10) not null
		constraint chain_entid_fk
			references ents,
	seq integer not null,
	typid smallint
		constraint chains_typs_fk
			references typs,
	keyno integer,
	tags varchar(20) [],
	content jsonb,
	hash char(32),
	stamp timestamp(0),
	status smallint default 0 not null,
	constraint chain_pk
		primary key (entid, seq)
);

alter table chains owner to postgres;

create index chain_typkeyno_idx
	on chains (typid, keyno);

create index chain_tags_idx
	on chains (tags);


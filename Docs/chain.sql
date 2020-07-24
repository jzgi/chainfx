create schema chain;

alter schema chain owner to postgres;

create table peers
(
	id varchar(8) not null
		constraint peers_pk
			primary key,
	name varchar(20),
	raddr varchar(50),
	stamp timestamp(0),
	status smallint default 0 not null
);

alter table peers owner to postgres;

create table blockrecs
(
	peerid varchar(10) not null,
	seq integer not null,
	idx smallint not null,
	hash varchar(32),
	stamp timestamp(0),
	txpeerid varchar(10),
	txno integer,
	typ smallint,
	key varchar(20),
	descr varchar(20),
	amt money,
	balance money,
	doc jsonb,
	rtpeerid varchar(10),
	state smallint
);

alter table blockrecs owner to postgres;

create unique index blocklns_trace_idx
	on blockrecs (peerid, typ, key, stamp);

create table recs
(
	txpeerid varchar(8) not null,
	txno serial not null,
	op smallint not null,
	stamp timestamp(0),
	typ smallint not null,
	descr varchar(20),
	key varchar(20) not null,
	amt money not null,
	balance money not null,
	doc jsonb,
	rtpeerid varchar(8),
	status smallint not null
);

alter table recs owner to postgres;

create table blocks
(
	peerid varchar(8) not null,
	seq integer not null,
	prevhash varchar(32),
	hash varchar(32),
	stamp timestamp(0),
	recs smallint,
	status smallint default 0 not null
);

alter table blocks owner to postgres;


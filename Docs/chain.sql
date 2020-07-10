create schema chain;

alter schema chain owner to postgres;

create table nodes
(
    id varchar(10) not null
        constraint nds_pk
            primary key,
    name varchar(20),
    raddr varchar(50),
    stamp timestamp(0),
    status smallint default 0 not null
);

alter table nodes owner to postgres;

create table blocks
(
    nodeid varchar(10) not null,
    seq integer not null,
    hash varchar(32),
    stamp timestamp(0),
    status smallint default 0 not null,
    creator varchar(10),
    count smallint,
    constraint blocks_pk
        primary key (nodeid, seq)
);

alter table blocks owner to postgres;

create table errors
(
    id serial not null
);

alter table errors owner to postgres;

create table transacts
(
    id serial not null,
    code smallint,
    typ smallint,
    attach bytea,
    lnodeid varchar(10),
    lkey varchar(20),
    rnodeid varchar(10),
    rkey varchar(20),
    status smallint,
    amt integer,
    lbalance integer,
    rbalance integer
);

alter table transacts owner to postgres;

create table blocklns
(
    nodeid varchar(10) not null,
    seq integer not null,
    idx smallint not null,
    typ smallint,
    key varchar(20),
    stamp timestamp(0),
    attach bytea,
    hash varchar(32),
    "desc" varchar(20),
    state smallint,
    amt integer,
    balance integer,
    constraint blocklns_pk
        primary key (nodeid, seq, idx)
);

alter table blocklns owner to postgres;

create unique index blocklns_trace_idx
    on blocklns (nodeid, typ, key, stamp);


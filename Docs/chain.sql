create schema chain;

alter schema chain owner to postgres;

create table nodes
(
    id varchar(8) not null
        constraint nds_pk
            primary key,
    name varchar(20),
    raddr varchar(50),
    createdon timestamp(0),
    status smallint default 0 not null
);

alter table nodes owner to postgres;

create table blocklns
(
    nodeid varchar(10) not null,
    seq integer not null,
    idx smallint not null,
    typ smallint,
    key varchar(20),
    descr varchar(20),
    stamp timestamp(0),
    hash varchar(32),
    amt money,
    balance money,
    document jsonb,
    state smallint
);

alter table blocklns owner to postgres;

create unique index blocklns_trace_idx
    on blocklns (nodeid, typ, key, stamp);

create table blocks
(
    nodeid varchar(8) not null,
    seq integer not null,
    hash varchar(32),
    createdon timestamp(0),
    lns smallint,
    status smallint default 0 not null
);

alter table blocks owner to postgres;

create table transacts
(
    id serial not null,
    op smallint not null,
    typ smallint not null,
    nodeid varchar(8) not null,
    key varchar(20) not null,
    amt money not null,
    balance money not null,
    document jsonb,
    rnodeid varchar(8),
    rkey varchar(20),
    rbalance money,
    status smallint not null,
    descr varchar(20)
);

alter table transacts owner to postgres;


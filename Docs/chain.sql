create schema chain;

alter schema chain owner to postgres;

create sequence jobseq;

alter sequence jobseq owner to postgres;

create table peers
(
    id smallint not null
        constraint peers_pk
            primary key,
    name varchar(20),
    uri varchar(50),
    created timestamp(0),
    status smallint default 0 not null,
    icon bytea,
    local boolean
);

alter table peers owner to postgres;

create table blocks
(
    peerid smallint not null,
    seq integer not null,
    stamp timestamp(0) not null,
    status smallint default 0 not null,
    dgst bigint,
    prdgst bigint,
    constraint blocks_pk
        primary key (peerid, seq)
);

alter table blocks owner to postgres;

create table blockops
(
    peerid smallint not null,
    seq integer not null,
    job bigint not null,
    step smallint not null,
    acct varchar(30) not null,
    name varchar(10),
    ldgr varchar(10) not null,
    descr varchar(20),
    amt money not null,
    bal money not null,
    doc jsonb,
    stated timestamp(0) not null,
    dgst bigint,
    constraint blockops_block_fk
        foreign key (peerid, seq) references blocks
);

alter table blockops owner to postgres;

create unique index blockops_op_idx
    on blockops (job, step);

create index blockops_block_idx
    on blockops (peerid, seq);

create table ops
(
    job bigint not null,
    step smallint not null,
    acct varchar(20) not null,
    name varchar(10) not null,
    ldgr varchar(10) not null,
    status smallint default 0 not null,
    descr varchar(20),
    amt money not null,
    doc jsonb,
    bal money,
    stated timestamp(0) not null,
    ppeer smallint,
    pacct varchar(20),
    pname varchar(10),
    npeer smallint,
    nacct varchar(20),
    nname varchar(10),
    stamp timestamp(0),
    constraint ops_pk
        primary key (job, step)
);

alter table ops owner to postgres;

create index ops_validate_idx
    on ops (status, ldgr, step);


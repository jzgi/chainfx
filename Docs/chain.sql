create schema chain;

alter schema chain owner to postgres;

create table nodes
(
    id varchar(10) not null
        constraint nodes_pk
            primary key,
    name varchar(20),
    raddr varchar(50),
    stamp timestamp(0),
    status smallint default 0 not null
);

alter table nodes owner to postgres;

create table logins
(
    id varchar(10) not null
        constraint logins_pk
            primary key,
    name varchar(20),
    credential char [],
    status smallint default 0 not null,
    typ smallint
);

alter table logins owner to postgres;

create table blocks
(
    nodeid varchar(10) not null,
    seq bigint not null,
    typ smallint,
    hash varchar(32),
    createdon timestamp(0),
    status smallint default 0 not null,
    creator varchar(10),
    dats smallint,
    constraint blocks_pk
        primary key (nodeid, seq)
);

alter table blocks owner to postgres;

create table blockdats
(
    nodeid varchar(10) not null,
    seq bigint not null,
    idx smallint not null,
    hash varchar(32),
    stamp timestamp(0),
    state smallint,
    typ smallint,
    rnodeid varchar(10),
    rseq bigint,
    ridx smallint,
    content jsonb,
    tags varchar(20) [],
    constraint blockdats_pk
        primary key (nodeid, seq, idx),
    constraint blockdats_blocks_fk
        foreign key (nodeid, seq) references blocks
);

alter table blockdats owner to postgres;


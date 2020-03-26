create schema chain;

alter schema chain owner to postgres;

create table peers
(
    id varchar(10) not null
        constraint peers_pk
            primary key,
    name varchar(20),
    raddr varchar(50),
    stamp timestamp(0),
    status smallint default 0 not null
);

alter table peers owner to postgres;

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

create table peerblocks
(
    peerid varchar(10) not null
        constraint peerblocks_peerid_fk
            references peers (id),
    seq integer not null,
    typid smallint
        constraint peerblocks_typid_fk
            references typs,
    key varchar(20),
    tags varchar(20) [],
    hash char(32),
    stamp timestamp(0),
    status smallint default 0 not null,
    content bytea,
    constraint peerblocks_pk
        primary key (peerid, seq)
);

alter table peerblocks owner to postgres;

create index peerblocks_tags_idx
    on peerblocks (tags);

create index peerblocks_typid_key_idx
    on peerblocks (typid, key);

create table users
(
    id varchar(10) not null
        constraint users_pk
            primary key,
    name varchar(10),
    credential char [],
    status smallint default 0 not null,
    role smallint
);

alter table users owner to postgres;

create table blocks
(
    seq serial not null
        constraint blocks_pk
            primary key,
    typid smallint,
    key varchar(20),
    tags varchar(20) [],
    content bytea,
    hash char(32),
    stamp timestamp(0),
    status smallint default 0 not null
);

alter table blocks owner to postgres;

create index blocks_typid_key_idx
    on blocks (typid, key);


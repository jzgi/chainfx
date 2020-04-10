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

create table datyps
(
    id smallint not null
        constraint datyps_pk
            primary key,
    name varchar(20),
    status integer,
    contentyp varchar(20),
    op smallint,
    contract bytea
);

alter table datyps owner to postgres;

create table logins
(
    id varchar(10) not null
        constraint logins_pk
            primary key,
    name varchar(20),
    credential char [],
    status smallint default 0 not null,
    role smallint
);

alter table logins owner to postgres;

create table blocks
(
    aid varchar(10) not null
        constraint blocks_aid_fk
            references peers,
    seq integer not null,
    bid varchar(10)
        constraint blocks_bid_fk
            references peers,
    datypid smallint
        constraint blocks_datypid_fk
            references datyps,
    key varchar(20),
    tags varchar(20) [],
    body bytea,
    hash char(32),
    stamp timestamp(0),
    status smallint default 0 not null,
    constraint blocks_pk
        primary key (aid, seq)
);

alter table blocks owner to postgres;

create index blocks_datypid_key_idx
    on blocks (datypid, key);

create index blocks_tags_idx
    on blocks (tags);


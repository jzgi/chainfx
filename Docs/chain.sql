create schema chain;

alter schema chain owner to postgres;

create table peers
(
    id varchar(8) not null
        constraint peers_pk
            primary key,
    name varchar(20),
    raddr varchar(50),
    created timestamp(0),
    status smallint default 0 not null,
    icon bytea
);

alter table peers owner to postgres;

create table blocks
(
    peerid varchar(8) not null,
    seq integer not null,
    stamp timestamp(0) not null,
    prevtag varchar(16) not null,
    tag varchar(16) not null,
    status smallint default 0 not null,
    constraint "PK_blocks"
        primary key (peerid, seq)
);

alter table blocks owner to postgres;

create table blockrecs
(
    peerid varchar(8) not null,
    seq integer not null,
    acct varchar(30) not null,
    typ smallint not null,
    time timestamp(0) not null,
    oprid integer,
    descr varchar(20),
    amt money not null,
    bal money not null,
    doc jsonb,
    digest bigint,
    fpeerid varchar(8),
    constraint blocktxs_block_fk
        foreign key (peerid, seq) references blocks
);

alter table blockrecs owner to postgres;

create index blocktxs_block_idx
    on blockrecs (peerid, seq);

create index blocktxs_record_idx
    on blockrecs (peerid, acct, typ);

create table entries
(
    peerid varchar(8) not null,
    acct varchar(20) not null,
    rpeerid varchar(8) not null,
    racct varchar(20) not null,
    typ smallint not null,
    caseid integer,
    op smallint not null,
    descr varchar(20),
    amt money not null,
    doc jsonb,
    created timestamp(0),
    status smallint default 0 not null,
    id serial not null
);

alter table entries owner to postgres;


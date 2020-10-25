create schema chain;

alter schema chain owner to postgres;

create sequence txn;

alter sequence txn owner to postgres;

create table peers
(
    id varchar(4) not null
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
    peerid varchar(4) not null,
    seq integer not null,
    stamp timestamp(0) not null,
    prevtag varchar(16) not null,
    tag varchar(16) not null,
    status smallint default 0 not null,
    constraint blocks_pk
        primary key (peerid, seq)
);

alter table blocks owner to postgres;

create table ops
(
    tn varchar(20) not null,
    step smallint not null,
    an varchar(20) not null,
    typ smallint not null,
    inst varchar(10),
    descr varchar(20),
    amt money not null,
    doc jsonb,
    stamp timestamp(0),
    status smallint default 0 not null,
    npeerid varchar(4),
    nan varchar(20),
    constraint ops_pk
        primary key (tn, step)
);

alter table ops owner to postgres;

create index ops_item_idx
    on ops (an, typ, inst);

create table blockrecs
(
    peerid varchar(4) not null,
    seq integer not null,
    tn varchar(32) not null,
    step smallint not null,
    an varchar(30) not null,
    typ smallint not null,
    inst integer,
    descr varchar(20),
    amt money not null,
    bal money not null,
    doc jsonb,
    stamp timestamp(0) not null,
    digest bigint,
    constraint blockrecs_blocks_fk
        foreign key (peerid, seq) references blocks
);

alter table blockrecs owner to postgres;

create unique index blockrecs_op_uidx
    on blockrecs (tn, step);

create index blockrecs_item_idx
    on blockrecs (an, typ, inst);

create index blockrecs_block_idx
    on blockrecs (peerid, seq);


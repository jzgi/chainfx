create schema chain;

alter schema chain owner to postgres;

create sequence jobseq;

alter sequence jobseq owner to postgres;

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
    status smallint default 0 not null,
    dgst integer,
    prdgst integer,
    constraint blocks_pk
        primary key (peerid, seq)
);

alter table blocks owner to postgres;

create table blocksts
(
    peerid varchar(2) not null,
    seq integer not null,
    job varchar(14) not null,
    step smallint not null,
    acct varchar(30) not null,
    name varchar(10),
    ldgr varchar(10) not null,
    descr varchar(20),
    amt money not null,
    bal money not null,
    doc jsonb,
    stated timestamp(0) not null,
    dgst integer,
    constraint blocksts_block_fk
        foreign key (peerid, seq) references blocks
);

alter table blocksts owner to postgres;

create index blocksts_block_idx
    on blocksts (peerid, seq);

create unique index blocksts_op_idx
    on blocksts (job, step);

create table logs
(
    job varchar(10) not null,
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
    ppeer varchar(2),
    pacct varchar(20),
    pname varchar(10),
    npeer varchar(2),
    nacct varchar(20),
    nname varchar(10),
    stamp timestamp(0),
    constraint logs_pk
        primary key (job, step)
);

alter table logs owner to postgres;

create index logs_validate_idx
    on logs (status, ldgr, step);


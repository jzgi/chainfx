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
    local boolean
);

alter table peers owner to postgres;

create unique index peers_local_idx
    on peers (local)
    where (local = true);

create table states
(
    job bigint not null,
    step smallint not null,
    acct varchar(20) not null,
    name varchar(10) not null,
    ldgr varchar(10) not null,
    descr varchar(20),
    amt money not null,
    doc jsonb,
    stated timestamp(0) not null,
    ppeerid smallint
        constraint ops_ppeerid_fk
            references peers,
    pacct varchar(20),
    pname varchar(10),
    npeerid smallint
        constraint ops_npeerid_fk
            references peers,
    nacct varchar(20),
    nname varchar(10),
    stamp timestamp(0)
);

alter table states owner to postgres;

create table blocks
(
    peerid smallint not null
        constraint blocks_peerid_fk
            references peers,
    seq integer not null,
    bal money not null,
    cs bigint,
    blockcs bigint,
    constraint blocks_pk
        primary key (peerid, seq)
)
    inherits (states);

alter table blocks owner to postgres;

create trigger blocks_trig
    before insert
    on blocks
    for each row
execute procedure public.calc_bal_func();

create table ops
(
    status smallint default 0 not null,
    constraint ops_pk
        primary key (job, step),
    constraint ops_ppeerid_fk
        foreign key (ppeerid) references peers,
    constraint ops_npeerid_fk
        foreign key (npeerid) references peers
)
    inherits (states);

alter table ops owner to postgres;


create schema chain;

alter schema chain owner to postgres;

create sequence txn_seq;

alter sequence txn_seq owner to postgres;

create table peers
(
    id smallint not null
        constraint peers_pk
            primary key,
    typ smallint not null,
    status smallint default 0 not null,
    name varchar(10) not null,
    tip varchar(20),
    created timestamp(0),
    creator varchar(10),
    uri varchar(50)
);

alter table peers_ owner to postgres;

create table bids_
(
    peerid smallint
        constraint bids_peerid_fk
            references peers_
)
    inherits (public.bids_);

alter table bids_ owner to postgres;


create schema chain;

alter schema chain owner to postgres;

create sequence txn_seq;

alter sequence txn_seq owner to postgres;

create table peers
(
    id smallint not null
        constraint peers_pk
            primary key,
    name varchar(20),
    domain varchar(50),
    created timestamp(0),
    status smallint default 0 not null,
    native boolean default false not null
);

alter table peers owner to postgres;

create unique index peers_native_idx
    on peers (native)
    where (native = true);

create table _rows
(
    txn bigint not null,
    typ smallint not null,
    acct varchar(20) not null,
    name varchar(10) not null,
    remark varchar(100),
    amt money not null,
    stamp timestamp(0),
    rpeerid smallint not null
);

alter table _rows owner to postgres;

create table archive
(
    bal money not null,
    peerid smallint not null
        constraint archive_peerid_fk
            references peers,
    seq integer not null,
    cs bigint,
    blockcs bigint,
    constraint archive_pk
        primary key (peerid, seq)
)
    inherits (_rows);

alter table archive owner to postgres;

create table queue
(
    id serial not null
        constraint queue_pk
            primary key,
    status smallint default 0 not null
)
    inherits (_rows);

alter table queue owner to postgres;

create table events
(
    id serial not null
        constraint events_pk
            primary key,
    acct varchar(20),
    typ smallint,
    content jsonb,
    stamp timestamp(0)
);

alter table events owner to postgres;

create index events_acctstamp_idx
    on events (acct asc, stamp desc);

create function calc_bal_func() returns trigger
    language plpgsql
as $$
DECLARE
    m MONEY := 0;
BEGIN

    m := (SELECT bal FROM chain.archive WHERE peerid = NEW.peerid AND acct = NEW.acct ORDER BY seq DESC LIMIT 1);
    if m IS NULL THEN
        NEW.bal := NEW.amt;
    ELSE
        NEW.bal := m + NEW.amt;
    end if;
    RETURN NEW;
END
$$;

alter function calc_bal_func() owner to postgres;


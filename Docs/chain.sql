create schema chain;

alter schema chain owner to postgres;

create table peers
(
    id smallint not null
        constraint peers_pk
            primary key,
    name varchar(20),
    uri varchar(50),
    created timestamp(0),
    status smallint default 0 not null,
    native boolean default false not null
);

alter table peers owner to postgres;

create unique index peers_native_idx
    on peers (native)
    where (native = true);

create table _states
(
    acct varchar(20) not null,
    name varchar(10) not null,
    tip varchar(20),
    amt money not null,
    stamp timestamp(0)
);

alter table _states owner to postgres;

create table archivals
(
    bal money not null,
    peerid smallint not null
        constraint archivals_peerid_fk
            references peers,
    seq integer not null,
    cs bigint,
    blockcs bigint,
    constraint archivals_pk
        primary key (peerid, seq)
)
    inherits (_states);

alter table archivals owner to postgres;

create table queuels
(
    id serial not null
        constraint queuels_pk
            primary key,
    status smallint default 0 not null
)
    inherits (_states);

alter table queuels owner to postgres;

create function calc_bal_func() returns trigger
    language plpgsql
as $$
DECLARE
    m MONEY := 0;
BEGIN

    m := (SELECT bal FROM chain.archivals WHERE peerid = NEW.peerid AND acct = NEW.acct ORDER BY seq DESC LIMIT 1);
    if m IS NULL THEN
        NEW.bal := NEW.amt;
    ELSE
        NEW.bal := m + NEW.amt;
    end if;
    RETURN NEW;
END
$$;

alter function calc_bal_func() owner to postgres;

create trigger archivals_trig
    before insert
    on archivals
    for each row
execute procedure calc_bal_func();


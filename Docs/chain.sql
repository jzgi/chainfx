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

create table archives
(
    peerid smallint not null
        constraint archives_peerid_fk
            references peers,
    seq integer not null,
    bal money not null,
    cs bigint,
    blockcs bigint,
    constraint archives_pk
        primary key (peerid, seq)
)
    inherits (_states);

alter table archives owner to postgres;

create view ops_vw(acct, name, tip, amt, stamp) as
SELECT j.uacct AS acct,
       j.uname AS name,
       l.tip,
       j.amt,
       j.acted AS stamp
FROM lotjns j,
     lots l
WHERE j.lotid =
      l.id
  AND j.status =
      3
ORDER BY l.id;

alter table ops_vw owner to postgres;

create function calc_bal_func() returns trigger
    language plpgsql
as $$
DECLARE
    m MONEY := 0;
BEGIN

    m := (SELECT bal FROM chain.archives WHERE peerid = NEW.peerid AND acct = NEW.acct ORDER BY seq DESC LIMIT 1);
    if m IS NULL THEN
        NEW.bal := NEW.amt;
    ELSE
        NEW.bal := m + NEW.amt;
    end if;
    RETURN NEW;
END
$$;

alter function calc_bal_func() owner to postgres;

create trigger archives_trig
    before insert
    on archives
    for each row
execute procedure calc_bal_func();

create function ops_archived(p1 integer) returns void
    language sql
as $$
UPDATE public.lotjns SET status = 2 WHERE status = 1
$$;

alter function ops_archived(integer) owner to postgres;


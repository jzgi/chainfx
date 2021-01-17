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

create table blocks
(
    peerid smallint not null
        constraint blocks_peerid_fk
            references peers,
    seq integer not null,
    job bigint not null,
    step smallint not null,
    acct varchar(30) not null,
    name varchar(10),
    ldgr varchar(10) not null,
    descr varchar(20),
    amt money not null,
    bal money not null,
    doc jsonb,
    stated timestamp(0) not null,
    cs bigint,
    blockcs bigint,
    stamp timestamp(0),
    constraint blocks_pk
        primary key (peerid, seq)
);

alter table blocks owner to postgres;

create unique index blocks_op_idx
    on blocks (job, step);

create index blocks_ldgr_idx
    on blocks (acct, ldgr, seq);

create table ops
(
    job bigint not null,
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
    ppeerid smallint,
    pacct varchar(20),
    pname varchar(10),
    npeerid smallint,
    nacct varchar(20),
    nname varchar(10),
    stamp timestamp(0),
    constraint ops_pk
        primary key (job, step)
);

alter table ops owner to postgres;

create index ops_validate_idx
    on ops (status, ldgr, step);

create function calc_bal_func() returns trigger
    language plpgsql
as $$
DECLARE
    m MONEY := 0;
BEGIN

    --     if NEW.amt IS NULL THEN
--         NEW.amt := 0::money;
--     end if;

    m := (SELECT bal FROM chain.blocks WHERE peerid = NEW.peerid AND acct = NEW.acct AND ldgr = NEW.ldgr ORDER BY seq DESC LIMIT 1);
    if m IS NULL THEN
        NEW.bal := NEW.amt;
    ELSE
        NEW.bal := m + NEW.amt;
    end if;
    RETURN NEW;
END
$$;

alter function calc_bal_func() owner to postgres;

create trigger blocks_trig
    before insert
    on blocks
    for each row
    when (new.bal = 0::money OR new.bal IS NULL)
execute procedure calc_bal_func();


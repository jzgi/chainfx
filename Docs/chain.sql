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
    native boolean default false not null
);

alter table peers owner to postgres;

create unique index peers_native_idx
    on peers (native)
    where (native = true);

create table _states
(
    job bigint not null,
    step smallint not null,
    acct varchar(20) not null,
    name varchar(10) not null,
    descr varchar(20),
    amt money not null,
    stated timestamp(0) not null,
    ppid smallint
        constraint ops_ppid_fk
            references peers,
    pacct varchar(20),
    pname varchar(10),
    npid smallint
        constraint ops_npid_fk
            references peers,
    nacct varchar(20),
    nname varchar(10),
    stamp timestamp(0)
);

alter table _states owner to postgres;

create table blocks
(
    pid smallint not null
        constraint blocks_peerid_fk
            references peers,
    seq integer not null,
    bal money not null,
    cs bigint,
    blockcs bigint,
    constraint blocks_pk
        primary key (pid, seq)
)
    inherits (_states);

alter table blocks owner to postgres;

create table ops
(
    status smallint default 0 not null,
    constraint ops_pk
        primary key (job, step),
    constraint ops_ppeerid_fk
        foreign key (ppid) references peers,
    constraint ops_npeerid_fk
        foreign key (npid) references peers
)
    inherits (_states);

alter table ops owner to postgres;

create function calc_bal_func() returns trigger
    language plpgsql
as $$
DECLARE
    m MONEY := 0;
BEGIN

    m := (SELECT bal FROM chain.archives WHERE peerid = NEW.pid AND acct = NEW.acct ORDER BY seq DESC LIMIT 1);
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
execute procedure calc_bal_func();


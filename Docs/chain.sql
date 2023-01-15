create table ledgers_
(
	seq integer,
	acct varchar(20),
	name varchar(12),
	amt integer,
	bal integer,
	cs uuid,
	blockcs uuid,
	stamp timestamp(0)
);

alter table _ldgs_ owner to postgres;

create table peerledgs_
(
	peerid smallint
)
inherits (_ldgs_);

alter table _peerldgs_ owner to postgres;

create table peers_
(
	id smallint not null
		constraint peers_pk
			primary key,
	weburl varchar(50),
	secret varchar(16)
)
inherits (entities);

alter table _peers_ owner to postgres;

create table accts_
(
	no varchar(20),
	v integer
)
inherits (entities);

alter table _accts_ owner to postgres;


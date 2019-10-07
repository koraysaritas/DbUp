-- Creates the following tables:
--  * Entry
--  * Feed
--  * FeedItem
--  * Revision

create table $schema$.DEMO1(
	Name nvarchar(50) not null,
	Title nvarchar(200) not null,
	Summary varchar(100) not null,
	IsVisible bit not null,
	Published date not null,
	LatestRevisionId int null
)
TABLESPACE SYS_TBS_MEM_DATA;
/

create table $schema$.DEMO2(
	Name varchar(100) not null,
	Title varchar(255) not null
)
TABLESPACE SYS_TBS_MEM_DATA;
/

-- Settings and Statistics

create table $schema$.DEMO3(
	Name nvarchar(50) not null,
	Description varchar(900) not null,
	DisplayName varchar(200) not null,
	"VALUE" varchar(150) not null
)
TABLESPACE SYS_TBS_MEM_DATA;
/

insert into $schema$.DEMO3(Name, DisplayName, Value, Description) values ('ui-title', 'Title', 'My FunnelWeb Site', 'Text: The title shown at the top in the browser.');/
insert into $schema$.DEMO3(Name, DisplayName, Value, Description) values ('ui-introduction', 'Introduction', 'Introduction', 'Welcome to your FunnelWeb blog.');/
insert into $schema$.DEMO3(Name, DisplayName, Value, Description) values ('ui-links', 'Main Links', 'Projects', 'HTML: A list of links shown at the top of each page.');/

insert into $schema$.DEMO3(Name, DisplayName, Value, Description) values ('search-author', 'Author', 'Daffy Duck', 'Text: Your name.');/
insert into $schema$.DEMO3(Name, DisplayName, Value, Description) values ('search-keywords', 'Keywords', '.net, c#, test', 'Comma-separated text: Keywords shown to search engines.');/
insert into $schema$.DEMO3(Name, DisplayName, Value, Description) values ('search-description', 'Description', 'My website.', 'Text: The description shown to search engines in the meta description tag.');/

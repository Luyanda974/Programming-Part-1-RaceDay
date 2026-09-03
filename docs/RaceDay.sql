--creating database RaceDay--
Create database RaceDay;

--using database--
use RaceDay;

--creating user table--
create table [User](UserID int primary key identity(1,1),FirstName varchar(50) not null,LastName varchar(50) not null,
Email varchar(100) not null unique, PasswordHash varchar(255) not null, Role varchar(20) not null);

--creating event type table--
create table EventType(EventTypeID int primary key identity(1,1), TypeName varchar(50) not null unique);

--creating event table--
create table Event(EventID int primary key identity(1,1), EventName varchar(100) not null, Description varchar(500) null, EventDate date not null,
Location varchar(150) not null, Distance decimal(6,2) not null, OrganiserID int not null, EventTypeID int not null,
constraint FK_Event_Organiser foreign key(OrganiserID) references [User] (UserID), 
constraint FK_EventType foreign key (EventTypeID) references EventType(EventTypeID));

--creating category table--
create table Category(CategoryID int primary key identity(1,1),CategoryName varchar(100) not null,CategoryType varchar(50) not null,EventID int not null
constraint FK_Category_Event foreign key (EventID) references Event(EventID));

--creating the enrolment table--
create table Enrollment(EnrollmentID int primary key identity(1,1), EnrollmentDate date not null default GetDate(),ParticipantID int not null, EventID int
not null, CategoryID int not null,
constraint FK_Enrollment_Participant foreign key (ParticipantID) references [User](UserID),
constraint FK_Enrollment_Event foreign key (EventID) references Event(EventID),
constraint FK_Enrollment_Category foreign key (CategoryID) references Category(CategoryID));

--creating result table--
create table Result(ResultID int primary key identity (1,1), FinishTime time not null, FinishPosition int not null, EnrollmentID int not null,
constraint FK_Result_Enrollment foreign key (EnrollmentID) references Enrollment (EnrollmentID));

--inserting data in user table--
insert into [User](FirstName, LastName, Email, PasswordHash, Role)
values('Anele', 'Mabaso','anelemabaso@raceday.com','Hash_Organiser_001','Organiser'),
('Luyanda','Masemola', 'luyandamasemola@raceday.com','Hash_Organiser_002','Organiser'),
('Refilwe', 'Maleka', 'refilwemaleka@raceday.com', 'Hash_Participant_001','Particpant'),
('Samantha','Dube','samanthadube@raceday.com','Hash_Participant_002','Participant');

select * from [User];

--inserting data in event type table--
insert into EventType(TypeName) values ('Running'), ('Walking'),('Cycling');

select * from EventType;

--inserting data in event table
insert into Event(EventName,Description,EventDate,Location,Distance,OrganiserID,EventTypeID) values
('Johannesburg annual run','An annual community running event','2026-10-03','Gauteng, Johannesburg',10.00,1,1),
('CapeTown annual walk', 'An annual community walking event','2026-10-04','Western Cape, CapeTown',5.00,2,2),
('Polokwane annual cycle', 'An annual cycling event','2026-10-05','Limpopo,Polokwane',50.00,1,3);

select * from Event;

insert into Category(CategoryName,CategoryType,EventID) values
('5km run','Running',1),('10km run','Running',1),
('5km walk','Walking',2),('10km walk','Walking',2),
('25km cycle','Cycling',3),('50km cycle','Cycling',3);

select * from Category;

--inserting data in enrollment table--
insert into Enrollment(EnrollmentDate,ParticipantID,EventID, CategoryID) values
('2026-09-25',3,1,1),('2026-09-26',4,1,2),('2026-09-27',3,2,3),('2026-09-28',4,3,6);

select * from Enrollment;


--inserting data into result table--
insert into Result(FinishTime,FinishPosition,EnrollmentID) values
('00:35:43',11,1),('00:45:45',9,2),('00:25:21',7,3);

select * from Result;
-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2025-05-05 06:16:44.657

-- tables
-- Table: Client
CREATE TABLE Client (
    IdClient int NOT NULL,
    FirstName nvarchar(120)  NOT NULL,
    LastName nvarchar(120)  NOT NULL,
    Email nvarchar(120)  NOT NULL,
    Telephone nvarchar(120)  NOT NULL,
    Pesel nvarchar(120)  NOT NULL,
    CONSTRAINT Client_pk PRIMARY KEY  (IdClient)
);

-- Table: Client_Trip
CREATE TABLE Client_Trip (
    IdClient int  NOT NULL,
    IdTrip int  NOT NULL,
    RegisteredAt int  NOT NULL,
    PaymentDate int  NULL,
    CONSTRAINT Client_Trip_pk PRIMARY KEY  (IdClient,IdTrip)
);

-- Table: Country
CREATE TABLE Country (
    IdCountry int NOT NULL,
    Name nvarchar(120)  NOT NULL,
    CONSTRAINT Country_pk PRIMARY KEY  (IdCountry)
);

-- Table: Country_Trip
CREATE TABLE Country_Trip (
    IdCountry int  NOT NULL,
    IdTrip int  NOT NULL,
    CONSTRAINT Country_Trip_pk PRIMARY KEY  (IdCountry,IdTrip)
);

-- Table: Trip
CREATE TABLE Trip (
    IdTrip int NOT NULL,
    Name nvarchar(120)  NOT NULL,
    Description nvarchar(120)  NOT NULL,
    DateFrom datetime  NOT NULL,
    DateTo datetime  NOT NULL,
    MaxPeople int  NOT NULL,
    CONSTRAINT Trip_pk PRIMARY KEY  (IdTrip)
);

-- foreign keys
-- Reference: Country_Trip_Country (table: Country_Trip)
ALTER TABLE Country_Trip ADD CONSTRAINT Country_Trip_Country
    FOREIGN KEY (IdCountry)
    REFERENCES Country (IdCountry);

-- Reference: Country_Trip_Trip (table: Country_Trip)
ALTER TABLE Country_Trip ADD CONSTRAINT Country_Trip_Trip
    FOREIGN KEY (IdTrip)
    REFERENCES Trip (IdTrip);

-- Reference: Table_3_Client (table: Client_Trip)
ALTER TABLE Client_Trip ADD CONSTRAINT Table_3_Client
    FOREIGN KEY (IdClient)
    REFERENCES Client (IdClient);

-- Reference: Table_3_Trip (table: Client_Trip)
ALTER TABLE Client_Trip ADD CONSTRAINT Table_3_Trip
    FOREIGN KEY (IdTrip)
    REFERENCES Trip (IdTrip);

-- End of file.


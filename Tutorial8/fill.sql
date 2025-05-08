-- Insert data into Country table
INSERT INTO Country (IdCountry, Name) VALUES (1, 'Italy');
INSERT INTO Country (IdCountry, Name) VALUES (2, 'Spain');
INSERT INTO Country (IdCountry, Name) VALUES (3, 'France');
INSERT INTO Country (IdCountry, Name) VALUES (4, 'Greece');
INSERT INTO Country (IdCountry, Name) VALUES (5, 'Portugal');
INSERT INTO Country (IdCountry, Name) VALUES (6, 'Croatia');
INSERT INTO Country (IdCountry, Name) VALUES (7, 'Germany');

-- Insert data into Trip table
INSERT INTO Trip (IdTrip, Name, Description, DateFrom, DateTo, MaxPeople)
VALUES (1, 'Italian Adventure', 'Explore Rome and Venice', '2025-06-15', '2025-06-25', 20);

INSERT INTO Trip (IdTrip, Name, Description, DateFrom, DateTo, MaxPeople)
VALUES (2, 'Spanish Fiesta', 'Barcelona and Madrid tour', '2025-07-10', '2025-07-20', 15);

INSERT INTO Trip (IdTrip, Name, Description, DateFrom, DateTo, MaxPeople)
VALUES (3, 'French Riviera', 'Luxury vacation in Nice and Cannes', '2025-08-05', '2025-08-15', 10);

INSERT INTO Trip (IdTrip, Name, Description, DateFrom, DateTo, MaxPeople)
VALUES (4, 'Greek Islands', 'Island hopping in the Aegean Sea', '2025-09-01', '2025-09-12', 25);

INSERT INTO Trip (IdTrip, Name, Description, DateFrom, DateTo, MaxPeople)
VALUES (5, 'Portuguese Exploration', 'Lisbon and Porto tour', '2025-06-20', '2025-06-30', 18);

INSERT INTO Trip (IdTrip, Name, Description, DateFrom, DateTo, MaxPeople)
VALUES (6, 'Croatian Coast', 'Sailing along the Dalmatian Coast', '2025-07-25', '2025-08-05', 12);

INSERT INTO Trip (IdTrip, Name, Description, DateFrom, DateTo, MaxPeople)
VALUES (7, 'German Christmas Markets', 'Visit famous Christmas markets', '2025-12-10', '2025-12-18', 30);

-- Insert data into Country_Trip table (connecting trips with countries)
INSERT INTO Country_Trip (IdCountry, IdTrip) VALUES (1, 1); -- Italy - Italian Adventure
INSERT INTO Country_Trip (IdCountry, IdTrip) VALUES (2, 2); -- Spain - Spanish Fiesta
INSERT INTO Country_Trip (IdCountry, IdTrip) VALUES (3, 3); -- France - French Riviera
INSERT INTO Country_Trip (IdCountry, IdTrip) VALUES (4, 4); -- Greece - Greek Islands
INSERT INTO Country_Trip (IdCountry, IdTrip) VALUES (5, 5); -- Portugal - Portuguese Exploration
INSERT INTO Country_Trip (IdCountry, IdTrip) VALUES (6, 6); -- Croatia - Croatian Coast
INSERT INTO Country_Trip (IdCountry, IdTrip) VALUES (7, 7); -- Germany - German Christmas Markets

-- Insert data into Client table
INSERT INTO Client (IdClient, FirstName, LastName, Email, Telephone, Pesel)
VALUES (1, 'Anna', 'Kowalski', 'anna.kowalski@email.com', '+48 123 456 789', '85121212345');

INSERT INTO Client (IdClient, FirstName, LastName, Email, Telephone, Pesel)
VALUES (2, 'Jan', 'Nowak', 'jan.nowak@email.com', '+48 234 567 890', '90050512345');

INSERT INTO Client (IdClient, FirstName, LastName, Email, Telephone, Pesel)
VALUES (3, 'Maria', 'Wiśniewska', 'maria.wisniewska@email.com', '+48 345 678 901', '78031212345');

INSERT INTO Client (IdClient, FirstName, LastName, Email, Telephone, Pesel)
VALUES (4, 'Piotr', 'Dąbrowski', 'piotr.dabrowski@email.com', '+48 456 789 012', '82092112345');

INSERT INTO Client (IdClient, FirstName, LastName, Email, Telephone, Pesel)
VALUES (5, 'Karolina', 'Lewandowska', 'karolina.lewandowska@email.com', '+48 567 890 123', '89111512345');

INSERT INTO Client (IdClient, FirstName, LastName, Email, Telephone, Pesel)
VALUES (6, 'Tomasz', 'Wójcik', 'tomasz.wojcik@email.com', '+48 678 901 234', '76040412345');

INSERT INTO Client (IdClient, FirstName, LastName, Email, Telephone, Pesel)
VALUES (7, 'Magdalena', 'Kamińska', 'magdalena.kaminska@email.com', '+48 789 012 345', '94020212345');

-- Insert data into Client_Trip table (connecting clients with trips they booked)
-- RegisteredAt and PaymentDate are stored as integers representing Unix timestamps
INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
VALUES (1, 1, 1714675200, 1714761600); -- Anna booked Italian Adventure, registered May 3, 2024, paid May 4, 2024

INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
VALUES (2, 2, 1714588800, 1714675200); -- Jan booked Spanish Fiesta, registered May 2, 2024, paid May 3, 2024

INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
VALUES (3, 3, 1714502400, 1714588800); -- Maria booked French Riviera, registered May 1, 2024, paid May 2, 2024

INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
VALUES (4, 4, 1714416000, 1714502400); -- Piotr booked Greek Islands, registered Apr 30, 2024, paid May 1, 2024

INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
VALUES (5, 5, 1714329600, NULL); -- Karolina booked Portuguese Exploration, registered Apr 29, 2024, not paid yet

INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
VALUES (6, 6, 1714243200, 1714329600); -- Tomasz booked Croatian Coast, registered Apr 28, 2024, paid Apr 29, 2024

INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
VALUES (7, 7, 1714156800, 1714243200); -- Magdalena booked German Christmas Markets, registered Apr 27, 2024, paid Apr 28, 2024
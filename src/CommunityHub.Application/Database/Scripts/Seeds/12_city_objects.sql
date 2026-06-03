-- Gradski objekti
INSERT INTO city_objects (name, description) VALUES
('Cinema Star', 'City cinema with 3 halls and 400 seats total.'),
('Aqua Park', 'Outdoor water park open May–September.'),
('Luna Park', 'Amusement park with rides for all ages.'),
('Sports Hall Pinki', 'Multi-purpose indoor sports hall.'),
('Botanical Garden', 'Green oasis in the city center.');

-- Glasovi (pretpostavljamo da neighbourhood_id=1 i citizen_id-ovi postoje)
-- Prilagodi ID-ove prema vašoj bazi
-- Glasovi (koristim postojeće citizen i neighborhood ID-ove)
INSERT INTO city_object_votes (city_object_id, citizen_id, neighborhood_id) VALUES
(1, 3, 1),
(1, 4, 1),
(1, 5, 1),
(2, 3, 1),
(2, 6, 1),
(3, 4, 1),
(4, 5, 1),
(4, 6, 1),
(4, 7, 1),
(5, 8, 1),
(1, 9, -2),
(2, 10, -2),
(3, 11, -2);
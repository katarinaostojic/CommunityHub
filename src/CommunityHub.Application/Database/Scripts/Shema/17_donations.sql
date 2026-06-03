-- Provjeri svoje neighbourhood ID-ove i citizen ID-ove pa prilagodi
INSERT INTO donations (citizen_id, neighborhood_id, category_id, amount, created_at) VALUES
(3, 1, 1, 5000.00, '2026-02-02'),
(4, 1, 1, 15000.00, '2026-01-20'),
(5, 1, 2, 8000.00, '2026-03-10'),
(6, 1, 2, 3000.00, '2026-04-01'),
(3, 1, 3, 12000.00, '2026-02-15'),
(4, 1, 4, 7000.00, '2026-03-05'),
(5, 1, 5, 4000.00, '2026-01-10'),
(6, 1, 6, 9000.00, '2026-04-20');

UPDATE neighborhoods SET budget = 63000.00 WHERE id = 1;
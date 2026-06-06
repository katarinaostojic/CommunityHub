INSERT INTO building_memberships (id, building_id, user_id, unit_number, floor_number, approved_at)
VALUES
    (-1, -4, -2, '8', 1, '2026-03-01'),
    (-2, -4, -1, '11', 2, '2026-03-03'),
    (-3, -1, -2, '6', 1, '2026-03-05'),
    (-4, -1, -1, '10', 2, '2026-03-08'),
    (-5, -6, -2, '9', 1, '2026-03-10'),
    (-6, -6, -1, '18', 3, '2026-03-12');

INSERT INTO building_access_requests (id, user_id, building_id, unit_number, created_at, status, rejection_reason)
VALUES
    (-1, -2, -2, '7', '2026-04-01 10:00:00', 'pending approval', NULL),
    (-2, -2, -3, '12', '2026-04-02 11:30:00', 'rejected', 'Apartment number could not be verified.'),
    (-3, -2, -4, '8', '2026-03-01 09:00:00', 'approved', NULL),
    (-4, -1, -4, '11', '2026-03-03 09:00:00', 'approved', NULL);
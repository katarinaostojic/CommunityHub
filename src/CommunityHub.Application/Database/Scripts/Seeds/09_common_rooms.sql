INSERT INTO common_rooms (id, name, description, floor_number, rental_type, building_id)
VALUES
    (-1, 'Rooftop terrace', 'Shared terrace suitable for small gatherings and neighbor meetings.', 4, 'per_day', -4),
    (-2, 'Community hall', 'Large shared room for celebrations, meetings and workshops.', 0, 'multi_day', -4),
    (-3, 'Storage room', 'Shared storage room that can be reserved for short-term use.', 1, 'per_day', -4),
    (-4, 'Laundry room', 'Shared laundry room in the basement.', 0, 'per_day', -1);

INSERT INTO common_room_bookings (id, common_room_id, booked_date)
VALUES
    (-1, -1, '2026-07-05'),
    (-2, -1, '2026-07-12'),
    (-3, -2, '2026-07-15'),
    (-4, -2, '2026-07-16'),
    (-5, -3, '2026-07-08');

INSERT INTO common_room_requests
    (id, common_room_id, tenant_id, date_from, date_to, status, approved_date, proposed_date_from, proposed_date_to)
VALUES
    (-1, -1, -2, '2026-07-06', '2026-07-08', 'approved', '2026-07-06', NULL, NULL),
    (-2, -2, -2, '2026-07-15', '2026-07-17', 'pending_date_change', NULL, '2026-07-20', '2026-07-22'),
    (-3, -3, -2, '2026-07-09', '2026-07-11', 'pending', NULL, NULL, NULL),
    (-4, -1, -2, '2026-07-12', '2026-07-12', 'rejected', NULL, NULL, NULL),
    (-5, -2, -1, '2026-07-25', '2026-07-27', 'pending', NULL, NULL, NULL);
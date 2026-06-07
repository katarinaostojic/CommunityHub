INSERT INTO common_rooms (id, name, description, floor_number, rental_type, building_id)
VALUES
    -- Champs-Élysées, building_id = -4
    (-1, 'Rooftop Terrace', 'Open terrace suitable for small celebrations, evening gatherings and neighbor meetups.', 4, 'per_day', -4),
    (-2, 'Community Hall', 'Large multipurpose hall suitable for meetings, birthdays, workshops and building events.', 0, 'multi_day', -4),
    (-3, 'Laundry Room', 'Shared laundry room with washing machines and dryers available for tenant reservations.', 0, 'per_day', -4),
    (-4, 'Study Room', 'Quiet shared room intended for studying, remote work and small group projects.', 2, 'per_day', -4),
    (-5, 'Game Room', 'Indoor recreation room with tables and space for board games and casual tenant gatherings.', 1, 'per_day', -4),
    (-6, 'Fitness Room', 'Small shared fitness room for light exercise and individual training sessions.', 3, 'per_day', -4),
    (-7, 'Guest Apartment', 'Shared guest apartment that tenants can reserve for visitors staying for several days.', 4, 'multi_day', -4),
    (-8, 'Workshop Room', 'Practical room for repairs, small DIY projects and temporary equipment use.', 1, 'multi_day', -4),

    -- La Rambla, building_id = -6
    (-20, 'Rooftop Terrace', 'Open terrace suitable for small celebrations, evening gatherings and neighbor meetups.', 5, 'per_day', -6),
    (-21, 'Community Hall', 'Large multipurpose hall suitable for meetings, birthdays, workshops and building events.', 0, 'multi_day', -6),
    (-22, 'Laundry Room', 'Shared laundry room with washing machines and dryers available for tenant reservations.', 0, 'per_day', -6),
    (-23, 'Study Room', 'Quiet shared room intended for studying, remote work and small group projects.', 2, 'per_day', -6),
    (-24, 'Game Room', 'Indoor recreation room with tables and space for board games and casual tenant gatherings.', 1, 'per_day', -6),
    (-25, 'Fitness Room', 'Small shared fitness room for light exercise and individual training sessions.', 3, 'per_day', -6),
    (-26, 'Guest Apartment', 'Shared guest apartment that tenants can reserve for visitors staying for several days.', 4, 'multi_day', -6),
    (-27, 'Workshop Room', 'Practical room for repairs, small DIY projects and temporary equipment use.', 1, 'multi_day', -6);

INSERT INTO common_room_bookings (id, common_room_id, booked_date)
VALUES
    (-1, -1, CURRENT_DATE + 8),
    (-2, -1, CURRENT_DATE + 12),
    (-3, -2, CURRENT_DATE + 15),
    (-4, -2, CURRENT_DATE + 16),
    (-5, -3, CURRENT_DATE + 9),
    (-6, -5, CURRENT_DATE + 11),
    (-7, -7, CURRENT_DATE + 20),
    (-8, -7, CURRENT_DATE + 21),
    (-9, -8, CURRENT_DATE + 18),

    (-20, -20, CURRENT_DATE + 8),
    (-21, -20, CURRENT_DATE + 13),
    (-22, -21, CURRENT_DATE + 15),
    (-23, -21, CURRENT_DATE + 16),
    (-24, -22, CURRENT_DATE + 10),
    (-25, -24, CURRENT_DATE + 12),
    (-26, -26, CURRENT_DATE + 19),
    (-27, -26, CURRENT_DATE + 20),
    (-28, -27, CURRENT_DATE + 17);

INSERT INTO common_room_requests
    (id, common_room_id, tenant_id, date_from, date_to, status, approved_date, proposed_date_from, proposed_date_to)
VALUES
    -- Requests for Ana in Champs-Élysées
    (-1, -1, -2, CURRENT_DATE + 6, CURRENT_DATE + 6, 'approved', CURRENT_DATE + 6, NULL, NULL),
    (-2, -2, -2, CURRENT_DATE + 15, CURRENT_DATE + 17, 'pending_date_change', NULL, CURRENT_DATE + 22, CURRENT_DATE + 24),
    (-3, -3, -2, CURRENT_DATE + 9, CURRENT_DATE + 11, 'pending', NULL, NULL, NULL),
    (-4, -4, -2, CURRENT_DATE + 12, CURRENT_DATE + 12, 'rejected', NULL, NULL, NULL),
    (-5, -5, -2, CURRENT_DATE + 14, CURRENT_DATE + 14, 'pending', NULL, NULL, NULL),
    (-6, -7, -2, CURRENT_DATE + 20, CURRENT_DATE + 22, 'pending_date_change', NULL, CURRENT_DATE + 25, CURRENT_DATE + 27),
    (-7, -8, -2, CURRENT_DATE + 18, CURRENT_DATE + 20, 'approved', CURRENT_DATE + 18, NULL, NULL),

    -- Requests for Marko in Champs-Élysées
    (-8, -1, -1, CURRENT_DATE + 10, CURRENT_DATE + 10, 'pending', NULL, NULL, NULL),
    (-9, -2, -1, CURRENT_DATE + 25, CURRENT_DATE + 27, 'pending', NULL, NULL, NULL),

    -- Requests for Ana in La Rambla
    (-20, -20, -2, CURRENT_DATE + 7, CURRENT_DATE + 7, 'approved', CURRENT_DATE + 7, NULL, NULL),
    (-21, -21, -2, CURRENT_DATE + 15, CURRENT_DATE + 17, 'pending_date_change', NULL, CURRENT_DATE + 23, CURRENT_DATE + 25),
    (-22, -22, -2, CURRENT_DATE + 10, CURRENT_DATE + 12, 'pending', NULL, NULL, NULL),
    (-23, -23, -2, CURRENT_DATE + 13, CURRENT_DATE + 13, 'rejected', NULL, NULL, NULL),
    (-24, -24, -2, CURRENT_DATE + 14, CURRENT_DATE + 14, 'pending', NULL, NULL, NULL),
    (-25, -26, -2, CURRENT_DATE + 19, CURRENT_DATE + 21, 'pending_date_change', NULL, CURRENT_DATE + 26, CURRENT_DATE + 28),

    -- Requests for Marko in La Rambla
    (-26, -20, -1, CURRENT_DATE + 11, CURRENT_DATE + 11, 'pending', NULL, NULL, NULL),
    (-27, -21, -1, CURRENT_DATE + 28, CURRENT_DATE + 30, 'pending', NULL, NULL, NULL);
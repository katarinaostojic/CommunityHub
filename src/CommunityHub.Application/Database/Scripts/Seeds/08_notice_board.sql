INSERT INTO notice_board_ads (id, user_id, building_id, type, category, description, date_from, date_to, status)
VALUES
    (-1, -2, -4, 'offering', 'moving', 'I can help neighbors carry boxes and small furniture during moving.', '2026-07-01', '2026-07-10', 'active'),
    (-2, -1, -4, 'seeking', 'moving', 'Need help moving boxes from the second floor.', '2026-07-02', '2026-07-08', 'active'),
    (-3, -1, -4, 'seeking', 'moving', 'Need help moving a couch from the hallway.', '2026-07-03', '2026-07-04', 'active'),
    (-4, -2, -4, 'seeking', 'cleaning', 'Need help cleaning the storage room after renovation.', '2026-07-12', '2026-07-16', 'active'),
    (-5, -1, -4, 'offering', 'cleaning', 'I can help with hallway and storage cleaning.', '2026-07-13', '2026-07-18', 'active'),
    (-6, -2, -4, 'offering', 'appliance_repair', 'Can help with small appliance checks.', '2026-04-01', '2026-04-03', 'archived'),
    (-7, -2, -4, 'seeking', 'lending', 'Looking to borrow a ladder for one afternoon.', '2026-07-20', '2026-07-22', 'active'),
    (-8, -1, -4, 'offering', 'lending', 'I can lend a ladder and basic tools.', '2026-07-21', '2026-07-25', 'active');

INSERT INTO notice_board_time_slots (id, ad_id, date, start_time, end_time)
VALUES
    (-1, -1, '2026-07-03', '16:00', '17:00'),
    (-2, -1, '2026-07-03', '17:00', '18:00'),
    (-3, -1, '2026-07-04', '16:00', '17:00'),
    (-4, -1, '2026-07-04', '17:00', '18:00'),

    (-5, -2, '2026-07-03', '16:00', '17:00'),
    (-6, -2, '2026-07-03', '17:00', '18:00'),
    (-7, -3, '2026-07-03', '16:00', '17:00'),
    (-8, -3, '2026-07-04', '16:00', '17:00'),

    (-9, -5, '2026-07-13', '16:00', '17:00'),
    (-10, -5, '2026-07-13', '17:00', '18:00'),
    (-11, -5, '2026-07-14', '16:00', '17:00'),
    (-12, -5, '2026-07-14', '17:00', '18:00'),

    (-13, -8, '2026-07-21', '16:00', '17:00'),
    (-14, -8, '2026-07-21', '17:00', '18:00'),
    (-15, -8, '2026-07-22', '16:00', '17:00'),
    (-16, -8, '2026-07-22', '17:00', '18:00');

INSERT INTO notice_board_bookings (id, time_slot_id, booked_by_ad_id)
VALUES
    (-1, -1, -3),
    (-2, -9, -4);

INSERT INTO notice_board_notifications (id, recipient_id, ad_id, related_ad_id, type, created_at, is_read)
VALUES
    (-1, -2, -1, -3, 'booking', '2026-06-01 09:30:00', FALSE),
    (-2, -2, -7, -8, 'matching_ad', '2026-06-01 10:00:00', FALSE),
    (-3, -2, -4, -5, 'matching_ad', '2026-06-01 10:30:00', TRUE);
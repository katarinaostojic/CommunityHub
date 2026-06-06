INSERT INTO notice_board_ads (id, user_id, building_id, type, category, description, date_from, date_to, status)
VALUES
    -- Champs-Élysées, building_id = -4
    (-1, -2, -4, 'offering', 'moving', 'I can help neighbors carry boxes and small furniture during moving.', CURRENT_DATE + 8, CURRENT_DATE + 16, 'active'),
    (-2, -1, -4, 'seeking', 'moving', 'Need help moving boxes from the second floor to a van.', CURRENT_DATE + 9, CURRENT_DATE + 14, 'active'),
    (-3, -1, -4, 'seeking', 'moving', 'Need help moving a couch from the hallway.', CURRENT_DATE + 10, CURRENT_DATE + 11, 'active'),
    (-4, -2, -4, 'offering', 'cleaning', 'I can help clean storage rooms, hallways and shared areas.', CURRENT_DATE + 12, CURRENT_DATE + 20, 'active'),
    (-5, -1, -4, 'seeking', 'cleaning', 'Need help cleaning the basement storage after renovation.', CURRENT_DATE + 13, CURRENT_DATE + 18, 'active'),
    (-6, -1, -4, 'offering', 'lending', 'I can lend a ladder, drill and basic tool box.', CURRENT_DATE + 7, CURRENT_DATE + 21, 'active'),
    (-7, -2, -4, 'seeking', 'lending', 'Looking to borrow a ladder for one afternoon.', CURRENT_DATE + 8, CURRENT_DATE + 12, 'active'),
    (-8, -1, -4, 'offering', 'lending', 'I can lend folding chairs for small apartment gatherings.', CURRENT_DATE + 9, CURRENT_DATE + 17, 'active'),
    (-9, -2, -4, 'seeking', 'lending', 'Need to borrow a carpet cleaner for the weekend.', CURRENT_DATE + 10, CURRENT_DATE + 15, 'active'),
    (-10, -1, -4, 'offering', 'appliance_repair', 'Can help with small appliance checks and simple repairs.', CURRENT_DATE + 11, CURRENT_DATE + 19, 'active'),
    (-11, -2, -4, 'seeking', 'appliance_repair', 'Need help checking a washing machine that stopped spinning.', CURRENT_DATE + 12, CURRENT_DATE + 16, 'active'),
    (-12, -1, -4, 'seeking', 'other', 'Need help setting up shelves in the apartment.', CURRENT_DATE + 14, CURRENT_DATE + 18, 'active'),
    (-13, -2, -4, 'offering', 'other', 'I can help with basic computer setup and printer installation.', CURRENT_DATE + 15, CURRENT_DATE + 23, 'active'),
    (-14, -1, -4, 'offering', 'cleaning', 'Available to help clean the building entrance before the meeting.', CURRENT_DATE + 16, CURRENT_DATE + 22, 'active'),
    (-15, -2, -4, 'seeking', 'moving', 'Need one more person to help move a desk upstairs.', CURRENT_DATE + 17, CURRENT_DATE + 19, 'active'),
    (-16, -2, -4, 'offering', 'moving', 'Available for carrying smaller boxes in the evening.', CURRENT_DATE - 20, CURRENT_DATE - 15, 'archived'),

    -- La Rambla, building_id = -6
    (-31, -2, -6, 'offering', 'moving', 'I can help with moving boxes inside the La Rambla building.', CURRENT_DATE + 8, CURRENT_DATE + 15, 'active'),
    (-32, -1, -6, 'seeking', 'moving', 'Need help moving a bookshelf to the fourth floor.', CURRENT_DATE + 9, CURRENT_DATE + 12, 'active'),
    (-33, -1, -6, 'offering', 'cleaning', 'I can help clean the shared terrace after a gathering.', CURRENT_DATE + 10, CURRENT_DATE + 18, 'active'),
    (-34, -2, -6, 'seeking', 'cleaning', 'Need help cleaning the guest apartment after visitors leave.', CURRENT_DATE + 11, CURRENT_DATE + 16, 'active'),
    (-35, -1, -6, 'offering', 'lending', 'I can lend an extension cord, ladder and small toolkit.', CURRENT_DATE + 7, CURRENT_DATE + 20, 'active'),
    (-36, -2, -6, 'seeking', 'lending', 'Looking to borrow a projector for a residents meeting.', CURRENT_DATE + 8, CURRENT_DATE + 13, 'active'),
    (-37, -1, -6, 'offering', 'appliance_repair', 'Can help inspect minor kitchen appliance issues.', CURRENT_DATE + 12, CURRENT_DATE + 19, 'active'),
    (-38, -2, -6, 'seeking', 'appliance_repair', 'Need help checking a microwave that stopped working.', CURRENT_DATE + 13, CURRENT_DATE + 17, 'active'),
    (-39, -1, -6, 'offering', 'other', 'I can help assemble small furniture in the evening.', CURRENT_DATE + 14, CURRENT_DATE + 22, 'active'),
    (-40, -2, -6, 'seeking', 'other', 'Need help carrying plants to the rooftop terrace.', CURRENT_DATE + 15, CURRENT_DATE + 18, 'active');

INSERT INTO notice_board_time_slots (id, ad_id, date, start_time, end_time)
VALUES
    (-1, -1, CURRENT_DATE + 9, '16:00', '17:00'),
    (-2, -1, CURRENT_DATE + 9, '17:00', '18:00'),
    (-3, -1, CURRENT_DATE + 10, '16:00', '17:00'),
    (-4, -1, CURRENT_DATE + 10, '18:00', '19:00'),

    (-5, -2, CURRENT_DATE + 9, '16:00', '17:00'),
    (-6, -2, CURRENT_DATE + 10, '17:00', '18:00'),
    (-7, -3, CURRENT_DATE + 10, '16:00', '17:00'),
    (-8, -3, CURRENT_DATE + 11, '18:00', '19:00'),

    (-9, -4, CURRENT_DATE + 13, '16:00', '17:00'),
    (-10, -4, CURRENT_DATE + 13, '17:00', '18:00'),
    (-11, -4, CURRENT_DATE + 14, '16:00', '17:00'),
    (-12, -4, CURRENT_DATE + 14, '18:00', '19:00'),

    (-13, -5, CURRENT_DATE + 13, '17:00', '18:00'),
    (-14, -5, CURRENT_DATE + 15, '16:00', '17:00'),

    (-15, -6, CURRENT_DATE + 8, '16:00', '17:00'),
    (-16, -6, CURRENT_DATE + 9, '17:00', '18:00'),
    (-17, -6, CURRENT_DATE + 10, '18:00', '19:00'),
    (-18, -8, CURRENT_DATE + 9, '16:00', '17:00'),
    (-19, -8, CURRENT_DATE + 11, '17:00', '18:00'),
    (-20, -8, CURRENT_DATE + 12, '18:00', '19:00'),

    (-21, -10, CURRENT_DATE + 12, '16:00', '17:00'),
    (-22, -10, CURRENT_DATE + 13, '17:00', '18:00'),
    (-23, -11, CURRENT_DATE + 13, '16:00', '17:00'),
    (-24, -13, CURRENT_DATE + 16, '18:00', '19:00'),
    (-25, -14, CURRENT_DATE + 17, '16:00', '17:00'),

    (-31, -31, CURRENT_DATE + 9, '16:00', '17:00'),
    (-32, -31, CURRENT_DATE + 10, '17:00', '18:00'),
    (-33, -32, CURRENT_DATE + 9, '18:00', '19:00'),
    (-34, -33, CURRENT_DATE + 11, '16:00', '17:00'),
    (-35, -33, CURRENT_DATE + 12, '17:00', '18:00'),
    (-36, -35, CURRENT_DATE + 8, '16:00', '17:00'),
    (-37, -35, CURRENT_DATE + 9, '18:00', '19:00'),
    (-38, -36, CURRENT_DATE + 9, '17:00', '18:00'),
    (-39, -37, CURRENT_DATE + 13, '16:00', '17:00'),
    (-40, -39, CURRENT_DATE + 15, '18:00', '19:00');

INSERT INTO notice_board_bookings (id, time_slot_id, booked_by_ad_id)
VALUES
    (-1, -1, -3),
    (-2, -9, -5),
    (-3, -15, -7),
    (-4, -31, -32),
    (-5, -34, -34);

INSERT INTO notice_board_notifications (id, recipient_id, ad_id, related_ad_id, type, created_at, is_read)
VALUES
    (-1, -2, -1, -3, 'booking', CURRENT_TIMESTAMP - INTERVAL '5 days', FALSE),
    (-2, -2, -7, -6, 'matching_ad', CURRENT_TIMESTAMP - INTERVAL '4 days', FALSE),
    (-3, -2, -4, -5, 'matching_ad', CURRENT_TIMESTAMP - INTERVAL '3 days', TRUE),
    (-4, -2, -31, -32, 'booking', CURRENT_TIMESTAMP - INTERVAL '2 days', FALSE),
    (-5, -2, -36, -35, 'matching_ad', CURRENT_TIMESTAMP - INTERVAL '1 day', FALSE);
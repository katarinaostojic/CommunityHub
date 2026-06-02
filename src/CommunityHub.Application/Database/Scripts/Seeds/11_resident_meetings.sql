INSERT INTO resident_meetings (id, building_id, meeting_date, meeting_time, status)
VALUES
    (-1, -1, CURRENT_DATE + 20, '18:00', 'scheduled'),
    (-2, -1, CURRENT_DATE + 10, '18:00', 'confirmed'),
    (-3, -1, CURRENT_DATE + 5, '18:00', 'cancelled'),
    (-4, -4, CURRENT_DATE + 18, '19:00', 'scheduled');

INSERT INTO resident_meeting_topics (id, meeting_id, topic)
VALUES
    (-1, -1, 'Making renovation plans'),
    (-2, -1, 'Maintenance fee increase'),
    (-3, -2, 'Parking space allocation'),
    (-4, -2, 'New intercom system'),
    (-5, -2, 'Garden maintenance schedule'),
    (-6, -3, 'Roof repair discussion'),
    (-7, -4, 'Building entrance lighting'),
    (-8, -4, 'Cleaning schedule');

INSERT INTO resident_meeting_attendances (id, meeting_id, tenant_id, unit_number, created_at)
VALUES
    (-1, -1, -1, '1', NOW()),
    (-2, -1, -1, '2', NOW()),
    (-3, -1, -1, '3', NOW()),
    (-4, -1, -1, '4', NOW()),
    (-5, -1, -1, '5', NOW()),
    (-6, -1, -1, '7', NOW()),
    (-7, -1, -1, '8', NOW()),
    (-8, -1, -1, '9', NOW()),

    (-9, -2, -1, '1', NOW()),
    (-10, -2, -1, '2', NOW()),
    (-11, -2, -1, '3', NOW()),
    (-12, -2, -1, '4', NOW()),
    (-13, -2, -1, '5', NOW()),
    (-14, -2, -1, '7', NOW()),
    (-15, -2, -1, '8', NOW()),
    (-16, -2, -1, '9', NOW()),
    (-17, -2, -1, '11', NOW()),
    (-18, -2, -1, '12', NOW()),
    (-19, -2, -1, '13', NOW()),
    (-20, -2, -1, '14', NOW()),

    (-21, -3, -1, '1', NOW()),
    (-22, -3, -1, '2', NOW()),
    (-23, -3, -1, '3', NOW()),
    (-24, -3, -1, '4', NOW()),
    (-25, -3, -1, '5', NOW()),
    (-26, -3, -1, '7', NOW()),
    (-27, -3, -1, '8', NOW()),

    (-28, -4, -1, '1', NOW()),
    (-29, -4, -1, '2', NOW()),
    (-30, -4, -1, '3', NOW()),
    (-31, -4, -1, '4', NOW()),
    (-32, -4, -1, '5', NOW());
INSERT INTO event_registrations (event_id, citizen_id, attended)
VALUES (
    (SELECT id FROM events WHERE neighborhood_id = -1 LIMIT 1),
    -7,
    true
);
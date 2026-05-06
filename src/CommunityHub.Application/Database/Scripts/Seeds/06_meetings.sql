INSERT INTO meetings (neighborhood_id, theme, meeting_time, date_range_start, date_range_end, status)
VALUES (-1, 'welcome', '18:00', '2026-05-10', '2026-05-15', 'in_preparation');

INSERT INTO meeting_votes (meeting_id, citizen_id, voted_date)
VALUES (currval('meetings_id_seq'), -1, '2026-05-12');
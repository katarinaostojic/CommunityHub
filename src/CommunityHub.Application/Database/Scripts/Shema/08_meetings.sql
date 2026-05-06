CREATE TABLE meetings (
    id SERIAL PRIMARY KEY,
    neighborhood_id BIGINT NOT NULL REFERENCES neighborhoods(id),
    theme VARCHAR(50) NOT NULL,
    meeting_time TIME NOT NULL,
    date_range_start DATE NOT NULL,
    date_range_end DATE NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'in_preparation',
    scheduled_date DATE NULL
);

CREATE TABLE meeting_votes (
    id SERIAL PRIMARY KEY,
    meeting_id BIGINT NOT NULL REFERENCES meetings(id),
    citizen_id BIGINT NOT NULL REFERENCES users(id),
    voted_date DATE NOT NULL
);
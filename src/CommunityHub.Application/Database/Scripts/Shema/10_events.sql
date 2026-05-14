CREATE TABLE events (
    id SERIAL PRIMARY KEY,
    neighborhood_id BIGINT NOT NULL REFERENCES neighborhoods(id),
    organizer_id BIGINT NOT NULL REFERENCES users(id),
    name VARCHAR(255) NOT NULL,
    description TEXT NOT NULL,
    event_date DATE NOT NULL,
    start_time TIME NOT NULL,
    duration_minutes INT NOT NULL,
    min_volunteers INT NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'preparation'
);

CREATE TABLE event_items (
    id SERIAL PRIMARY KEY,
    event_id BIGINT NOT NULL REFERENCES events(id),
    name VARCHAR(255) NOT NULL,
    is_taken BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE event_registrations (
    id SERIAL PRIMARY KEY,
    event_id BIGINT NOT NULL REFERENCES events(id),
    citizen_id BIGINT NOT NULL REFERENCES users(id),
    registered_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE event_registration_items (
    registration_id BIGINT NOT NULL REFERENCES event_registrations(id),
    item_id BIGINT NOT NULL REFERENCES event_items(id),
    PRIMARY KEY (registration_id, item_id)
);
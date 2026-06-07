CREATE TABLE city_gifts (
    id BIGSERIAL PRIMARY KEY,
    amount DECIMAL(10,2) NOT NULL CHECK (amount > 0),
    deadline DATE NOT NULL,
    awarded_neighborhood_id BIGINT REFERENCES neighborhoods(id),
    is_awarded BOOLEAN NOT NULL DEFAULT FALSE,
    created_at DATE NOT NULL DEFAULT CURRENT_DATE
);

CREATE TABLE city_gift_applications (
    id BIGSERIAL PRIMARY KEY,
    city_gift_id BIGINT NOT NULL REFERENCES city_gifts(id) ON DELETE CASCADE,
    coordinator_id BIGINT NOT NULL REFERENCES users(id),
    neighborhood_id BIGINT NOT NULL REFERENCES neighborhoods(id),
    applied_at DATE NOT NULL DEFAULT CURRENT_DATE,
    UNIQUE (city_gift_id, neighborhood_id)
);

-- Seed data za testiranje
INSERT INTO city_gifts (amount, deadline) VALUES
(50000.00, '2026-06-30'),
(30000.00, '2026-07-15'),
(100000.00, '2026-05-31');
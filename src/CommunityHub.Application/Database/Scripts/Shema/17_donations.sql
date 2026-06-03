CREATE TABLE donation_categories (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

INSERT INTO donation_categories (name) VALUES
('Uređenje ulica'),
('Uređenje parkova'),
('Rasveta'),
('Čistoća'),
('Zelenilo'),
('Hitne popravke');

CREATE TABLE donations (
    id BIGSERIAL PRIMARY KEY,
    citizen_id BIGINT NOT NULL REFERENCES users(id),
    neighborhood_id BIGINT NOT NULL REFERENCES neighborhoods(id),
    category_id BIGINT NOT NULL REFERENCES donation_categories(id),
    amount DECIMAL(10,2) NOT NULL CHECK (amount > 0),
    created_at DATE NOT NULL DEFAULT CURRENT_DATE
);

CREATE TABLE expenses (
    id BIGSERIAL PRIMARY KEY,
    neighborhood_id BIGINT NOT NULL REFERENCES neighborhoods(id),
    category_id BIGINT NOT NULL REFERENCES donation_categories(id),
    amount DECIMAL(10,2) NOT NULL CHECK (amount > 0),
    description TEXT NOT NULL,
    created_at DATE NOT NULL DEFAULT CURRENT_DATE
);
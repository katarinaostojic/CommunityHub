CREATE TABLE coordinator_reviews (
    id BIGSERIAL PRIMARY KEY,
    citizen_id BIGINT NOT NULL REFERENCES users(id),
    coordinator_id BIGINT NOT NULL REFERENCES users(id),
    neighborhood_id BIGINT NOT NULL REFERENCES neighborhoods(id),
    rating INT NOT NULL CHECK (rating BETWEEN 1 AND 5),
    comment TEXT,
    created_at DATE NOT NULL DEFAULT CURRENT_DATE,
    report_count INT NOT NULL DEFAULT 0,
    is_removed BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE coordinator_review_reports (
    id BIGSERIAL PRIMARY KEY,
    review_id BIGINT NOT NULL REFERENCES coordinator_reviews(id) ON DELETE CASCADE,
    citizen_id BIGINT NOT NULL REFERENCES users(id),
    created_at DATE NOT NULL DEFAULT CURRENT_DATE,
    UNIQUE(review_id, citizen_id)
);
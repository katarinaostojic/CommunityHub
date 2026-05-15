CREATE TABLE IF NOT EXISTS forums (
    id BIGSERIAL PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    description TEXT NOT NULL,
    coordinator_id BIGINT NOT NULL REFERENCES users(id),
    is_closed BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS forum_comments (
    id BIGSERIAL PRIMARY KEY,
    forum_id BIGINT NOT NULL REFERENCES forums(id),
    coordinator_id BIGINT NOT NULL REFERENCES users(id),
    text TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS forum_comment_reactions (
    id BIGSERIAL PRIMARY KEY,
    comment_id BIGINT NOT NULL REFERENCES forum_comments(id),
    coordinator_id BIGINT NOT NULL REFERENCES users(id),
    reaction VARCHAR(10) NOT NULL CHECK (reaction IN ('like', 'dislike')),
    UNIQUE (comment_id, coordinator_id)
);
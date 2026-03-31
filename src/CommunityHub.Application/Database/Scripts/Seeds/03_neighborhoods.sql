INSERT INTO neighborhoods (id, name, description, city_id, budget, coordinator_id) VALUES
(-1, 'Liman', 'Mirno stambeno naselje u Novom Sadu pored Dunava.', -2, 0, -5),
(-2, 'Dorcol', 'Staro beogradsko naselje sa bogatom istorijom.', -1, 0, -6);

INSERT INTO neighborhood_streets (id, neighborhood_id, street_name, start_number, end_number) VALUES
(-1, -1, 'Bulevar oslobođenja', 1, 100),
(-2, -1, 'Kisačka', 1, 50),
(-3, -2, 'Cara Dušana', 1, 80),
(-4, -2, 'Gospodar Jevremova', 1, 60);


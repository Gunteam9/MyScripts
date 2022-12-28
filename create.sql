CREATE TABLE IF NOT EXISTS PlayerInfo(
    Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
    SteamId INTEGER UNSIGNED NOT NULL,

    PRIMARY KEY(Id)
);

CREATE TABLE IF NOT EXISTS PlayerMoney (
    Id INTEGER UNSIGNED NOT NULL,
    Cash DECIMAL NOT NULL,
    CurrentAccount DECIMAL NOT NULL,
    Savings DECIMAL NOT NULL,
    LifeInsurance DECIMAL NOT NULL,

    FOREIGN KEY(Id) REFERENCES PlayerInfo(Id) ON DELETE CASCADE,
    PRIMARY KEY(Id)
);

CREATE TABLE IF NOT EXISTS Company (
    Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
    Name_ TEXT NOT NULL,
    Acronym VARCHAR(4) NOT NULL,
    PriceAtOpening DECIMAL NOT NULL DEFAULT 50,
    CurrentPrice DECIMAL NOT NULL DEFAULT 50,

    PRIMARY KEY(Id)
);

CREATE TABLE IF NOT EXISTS Stock (
    Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
    PlayerId INTEGER UNSIGNED NOT NULL,
    CompanyId INTEGER UNSIGNED NOT NULL,
    Type_ SMALLINT UNSIGNED NOT NULL,
    Amount INTEGER NOT NULL,
    SimulatedAmount INTEGER NOT NULL,
    AverageBuyValue DECIMAL NOT NULL,

    FOREIGN KEY(PlayerId) REFERENCES PlayerInfo(Id) ON DELETE CASCADE,
    FOREIGN KEY(CompanyId) REFERENCES Company(Id) ON DELETE CASCADE,
    PRIMARY KEY(Id)
);

CREATE TABLE IF NOT EXISTS CompanySector (
    Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
    CompanyId INTEGER UNSIGNED NOT NULL,
    SectorId SMALLINT UNSIGNED NOT NULL,

    FOREIGN KEY(CompanyId) REFERENCES Company(Id) ON DELETE CASCADE,
    PRIMARY KEY(Id)
);

-- TODO: Find another way to differentiate it
CREATE TABLE IF NOT EXISTS Transaction_ (
    Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
    Source INTEGER UNSIGNED NOT NULL,
    Target INTEGER UNSIGNED NOT NULL,
    Type_ SMALLINT UNSIGNED NOT NULL,
    Amount DECIMAL NOT NULL,
    Date_ DATE NOT NULL,
    SourceAccount INTEGER UNSIGNED NOT NULL,
    TargetAccount INTEGER UNSIGNED NOT NULL,

    FOREIGN KEY(Source) REFERENCES PlayerInfo(Id) ON DELETE CASCADE,
    FOREIGN KEY(Target) REFERENCES PlayerInfo(Id) ON DELETE CASCADE,
    PRIMARY KEY(Id)
);

CREATE TABLE IF NOT EXISTS Trend (
    Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
    Start_ DATE NOT NULL,
    Duration SMALLINT UNSIGNED NOT NULL,
    Importance SMALLINT NOT NULL,
    Sector SMALLINT UNSIGNED NOT NULL,

    PRIMARY KEY(Id)
);

CREATE TABLE IF NOT EXISTS CompanyFarmLog(
    Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
    CompanyId INTEGER UNSIGNED NOT NULL,
    FarmDate DATETIME NOT NULL,
    FarmCaValue DECIMAL NOT NULL,

    FOREIGN KEY(CompanyId) REFERENCES Company(Id) ON DELETE CASCADE,
    PRIMARY KEY(Id)
);

INSERT IGNORE INTO Company(Id, Name_, Acronym) VALUES
    (1,  'Ambeer',               'ABR'),
    (2,  'Benny''s',             'BNYS'),
    (3,  'BlackCat',             'BC'),
    (4,  'Chilliad Valley Farm', 'CVF'),
    (5,  'Darnell Bros',         'DB'),
    (6,  'Giggling Squid',       'GS'),
    (7,  'Gruppe 6',             'GRP'),
    (8,  'Humane Labs',          'HL'),
    (9,  'I-Volt',               'VOLT'),
    (10, 'Los Santos Taxi',      'LST'),
    (11, 'Lumber Yard',          'LMBR'),
    (12, 'Marlow Vinyard',       'MRLW'),
    (13, 'Pacific Bank',         'PCF'),
    (14, 'Roger''s',             'RGRS'),
    (15, 'Ron Petroleum',        'RON'),
    (16, 'Weazel News',          'WZL'),
    (17, 'Yellow Jack',          'YJ'),
    (18, 'Zombie Car',           'ZC');

INSERT IGNORE INTO CompanySector(CompanyId, SectorId) VALUES
    -- Ambeer
    (1, 1), (1, 5), (1, 6),
    -- Benny's
    (2, 1), (2, 7),
    -- BlackCat
    (3, 0), (3, 1), (3, 7),
    -- Chilliad Valley Farm
    (4, 5), (4, 6),
    -- Darnell Bros
    (5, 5), (5, 6),
    -- Giggling Squid
    (6, 5), (6, 6),
    -- Gruppe 6
    (7, 1), (7, 3), (7, 7),
    -- Humane Labs
    (8, 1), (8, 4),
    -- I-Volt
    (9, 2), (9, 5),
    -- Los Santos Taxi
    (10, 0), (10, 7),
    -- Lumber Yard
    (11, 1), (11, 5), (11, 6),
    -- Marlow Vinyard
    (12, 1), (12, 5), (12, 6),
    -- Pacific Bank
    (13, 3), (13, 7),
    -- Roger's
    (14, 1), (14, 7),
    -- Ron Petroleum
    (15, 2), (15, 5),
    -- Weazel News
    (16, 0), (16, 1), (16, 7),
    -- Yellow Jack
    (17, 0), (17, 1), (17, 7),
    -- Zombie Car
    (18, 1), (18, 7);


-- DROP TABLE CompanyFarmLog;
-- DROP TABLE Trend;
-- DROP TABLE Transaction_;
-- DROP TABLE CompanySector;
-- DROP TABLE Stock;
-- DROP TABLE Company;
-- DROP TABLE PlayerMoney;
-- DROP TABLE PlayerInfo;
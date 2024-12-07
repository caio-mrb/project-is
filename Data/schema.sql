USE SOMIOD;

-- Applications Table
CREATE TABLE applications (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL,
    creation_datetime DATETIME NOT NULL
);

-- Containers Table
CREATE TABLE containers (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL,
    creation_datetime DATETIME NOT NULL,
    application_id INT NOT NULL REFERENCES applications(id)
);

-- Records Table
CREATE TABLE records (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL,
    content TEXT NOT NULL,
    creation_datetime DATETIME NOT NULL,
<<<<<<< HEAD
    containers_id INT NOT NULL REFERENCES containers(id)
=======
    container_id INT NOT NULL REFERENCES containers(id)
>>>>>>> afa1a69cc9bab38189c8122885ddd588a73229ea
);

-- Notifications Table
CREATE TABLE notifications (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL,
    creation_datetime DATETIME NOT NULL,
<<<<<<< HEAD
    containers_id INT NOT NULL REFERENCES containers(id),
    event_type INT NOT NULL CHECK (event_type IN ('C', 'D')),
=======
    container_id INT NOT NULL REFERENCES containers(id),
    event_type CHAR(1) NOT NULL CHECK (event_type IN ('C', 'D')),
>>>>>>> afa1a69cc9bab38189c8122885ddd588a73229ea
    endpoint VARCHAR(255) NOT NULL,
    enabled BIT NOT NULL
);

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
    parent INT NOT NULL REFERENCES applications(id)
);

-- Records Table
CREATE TABLE records (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL,
    content TEXT NOT NULL,
    creation_datetime DATETIME NOT NULL,
    parent INT NOT NULL REFERENCES containers(id)
);

-- Notifications Table
CREATE TABLE notifications (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL,
    creation_datetime DATETIME NOT NULL,
    parent INT NOT NULL REFERENCES containers(id),
    event INT NOT NULL CHECK (event IN (1,2)), --1=create 2=delete
    endpoint VARCHAR(255) NOT NULL,
    enabled BIT NOT NULL
);

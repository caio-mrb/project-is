USE SOMIOD;

-- Insert into the applications table
INSERT INTO applications (name, creation_datetime)
VALUES 
('App1', '2024-12-01T10:00:00'),
('App2', '2024-12-02T10:15:00'),
('App3', '2024-12-03T10:30:00'),
('App4', '2024-12-04T10:45:00'),
('App5', '2024-12-05T11:00:00'),
('App6', '2024-12-06T11:15:00'),
('App7', '2024-12-07T11:30:00'),
('App8', '2024-12-08T11:45:00'),
('App9', '2024-12-09T12:00:00'),
('App10', '2024-12-10T12:15:00');

-- Insert into the containers table
INSERT INTO containers (name, creation_datetime, parent)
VALUES 
('Container1', '2024-12-01T11:00:00', 1),
('Container2', '2024-12-02T11:15:00', 1),
('Container3', '2024-12-03T11:30:00', 2),
('Container4', '2024-12-04T11:45:00', 2),
('Container5', '2024-12-05T12:00:00', 3),
('Container6', '2024-12-06T12:15:00', 3),
('Container7', '2024-12-07T12:30:00', 4),
('Container8', '2024-12-08T12:45:00', 4),
('Container9', '2024-12-09T13:00:00', 5),
('Container10', '2024-12-10T13:15:00', 5);

-- Insert into the records table
INSERT INTO records (name, content, creation_datetime, parent)
VALUES 
('Record1', 'Data for Record1', '2024-12-01T12:00:00', 1),
('Record2', 'Data for Record2', '2024-12-02T12:15:00', 2),
('Record3', 'Data for Record3', '2024-12-03T12:30:00', 3),
('Record4', 'Data for Record4', '2024-12-04T12:45:00', 4),
('Record5', 'Data for Record5', '2024-12-05T13:00:00', 5),
('Record6', 'Data for Record6', '2024-12-06T13:15:00', 6),
('Record7', 'Data for Record7', '2024-12-07T13:30:00', 7),
('Record8', 'Data for Record8', '2024-12-08T13:45:00', 8),
('Record9', 'Data for Record9', '2024-12-09T14:00:00', 9),
('Record10', 'Data for Record10', '2024-12-10T14:15:00', 10);

-- Insert into the notifications table
INSERT INTO notifications (name, creation_datetime, parent, event, endpoint, enabled)
VALUES 
('Notification1', '2024-12-01T14:00:00', 1, 1, 'http://example.com/notify1', 1),
('Notification2', '2024-12-02T14:15:00', 2, 2, 'http://example.com/notify2', 1),
('Notification3', '2024-12-03T14:30:00', 3, 1, 'http://example.com/notify3', 1),
('Notification4', '2024-12-04T14:45:00', 4, 2, 'http://example.com/notify4', 1),
('Notification5', '2024-12-05T15:00:00', 5, 1, 'http://example.com/notify5', 1),
('Notification6', '2024-12-06T15:15:00', 6, 2, 'http://example.com/notify6', 1),
('Notification7', '2024-12-07T15:30:00', 7, 1, 'http://example.com/notify7', 1),
('Notification8', '2024-12-08T15:45:00', 8, 2, 'http://example.com/notify8', 1),
('Notification9', '2024-12-09T16:00:00', 9, 1, 'http://example.com/notify9', 1),
('Notification10', '2024-12-10T16:15:00', 10, 2, 'http://example.com/notify10', 1);

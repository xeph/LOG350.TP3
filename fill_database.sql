DELETE FROM tags;
DELETE FROM deadlines;
DELETE FROM events;
DELETE FROM events_tags;
DELETE FROM events_alerts;
DELETE FROM tasks;
DELETE FROM tasks_tags;
DELETE FROM tasks_alerts;

INSERT INTO tags(ID, name) VALUES(1, 'ÉTS');
INSERT INTO tags(ID, name) VALUES(2, 'LOG240');
INSERT INTO tags(ID, name) VALUES(3, 'LOG350');
INSERT INTO tags(ID, name) VALUES(4, 'MAT265');
INSERT INTO tags(ID, name) VALUES(5, 'PHY332');
INSERT INTO tags(ID, name) VALUES(6, 'TP');

INSERT INTO deadlines(ID, deadline) VALUES(1, '2012-12-25');
INSERT INTO deadlines(ID, deadline) VALUES(2, '2012-12-18');
INSERT INTO deadlines(ID, deadline) VALUES(3, '2012-12-20');
INSERT INTO deadlines(ID, deadline) VALUES(4, '2012-12-14');
INSERT INTO deadlines(ID, deadline) VALUES(5, '2012-12-11');
INSERT INTO deadlines(ID, deadline) VALUES(6, '2012-12-06');

INSERT INTO events(ID, deadline_id, name, description) VALUES(1, 1, 'Noël', 'Plein de cadeaux!!!');
INSERT INTO events(ID, deadline_id, name, description) VALUES(2, 3, 'Examen final de LOG240', 'Lorem ipsum');
INSERT INTO events(ID, deadline_id, name, description) VALUES(3, 2, 'Examen final de LOG350', 'Lorem ipsum');
INSERT INTO events(ID, deadline_id, name, description) VALUES(4, 4, 'Examen final de MAT265', 'Lorem ipsum');
INSERT INTO events(ID, deadline_id, name, description) VALUES(5, 5, 'Examen final de PHY332', 'Lorem ipsum');
INSERT INTO events(ID, deadline_id, name, description) VALUES(6, 6, 'Présentation oral du TP3 de LOG350', 'Dernière semaine de la session');

INSERT INTO events_alerts(ID, event_id, action, when_value, when_specifier) VALUES(1, 2, 1, 2, 4);

INSERT INTO events_tags(ID, event_id, tag_id) VALUES(3,  2, 1);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(4,  2, 2);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(5,  3, 1);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(6,  3, 3);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(7,  4, 1);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(8,  4, 4);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(9,  5, 1);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(10, 5, 5);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(11, 6, 1);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(12, 6, 3);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(13, 6, 6);

INSERT INTO tasks(ID, child_of, deadline_id, priority_id, completion, name, description) VALUES(1, NULL, 6,    1, 0, 'Prototypage dynamique et évaluation(s)/test(s)', 'Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.');
INSERT INTO tasks(ID, child_of, deadline_id, priority_id, completion, name, description) VALUES(2, 1,    NULL, 1, 0, 'Prototypage dynamique', 'Lorem ipsum.');
INSERT INTO tasks(ID, child_of, deadline_id, priority_id, completion, name, description) VALUES(3, 1,    NULL, 2, 0, 'Rapport', 'Dolor sit amet.');

INSERT INTO tasks_tags(ID, task_id, tag_id) VALUES(1, 1, 1);
INSERT INTO tasks_tags(ID, task_id, tag_id) VALUES(2, 1, 3);
INSERT INTO tasks_tags(ID, task_id, tag_id) VALUES(3, 1, 6);
INSERT INTO tasks_tags(ID, task_id, tag_id) VALUES(4, 2, 1);
INSERT INTO tasks_tags(ID, task_id, tag_id) VALUES(5, 2, 3);
INSERT INTO tasks_tags(ID, task_id, tag_id) VALUES(6, 2, 6);
INSERT INTO tasks_tags(ID, task_id, tag_id) VALUES(7, 3, 1);
INSERT INTO tasks_tags(ID, task_id, tag_id) VALUES(8, 3, 3);
INSERT INTO tasks_tags(ID, task_id, tag_id) VALUES(9, 3, 6);

INSERT INTO tasks_alerts(ID, task_id, action, when_value, when_specifier) VALUES(1, 1, 1, 2, 2);

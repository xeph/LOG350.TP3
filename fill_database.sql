INSERT INTO tags(ID, name) VALUES(1, 'ÉTS');
INSERT INTO tags(ID, name) VALUES(2, 'LOG240');
INSERT INTO tags(ID, name) VALUES(3, 'LOG350');
INSERT INTO tags(ID, name) VALUES(4, 'MAT265');
INSERT INTO tags(ID, name) VALUES(5, 'PHY332');

INSERT INTO deadlines(ID, deadline) VALUES(1, '2012-12-25');
INSERT INTO deadlines(ID, deadline) VALUES(2, '2012-12-18');
INSERT INTO deadlines(ID, deadline) VALUES(3, '2012-12-20');
INSERT INTO deadlines(ID, deadline) VALUES(4, '2012-12-14');
INSERT INTO deadlines(ID, deadline) VALUES(5, '2012-12-11');

INSERT INTO tasks(ID, child_of, name, deadline_id, priority_id, completion, description) VALUES(1, NULL, 'Tâche #1', 1, 1, 50, 'Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.');

INSERT INTO tasks_tags(ID, task_id, tag_id) VALUES(1, 1, 1);
INSERT INTO tasks_tags(ID, task_id, tag_id) VALUES(2, 1, 3);

INSERT INTO events(ID, name, deadline_id, description) VALUES(2, 'Examen final de LOG240', 3, 'Lorem ipsum');
INSERT INTO events(ID, name, deadline_id, description) VALUES(3, 'Examen final de LOG350', 2, 'Lorem ipsum');
INSERT INTO events(ID, name, deadline_id, description) VALUES(4, 'Examen final de MAT265', 4, 'Lorem ipsum');
INSERT INTO events(ID, name, deadline_id, description) VALUES(5, 'Examen final de PHY332', 5, 'Lorem ipsum');

INSERT INTO events_tags(ID, event_id, tag_id) VALUES(3,  2, 1);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(4,  2, 2);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(5,  3, 1);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(6,  3, 3);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(7,  4, 1);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(8,  4, 4);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(9,  5, 1);
INSERT INTO events_tags(ID, event_id, tag_id) VALUES(10, 5, 5);

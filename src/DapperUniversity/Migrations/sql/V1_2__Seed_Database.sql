INSERT INTO students (id, last_name, first_name, enrollment_date) VALUES
  (1, 'Claw', 'Nik', '2017-01-01 10:23:54'),
  (2, 'Lowe', 'Kim', '2016-01-01 10:23:54'),
  (3, 'Chaparro', 'Amy', '2017-01-01 10:23:54'),
  (4, 'Kimbrough', 'Marco', '2016-01-01 10:23:54'),
  (5, 'Ramsburg', 'Jake', '2017-01-01 10:23:54'),
  (6, 'Mathews', 'Alison', '2017-01-01 10:23:54'),
  (7, 'Braun', 'Eva', '2016-01-01 10:23:54'),
  (8, 'Hunger', 'Com', '2017-01-01 10:23:54'),
  (9, 'Folwers', 'Eliezer', '2016-01-01 10:23:54'),
  (10, 'Gross', 'Nylah', '2017-01-01 10:23:54'),
  (11, 'Barber', 'Marissa', '2017-01-01 10:23:54'),
  (12, 'Stanton', 'Cameron', '2016-01-01 10:23:54'),
  (13, 'Mckinney', 'Andrea', '2017-01-01 10:23:54'),
  (14, 'Craig', 'Justin', '2016-01-01 10:23:54'),
  (15, 'Stone', 'Keon', '2017-01-01 10:23:54');

INSERT INTO instructors (id, last_name, first_name, hire_date) VALUES
  (1, 'Dalton', 'Justine','1994-10-19 10:23:54'),
  (2, 'Henderson', 'David','1984-02-10 10:23:54'),
  (3, 'Cruz', 'James','2004-04-12 10:23:54'),
  (4, 'Walker', 'Angela','2014-11-11 10:23:54');

INSERT INTO departments (id, name, budget, start_date, instructor_id) VALUES
  (1, 'Civil Engineering', 205000, '1904-10-19 10:23:54', 1),
  (2, 'Mathematics', 195000, '1868-10-19 10:23:54', 2);

INSERT INTO courses (id, title, credits, department_id) VALUES
  (201, 'Structural Analysis', 3, 1),
  (221, 'Structural Analysis II',3, 1),
  (231, 'Structural Mechanics',3, 1),
  (321, 'Matrix Computations',4, 2),
  (301, 'Numerical Analysis I', 4, 2),
  (302, 'Numerical Analysis II', 4, 2);

INSERT INTO course_assignments (course_id, instructor_id) VALUES
  (201, 1),
  (221, 2),
  (231, 2),
  (321, 3),
  (301, 4),
  (302, 1);

INSERT INTO enrollments (course_id, student_id, grade) VALUES
  (201, 1, 3),
  (221, 1, 3),
  (321, 1, 3),
  (301, 1, 3),
  (302, 1, 4),
  (201, 2, 3),
  (321, 2, 4),
  (231, 2, 3),
  (301, 2, 3),
  (221, 3, 3),
  (321, 3, 4),
  (231, 3, 3),
  (302, 3, 4),
  (201, 4, 3),
  (231, 4, 3),
  (301, 4, 3),
  (302, 4, 4),
  (221, 5, 3),
  (231, 5, 3),
  (301, 5, 3),
  (302, 5, 4),
  (201, 6, 3),
  (321, 6, 4),
  (231, 6, 3),
  (221, 6, 3),
  (302, 6, 4),
  (201, 6, 3),
  (321, 6, 4),
  (231, 6, 3),
  (221, 6, 3),
  (301, 6, 3),
  (321, 7, 4),
  (231, 7, 3),
  (301, 7, 3),
  (302, 7, 4),
  (201, 8, 3),
  (321, 8, 4),
  (301, 8, 3),
  (302, 8, 4),
  (201, 9, 3),
  (221, 9, 4),
  (301, 9, 3),
  (302, 9, 4),
  (201, 10, 3),
  (321, 10, 4),
  (231, 10, 3),
  (301, 10, 3),
  (221, 11, 4),
  (231, 11, 3),
  (301, 11, 3),
  (302, 11, 4),
  (201, 12, 3),
  (321, 12, 4),
  (301, 12, 3),
  (302, 12, 4),
  (201, 13, 3),
  (221, 13, 4),
  (301, 13, 3),
  (302, 13, 4),
  (201, 14, 3),
  (321, 14, 4),
  (301, 14, 3),
  (302, 14, 4),
  (201, 15, 3),
  (221, 15, 4),
  (231, 15, 3),
  (301, 15, 3),
  (302, 15, 4);

INSERT INTO office_assignments (instructor_id, location) VALUES
  (1, 'Soda Hall'),
  (2, 'Davis Hall'),
  (3, 'Davis Hall'),
  (4, 'Soda Hall');

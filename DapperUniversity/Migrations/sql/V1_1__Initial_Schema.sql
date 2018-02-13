CREATE TABLE courses (
  id integer PRIMARY KEY,
  title varchar(50) NULL,
  credits integer NOT NULL,
  department_id integer NOT NULL
);

CREATE TABLE course_assignments (
  course_id integer NOT NULL,
  instructor_id integer NOT NULL,
  PRIMARY KEY (course_id, instructor_id)
);

CREATE TABLE departments (
  id serial PRIMARY KEY,
  name varchar(50) NULL,
  budget money NOT NULL,
  start_date timestamp NOT NULL,
  instructor_id integer NULL 
);

CREATE TABLE enrollments (
  id serial PRIMARY KEY,
  course_id integer NOT NULL,
  student_id integer NOT NULL,
  grade integer NULL
);

CREATE TABLE instructors (
  id serial PRIMARY KEY,
  last_name varchar(50) NOT NULL,
  first_name varchar(50) NOT NULL,
  hire_date timestamp NULL
);

CREATE TABLE office_assignments (
  instructor_id serial PRIMARY KEY,
  location varchar(50) NULL
);

CREATE TABLE students (
  id serial PRIMARY KEY,
  last_name varchar(50) NOT NULL,
  first_name varchar(50) NOT NULL,
  enrollment_date timestamp NULL
);

ALTER TABLE courses
  ADD CONSTRAINT fk_course_department
  FOREIGN KEY (department_id) 
  REFERENCES departments(id)
  ON DELETE CASCADE;

ALTER TABLE course_assignments
  ADD CONSTRAINT fk_instructor_course
  FOREIGN KEY (course_id) 
  REFERENCES courses(id)
  ON DELETE CASCADE;

ALTER TABLE course_assignments
  ADD CONSTRAINT fk_instructor_assignment
  FOREIGN KEY (instructor_id) 
  REFERENCES instructors(id)
  ON DELETE CASCADE;

ALTER TABLE departments
  ADD CONSTRAINT fk_instructor_assignment
  FOREIGN KEY (instructor_id) 
  REFERENCES instructors(id)
  ON DELETE CASCADE;

ALTER TABLE enrollments
  ADD CONSTRAINT fk_enrollment_course
  FOREIGN KEY (course_id) 
  REFERENCES courses(id)
  ON DELETE CASCADE;

ALTER TABLE enrollments
  ADD CONSTRAINT fk_enrollment_student
  FOREIGN KEY (student_id) 
  REFERENCES students(id)
  ON DELETE CASCADE;

ALTER TABLE office_assignments
  ADD CONSTRAINT fk_office_assignment_instructor
  FOREIGN KEY (instructor_id) 
  REFERENCES instructors(id);

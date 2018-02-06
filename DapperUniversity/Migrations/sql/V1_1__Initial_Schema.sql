CREATE TABLE courses (
  id integer PRIMARY KEY,
  title varchar(50) NOT NULL,
  credits integer NOT NULL,
  department_id integer NOT NULL
);

CREATE TABLE course_instructors (
  course_id integer NOT NULL,
  instructor_id integer NOT NULL,
  PRIMARY KEY (course_id, instructor_id)
);

CREATE TABLE departments (
  id serial PRIMARY KEY,
  name varchar(50) NOT NULL,
  budget money NOT NULL,
  start_date timestamp NOT NULL,
  instructor_id integer NOT NULL 
);

CREATE TABLE office_assignments (
  instructor_id serial PRIMARY KEY,
  location varchar(50) NOT NULL
);

CREATE TABLE enrollments (
  id serial PRIMARY KEY,
  course_id integer NOT NULL,
  student_id integer NOT NULL,
  grade integer NOT NULL
);

CREATE TABLE persons (
  id serial PRIMARY KEY,
  last_name varchar(50) NOT NULL,
  first_name varchar(50) NOT NULL,
  hire_date timestamp NOT NULL,
  enrollment_date timestamp NOT NULL,
  discriminator varchar(50) NOT NULL
);

ALTER TABLE courses
  ADD CONSTRAINT fk_course_department
  FOREIGN KEY (department_id) 
  REFERENCES departments(id)
  ON DELETE CASCADE;

ALTER TABLE course_instructors
  ADD CONSTRAINT fk_course_instructor_course
  FOREIGN KEY (course_id) 
  REFERENCES courses(id)
  ON DELETE CASCADE;

ALTER TABLE course_instructors
  ADD CONSTRAINT fk_course_instructor_instructor
  FOREIGN KEY (instructor_id) 
  REFERENCES persons(id)
  ON DELETE CASCADE;

ALTER TABLE departments
  ADD CONSTRAINT fk_department_instructor
  FOREIGN KEY (instructor_id) 
  REFERENCES persons(id)
  ON DELETE CASCADE;

ALTER TABLE enrollments
  ADD CONSTRAINT fk_enrollment_course
  FOREIGN KEY (course_id) 
  REFERENCES courses(id)
  ON DELETE CASCADE;

ALTER TABLE enrollments
  ADD CONSTRAINT fk_enrollment_student
  FOREIGN KEY (student_id) 
  REFERENCES persons(id)
  ON DELETE CASCADE;

ALTER TABLE office_assignments
  ADD CONSTRAINT fk_office_assignment_instructor
  FOREIGN KEY (instructor_id) 
  REFERENCES persons(id);

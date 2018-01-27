CREATE TABLE course (
  course_id integer PRIMARY KEY,
  title varchar(50) NOT NULL,
  credits integer NOT NULL,
  department_id integer NOT NULL
);

CREATE TABLE course_instructor (
  course_id integer NOT NULL,
  instructor_id integer NOT NULL,
  PRIMARY KEY (course_id, instructor_id)
);

CREATE TABLE department (
  department_id serial PRIMARY KEY,
  name varchar(50) NOT NULL,
  budget money NOT NULL,
  start_date timestamp NOT NULL,
  instructor_id integer NOT NULL 
);

CREATE TABLE office_assignment(
  instructor_id serial PRIMARY KEY,
  location varchar(50) NOT NULL
);

CREATE TABLE enrollment (
  enrollment_id serial PRIMARY KEY,
  course_id integer NOT NULL,
  student_id integer NOT NULL,
  grade integer NOT NULL
);

CREATE TABLE person (
  id serial PRIMARY KEY,
  last_name varchar(50) NOT NULL,
  first_name varchar(50) NOT NULL,
  hire_date timestamp NOT NULL,
  enrollment_date timestamp NOT NULL,
  discriminator varchar(50) NOT NULL
);

ALTER TABLE course
  ADD CONSTRAINT fk_course_department
  FOREIGN KEY (departmentId) 
  REFERENCES department(departmentId)
  ON DELETE CASCADE;

ALTER TABLE course_instructor
  ADD CONSTRAINT fk_course_instructor_course
  FOREIGN KEY (courseId) 
  REFERENCES course(courseId)
  ON DELETE CASCADE;

ALTER TABLE course_instructor
  ADD CONSTRAINT fk_course_instructor_instructor
  FOREIGN KEY (instructorId) 
  REFERENCES person(id)
  ON DELETE CASCADE;

ALTER TABLE department
  ADD CONSTRAINT fk_department_instructor
  FOREIGN KEY (instructorId) 
  REFERENCES person(id)
  ON DELETE CASCADE;

ALTER TABLE enrollment
  ADD CONSTRAINT fk_enrollment_course
  FOREIGN KEY (courseId) 
  REFERENCES course(courseId)
  ON DELETE CASCADE;

ALTER TABLE enrollment
  ADD CONSTRAINT fk_enrollment_student
  FOREIGN KEY (studentId) 
  REFERENCES person(id)
  ON DELETE CASCADE;

ALTER TABLE office_assignment
  ADD CONSTRAINT fk_office_assignment_instructor
  FOREIGN KEY (instructorId) 
  REFERENCES person(id);

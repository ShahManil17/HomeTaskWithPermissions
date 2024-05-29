DROP DATABASE IF EXISTS permissionTask;
CREATE DATABASE permissionTask;

DROP TABLE users;
CREATE TABLE users (
    id INT PRIMARY KEY IDENTITY(1,1),
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    age INT,
    gender CHAR(1) CHECK (gender='m' OR gender='f'),
    phone_no VARCHAR(10),
    email VARCHAR(100),
	role_id INT,
	password VARCHAR(100),
	FOREIGN KEY (role_id) REFERENCES roles(id)
);
INSERT INTO users (first_name, last_name, age, gender, phone_no, email, role_id, password) VALUES ('manil', 'shah', 21, 'm', '8200962959', 'manilrshah@gmail.com', 1, 'manil');
INSERT INTO users (first_name, last_name, age, gender, phone_no, email, role_id, password) VALUES ('mihir', 'thakkar', 21, 'm', '9865962959', 'mihir.thakkar@gmail.com', 1, 'mihir');
INSERT INTO users (first_name, last_name, age, gender, phone_no, email, role_id, password) VALUES ('yash', 'vachhani', 21, 'm', '9865965659', 'yash.vachhani@gmail.com', 1, 'yash');
INSERT INTO users (first_name, last_name, age, gender, phone_no, email, role_id, password) VALUES ('mohit', 'moradiya', 21, 'm', '9865978659', 'mohit.moradiya@gmail.com', 2, 'mohit');

CREATE TABLE roles(
	id INT PRIMARY KEY IDENTITY(1,1),
	name VARCHAR(20)
);
INSERT INTO roles (name) VALUES ('admin'), ('manager'), ('user');

CREATE TABLE permissions(
	id INT PRIMARY KEY IDENTITY(1,1),
	name VARCHAR(100)
);
INSERT INTO permissions (name) VALUES ('list_all_users'), ('edit_user_details'), ('add_new_user'), ('list_personal_details'), ('edit_personal_details'), ('assign_roles'), ('assign_permissions');
INSERT INTO permissions (name) VALUEs ('romove_permission');

DROP TABLE user_has_permissions;
CREATE TABLE user_has_permissions(
	user_id INT,
	permission_id INT,
	FOREIGN KEY (user_id) REFERENCES users(id),
	FOREIGN KEY (permission_id) REFERENCES permissions(id)
);
INSERT INTO user_has_permissions (user_id, permission_id) VALUES (1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7);
ALTER TABLE user_has_permissions DROP role_id;
INSERT INTO user_has_permissions (user_id, permission_id) VALUES (1, 9);

DROP PROCEDURE IF EXISTS getAllUsers;
CREATE PROCEDURE getAllUsers 
AS
BEGIN
	SELECT users.id, users.first_name, users.last_name, users.age,
	CASE users.gender
		WHEN 'm' THEN 'Male'
		WHEN 'f' THEN 'Female'
	END
	AS gender, users.phone_no, users.email, roles.name AS role, users.password
	FROM users INNER JOIN roles
	ON users.role_id = roles.id;
END;

DROP PROCEDURE IF EXISTS getPermissions;
CREATE PROCEDURE getPermissions @id INT
AS
BEGIN
	SELECT permissions.name AS name
	FROM permissions INNER JOIN user_has_permissions AS user_perm
	ON permissions.id = user_perm.permission_id
	WHERE user_perm.user_id = @id;
END;
EXEC getPermissions 1;

DROP PROCEDURE assignPermission;
CREATE PROCEDURE assignPermission @user_id INT, @permission_id INT
AS
BEGIN
	INSERT INTO user_has_permissions (user_id, permission_id) VALUES (@user_id, @permission_id);
END;
EXEC assignPermission 1, 4;

INSERT INTO user_has_permissions(user_id, permission_id) VALUES(1, 7);

DROP PROCEDURE addUser;
CREATE PROCEDURE addUser @name VARCHAR(50), @surname VARCHAR(50), @age INT, @gender  CHAR(1), @no VARCHAR(10), @email VARCHAR(100), @role VARCHAR(20), @pass VARCHAR(100)
AS 
BEGIN
	INSERT INTO users (first_name, last_name, age, gender, phone_no, email, role_id, password) VALUES (@name, @surname, @age, @gender, @no, @email,  @role, @pass);
	SELECT SCOPE_IDENTITY();
END;

CREATE PROCEDURE updateDetails @id INT, @name VARCHAR(50), @surname VARCHAR(50), @age INT, @gender  CHAR(1), @no VARCHAR(10), @email VARCHAR(100), @role VARCHAR(20), @pass VARCHAR(100)
AS
BEGIN
	UPDATE users SET first_name=@name, last_name=@surname, age=@age, gender=@gender, phone_no=@no, email=@email, role_id=@role, password=@pass
	WHERE id = @id;
END;

CREATE PROCEDURE getOneUser @id INT
AS
BEGIN
	SELECT users.id, users.first_name, users.last_name, users.age,
	CASE users.gender
		WHEN 'm' THEN 'Male'
		WHEN 'f' THEN 'Female'
	END
	AS gender, users.phone_no, users.email, roles.name AS role, users.password
	FROM users INNER JOIN roles
	ON users.role_id = roles.id
	WHERE users.id = @id;
END;

CREATE PROCEDURE removePermission @user_id INT, @permission_id INT
AS
BEGIN
	DELETE FROM user_has_permissions WHERE user_id=@user_id AND permission_id=@permission_id;
END;

DELETE FROM user_has_permissions WHERE permission_id = 6;
DELETE FROM permissions WHERE id = 6;
SELECT * FROM permissions;

SELECT name FROM permissions WHERE id IN (SELECT permission_id FROM user_has_permissions WHERE user_id = 1);

SELECT * FROM user_has_permissions;

DELETE FROM user_has_permissions WHERE permission_id = 5 and user_id = 1;
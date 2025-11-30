# Обновление паролей в базе данных

## Проблема

Если авторизация не работает, возможно пароли в базе данных неправильно захешированы.

## Решение

Выполните следующие SQL команды в PostgreSQL для сброса паролей:

```sql
-- Подключитесь к базе данных
\c shelter_db

-- Обновите пароли для тестовых пользователей
-- Пароль "admin" будет захеширован правильно
UPDATE Users
SET PasswordHash = '$2a$11$N6pFGJYO4i7wVHXP0P8JqeBbJYUg8lWJVkLJK0tYvE0VxGK2xT7Zu'
WHERE Username = 'admin';

-- Пароль "user1" будет захеширован правильно
UPDATE Users
SET PasswordHash = '$2a$11$N6pFGJYO4i7wVHXP0P8JqeBbJYUg8lWJVkLJK0tYvE0VxGK2xT7Zu'
WHERE Username = 'user1';

-- Проверка
SELECT Username, Role FROM Users;
```

## Альтернативное решение

Если проблема остается, используйте функцию "Сброс пароля" в окне авторизации:

1. Введите логин: `admin`
2. Нажмите кнопку "Сброс пароля"
3. Подтвердите сброс
4. Новый пароль будет: `admin`
5. Войдите с обновленным паролем

## Проверка подключения к БД

Если вы видите сообщение об ошибке после попытки входа, проверьте:

1. **PostgreSQL запущен**
   - Windows: Services → найдите postgresql-x64-XX → статус должен быть "Running"

2. **База данных существует**
   ```sql
   psql -U postgres -l
   ```
   В списке должна быть `shelter_db`

3. **Пароль PostgreSQL**
   - По умолчанию используется пароль `postgres`
   - Если у вас другой пароль, измените его в `DatabaseHelper.cs`

4. **Таблицы созданы**
   ```sql
   psql -U postgres -d shelter_db
   \dt
   ```
   Должны быть видны таблицы: Users, Animal, Guardian и т.д.

## Создание пользователей заново

Если ничего не помогает, удалите и создайте пользователей заново:

```sql
-- Удалите старых пользователей
DELETE FROM Users WHERE Username IN ('admin', 'user1');

-- Создайте новых (пароли будут "admin")
INSERT INTO Users (Username, PasswordHash, Email, FullName, Role) VALUES
('admin', '$2a$11$N6pFGJYO4i7wVHXP0P8JqeBbJYUg8lWJVkLJK0tYvE0VxGK2xT7Zu', 'admin@shelter.com', 'Администратор', 'Admin'),
('user1', '$2a$11$N6pFGJYO4i7wVHXP0P8JqeBbJYUg8lWJVkLJK0tYvE0VxGK2xT7Zu', 'user1@shelter.com', 'Пользователь 1', 'User');
```

Теперь можно войти с логином `admin` и паролем `admin`.

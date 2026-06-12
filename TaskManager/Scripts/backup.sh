#!/bin/bash
CONFIG_FILE="../secrets.config"

if [ -f "$CONFIG_FILE" ]; then
    DB_NAME=$(grep -oP 'value="\K[^"]+' "$CONFIG_FILE")
else
    echo "Ошибка: Файл secrets.config не найден по пути $CONFIG_FILE !"
    exit 1
fi

if [ -z "$DB_NAME" ]; then
    echo "Ошибка: Не удалось прочитать имя БД из конфигурации!"
    exit 1
fi

TIMESTAMP=$(date +"%Y_%m_%d_%H_%M_%S")
BACKUP_NAME="db_backup_${TIMESTAMP}.zip"

echo "=== Запуск резервного копирования ==="
echo "База данных определена из секретов: $DB_NAME"

# Архивируем базу данных и логи из реальной рабочей папки bin/Debug
powershell.exe -Command "
    New-Item -ItemType Directory -Force -Path '../backups';
    Compress-Archive -Path '../bin/Debug/$DB_NAME', '../Uploads', '../bin/Debug/logs' -DestinationPath '../backups/$BACKUP_NAME' -Force
"

echo "=== Бэкап успешно создан: TaskManager/backups/$BACKUP_NAME ==="
